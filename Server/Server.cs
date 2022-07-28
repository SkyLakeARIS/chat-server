using Core;
using Server.Network;

namespace Server;


// 채팅 서버도 여러개가 될 수 있으므로 세션매니저 처럼 이에 대한 매니저 클래스를 둬서
// 채팅방을 관리하도록 할 수 있다.
public class Server : IJobQueue
{
    private readonly List<ChatSession> _sessionList = new List<ChatSession>();
    private string _chatServerName;
    private long _serverID;
    JobQueue _jobQueue = new JobQueue();
    // jobQueue를 사용하여 직접 호출이 아니라 델리게이트를 push하는 방식으로 변경. 즉, 함수 호출이라는 액션.
    // 주문서와 비슷한 역할을 한다.
    // JobQueue에서 스레드 세이프하게(lock) 동작하므로 Server에서는 lock이 필요 없다.
    // JobQueue의 경우, 프로젝트마다 위치가 조금 달라질 수 있는데, 오픈되어있는 맵이 아닌 로딩하여 다음 맵으로 넘어가는 방식의 경우에는 
    // 현재 방식처럼( 채팅방하나) job queue를 각자 가지고 있는 방식.
    // 그렇지 않고 mmo나 오픈월드(심리스) 같은 경우는 모든 개체가 잡큐를 가지고 있기도 한다고 함.(무기, 스킬, 유저, 등등)
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private List<long> _userList = new List<long>();

    public Server(string serverName)
    {
        _chatServerName = serverName;
    }

    // 패킷 모아보내기
    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Broadcast(ChatSession session, string message)
    {
        // 핸들러에서 넘겨받은 정보를 통해 패킷을 만들어서 같은 채팅방의 클라들에게
        // 패킷을 브로드캐스트한다.
        // 하지만 이렇게 N명이 N명에게(자신 포함) 패킷을 브로드캐스트 하는 것은
        // 당연히 부하가 심할 수 밖에 없는데, 이를 완화하는 방법 중 하나가 패킷 모아보내기.(send처럼)
        // 이런 방식은 바로바로 브로드캐스트를 하지 않고 조금 모았다가 한번에 보내는 방식. (잡큐와 같이 매우 중요한 개념 중 하나.)

        S_SendChat respondPacket = new S_SendChat();
        respondPacket.Nickname = session.GetNickname();
        respondPacket.Message = message;
        ArraySegment<byte> segment = respondPacket.Write();
  
        // 락은 할 필요가 없음. (앞에서 스레드 하나만 실행되도록 동작하기 때문)
        // send와 같은 원리로, list에 밀어주는 부분과
        // 실제로 전송하는 부분을 분리.
        // 전송하는 역할은 main에서 메인스레드가, 리스트에 삽입은 서브스레드들이.
        _pendingList.Add(segment);

    }

    public void Broadcast(ChatSession session, ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }

    public void Flush()
    {
        foreach (ChatSession s in _sessionList)
        {
            s.Send(_pendingList);
        }
        //if(_pendingList.Count > 0)
        //{
        //   Console.WriteLine($"Flushed : {_pendingList.Count}");

        //}
        _pendingList.Clear();
    }

    public void Enter(ChatSession session)
    {

	    _sessionList.Add(session);
        session.chatServer = this;
    }

    public void Leave(ChatSession session)
    {
	    _sessionList.Remove(session);
    }

    public ChatSession Find(long id)
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

    public string GetServerName()
    {
	    return _chatServerName;
    }

    public void ChangeServerName(string newName)
    {
        _chatServerName = newName;
    }

    public long GetServerID()
    {
	    return _serverID;
    }

    public void SetServerID(long serverID)
    {
        _serverID = serverID;
    }

    public List<S_CurrentUserList.UserList> GetNicknameList()
    {
        List<S_CurrentUserList.UserList> nicknameList = new List<S_CurrentUserList.UserList>();
	    foreach (var user in _sessionList)
	    {
		    S_CurrentUserList.UserList nickname = new S_CurrentUserList.UserList();
		    nickname.Nickname = user.GetNickname();

		    nicknameList.Add(nickname);
	    }
        return nicknameList;
    }
}