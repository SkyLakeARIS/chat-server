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
		_onRecv.Add((ushort)PacketID.S_SuccessSignOut, MakePacket<S_SuccessSignOut>);
		_handler.Add((ushort)PacketID.S_SuccessSignOut, ChatHandler.S_SuccessSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_FailSignOut, MakePacket<S_FailSignOut>);
		_handler.Add((ushort)PacketID.S_FailSignOut, ChatHandler.S_FailSignOutHandler);
		_onRecv.Add((ushort)PacketID.S_UserSignIn, MakePacket<S_UserSignIn>);
		_handler.Add((ushort)PacketID.S_UserSignIn, ChatHandler.S_UserSignInHandler);
		_onRecv.Add((ushort)PacketID.S_UserSignOut, MakePacket<S_UserSignOut>);
		_handler.Add((ushort)PacketID.S_UserSignOut, ChatHandler.S_UserSignOutHandler);
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
		_onRecv.Add((ushort)PacketID.S_SuccessCreateServer, MakePacket<S_SuccessCreateServer>);
		_handler.Add((ushort)PacketID.S_SuccessCreateServer, ChatHandler.S_SuccessCreateServerHandler);
		_onRecv.Add((ushort)PacketID.S_FailCreateServer, MakePacket<S_FailCreateServer>);
		_handler.Add((ushort)PacketID.S_FailCreateServer, ChatHandler.S_FailCreateServerHandler);
		_onRecv.Add((ushort)PacketID.S_CurrentUserList, MakePacket<S_CurrentUserList>);
		_handler.Add((ushort)PacketID.S_CurrentUserList, ChatHandler.S_CurrentUserListHandler);

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
		// T는 MakePacket 메서드가 _onRecv(딕셔너리)의 값으로 지정되어있는 타입이므로(MakePacket<~~>)
		// 그 타입(~~부분)에 맞는 형으로 할당함.
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