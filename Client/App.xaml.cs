using System;
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
	    public Connector Connector;
	    private static double ConnectionTimeLimit = 5000;
		// 서버 연결시에 제한시간을 담당하는 타이머입니다.
	    private System.Timers.Timer _ConnectionTimer = null;
	    private bool _TryPublicIP = false;
	    public void ConnectServer()
	    {
			// 한번 로드하도록 개선하기.
		    if (!Configuration.Load())
		    {
			    return;
		    }

		   MainWindow mainWindow = MainWindow as MainWindow;
		   mainWindow.StateBlock.Text = "서버에 연결 중입니다.";
		   mainWindow.StateBlock.Foreground = Brushes.Orange;

			// "106.241.146.247"
			//    192.168.0.15
			// 목적지    18017

			// "106.241.146.247"
			//    192.168.0.15
			// 목적지    18017
			Connector = new Connector();
		    _ConnectionTimer = new System.Timers.Timer(ConnectionTimeLimit);
			// 타이머가 종료되었을 때 외부/내부 아이디를 전환하며 재연결을 시도합니다.
			// 타이머는 OnConnected 가 호출되면 종료됩니다.
			_ConnectionTimer.Elapsed += Reconnect;
			if (_TryPublicIP)
			{
				var publicEndPoint = new IPEndPoint(Configuration.PublicIP, Configuration.Port);
				Connector.Connect(publicEndPoint, () => { return SessionManager.Instance.Generate(); });
				_TryPublicIP = false;
			}
			else
			{
				var privateEndPoint = new IPEndPoint(Configuration.PrivateIP, Configuration.Port);
				Connector.Connect(privateEndPoint, () => { return SessionManager.Instance.Generate(); });
				_TryPublicIP = true;
			}
		}

		public void StartupApp(object sender, StartupEventArgs eventeArgs)
	    {
		    MainWindow = new MainWindow();
			MainWindow.Show();
			ConnectServer();
	    }

		public void TimerStop()
		{
			_ConnectionTimer.Stop();
			_ConnectionTimer.Close();
		}

		private void Reconnect(object sender, EventArgs e)
		{
			_ConnectionTimer.Close();
			// 기존에 작업을 진행중인 소켓을 폐기합니다.
			Connector.CancelConnect();
			ConnectServer();
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