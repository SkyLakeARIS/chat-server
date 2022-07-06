using System;
using System.Net;
using Core;
using Core.Data;
using Core.Packet;

namespace Client.Network
{
    public class ChatSession : PacketSession
    {
        public static ChatSession Instance { get; private set; }
        public string _NickName;
        public string _UserID;
        public long _UID; // 추후 사용 
        // accountType은 추후에 추가

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
