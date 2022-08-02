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
	    private Connector _connector = null;

	    public void ConnectServer()
	    {
			MainWindow mainWindow = MainWindow as MainWindow;
			mainWindow.StateBlock.Text = "서버에 연결 중입니다.";
			mainWindow.StateBlock.Foreground = Brushes.Orange;

			var endPoint = new IPEndPoint(Configuration.PrivateIP, Configuration.Port);
			_connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); });
	    }

		public void StartupApp(object sender, StartupEventArgs eventeArgs)
	    {
		    MainWindow = new MainWindow();
			MainWindow.Show();
			if (!Configuration.Load())
			{
				return;
			}
			
			_connector = new Connector();

			ConnectServer();
	    }

    }
}