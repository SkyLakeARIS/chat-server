using System;
using System.Net;
using Core;
using Core.Packet;

namespace Client.Network
{
    public class ChatSession : PacketSession
    {
        public static ChatSession Instance { get; private set; }
        public ChatSession()
        {
            Instance = this;
        }

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
