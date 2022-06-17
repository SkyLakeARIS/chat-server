using Server.Network;

namespace Server;

public class SessionManager
{
    private static SessionManager _session = new SessionManager();
    public static SessionManager instance
    {
        get { return _session; }
    }

    private int _sessionId = 0;
    private Dictionary<int, ChatSession> _sessions = new Dictionary<int, ChatSession>();
    private object _lock = new object();

    public ChatSession Generate()
    {
        lock (_lock)
        {
            int sessionId = ++_sessionId;
            ChatSession session = new ChatSession();
            session.SessionId = sessionId;
            _sessions.Add(sessionId, session);

            Console.WriteLine($"Connected : {sessionId}");
            return session;
        }
    }

    public ChatSession Find(int id)
    {
        lock (_lock)
        {
            ChatSession session = null;
            _sessions.TryGetValue(id, out session);
            return session;
        }
    }

    public void Remove(ChatSession session)
    {
        lock (_lock)
        {
            _sessions.Remove(session.SessionId);
        }
    }
}