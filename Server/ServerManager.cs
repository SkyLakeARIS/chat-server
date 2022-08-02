using System.Reflection.Metadata.Ecma335;
using MongoDB.Driver;
using Server.Database;
using Server.Database.Entities;

namespace Server;

public class ServerManager
{
	private static ServerManager _serverManager = new ServerManager();
	// serverID, server object
	private Dictionary<long, Server> _serverList;

	public static readonly long MainServerID = 0;
	public static ServerManager Instance
	{
		get
		{
			return _serverManager;
		}
	}

	ServerManager()
	{
		_serverList = new Dictionary<long, Server>();
	}

	// 서버를 가동할 때 DB에서 만들어진 채팅 서버들을 로드합니다.
	public void initializeServer()
	{
		var chatServerDB = DatabaseManager.Instance.GetCollection<ChatServerEntity>(DatabaseManager.ChatServerCollection);
		//Builders<ChatServerEntity>.Filter.All();
		var serverEntities = chatServerDB.Find(Builders<ChatServerEntity>.Filter.Empty);
		foreach (var entity in serverEntities.ToList())
		{
			Server server = new Server(entity);
			_serverList.Add(server.GetServerID(), server);
			Console.WriteLine($"[LOG] found : id- {entity.serverID}, owner-{entity.ownerID}, name-{entity.serverName}");
		}
	}

	public Server GanerateServer(string serverName, long ownerID)
	{
		Server server = new Server(serverName, ownerID);
		_serverList.Add(server.GetServerID(), server);

		return null;
	}

	public Server FindServerOrNull(long serverID)
	{
		Server foundServer = null;
		_serverList.TryGetValue(serverID, out foundServer);

		return foundServer;
	}

	public void RemoveServer(long serverID)
	{
		_serverList.Remove(serverID);
	}

	public void AddServer(Server server)
	{
		//_serverList.Add(server);
	}

	public List<Server> GetServerList()
	{
		List<Server> serverList = new List<Server>();
		foreach (var server in _serverList)
		{
			serverList.Add(server.Value);
		}
		return serverList;
	}
}