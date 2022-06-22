using System.Net;
using System.Text;
using Core;
using Core.Data;
using Core.Packet;

namespace Server.Network;


public class ChatSession : PacketSession
{
    /*--------------------
     세션이 알고 있어야 할 유저 정보들
   username(id), nickname, accountType 
 
    0 유저 1 호스트 2 관리자
  --------------------*/
    public long UserID { get; set; }
    public string UserName { get; set; } = "qwer";
    public string? NickName { get; set; }
    public AccountPermissions Permissions { get; set; }

    public Server chatServer = new Server();

    public int SessionId { get; set; }

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
        NickName = SessionId.ToString();
        Server server = chatServer;
        // server.Push(() => server.Enter(this));
        Program.Server.Push(() => Program.Server.Enter(this));

        //Program.Server.Enter(this);

    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.instance.Remove(this);
        if(chatServer != null)
        {
            // 이렇게 람다를 사용하면 해당 개체가 사라지면 에러다. Program.Server.Push(() => { Program.Server.Leave(this); })
            // 따라서 아래처럼 server를 추출한 뒤에 사용한다. 
            // 이유는 onDisconnect는 접속이 끊기면 바로 실행되는데, JobQueue는 상대적으로 미래에 실행되기 때문이다.
            // 즉, onDisconnect에서 server(채팅방)을 null로 밀어버려서 나중에 action이 실행될 때 null참조가 된다.
            Server server = chatServer;
            server.Push(() => server.Leave(this) );
            chatServer = null;
        }
   
        Console.WriteLine($"{endPoint}가 접속 해제함.");
    }
}