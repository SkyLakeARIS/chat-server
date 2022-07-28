using Client.Network;
using System;
using System.Collections.Generic;

namespace Client
{
    public class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ChatSession> _sessions = new List<ChatSession>();
        object _lock = new object();

        public ChatSession Generate()
        {
            lock(_lock)
            {
                ChatSession session = new ChatSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void DummyChatForeach()
        {

            lock(_lock)
            {
                foreach(ChatSession s in _sessions)
                {
                    C_SendChat packet = new C_SendChat();
                    packet.Message = $"TestMessage     123456789          테스트 메세지 전송";
                    ArraySegment<byte> chatPacket = packet.Write();
                    s.Send(chatPacket);
                }
            }
        }
    }
}
