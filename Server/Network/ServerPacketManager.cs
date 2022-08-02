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
		_onRecv.Add((ushort)PacketID.C_HeartBeatRequest, MakePacket<C_HeartBeatRequest>);
		_handler.Add((ushort)PacketID.C_HeartBeatRequest, ChatHandler.C_HeartBeatRequestHandler);
		_onRecv.Add((ushort)PacketID.C_SendChat, MakePacket<C_SendChat>);
		_handler.Add((ushort)PacketID.C_SendChat, ChatHandler.C_SendChatHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignIn, MakePacket<C_RequestSignIn>);
		_handler.Add((ushort)PacketID.C_RequestSignIn, ChatHandler.C_RequestSignInHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignUp, MakePacket<C_RequestSignUp>);
		_handler.Add((ushort)PacketID.C_RequestSignUp, ChatHandler.C_RequestSignUpHandler);
		_onRecv.Add((ushort)PacketID.C_RequestSignOut, MakePacket<C_RequestSignOut>);
		_handler.Add((ushort)PacketID.C_RequestSignOut, ChatHandler.C_RequestSignOutHandler);
		_onRecv.Add((ushort)PacketID.C_EnterServer, MakePacket<C_EnterServer>);
		_handler.Add((ushort)PacketID.C_EnterServer, ChatHandler.C_EnterServerHandler);
		_onRecv.Add((ushort)PacketID.C_FindServer, MakePacket<C_FindServer>);
		_handler.Add((ushort)PacketID.C_FindServer, ChatHandler.C_FindServerHandler);
		_onRecv.Add((ushort)PacketID.C_JoinServer, MakePacket<C_JoinServer>);
		_handler.Add((ushort)PacketID.C_JoinServer, ChatHandler.C_JoinServerHandler);
		_onRecv.Add((ushort)PacketID.C_LeaveServer, MakePacket<C_LeaveServer>);
		_handler.Add((ushort)PacketID.C_LeaveServer, ChatHandler.C_LeaveServerHandler);
		_onRecv.Add((ushort)PacketID.C_CreateServer, MakePacket<C_CreateServer>);
		_handler.Add((ushort)PacketID.C_CreateServer, ChatHandler.C_CreateServerHandler);
		_onRecv.Add((ushort)PacketID.C_DeleteServer, MakePacket<C_DeleteServer>);
		_handler.Add((ushort)PacketID.C_DeleteServer, ChatHandler.C_DeleteServerHandler);
		_onRecv.Add((ushort)PacketID.C_RequestCommand, MakePacket<C_RequestCommand>);
		_handler.Add((ushort)PacketID.C_RequestCommand, ChatHandler.C_RequestCommandHandler);
		_onRecv.Add((ushort)PacketID.C_RequestUserList, MakePacket<C_RequestUserList>);
		_handler.Add((ushort)PacketID.C_RequestUserList, ChatHandler.C_RequestUserListHandler);
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
		//// dictionary에 있는 MakePacket 함수를 호출.
		if (_onRecv.TryGetValue(id, out action))
		{
			action.Invoke(session, buffer);
		}
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		//// 클라에서 보낸 패킷 타입에 맞게 받아서
		//// 역직렬화 시킨 후에 패킷 타입에 맞는 함수를 콜백함.
		//// 애초에 콜백을 하려면 여기에서 패킷을 역직렬화 할 수 밖에 없음.
		T pkt = new T();
		pkt.Read(buffer);
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(pkt.Protocol, out action))
		{
			// 핸들러 함수 호출.
			action.Invoke(session, pkt);
		}
	}
}