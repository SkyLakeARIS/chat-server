using System.IO;
using System.Net;

namespace Client.Network;

public static class Configuration
{
	private static string Path = "config.txt";

	public static IPAddress PublicIP = null;
	public static IPAddress PrivateIP = null;
	public static int Port;
	public static bool Load()
	{
		StreamReader config = File.OpenText(Path);
		if (config == null)
		{
			// log - cannot open file
			return false;
		}

		while (!config.EndOfStream)
		{
			string[] info = config.ReadLine().Split(":");
			if (info.Length > 2)
			{
				// log - invalid information (wrong format)
				config.Close();
				return false;
			}

			if (info[0].Equals("PublicIP"))
			{
				PublicIP = IPAddress.Parse(info[1]);
			}
			else if (info[0].Equals("PrivateIP"))
			{
				PrivateIP = IPAddress.Parse(info[1]);
			}
			else if (info[0].Equals("Port"))
			{
				Port = int.Parse(info[1]);
			}
			else
			{
				config.Close();
				// log - invalid information (wrong word or wrong format)
				return false;
			}
		}

		config.Close();
		return true;
	}
}