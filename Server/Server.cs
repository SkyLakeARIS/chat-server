using Core.Packet;
using Server.Network;

namespace Server;

public class Server
{
    private readonly List<ChatSession> _sessions = new List<ChatSession>();
    private readonly object _lock = new object();

    public void Broadcast(ArraySegment<byte> msg)
    {
        lock (_lock)
        {
            foreach (var s in _sessions)
            {
                s.Send(msg);
            }
        }
    }
    public void Enter(ChatSession session)
    {
        lock (_lock)
        {
            _sessions.Add(session);
        }

    }

    public void Leave(ChatSession session)
    {
        lock (_lock)
        {
           _sessions.Remove(session);
        }
    }
}