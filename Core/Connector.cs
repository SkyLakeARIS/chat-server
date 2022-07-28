using System.Net;
using System.Net.Sockets;


namespace Core
{
    public class Connector
    {
	    // 리스너와 유사하게
        private Func<Session> _sessionFactory;
        private SocketAsyncEventArgs args = new SocketAsyncEventArgs();

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
	        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // keepalive로 하부단에서 주기적으로 통신하여 상대가 응답을 하는지를 체크한다.
            int size = sizeof(UInt32);
            UInt32 on = 1;
            UInt32 keepAliveInterval = 10000;
            UInt32 retryInterval = 1000;
            byte[] inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);

            socket.IOControl(IOControlCode.KeepAliveValues, inArray, null);

            _sessionFactory = sessionFactory;

            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;

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
    }
}
