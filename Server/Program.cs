using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;
using Server.Network;

namespace Server;

/*--------------------
         Server
 --------------------*/
public class Packet
{
    public ushort size;
    public ushort packetid;
}

public class GameSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"Connected {endPoint}");
        // string -> byte 변환
        //var sendBuffer = Encoding.UTF8.GetBytes("[server] Hello");

        //Packet packet = new Packet() { size = 4, packetid = 7 };

        // 패킷처럼 바이트로 변환하여 전송
        // 버퍼에 정보를 넣어서

        //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

        //byte[] buffer1 = BitConverter.GetBytes(packet.size);
        //byte[] buffer2 = BitConverter.GetBytes(packet.packetid);

        //Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
        //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
        //// 실제 사용한 버퍼양을 알려주고 닫음.
        //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

        //Send(sendBuff.Array);
        //Send(sendBuff);
        Thread.Sleep(5000);
        Disconnect();
    }

    //public override int OnReceived(ArraySegment<byte> buffer)
    //{
    //    // args에 Buffer(내용물), Offset이 다 들어있음.
    //    var text = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
    //    Console.WriteLine("[client]" + text);
    //    return buffer.Count;
    //}
    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        Console.WriteLine($"RecvPacketID {id}, size : {size}");
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

public static class Program
{
    private static Listener _Listener = new Listener();

    private static void Main(string[] args)
    {
        var instance = DatabaseManager.Instance;
        var collection = instance.GetCollection<AccountEntity>("accounts");
        // var account = new AccountEntity()
        // {
        //     Id = ObjectId.GenerateNewId(),
        //     UserName = "test",
        //     Password = "test",
        //     Email = "test@test.com",
        //     NickName = "TEST",
        //     AccountType = 0,
        //     RegisterDate = DateTime.Now
        // };
        // collection.InsertOne(account);

        // var account = collection.Find(x => x.UserName.Contains("t")).ToList();
        //
        // var newAccount = account[0] with {NickName = "tttt"};
        // var filter = Builders<AccountEntity>.Filter.Eq(x => x.Id, newAccount.Id);
        // collection.ReplaceOne(filter, newAccount);

        var host = Dns.GetHostName();
        // 도메인을 통해서 아이피를 가져와 주는 역할
        var ipHost = Dns.GetHostEntry(host);
        // 큰 서버의 경우 한 도메인에 여러 아이피를 가지고 있을 수 있음.
        var ip = ipHost.AddressList[0];
        // 목적지를 설정하는 부분, 최종 주소
        var endPoint = new IPEndPoint(ip, 9999);

        // 문지기 교육<
        //listenSocket.Bind(endPoint);
        //listenSocket.Listen(100);   // backlog == 대기열


        // 델리게이트에 추가하기 위해서 OnAcceptHandler()를 전달함.
        // 클라이언트가 접속하면 등록한 함수가 호출될 것임.
        _Listener.Init(endPoint, () => { return new ChatSession(); });

        Console.WriteLine("[server] Listening..");


        while (true) { }
    }
}