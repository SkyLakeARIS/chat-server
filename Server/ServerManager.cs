namespace Server;

public class ServerManager
{
	private ServerManager _serverManager = new ServerManager();
	private List<Server> _serverList;

	public ServerManager instance
	{
		get
		{
			return _serverManager;
		}
	}

	public Server GanerateServer()
	{
		return null;
	}

	public Server FindServer(long serverID)
	{
		foreach (var server in _serverList)
		{
			if (server.GetServerID() == serverID)
			{
				return server;
			}
		}

		return null;
	}

	public void RemoveServer(Server server)
	{
		_serverList.Remove(server);
	}

	public void AddServer(Server server)
	{
		_serverList.Add(server);
	}


}