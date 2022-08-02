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
		_onRecv.Add((ushort)PacketID.S_HeartBeatRespond, MakePacket<S_HeartBeatRespond>);
		_handler.Add((ushort)PacketID.S_HeartBeatRespond, ChatHandler.S_HeartBeatRespondHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessSignIn, MakePacket<S_SuccessSignIn>);
		_handler.Add((ushort)PacketID.S_SuccessSignIn, ChatHandler.S_SuccessSignInHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignIn, MakePacket<S_FailSignIn>);
		_handler.Add((ushort)PacketID.S_FailSignIn, ChatHandler.S_FailSignInHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessSignUp, MakePacket<S_SuccessSignUp>);
		_handler.Add((ushort)PacketID.S_SuccessSignUp, ChatHandler.S_SuccessSignUpHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignUp, MakePacket<S_FailSignUp>);
		_handler.Add((ushort)PacketID.S_FailSignUp, ChatHandler.S_FailSignUpHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessSignOut, MakePacket<S_SuccessSignOut>);
		_handler.Add((ushort)PacketID.S_SuccessSignOut, ChatHandler.S_SuccessSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignOut, MakePacket<S_FailSignOut>);
		_handler.Add((ushort)PacketID.S_FailSignOut, ChatHandler.S_FailSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessCommand, MakePacket<S_SuccessCommand>);
		_handler.Add((ushort)PacketID.S_SuccessCommand, ChatHandler.S_SuccessCommandHandler);
		_onRecv.Add((ushort)PacketID.S_FailCommand, MakePacket<S_FailCommand>);
		_handler.Add((ushort)PacketID.S_FailCommand, ChatHandler.S_FailCommandHandler);
		_onRecv.Add((ushort)PacketID.S_SendChat, MakePacket<S_SendChat>);
		_handler.Add((ushort)PacketID.S_SendChat, ChatHandler.S_SendChatHandler);
		_onRecv.Add((ushort)PacketID.S_EnterServerSuccess, MakePacket<S_EnterServerSuccess>);
		_handler.Add((ushort)PacketID.S_EnterServerSuccess, ChatHandler.S_EnterServerSuccessHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessFindServer, MakePacket<S_SuccessFindServer>);
		_handler.Add((ushort)PacketID.S_SuccessFindServer, ChatHandler.S_SuccessFindServerHandler);
		_onRecv.Add((ushort)PacketID.S_FailFindServer, MakePacket<S_FailFindServer>);
		_handler.Add((ushort)PacketID.S_FailFindServer, ChatHandler.S_FailFindServerHandler);
		_onRecv.Add((ushort)PacketID.S_SuccessCreateServer, MakePacket<S_SuccessCreateServer>);
		_handler.Add((ushort)PacketID.S_SuccessCreateServer, ChatHandler.S_SuccessCreateServerHandler);
		_onRecv.Add((ushort)PacketID.S_FailCreateServer, MakePacket<S_FailCreateServer>);
		_handler.Add((ushort)PacketID.S_FailCreateServer, ChatHandler.S_FailCreateServerHandler);
		_onRecv.Add((ushort)PacketID.S_SendUserList, MakePacket<S_SendUserList>);
		_handler.Add((ushort)PacketID.S_SendUserList, ChatHandler.S_SendUserListHandler);
		_onRecv.Add((ushort)PacketID.S_SendServerList, MakePacket<S_SendServerList>);
		_handler.Add((ushort)PacketID.S_SendServerList, ChatHandler.S_SendServerListHandler);
		_onRecv.Add((ushort)PacketID.S_Announce, MakePacket<S_Announce>);
		_handler.Add((ushort)PacketID.S_Announce, ChatHandler.S_AnnounceHandler);
		_onRecv.Add((ushort)PacketID.S_RemoveServerAtList, MakePacket<S_RemoveServerAtList>);
		_handler.Add((ushort)PacketID.S_RemoveServerAtList, ChatHandler.S_RemoveServerAtListHandler);
		_onRecv.Add((ushort)PacketID.S_RemoveUserAtList, MakePacket<S_RemoveUserAtList>);
		_handler.Add((ushort)PacketID.S_RemoveUserAtList, ChatHandler.S_RemoveUserAtListHandler);

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