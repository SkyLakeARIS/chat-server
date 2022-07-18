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


public static class Program
{
		/*--------------------
			   Server
		--------------------*/
    private static Listener _Listener = new Listener();
    public static Server Server = new Server("main");

    // JobTimer를 제작하여 해당 함수를 주기적으로 호출할 것.
    static void FlushRoom()
    {
        // 메인 스레드가 패킷을 모아보내는 역할을 수행한다.
        Server.Push(() => Server.Flush());

        // 보통0.25초마다 수행하지만,
        // 컴퓨터의 사양에 따라 다르므로 테스트하며 적절히 조정.
        JobTimer.Instance.Push(FlushRoom, 100);
    }
    private static void Main(string[] args)
    {
        // 패킷 매니저 초기화.
        // 패킷들의 딕셔너리 초기화 및 델리게이트 연결.
       // PacketManager.Instance.Register();

        // 세팅은 되어 있으니 내일(06.24) 원격으로 db 켜보고 아래 명령어 대로 적용 해보기.
        // 그리고 db 테이블 설계하기
        // 패킷 핸들러에서 처리하는 방법은 종욱이 프로젝트 참고.
        //DatabaseManager instance = DatabaseManager.Instance;
        //IMongoCollection<AccountEntity> collection = instance.GetCollection<AccountEntity>("account");
        //Console.WriteLine("ID 입력: ");
        //string id = Console.ReadLine();
        //Console.WriteLine("pw 입력: ");
        //string pw = Console.ReadLine();
        //Console.WriteLine("nickname 입력: ");
        //string nickname = Console.ReadLine();

        ////var instance = DatabaseManager.Instance;
        ////var collection = instance.GetCollection<AccountEntity>("accounts");
        //AccountEntity account = new AccountEntity()
        //{
        //    Id = ObjectId.GenerateNewId(),
        //    ID = id,
        //    password = pw,
        //    nickName = nickname,
        //    accountType = EAccountType.User,
        //};
        //collection.InsertOne(account);
        //IFindFluent<AccountEntity, AccountEntity> find = collection.Find(x => x.ID.Contains(""));
        //var list = find.ToList();
        
        //foreach(AccountEntity user in list)
        //{
        //    Console.WriteLine($"uid: {{{user.Id}}}, id : {user.ID}, nickname : {user.nickName}, type : {user.accountType}");
        //}
        //Thread.Sleep(10000);
        //IFindFluent<AccountEntity, AccountEntity> account = collection.Find(){ "ID" : id};
        
        //IFindFluent<AccountEntity, Acc account = collection.Find(x => x.UserName.Contains("t")).ToList();
        //var newAccount = account[0] with { nickName = "tttt" };
        //var filter = Builders<AccountEntity>.Filter.Eq(x => x.Id, newAccount.Id);
        //collection.ReplaceOne(filter, newAccount);

        var host = Dns.GetHostName();
        // 도메인을 통해서 아이피를 가져와 주는 역할
        var ipHost = Dns.GetHostEntry(host);
        // 큰 서버의 경우 한 도메인에 여러 아이피를 가지고 있을 수 있음.
        var ip = ipHost.AddressList[0];
        // 목적지를 설정하는 부분, 최종 주소
        var endPoint = new IPEndPoint(ip, 9999);


        // 델리게이트에 추가하기 위해서 OnAcceptHandler()를 전달함.
        // 클라이언트가 접속하면 등록한 함수가 호출될 것임.
        _Listener.Init(endPoint, () => { return SessionManager.instance.Generate(); });

        Console.WriteLine("[server] Listening..");

        // 처음 한번 예약을 걸어준다. (queue에 push)
        FlushRoom();

        while (true)
        {
            // 직접 틱카운트를 관리하면 timer를 사용하는 개체만큼
            // 코드를 작성해야하는데, 이렇게 중앙에서 컨트롤 하는 방식으로
            // 작성하면 코드가 간편해진다.
            JobTimer.Instance.Flush();
        }
    }
}


// 코드 참고용으로 삭제 안함.
public class GameSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"Connected {endPoint}");

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
