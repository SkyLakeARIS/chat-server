using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Connector
    {
        // 리스너와 유사하게
        private Func<Session> _sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count =1)
        {
            for (int i = 0; i < count; i++) // dummy mode
            {
                var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sessionFactory = sessionFactory;

                var args = new SocketAsyncEventArgs();

                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                // 일종의 식별자
                // 이렇게 하는것도 한가지 방법.
                // 1. socket을 멤버 변수로 2. 매개변수로 넘기기 3. args의 UserToken이용
                // 1번의 경우 한번만 하는것이 아니라 여러개의 소켓을 연결해주므로 3번을 사용한다고 함.
                args.UserToken = socket;

                // 비동기 루프 시작
                RegisterConnect(args);
            }

        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            var socket = args.UserToken as Socket;
            if (socket == null)
            {
                return;
            }
            var pending = socket.ConnectAsync(args);
            if (!pending)
            {
                OnConnectCompleted(null, args);
            }
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                // 다시한번, 세션은 클라이언트에 대한 모든 정보를 가지고 있는 클래스
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
                // Register 를 다시 호출하지 않는 이유는
                // 클라이언트가 접속할때 쓰는거라서 
                // 굳이 session이나 listener처럼 비동기 루프를 할 이유가 없다.

            }
            else
            {
                Console.WriteLine($"OnConnectCompleted error : {args.SocketError}");
            }
        }

    }
}
