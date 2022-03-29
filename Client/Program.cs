// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Packet;

namespace Client;


/*--------------------
	     Client
 --------------------*/
public class Packet
{
    public int packetid;
    public int test;
}


public class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"Connected {endPoint}");

        Packet packet = new Packet() { packetid = 7, test = 111};

        for (int i = 0; i < 5; i++)
        {

            var openSegment = SendBufferHelper.Open(4096);
            var outPacket = new OutPacket(openSegment);

            outPacket.EncodeInt(packet.packetid);
            outPacket.EncodeInt(packet.test);
            outPacket.EncodeString("Encode String test message");
            var sendBuff = outPacket.Close();

            // byte[] buffer1 = BitConverter.GetBytes(packet.size);
            // byte[] buffer2 = BitConverter.GetBytes(packet.packetid);
            // var buffer3 = Encoding.UTF8.GetBytes(packet.msg);
            //
            // Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            // Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            // Array.Copy(buffer3, 0, openSegment.Array, openSegment.Offset + buffer1.Length + buffer2.Length, buffer3.Length);
            // // 실제 사용한 버퍼양을 알려주고 닫음.
            // ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length + buffer3.Length);


            Send(sendBuff);

        }
    }

    public override int OnReceived(ArraySegment<byte> buffer)
    {
        // args에 Buffer(내용물), Offset이 다 들어있음.
        var text = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine("[Server]" + text);
        return buffer.Count;
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"보낸 바이트: {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"Disconnected {endPoint}");
    }
}
internal static class Program
{
    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ip = ipHost.AddressList[0];
        // 목적지
        var endPoint = new IPEndPoint(ip, 9999);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return new GameSession();});


        while (true)
        {
            try
            {


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // 재접속 딜레이 1초 지정
            Thread.Sleep(1000);
        }

    }
}