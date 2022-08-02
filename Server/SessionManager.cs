using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;
using Server.Network;

namespace Server;

public class SessionManager
{
    private static SessionManager _session = new SessionManager();
    public static SessionManager Instance
    {
        get { return _session; }
    }

    private long _sessionId = 0;
    private List<ChatSession> _sessionList = new List<ChatSession>();   // 세션 리스트
    private Dictionary<long, ChatSession> _userList = new Dictionary<long, ChatSession>(); // 로그인한 유저 리스트
    private object _lock = new object();

    public ChatSession Generate()
    {
        lock (_lock)
        {
            ChatSession session = new ChatSession();
            _sessionList.Add(session);

            Console.WriteLine($"Connected : {session}");
            return session;
        }
    }

    public ChatSession FindOrNull(long uid)
    {
        lock (_lock)
        {
            foreach (var session in _sessionList)
            {
	            if (session.GetUID() == uid)
	            {
		            return session;
	            }
            }
            return null;
        }
    }

    public void Remove(ChatSession session)
    {
        lock (_lock)
        {
	        _sessionList.Remove(session);
        }
    }

}