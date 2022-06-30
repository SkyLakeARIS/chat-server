using Core;
using System;
using Server.Network;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance;
	public static PacketManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = new PacketManager();
			return _instance;
		}
	}
	#endregion

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

	PacketManager()
	{
		Register();
	}
	public void Register()
	{
		_onRecv.Add((ushort)PacketID.C_SendChat, MakePacket<C_SendChat>);
		_handler.Add((ushort)PacketID.C_SendChat, ChatHandler.C_SendChatHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignIn, MakePacket<C_RequestSignIn>);
		_handler.Add((ushort)PacketID.C_RequestSignIn, ChatHandler.C_RequestSignInHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignUp, MakePacket<C_RequestSignUp>);
		_handler.Add((ushort)PacketID.C_RequestSignUp, ChatHandler.C_RequestSignUpHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignOut, MakePacket<C_RequestSignOut>);
		_handler.Add((ushort)PacketID.C_RequestSignOut, ChatHandler.C_RequestSignOutHandler);
		_onRecv.Add((ushort)PacketID.C_RequestEnterServer, MakePacket<C_RequestEnterServer>);
		_handler.Add((ushort)PacketID.C_RequestEnterServer, ChatHandler.C_RequestEnterServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestLeaveServer, MakePacket<C_RequestLeaveServer>);
		_handler.Add((ushort)PacketID.C_RequestLeaveServer, ChatHandler.C_RequestLeaveServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestCreateServer, MakePacket<C_RequestCreateServer>);
		_handler.Add((ushort)PacketID.C_RequestCreateServer, ChatHandler.C_RequestCreateServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestDeleteServer, MakePacket<C_RequestDeleteServer>);
		_handler.Add((ushort)PacketID.C_RequestDeleteServer, ChatHandler.C_RequestDeleteServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestJoinServer, MakePacket<C_RequestJoinServer>);
		_handler.Add((ushort)PacketID.C_RequestJoinServer, ChatHandler.C_RequestJoinServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestCommand, MakePacket<C_RequestCommand>);
		_handler.Add((ushort)PacketID.C_RequestCommand, ChatHandler.C_RequestCommandHandler);
		_onRecv.Add((ushort)PacketID.C_RequestServerList, MakePacket<C_RequestServerList>);
		_handler.Add((ushort)PacketID.C_RequestServerList, ChatHandler.C_RequestServerListHandler);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(pkt.Protocol, out action))
			action.Invoke(session, pkt);
	}
}