using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Core
{
    public class Connector
    {

        // 리스너와 유사하게
        private Func<Session> _sessionFactory;
        private SocketAsyncEventArgs args;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count =1)
        {

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // recv, send 타임 아웃 설정하기
            //socket.ReceiveTimeout = 5000;
            //socket.SendTimeout = 5000;

            // keepalive로 하부단에서 주기적으로 통신하여 상대가 응답을 하는지를 체크한다.
            int size = sizeof(UInt32);
            UInt32 on = 1;
            UInt32 keepAliveInterval = 10000;   // Send a packet once every 10 seconds.
            UInt32 retryInterval = 1000;        // If no response, resend every second.
            byte[] inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);

            socket.IOControl(IOControlCode.KeepAliveValues, inArray, null);

            _sessionFactory = sessionFactory;
            args = new SocketAsyncEventArgs();
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

        public void CancelConnect()
        {
            ((Socket)args.UserToken).Dispose();
        }
    }
}
