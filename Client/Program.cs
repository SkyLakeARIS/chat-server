// See https://aka.ms/new-console-template for more information

using System.Net;
using Client.Network;
using Core;


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
        connector.Connect(endPoint, () => new ChatSession(), 1);


        // 임시로 커서 위치 분리
        Console.WriteLine("닉네임 설정: ");
        var nickname = Console.ReadLine();
        Console.Clear();
        int x, y;
        (x, y) = Console.GetCursorPosition();
        Tuple<int, int> chatPOS = new Tuple<int, int>(nickname.Length+3, 20);
        Console.SetCursorPosition(0, chatPOS.Item2);
        Console.WriteLine($"{nickname} | ");

        ChatSession.Instance.x = x;
        ChatSession.Instance.y = y;

        while (true)
        {
            Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
            var message = Console.ReadLine();
            Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
            Console.WriteLine("                                          ");

            if (!string.IsNullOrEmpty(message) && ChatSession.Instance != null)
            {
                string sendMessage = $"{nickname}: {message}";

                C_SendChat packet = new C_SendChat() {Message= sendMessage};
                ChatSession.Instance.Send(packet.Write());
            }
        }
    }
}