using System.Net;
using Client.Net.Handler;
using Core;
using Core.Packet;

namespace Client.Net
{
    public class ChatSession : Session
    {
        public static ChatSession Instance { get; private set; }

        public ChatSession()
        {
            Instance = this;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Connected {endPoint}");
        }

        public override int OnReceived(ArraySegment<byte> buffer)
        {
            var inPacket = new InPacket(buffer);
            var id = (ServerPacket)inPacket.DecodeInt();

            switch (id)
            {
                case ServerPacket.Chat:
                    ChatHandler.OnUserChat(this, inPacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
}
