using System.Net;
using Core;
using Core.Packet;

namespace Server.Network;

public class ChatSession : PacketSession
{
    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"{numOfBytes}바이트 전송함.");
    }

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        //test - Console.WriteLine($"buffer= contents: {buffer.ToString()}, offset : { buffer.Offset}, count : {buffer.Count}");
        //var id = BitConverter.ToInt32(buffer.Array, buffer.Offset + 4);
        //test - Console.WriteLine($"buffer= contents: {buffer.ToString()}, offset : { buffer.Offset}, count : {buffer.Count}");

        // var data = new ArraySegment<byte>(buffer.Array, 8, buffer.Count - 4);
        var inPacket = new InPacket(buffer);
        //inPacket.DecodeInt();
        var id = inPacket.DecodeInt();
        switch (id)
        {
            case 0: //chat

                break;
            case 7:
                //var revMsg = BitConverter.ToString(new ArraySegment<byte>(buffer.Array, ));
                Console.WriteLine($"packet content: id:{id}, test: { inPacket.DecodeInt()}, msg: { inPacket.DecodeString()}");
                break;
        }
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"{endPoint}가 접속함.");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"{endPoint}가 접속 해제함.");
    }
}