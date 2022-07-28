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
	    public Connector Connector = null;

	    public void ConnectServer()
	    {
			MainWindow mainWindow = MainWindow as MainWindow;
			mainWindow.StateBlock.Text = "서버에 연결 중입니다.";
			mainWindow.StateBlock.Foreground = Brushes.Orange;

			var endPoint = new IPEndPoint(Configuration.PrivateIP, Configuration.Port);
			Connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); });
	    }

		public void StartupApp(object sender, StartupEventArgs eventeArgs)
	    {
		    MainWindow = new MainWindow();
			MainWindow.Show();
			if (!Configuration.Load())
			{
				return;
			}
			
			Connector = new Connector();

			ConnectServer();
	    }

    }
}