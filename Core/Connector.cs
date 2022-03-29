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
        private Func<Session> _sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            var args = new SocketAsyncEventArgs();

            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            // 일종의 식별자
            args.UserToken = socket;

            // 비동기 루프 시작
            RegisterConnect(args);
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
