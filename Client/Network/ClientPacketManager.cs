using Core;
using System;
using Client.Network;
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
		_onRecv.Add((ushort)PacketID.S_SuccessSignIn, MakePacket<S_SuccessSignIn>);
		_handler.Add((ushort)PacketID.S_SuccessSignIn, ChatHandler.S_SuccessSignInHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignIn, MakePacket<S_FailSignIn>);
		_handler.Add((ushort)PacketID.S_FailSignIn, ChatHandler.S_FailSignInHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessSignUp, MakePacket<S_SuccessSignUp>);
		_handler.Add((ushort)PacketID.S_SuccessSignUp, ChatHandler.S_SuccessSignUpHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignUp, MakePacket<S_FailSignUp>);
		_handler.Add((ushort)PacketID.S_FailSignUp, ChatHandler.S_FailSignUpHandler);
		_onRecv.Add((ushort)PacketID.S_SucessSignOut, MakePacket<S_SucessSignOut>);
		_handler.Add((ushort)PacketID.S_SucessSignOut, ChatHandler.S_SucessSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignOut, MakePacket<S_FailSignOut>);
		_handler.Add((ushort)PacketID.S_FailSignOut, ChatHandler.S_FailSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessCommand, MakePacket<S_SuccessCommand>);
		_handler.Add((ushort)PacketID.S_SuccessCommand, ChatHandler.S_SuccessCommandHandler);
		_onRecv.Add((ushort)PacketID.S_FailCommand, MakePacket<S_FailCommand>);
		_handler.Add((ushort)PacketID.S_FailCommand, ChatHandler.S_FailCommandHandler);
		_onRecv.Add((ushort)PacketID.S_SendChat, MakePacket<S_SendChat>);
		_handler.Add((ushort)PacketID.S_SendChat, ChatHandler.S_SendChatHandler);
		_onRecv.Add((ushort)PacketID.S_ExitServer, MakePacket<S_ExitServer>);
		_handler.Add((ushort)PacketID.S_ExitServer, ChatHandler.S_ExitServerHandler);
		_onRecv.Add((ushort)PacketID.S_JoinServer, MakePacket<S_JoinServer>);
		_handler.Add((ushort)PacketID.S_JoinServer, ChatHandler.S_JoinServerHandler);

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