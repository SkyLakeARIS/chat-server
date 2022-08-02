using AccountType;
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
        if (session.ChatServer == null)
        {
            return;
        }

        // 패킷에서 채팅을 보낸 서버와 서버에서 현재 접속해있는 서버가 같은지 검사합니다.
        // sendchat패킷을 message -> serverid, message
        //if (session.ChatServer != packet)
        {

        }

        // 공백이나 null을 보내는 경우를 검사합니다.
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
        Server server = session.ChatServer;
        server.Push(() => server.Broadcast(session, packet.Message));
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

	    var accountsCollection = DatabaseManager.Instance.GetCollection<AccountEntity>(DatabaseManager.AccountCollection);
        //BsonDocument
        var entities = accountsCollection.Find(x => x.ID.Equals(packet.ID) && x.password.Equals(packet.Password)).ToList();


        // 로그인 실패시 클라에게 결과를 알립니다.
        if (entities.Count <= 0 || entities == null)
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "아이디 또는 비밀번호가 다릅니다.";
            session.Send(failSignInPacket.Write());
            return;
        }

        // 누군가 로그인하고 있는 계정이면 실패합니다.
        if (SessionManager.Instance.FindOrNull(entities[0].UID) != null)
        {
	        S_FailSignIn failSignInPacket = new S_FailSignIn();
	        failSignInPacket.Reason = "이미 로그인 중인 계정입니다.";
	        session.Send(failSignInPacket.Write());
	        return;
        }

        // 로그인 성공시 처리 부분입니다.
        // 유저 정보들을 클라에게 전송합니다.
        session.SetUID(entities[0].UID);
        session.SetNickname(entities[0].nickName);

        S_SuccessSignIn successSignInPacket = new S_SuccessSignIn();
        successSignInPacket.UID = session.GetUID();
        successSignInPacket.Nickname = session.GetNickname();
        session.Send(successSignInPacket.Write());



        // db 작업 - 소속된 서버 리스트 가져오기
        // 유저가 소속되어 있는 서버 리스트를 보내줍니다.
        S_SendServerList serverListPacket = new S_SendServerList();

        // 멀티스레딩에서 안전한가?
       foreach (var serverID in entities[0].joinedServerList)
       {
	       Server foundServer = ServerManager.Instance.FindServerOrNull(serverID);
	       if (foundServer != null)
	       {
		       S_SendServerList.ServerList info = new S_SendServerList.ServerList();
		       info.serverID = serverID;
		       info.serverName = foundServer.GetServerName();

		       serverListPacket.serverLists.Add(info);
	       }
       }

       session.Send(serverListPacket.Write());
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

        var accountsCollection = DatabaseManager.Instance.GetCollection<AccountEntity>(DatabaseManager.AccountCollection);

        // 이미 가입된 계정이 존재하는 경우를 체크합니다.
        var entity = accountsCollection.Find(x => x.ID.Equals(packet.ID));

        if (entity.ToList().Count > 0)
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
            UID = DatabaseManager.Instance.GenerateUserID(),
            ID = packet.ID,
            password = packet.Password,
            nickName = packet.Nickname,
            accountTypeList = new List<EAccountType>(),
            joinedServerList = new List<long>()
        };

        // 기본적으로 메인채팅방에 들어가 있는 상태입니다.
        account.joinedServerList.Add(ServerManager.MainServerID);
        account.accountTypeList.Add(EAccountType.User);

        accountsCollection.InsertOne(account);

        // DB의 main서버에 유저를 등록합니다.
        var chatServerDB = DatabaseManager.Instance.GetCollection<ChatServerEntity>(DatabaseManager.ChatServerCollection);
        var mainServerEntity = chatServerDB.Find(x=>x.serverID.Equals(ServerManager.MainServerID)).ToList();
        //mainServerEntity[0].jointedUserList.Add(account.UID);

        //Builders<ChatServerEntity>.Update.Set($"{nameof(ChatServerEntity.jointedUserList)}.{nameof(Detail.Types)}.$[elem].{nameof(Type.Status)}", "P");
        //var arrayFilter = new JsonArrayFilterDefinition<BsonDocument>($"{{ 'elem.{nameof(Type.InvoiceType)}': 'OO' }}");
        //var updateOptions = new UpdateOptions { ArrayFilters = new[] { arrayFilter } };
        //chatServerDB.UpdateOne({ _id: 1 },{ $push: { scores: 89 } });
        //Builders<ChatServerEntity>.Update.Push($"{nameof(ChatServerEntity.jointedUserList)}", account.UID);
        chatServerDB.UpdateOne(x => x.serverID.Equals(ServerManager.MainServerID), Builders<ChatServerEntity>.Update.Push($"{nameof(ChatServerEntity.jointedUserList)}", account.UID));


        S_SuccessSignUp successSignUpPacket = new S_SuccessSignUp();
        successSignUpPacket.Message = "회원 가입이 완료되었습니다.";
        session.Send(successSignUpPacket.Write());
        
        Console.WriteLine($"[LOG] - ID: {packet.ID}({account.UID}) nickname:{account.nickName} was Sign Up.");
    }

    internal static void C_RequestSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        ChatSession session = arg1 as ChatSession;
        C_RequestSignOut packet = arg2 as C_RequestSignOut;

        // session이 유효한지 체크 후 로그아웃 처리합니다.
        ChatSession foundSession = SessionManager.Instance.FindOrNull(packet.UID);
        if (foundSession != session)
        {
	        return;
        }


        Server server = session.ChatServer;
        // 퇴장 메세지 브로드캐스트
        // 임시로 세션은 현재 로그인하는 세션을 사용해서, 나중에 나은 방법 찾을 필요가 있음.
        S_SendChat signOutMessage = new S_SendChat();
        signOutMessage.Nickname = "Server";
        signOutMessage.Message = $"{session.GetNickname()}님이 퇴장했습니다.";
        server.Push(() => { server.Broadcast(session, signOutMessage.Write()); });

        // deleteUserList 로 대체
        //S_UserSignOut userSignOutPacket = new S_UserSignOut();
        //userSignOutPacket.Nickname = session.GetNickname();

        //server.Push(() =>
        //{
        //    server.Broadcast(session, userSignOutPacket.Write());
        //});

        // 로그아웃 처리
        S_SuccessSignOut successPacket = new S_SuccessSignOut();
        successPacket.Message = "로그아웃 되었습니다.";
        session.Send(successPacket.Write());

        Console.WriteLine($"LOG - {session.GetNickname()}({session.GetUID()}) was Sign Out.");

        // 클라이언트 세션을 '채팅방'에서 제거한다.
        server.Push(() => { server.Leave(session.GetUID());});
        session.ChatServer = null;
    }

    internal static void C_RequestEnterServerHandler(PacketSession arg1, IPacket arg2)
    {
	    //var packet = arg2 as C_RequestEnterServer;
        // C_EnterServer - long(serverID), long(uid)
        // C_FindServer - long(serverID)
        // C_JoinServer - long(serverID), long(uid)
        // C_LeaveServer - long(serverID), long(uid)
        // C_CreateServer - string(name), long(Uid)
        // C_DeleteServer - string(name), long(uid)
        // C_RequestUserList - no content
        // C_RequestServerList - no content

        // S_FindServerSuccess - string(name), string(ownerNickname), int(memberCount) : 성공하면 서버 정보 전송
        // S_FindServerFail - string(message) : 실패 메세지
        // S_CreateServerFail - string(message)
        // S_SendUserList - list<string>(nickname), list<accountType>(accountType), list<long>(uid) : 클라의 접속자 리스트에 유저 추가(혹은 초기화)
        // S_SendServerList - List<string>(name), List<long>(serverid) : 클라의 서버 리스트에 서버 추가(혹은 초기화)
        // S_Announce - string(message) : 서버에서 브로드캐스트하는 메세지
        // S_RemoveUserAtList - long(uid) : 클라의 접속자 리스트에서 유저 제거
        // S_RemoveServerAtList - long(serverid) : 클라의 서버 리스트에서 서버 제거

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

    internal static void C_HeartBeatRequestHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_EnterServerHandler(PacketSession arg1, IPacket arg2)
    {

        // 서버에서 유저가 접속하려는 서버에 대해서 처리합니다.

        //// 구버전 코드
        //Program.Server.Push(() =>
        //{
        // Program.Server.Enter(session.GetUID(), session);
        //});

        // 서버에 접속한 클라에게 현재 접속중인 유저 리스트를 전달합니다.

        //// 구 버전 코드
        //S_SendServerList userListPacket = new S_CurrentUserList();
        //userListPacket.userLists = Program.Server.GetNicknameList();
        //if (userListPacket.userLists.Count > 0)
        //{
        // session.Send(userListPacket.Write());
        //}


        // 서버에 접속한 유저의 닉네임을 현재 접속중인 유저들에게 알립니다.


        // Server server = session.ChatServer;
        // server.Push(() => {server.Broadcast(session, userSignInPacket.Write()); });
        // Console.WriteLine($"LOG - {session.GetNickname()}({session.GetUID()}) was Sign In.");



        // 유저 입장 메세지를 브로드 캐스트합니다.

        //// 구버전 코드
        //S_SendChat signInMessage = new S_SendChat();
        //signInMessage.Nickname = "Server";
        //signInMessage.Message = $"{session.GetNickname()}님이 입장했습니다.";
        //server.Push(() => { server.Broadcast(session, signInMessage.Write());});
    }

    internal static void C_FindServerHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_JoinServerHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_LeaveServerHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_CreateServerHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_DeleteServerHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }

    internal static void C_RequestUserListHandler(PacketSession arg1, IPacket arg2)
    {
	    throw new NotImplementedException();
    }
}