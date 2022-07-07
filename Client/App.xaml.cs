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
		    var host = Dns.GetHostName();
		    var ipHost = Dns.GetHostEntry(host);
		    var ip = ipHost.AddressList[0];
		    // 목적지
		    var endPoint = new IPEndPoint(ip, 9999);

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
