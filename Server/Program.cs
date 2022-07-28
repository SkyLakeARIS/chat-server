using System.Net;
using Core;
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

        if (!Configuration.Load())
        {
	        Console.WriteLine("fail to load config information");
	        return;
        }

        IPAddress ip = IPAddress.Parse(Configuration.PrivateIP);
        IPEndPoint endPoint = new IPEndPoint(ip, Configuration.Port);


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