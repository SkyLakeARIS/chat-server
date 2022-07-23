﻿namespace Server.Network;


public static class Configuration
{
	private static string Path = "config.txt";
	public static string PublicIP = "not init";
	public static string PrivateIP = "not init";
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
				PublicIP = info[1];
			}
			else if (info[0].Equals("PrivateIP"))
			{
				PrivateIP = info[1];

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