using System.Net;
using System.Net.Sockets;


namespace Core;

/*--------------------
    Listener 538 035 486 
 --------------------*/

public class Listener
{
    private Socket _listenSocket;

    // 코어단에서는 어떤 세션이 어떻게 상속될지 모르기 때문에.
    private Func<Session> _sessionFactory;

    public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 500)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // main에서 람다로 세션을 생성하는 함수를 만들어 콜백으로 지정함.
        _sessionFactory += sessionFactory;

        _listenSocket.Bind(endPoint);

        // 거의 동시에 접속 가능한 수
        // QnA에 따르면 RegisterAccept를 한번하더라도 동시에 들어온게 있으면 그거도 처리되는 듯.

        _listenSocket.Listen(backlog);
 
        // 멀티스레드 환경에 대응하는 하나의 방법, 문지기 수를 늘린 셈.
        for(int i = 0; i < register; i++)
        {
            var args = new SocketAsyncEventArgs();
            // 델리게이트- 메서드 추가
            // RegisterAccept()에서 타이밍이 맞지 않았을때 접속시도가 들어오면
            // args에 등록한 메서드를 델리게이트가 호출해 줌
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

            // 최초 한번은 등록 시도
            RegisterAccept(args);
        }
    }

    private void RegisterAccept(SocketAsyncEventArgs args)
    {
        // 재사용전 초기화
        args.AcceptSocket = null;

        // 비동기 방식 Async 계열 함수들 (a 붙으면 반대되는 의미)
        // false == 타이밍 맞을 때, 안맞으면 true을 반환
        var pending = _listenSocket.AcceptAsync(args);

        if (!pending) // 타이밍 맞게 접속하면 
        {
            OnAcceptCompleted(null, args);
        }
    }

    private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        // SocketError에는 에러 메세지가 아닌 성공시의 메시지도 들어감.
        if (args.SocketError == SocketError.Success)
        {
            // args에 클라이언트 소켓 정보가 들어있음. (이벤트 발생시 데이터를 args가 가짐)
            // 동기 함수인 accept()에서 클라이언트 소켓을 반환해준 것과 동일한 동작.
            Session session = _sessionFactory.Invoke();
            session.Start(args.AcceptSocket);
            session.OnConnected(args.AcceptSocket.RemoteEndPoint);
        }
        else
        {
            Console.WriteLine(args.SocketError);
        }

        // 접속 시도한 클라이언트를 처리하고 다음 클라이언트를 받기
        // 위해서 다시 비동기 모드로 들어가서 다음 접속에 대한 사이클을 돈다.
        RegisterAccept(args);
    }

    public Socket Accept()
    {
        // 동기 방식 - 클라이언트가 접속을 하지 않으면 블록된다.
        // 클라이언트의 소켓을 리턴한다.
        return _listenSocket.Accept();
    }
}
