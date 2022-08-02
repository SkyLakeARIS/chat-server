using System.Net;
using AccountType;
using Core;

namespace Server.Network;

public class ChatSession : PacketSession
{
    /*--------------------
     세션이 알고 있어야 할 유저 정보들
   username(id), nickname, accountType 
 
    0 유저 1 호스트 2 관리자
  --------------------*/

    // 수정이 끝나면 private으로 전환하기
    public Server ChatServer = null;

    private long _uid;

    private string _nickname;


    // pair <server id, accounttype>
    // serverlist - pair로 가능한가?
    // joinedServer
    private Dictionary<long, EAccountType> _serverInfo = new Dictionary<long, EAccountType>();

    private long _joinedServer;
    //private List<long> _joinedServerList = new List<long>();
    public long SessionId { get; set; }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine($"{numOfBytes}바이트 전송함.");
    }

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"{endPoint}가 접속했습니다.");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        // 프로그램을 강제 종료 해버릴 경우를 대비해서 일단 남겨놓음.
        // (그럴것이라고 생각하지만 100%까지는 확신이 안듦)
        if(ChatServer != null)
        {
            // 이렇게 람다를 사용하면 해당 개체가 사라지면 에러다. Program.Server.Push(() => { Program.Server.Leave(this); })
            // 따라서 아래처럼 server를 추출한 뒤에 사용한다. 
            // 이유는 onDisconnect는 접속이 끊기면 바로 실행되는데, JobQueue는 상대적으로 미래에 실행되기 때문이다.
            // 즉, onDisconnect에서 server(채팅방)을 null로 밀어버려서 나중에 action이 실행될 때 null참조가 된다.
            Server server = ChatServer;
            // 클라이언트가 프로그램을 강제로 종료할 때 접속자들에게 그 사실을 알립니다.
            S_SendChat signOutMessage = new S_SendChat();
            signOutMessage.Nickname = "Server";
            signOutMessage.Message = $"{_nickname}님이 퇴장했습니다.";
            server.Push(() => { server.Broadcast(this, signOutMessage.Write()); });

            //S_UserSignOut signOutPacket = new S_UserSignOut();
           // signOutPacket.Nickname = _nickname;
           // server.Push(() => {server.Broadcast(this, signOutPacket.Write());});
            server.Push(() => server.Leave(_uid) );
            ChatServer = null;
        }
   
        Console.WriteLine($"{endPoint}가 접속 해제함.");
    }

    public long GetUID()
    {
	    return _uid;
    }

    public void SetUID(long uid)
    {
	    _uid = uid;
    }

    public string GetNickname()
    {
	    return _nickname;
    }

    public void SetNickname(string nickname)
    {
	    _nickname = nickname;
    }

    public void JoinServer(long serverID)
    {
	    _serverInfo.Add(serverID, EAccountType.User);
    }

    public void LeaveServer(long serverID)
    {
	    _serverInfo.Add(serverID, EAccountType.User);
    }
    public List<long> GetJoinedServerList()
    {
        List<long> serverList = new List<long>();
	    foreach (var information in _serverInfo)
	    {
		    serverList.Add(information.Key);
	    }
        return serverList;
    }
}