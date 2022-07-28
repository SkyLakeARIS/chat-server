using Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;

namespace Server.Network;

public static class ChatHandler
{

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

        if (string.IsNullOrWhiteSpace(packet.Message))
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
	    ChatSession session = arg1 as ChatSession;
	    C_RequestSignIn packet = arg2 as C_RequestSignIn;

	    // SignIn 패킷 개선사항
        // 계정 정보를 UID, ID, PW, accountType, nickname 로 수정

        packet.ID = packet.ID.Trim();
        packet.ID = packet.ID.Replace(" ", "");
        packet.Password = packet.Password.Trim();
        packet.Password = packet.Password.Replace(" ", "");

        bool isFail = false;
        // 패킷 정보를 간단하게 검사합니다.
        if (string.IsNullOrWhiteSpace(packet.ID) || string.IsNullOrWhiteSpace(packet.Password))
        {
	        isFail = true;
        }
        else if(packet.ID.Length < 4 || packet.ID.Length > 20)
        {
	        isFail = true;
        }
        else if (packet.Password.Length < 8 || packet.Password.Length > 20)
        {
	        isFail = true;
        }

        if (isFail)
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "아이디 또는 비밀번호가 다릅니다.";
	        session.Send(failSignInPacket.Write());
	        return;
        }

        List<AccountEntity> find = null;

	    DatabaseManager instance = DatabaseManager.Instance;
	    var accountsCollection = instance.GetCollection<AccountEntity>(DatabaseManager.AccountCollection);
	    //BsonDocument
	    find = accountsCollection.Find(x => x.ID.Equals(packet.ID) && x.password.Equals(packet.Password)).ToList();


        // 로그인 실패시 클라에게 결과를 알립니다.
        if (find.Count <= 0 || find == null)
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "아이디 또는 비밀번호가 다릅니다.";
            session.Send(failSignInPacket.Write());
            return;
        }
        
        // 로그인 성공시 처리 부분입니다.
        // 유저 정보들을 클라에게 전송합니다.
        session.SetUID(find[0].UID);
        session.SetNickname(find[0].nickName);

        S_SuccessSignIn successSignInPacket = new S_SuccessSignIn();
        successSignInPacket.UID = session.GetUID();
        successSignInPacket.Nickname = session.GetNickname();
        session.Send(successSignInPacket.Write());


        // 서버에 접속한 유저의 닉네임을 현재 접속중인 클라이언트들에게 브로드캐스트합니다
        // 이는 현재 구조가 메인채팅 서버 하나로 통일해서 구현 중이기 때문에 이곳에서 처리.
        S_UserSignIn userSignInPacket = new S_UserSignIn();
        userSignInPacket.Nickname = session.GetNickname();


        // 접속한 유저를 채팅방에 입장시키기 전에
        // 유저에게 접속자 리스트 보내기 위해서
        // 접속해있는 유저 리스트를 구성합니다.
        S_CurrentUserList userListPacket = new S_CurrentUserList();
        userListPacket.userLists = Program.Server.GetNicknameList();
        if (userListPacket.userLists.Count > 0)
        {
	        session.Send(userListPacket.Write());
        }

        // 로그인한 해당 세션을 채팅방에 입장 시킵니다.
        // 현재 main 채팅방으로 통일하여 나중에 서버 추가가 가능하면 그에 맞게 수정 필요.
        Program.Server.Push(() => Program.Server.Enter(session));

        // SendChat과 마찬가지로 server를 따로 빼서 사용합니다.
        Server server = session.chatServer;

        // 유저가 접속한 사실을 접속자들에게 브로드 캐스트하여 접속한 유저를 추가하도록 합니다.
        server.Push(() => {server.Broadcast(session, userSignInPacket.Write()); });
        Console.WriteLine($"LOG - {session.GetNickname()}({session.GetUID()}) was Sign In.");

        // 입장 메세지 브로드캐스트
        // 임시로 세션은 현재 로그인하는 세션을 사용해서, 나중에 나은 방법 찾을 필요가 있음.
        S_SendChat signInMessage = new S_SendChat();
        signInMessage.Nickname = "Server";
        signInMessage.Message = $"{session.GetNickname()}님이 입장했습니다.";
        server.Push(() => { server.Broadcast(session, signInMessage.Write());});

        // 채팅 서버에 가입할 때 처리할 DB 로직
        //ChatServerEntity chatServerEntity = new ChatServerEntity()
        //{
        // entityId = ObjectId.GenerateNewId(),
        // serverID = 1,
        // serverName = session.chatServer.GetServerName(),
        //    userList = new List<long>()
        //};

        //chatServerEntity.userList.Add(session.SessionId);
        //chatServerEntity.userList.Add(session.NickName+"111");
        //chatServerEntity.userList.Add(session.NickName+"222");

        //      var collection =  DatabaseManager.Instance.GetCollection<ChatServerEntity>(DatabaseManager.ChatServerCollection);
        //collection.InsertOne(chatServerEntity);
    }

    internal static void C_RequestSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        C_RequestSignUp packet = arg2 as C_RequestSignUp;
        ChatSession session = arg1 as ChatSession;

        // 패킷 정보가 올바른지 검사합니다.
        bool isFail = false;

        packet.ID = packet.ID.Trim();
        packet.ID = packet.ID.Replace(" ", "");
        packet.Password = packet.Password.Trim();
        packet.Password = packet.Password.Replace(" ", "");
        packet.Nickname = packet.Nickname.Trim();

        if (string.IsNullOrWhiteSpace(packet.ID) || string.IsNullOrWhiteSpace(packet.Password) || string.IsNullOrWhiteSpace(packet.Nickname))
        {
	        isFail = true;
        }

        if (packet.ID.Length < 4 || packet.ID.Length > 20 )
        {
	        isFail = true;
        }

        if (packet.Password.Length < 8 || packet.Password.Length > 20)
        {
	        isFail = true;
        }

        if (packet.Nickname.Length < 2 || packet.Nickname.Length > 10)
        {
	        isFail = true;
        }
 

        if (isFail)
        {
	        S_FailSignUp failSignUpPacket = new S_FailSignUp();
	        failSignUpPacket.Reason = "회원 가입 실패 : 올바르지 않은 정보입니다.";
	        session.Send(failSignUpPacket.Write());
	        return;
        }

        DatabaseManager instance = DatabaseManager.Instance;
        var accountsCollection = instance.GetCollection<AccountEntity>(DatabaseManager.AccountCollection);

        // 이미 가입된 계정이 존재하는 경우를 체크합니다.
        var find = accountsCollection.Find(x => x.ID.Equals(packet.ID)).ToList();

        if (find.Count > 0)
        {
	        S_FailSignUp failSignUpPacket = new S_FailSignUp();
	        failSignUpPacket.Reason = "이미 가입되어 있는 계정입니다.";
	        session.Send(failSignUpPacket.Write());
	        return;
        }

        // 회원 가입을 처리하는 부분입니다.
        AccountEntity account = new AccountEntity()
        {
	        entityId = ObjectId.GenerateNewId(),
            UID = SessionManager.instance.GenarateUID(),
            ID = packet.ID,
            password = packet.Password,
            nickName = packet.Nickname,
            accountType = EAccountType.User,
        };

        accountsCollection.InsertOne(account);


	    S_SuccessSignUp successSignUpPacket = new S_SuccessSignUp();
        successSignUpPacket.Message = "회원 가입이 완료되었습니다.";
        session.Send(successSignUpPacket.Write());
        
        Console.WriteLine($"LOG - ID: {packet.ID}({account.UID}) was Sign Up.");

    }

    internal static void C_RequestSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        ChatSession session = arg1 as ChatSession;
        C_RequestSignOut packet = arg2 as C_RequestSignOut;

        // session이 유효한지 체크 후 로그아웃 처리합니다.
        ChatSession foundSession = SessionManager.instance.Find(packet.UID);
        if (foundSession != session)
        {
	        return;
        }


        Server server = session.chatServer;
        // 퇴장 메세지 브로드캐스트
        // 임시로 세션은 현재 로그인하는 세션을 사용해서, 나중에 나은 방법 찾을 필요가 있음.
        S_SendChat signOutMessage = new S_SendChat();
        signOutMessage.Nickname = "Server";
        signOutMessage.Message = $"{session.GetNickname()}님이 퇴장했습니다.";
        server.Push(() => { server.Broadcast(session, signOutMessage.Write()); });

        S_UserSignOut userSignOutPacket = new S_UserSignOut();
        userSignOutPacket.Nickname = session.GetNickname();

        server.Push(() =>
        {
            server.Broadcast(session, userSignOutPacket.Write());
        });

        // 로그아웃 처리
        S_SuccessSignOut successPacket = new S_SuccessSignOut();
        successPacket.Message = "로그아웃 되었습니다.";
        session.Send(successPacket.Write());

        Console.WriteLine($"LOG - {session.GetNickname()}({session.GetUID()}) was Sign Out.");

        // 클라이언트 세션을 '채팅방'에서 제거한다.
        server.Push(() => { server.Leave(session);});
        session.chatServer = null;
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