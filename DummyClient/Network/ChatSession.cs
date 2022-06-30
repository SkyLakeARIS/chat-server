using System.Net;
using Core;
using Core.Packet;

namespace DummyClient.Network
{
    public class ChatSession : PacketSession
    {
        public static ChatSession Instance { get; private set; }
        public ChatSession()
        {
            Instance = this;
        }
        // 임시로 커서 위치 분리 변수
        public int x { get;  set; }
        public int y { get;  set; }



        public override void OnConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"Connected {endPoint}");
        }

        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);

        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"보낸 바이트: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Disconnected {endPoint}");
        }
    }
}
