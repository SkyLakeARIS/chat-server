// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using Client;
using Client.Network;
using Core;
using Core.Packet;


internal static class Program
{

		/*--------------------
			     Client
		 --------------------*/
    private static void Main(string[] args)
    {
        // 패킷 매니저 초기화.
        // 패킷들의 딕셔너리 초기화 및 델리게이트 연결.
        PacketManager.Instance.Register();

        var host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ip = ipHost.AddressList[0];
        // 목적지
        var endPoint = new IPEndPoint(ip, 9999);

        Connector connector = new Connector();
        connector.Connect(endPoint, ()=> { return SessionManager.Instance.Generate(); }, 500);




        /*
            var message = Console.ReadLine();

            if (!string.IsNullOrEmpty(message) && ChatSession.Instance != null)
            {
                C_SendChat packet = new C_SendChat() {Message= message};
                ChatSession.Instance.Send(packet.Write());
            }
       
         */
        while (true)
        {
            // dummy mode
            try
            {
                SessionManager.Instance.DummyChatForeach();

            }
            catch(Exception e)
            {
                Console.WriteLine($"dummy fail send : {e.ToString}");
            }

            // 250인 이유는 초당 4번을 보내도록 하기 위해서.
            // 예로 이동패킷 같은 경우는 초에 4번 보내도록 되어있기도 하므로 따라함.
            Thread.Sleep(250);
        }
    }
}