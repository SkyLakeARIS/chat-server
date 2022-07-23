using System.Net;
using System.Windows;
using System.Windows.Media;
using Client.Network;
using Core;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
	    public Connector connector;
	    public void ConnectServer()
	    {
		    if (!Configuration.Load())
		    {
			    return;
		    }

		    IPAddress ip = IPAddress.Parse(Configuration.PublicIP);
			// "106.241.146.247"
			//    192.168.0.15
			// 목적지    18017
			var endPoint = new IPEndPoint(ip, Configuration.Port);

		    connector = new Connector();
		    connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); });
	    }

	    public void StartupApp(object sender, StartupEventArgs eventeArgs)
	    {
			ConnectServer();
			MainWindow = new MainWindow();
			MainWindow.Show();
	    }
    }
}

/*
            else if (args.SocketError == SocketError.NotConnected)
            {
	            _ConnectFailCount++;
	            // Connect 메서드 바깥에서 연결 성공 여부를 체크하고 하도록.
	            // 클라가 서버와 같은 네트워크에 존재할 때
	            if (_ConnectFailCount > 50)
	            {
		            IPAddress internalIP = IPAddress.Parse("192.168.0.15");
		            // 목적지   18017
		            IPEndPoint endPoint = new IPEndPoint(internalIP, 8080);
		            args.UserToken = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		            args.RemoteEndPoint = endPoint;
		            //args.Completed += OnConnectCompleted;
	            }
		          RegisterConnect(args);

 */