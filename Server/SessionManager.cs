using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;
using Server.Network;

namespace Server;

public class SessionManager
{
    private static SessionManager _session = new SessionManager();
    public static SessionManager instance
    {
        get { return _session; }
    }

    // 회원가입시 유저에게 UID를 부여하기 위한 UID 변수. 1부터 부여됨.(0은 UID 없음)
    private long _sessionId = 0;
    private List<ChatSession> _sessionList = new List<ChatSession>();
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

    public ChatSession Find(long id)
    {
        lock (_lock)
        {
            foreach (var session in _sessionList)
            {
	            if (session.GetUID() == id)
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

    public long GenarateUID()
    {
	    lock (_lock)
	    {
		    DatabaseManager instance = DatabaseManager.Instance;

	        var accountManager = instance.GetCollection<ManagementEntity>(DatabaseManager.AccountManagement);
		    var UIDGenerator = accountManager.Find(x => x.managerName.Equals(DatabaseManager.UIDGenerator)).ToList();

		    long uid = UIDGenerator[0].UID++;
		    //Builders<ManagementEntity>.Update.Set("UID", UIDGenerator);
		    UpdateResult result = accountManager.UpdateOne(x => x.managerName.Equals(DatabaseManager.UIDGenerator), Builders<ManagementEntity>.Update.Set(x=>x.UID, UIDGenerator[0].UID));
		    if (result.MatchedCount < 1)
		    {
			    Console.WriteLine("[LOG] GenarateUID : UID를 생성하는 과정에 문제가 발생했습니다. (DB - UID 업데이트 실패)");
		    }
	        return uid;
	    }

    }
}