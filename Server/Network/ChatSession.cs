using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using Core;
using Core.Packet;

namespace Server.Network;


public class ChatSession : PacketSession
{
    /*--------------------
     세션이 알고 있어야 할 유저 정보들
   username(id), nickname, accountType 
 
    0 유저 1 호스트 2 관리자
  --------------------*/
    private long _UID;

    private string _Nickname;

    // 수정이 끝나면 private으로 전환하기
    public Server chatServer = null;

    private List<long> joinedServerList = new List<long>();

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
        Console.WriteLine($"{endPoint}가 접속함.");

        // 임시로 채팅 서버에 강제로 입장.
        //_Nickname = SessionId.ToString();
        //Server server = chatServer;
        // server.Push(() => server.Enter(this));
        //chatServer = Program.Server;
        //Program.Server.Enter(this);

    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.instance.Remove(this);
        // 프로그램을 강제 종료 해버릴 경우를 대비해서 일단 남겨놓음.
        // (그럴것이라고 생각하지만 100%까지는 확신이 안듦)
        if(chatServer != null)
        {
            // 이렇게 람다를 사용하면 해당 개체가 사라지면 에러다. Program.Server.Push(() => { Program.Server.Leave(this); })
            // 따라서 아래처럼 server를 추출한 뒤에 사용한다. 
            // 이유는 onDisconnect는 접속이 끊기면 바로 실행되는데, JobQueue는 상대적으로 미래에 실행되기 때문이다.
            // 즉, onDisconnect에서 server(채팅방)을 null로 밀어버려서 나중에 action이 실행될 때 null참조가 된다.
            Server server = chatServer;
            // 클라이언트가 프로그램을 강제로 종료할 때 접속자들에게 그 사실을 알립니다.
            S_SendChat signOutMessage = new S_SendChat();
            signOutMessage.Nickname = "Server";
            signOutMessage.Message = $"{_Nickname}님이 퇴장했습니다.";
            server.Push(() => { server.Broadcast(this, signOutMessage.Write()); });

            S_UserSignOut signOutPacket = new S_UserSignOut();
            signOutPacket.Nickname = _Nickname;
            server.Push(() => {server.Broadcast(this, signOutPacket.Write());});
            server.Push(() => server.Leave(this) );
            chatServer = null;
        }
   
        Console.WriteLine($"{endPoint}가 접속 해제함.");
    }

    public long GetUID()
    {
	    return _UID;
    }
    public void SetUID(long uid)
    {
	    _UID = uid;
    }

    public string GetNickname()
    {
	    return _Nickname;
    }

    public void SetNickname(string nickname)
    {
	    _Nickname = nickname;
    }

    public void AddJoinedServer(long serverID)
    {
	    joinedServerList.Add(serverID);
    }

    public List<long> GetJoinedServerList()
    {
        return joinedServerList;
    }
}