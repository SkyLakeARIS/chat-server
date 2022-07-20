using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
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
		    //var host = Dns.GetHostName();
			// 192.168.0.15 
			//var ipHost = Dns.GetHostEntry();
		    //var ip = ipHost.AddressList[0];
		    IPAddress ip = IPAddress.Parse("106.241.146.247");
			//    192.168.0.15
			// 목적지
			var endPoint = new IPEndPoint(ip, 18017);

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
