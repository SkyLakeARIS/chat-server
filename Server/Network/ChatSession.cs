using System.Net;
using Core;
using Core.Data;

namespace Server.Network;


public class ChatSession : PacketSession
{
    /*--------------------
     세션이 알고 있어야 할 유저 정보들
   username(id), nickname, accountType 
 
    0 유저 1 호스트 2 관리자
  --------------------*/
    public string UserName { get; set; } = "qwer";
    public string NickName { get; set; }
    public AccountPermissions Permissions { get; set; }

    public int SessionId { get; set; }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"{numOfBytes}바이트 전송함.");
    }

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"{endPoint}가 접속함.");
        Program.Server.Enter(this);
        NickName = SessionId.ToString();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.instance.Remove(this);
        Program.Server.Leave(this);
        Console.WriteLine($"{endPoint}가 접속 해제함.");
    }
}