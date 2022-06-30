// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using DummyClient;
using DummyClient.Network;
using Core;
using Core.Packet;


internal static class Program
{
    public static void FirstMenu()
    {
        Console.WriteLine("1. 로그인 ");
        Console.WriteLine("2. 회원 가입 ");

        int select = 0;
        if(int.TryParse(Console.ReadLine(), out select))
        {
            if(select == 1)
            {
                SignIn();
            }
            else if(select == 2)
            {
            }
        }

    }
    public static void SignIn()
    {
        do
        {

        }
        while (false);


        Console.WriteLine("ID : ");
        string id = Console.ReadLine();
        Console.WriteLine("PW : ");
        string pw = Console.ReadLine();

    }
    public static void Menu()
    {
        Console.WriteLine("1. 서버 접속");
        Console.WriteLine("2. 서버 검색");
        Console.WriteLine("3. 로그아웃");

    }

    /*--------------------
             Client
     --------------------*/
    private static void Main(string[] args)
    {
        // 패킷 매니저 초기화.
        // 패킷들의 딕셔너리 초기화 및 델리게이트 연결.
       // PacketManager.Instance.Register();

        var host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ip = ipHost.AddressList[0];
        // 목적지
        var endPoint = new IPEndPoint(ip, 9999);

        Connector connector = new Connector();
        connector.Connect(endPoint, ()=> { return SessionManager.Instance.Generate(); }, 500);
        

        int x = 0;
        int y = 0;
        (x, y) = Console.GetCursorPosition();
        Tuple<int, int> chatPOS = new Tuple<int, int>(0, 20);
        Console.SetCursorPosition(0, chatPOS.Item2);

        //ChatSession.Instance.x = x;
        //ChatSession.Instance.y = y;

        /*
              Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
            var message = Console.ReadLine();
            Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
            Console.WriteLine("                                          ");

            if (!string.IsNullOrEmpty(message) && ChatSession.Instance != null)
            {
                C_SendChat packet = new C_SendChat() {Message= message};
                ChatSession.Instance.Send(packet.Write());
            }
       
         */
        bool isSignIn = false;
        while (true)
        {
            if(isSignIn)
            {

            }
            // dummy mode
            try
            {
                SessionManager.Instance.DummyChatForeach();

            }
            catch(Exception e)
            {
                Console.WriteLine($"dummy mode / fail send : {e}");
            }

            // 250인 이유는 초당 4번을 보내도록 하기 위해서.
            // 예로 이동패킷 같은 경우는 초에 4번 보내도록 되어있기도 하므로 따라함.
            Thread.Sleep(250);
        }
    }
}