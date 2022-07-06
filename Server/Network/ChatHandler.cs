using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Core;
using Core.Packet;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;

namespace Server.Network;

public static class ChatHandler
{
    public static void OnUserChat(ChatSession session, InPacket packet)
    {
        var message = packet.DecodeString();
        var outPacket = new OutPacket((int)ServerPacket.Chat);
        outPacket.EncodeString(session.NickName);
        outPacket.EncodeString(message);

        var sendBuff = outPacket.Close();

       // Program.Server.Broadcast(sendBuff);
    }

    internal static void C_SendChatHandler(PacketSession arg1, IPacket arg2)
    {
        C_SendChat packet = arg2 as C_SendChat;
        ChatSession session = arg1 as ChatSession;


        // SendChat 패킷 변경 필요 사항
        // Server 클래스, ServerManager 클래스 제작 (채팅방 기능 만들어지면)
        // 클라가 어떤 서버에서 메세지를 보냈는지
        // 메세지 내용은 무엇인지
        // 메세지에 명령어가 있는지 (추후)


        // 채팅 서버에 속해있지 않으면 패킷을 무시한다.
        if (session.chatServer == null)
        {
            return;
        }
        
        // 중계 역할로, 클라에서 보낸 메세지를 세션 정보와 함께 채팅서버 클래스로 보내서 브로드캐스트 한다.
        // jobQueue를 사용하여 직접 호출이 아니라 델리게이트를 push하는 방식으로 변경. 즉, 함수 호출이라는 액션.
        // 주문서와 비슷한 역할을 한다.

        // 단 이렇게 람다를 사용하면 해당 개체가 사라지면 에러다. Program.Server.Push(() => { Program.Server.Broadcast(session, packet.Message); })
        // 따라서 아래처럼 server를 추출한 뒤에 사용한다. 
        // 이유는 onDisconnect는 접속이 끊기면 바로 실행되는데, JobQueue는 상대적으로 미래에 실행되기 때문이다.
        // 즉, onDisconnect에서 server(채팅방)을 null로 밀어버려서 나중에 action이 실행될 때 null참조가 된다.
        Server server = session.chatServer;
        server.Push(() => server.Broadcast(session, packet.Message));
        //Program.Server.Broadcast(session, packet.Message);
    }

    internal static void C_RequestSignInHandler(PacketSession arg1, IPacket arg2)
    {
	    C_RequestSignIn packet = arg2 as C_RequestSignIn;
	    ChatSession session = arg1 as ChatSession;
	    List<AccountEntity> find = null;

	    // SignIn 패킷 개선사항
        // 계정 정보를 UID, ID, PW, accountType, nickname 로 수정

        // 패킷 정보를 간단하게 검사합니다.
        if (string.IsNullOrWhiteSpace(packet.ID) || string.IsNullOrWhiteSpace(packet.Password))
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "아이디 또는 비밀번호가 다릅니다.";
	        session.Send(failSignInPacket.Write());
	        return;
        }

        try
	    {
		    DatabaseManager instance = DatabaseManager.Instance;
		    var accountsCollection = instance.GetCollection<AccountEntity>("accounts");
		    //BsonDocument
		    find = accountsCollection.Find(x => x.ID.Equals(packet.ID) && x.password.Equals(packet.Password)).ToList();
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    S_FailSignIn failSignInPacket = new S_FailSignIn();
		    failSignInPacket.Reason = "로그인에 실패했습니다.";
		    session.Send(failSignInPacket.Write());
            throw;
	    }

	    Console.WriteLine($"LOG - {session.NickName}({session.UserID}) was Sign In.");


        // 로그인 실패시 클라에게 결과를 알립니다.
        if (find.Count <= 0)
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "아이디 또는 비밀번호가 다릅니다.";
            session.Send(failSignInPacket.Write());
            return;
        }
        
        // 로그인 성공시 처리 부분입니다.
        // 유저 정보들을 클라에게 전송합니다.
        session.UserID = 0;
        session.NickName = find[0].nickName;

        S_SuccessSignIn successSignInPacket = new S_SuccessSignIn();
        successSignInPacket.UserID = session.UserID;
        successSignInPacket.UserName = session.NickName;
        session.Send(successSignInPacket.Write());

        // 서버에 접속한 유저의 닉네임을 현재 접속중인 클라이언트들에게 전부
        // 브로드캐스트.
        // 이는 현재 구조가 메인채팅 서버 하나로 통일해서 구현 중이기 때문에 이곳에서 처리.
        S_UserSignIn userSignInPacket = new S_UserSignIn();
        userSignInPacket.UserName = session.NickName;
        
        // SendChat과 마찬가지로 server를 따로 빼서 사용합니다.
        Server server = session.chatServer;
        server.Push(() => {server.Broadcast(session, userSignInPacket.Write()); });

    }

    internal static void C_RequestSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        C_RequestSignUp packet = arg2 as C_RequestSignUp;
        ChatSession session = arg1 as ChatSession;

        // 패킷 정보가 올바른지 검사합니다.
        bool isFail = false;
        if (packet.ID.Length <= 0 || packet.ID.Length >= 20 )
        {
	        isFail = true;
        }

        if (packet.Password.Length < 8 || packet.Password.Length > 20)
        {
	        isFail = true;
        }

        if (packet.UserName.Length <= 0 || packet.UserName.Length > 10)
        {
	        isFail = true;
        }


        if (isFail)
        {
	        S_FailSignUp failPacket = new S_FailSignUp();
	        failPacket.Reason = "회원 가입 처리 실패 : 올바르지 않은 정보입니다.";
	        session.Send(failPacket.Write());
	        return;
        }



        // 이미 가입된 계정이 존재하는 경우를 체크합니다.

        // to do

        try
        {
			DatabaseManager instance = DatabaseManager.Instance;
	        var accountsCollection = instance.GetCollection<AccountEntity>("accounts");
	        AccountEntity account = new AccountEntity()
	        {
		        entityId = ObjectId.GenerateNewId(),
	            ID = packet.ID,
	            password = packet.Password,
	            nickName = packet.UserName,
	            accountType = EAccountType.User,
	        };
	        accountsCollection.InsertOne(account);

        }
        catch (Exception e)
        {
	        Console.WriteLine($"Error - fail sign up : {e.ToString()}");
	        S_FailSignUp failPacket = new S_FailSignUp();
	        failPacket.Reason = "회원 가입 처리 실패";
            session.Send(failPacket.Write());
        }
        
 
        S_SuccessSignUp successSignUpPacket = new S_SuccessSignUp();
        successSignUpPacket.Reason = "회원 가입이 완료되었습니다.";
        session.Send(successSignUpPacket.Write());
        
        Console.WriteLine($"LOG - ID: {packet.ID}({packet.UserName}) was Sign Up.");

    }

    internal static void C_RequestSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        ChatSession session = arg1 as ChatSession;
        C_RequestSignOut packet = arg2 as C_RequestSignOut;


        // session이 올바른 정보를 가지고 있는지 체크합니다.

        // to do

        // session이 유효한지 체크 후 로그아웃 처리합니다.
        ChatSession foundSession = SessionManager.instance.Find(session.SessionId);


        S_UserSignOut userSignOutPacket = new S_UserSignOut();
        userSignOutPacket.UserName = session.NickName;

        Server server = session.chatServer;
        server.Push(() =>
        {
            server.Broadcast(session, userSignOutPacket.Write());
        });

        S_SucessSignOut successPacket = new S_SucessSignOut();
        successPacket.Message = "로그아웃 되었습니다.";
        session.Send(successPacket.Write());

        Console.WriteLine($"LOG - {session.NickName}({session.UserID}) was Sign Out.");

        // session의 정보도 초기화 할 것인지 고민 중

        Program.Server.Leave(session);
    }

    internal static void C_RequestEnterServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestLeaveServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestCreateServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestDeleteServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestJoinServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestServerListHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }
}