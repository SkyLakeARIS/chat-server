using System.Net;
using Core;
using Core.Data;
using Core.Packet;
using Server.Network.Handler;

namespace Server.Network;

public class ChatSession : PacketSession
{
    /*--------------------
     세션이 알고 있어야 할 유저 정보들
   username(id), nickname, accountType 
 
    0 유저 1 관리자 2 매니져 3 4
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
        var inPacket = new InPacket(buffer);
        var id = (ClientPacket) inPacket.DecodeInt();

        switch (id)
        {
            case ClientPacket.Chat:
                ChatHandler.OnUserChat(this, inPacket);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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