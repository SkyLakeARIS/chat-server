using System;
using System.Collections.Generic;
using System.Text;
using Core;


public enum PacketID
{
	C_SendChat = 1,
	C_RequestSignIn = 2,
	C_RequestSignUp = 3,
	C_RequestSignOut = 4,
	C_RequestEnterServer = 5,
	C_RequestLeaveServer = 6,
	C_RequestCreateServer = 7,
	C_RequestDeleteServer = 8,
	C_RequestJoinServer = 9,
	C_RequestCommand = 10,
	C_RequestServerList = 11,
	S_SuccessSignIn = 12,
	S_FailSignIn = 13,
	S_SuccessSignUp = 14,
	S_FailSignUp = 15,
	S_SuccessSignOut = 16,
	S_FailSignOut = 17,
	S_UserSignIn = 18,
	S_UserSignOut = 19,
	S_SuccessCommand = 20,
	S_FailCommand = 21,
	S_SendChat = 22,
	S_ExitServer = 23,
	S_JoinServer = 24,
	S_SuccessCreateServer = 25,
	S_FailCreateServer = 26,
	S_CurrentUserList = 27,
	
}

interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class C_SendChat : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.C_SendChat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_SendChat);
		count += sizeof(ushort);
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestSignIn : IPacket
{
	public string ID;
	public string Password;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort IDLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.ID = Encoding.Unicode.GetString(s.Slice(count, IDLen));
		count += IDLen;
		ushort PasswordLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Password = Encoding.Unicode.GetString(s.Slice(count, PasswordLen));
		count += PasswordLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestSignIn);
		count += sizeof(ushort);
		ushort IDLen = (ushort)Encoding.Unicode.GetBytes(this.ID, 0, this.ID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), IDLen);
		count += sizeof(ushort);
		count += IDLen;
		ushort PasswordLen = (ushort)Encoding.Unicode.GetBytes(this.Password, 0, this.Password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), PasswordLen);
		count += sizeof(ushort);
		count += PasswordLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestSignUp : IPacket
{
	public string ID;
	public string Password;
	public string Nickname;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestSignUp; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort IDLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.ID = Encoding.Unicode.GetString(s.Slice(count, IDLen));
		count += IDLen;
		ushort PasswordLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Password = Encoding.Unicode.GetString(s.Slice(count, PasswordLen));
		count += PasswordLen;
		ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
		count += NicknameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestSignUp);
		count += sizeof(ushort);
		ushort IDLen = (ushort)Encoding.Unicode.GetBytes(this.ID, 0, this.ID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), IDLen);
		count += sizeof(ushort);
		count += IDLen;
		ushort PasswordLen = (ushort)Encoding.Unicode.GetBytes(this.Password, 0, this.Password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), PasswordLen);
		count += sizeof(ushort);
		count += PasswordLen;
		ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
		count += sizeof(ushort);
		count += NicknameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestSignOut : IPacket
{
	public long UID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestSignOut);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestEnterServer : IPacket
{
	public long UID;
	public long ServerID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestEnterServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		this.ServerID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestEnterServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.ServerID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestLeaveServer : IPacket
{
	public long UID;
	public long ServerID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestLeaveServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		this.ServerID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestLeaveServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.ServerID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestCreateServer : IPacket
{
	public long UID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestCreateServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort ServerNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.ServerName = Encoding.Unicode.GetString(s.Slice(count, ServerNameLen));
		count += ServerNameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestCreateServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		ushort ServerNameLen = (ushort)Encoding.Unicode.GetBytes(this.ServerName, 0, this.ServerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ServerNameLen);
		count += sizeof(ushort);
		count += ServerNameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestDeleteServer : IPacket
{
	public long UID;
	public long ServerID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestDeleteServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		this.ServerID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestDeleteServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.ServerID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestJoinServer : IPacket
{
	public long UID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestJoinServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort ServerNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.ServerName = Encoding.Unicode.GetString(s.Slice(count, ServerNameLen));
		count += ServerNameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestJoinServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		ushort ServerNameLen = (ushort)Encoding.Unicode.GetBytes(this.ServerName, 0, this.ServerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ServerNameLen);
		count += sizeof(ushort);
		count += ServerNameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestCommand : IPacket
{
	public long UID;
	public string Command;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestCommand; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort CommandLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Command = Encoding.Unicode.GetString(s.Slice(count, CommandLen));
		count += CommandLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestCommand);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		ushort CommandLen = (ushort)Encoding.Unicode.GetBytes(this.Command, 0, this.Command.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), CommandLen);
		count += sizeof(ushort);
		count += CommandLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestServerList : IPacket
{
	public long UID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestServerList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestServerList);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessSignIn : IPacket
{
	public long UID;
	public string Nickname;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
		count += NicknameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessSignIn);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UID);
		count += sizeof(long);
		ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
		count += sizeof(ushort);
		count += NicknameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_FailSignIn : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_FailSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ReasonLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Reason = Encoding.Unicode.GetString(s.Slice(count, ReasonLen));
		count += ReasonLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FailSignIn);
		count += sizeof(ushort);
		ushort ReasonLen = (ushort)Encoding.Unicode.GetBytes(this.Reason, 0, this.Reason.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ReasonLen);
		count += sizeof(ushort);
		count += ReasonLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessSignUp : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessSignUp; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessSignUp);
		count += sizeof(ushort);
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_FailSignUp : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_FailSignUp; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ReasonLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Reason = Encoding.Unicode.GetString(s.Slice(count, ReasonLen));
		count += ReasonLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FailSignUp);
		count += sizeof(ushort);
		ushort ReasonLen = (ushort)Encoding.Unicode.GetBytes(this.Reason, 0, this.Reason.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ReasonLen);
		count += sizeof(ushort);
		count += ReasonLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessSignOut : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessSignOut);
		count += sizeof(ushort);
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_FailSignOut : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_FailSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ReasonLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Reason = Encoding.Unicode.GetString(s.Slice(count, ReasonLen));
		count += ReasonLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FailSignOut);
		count += sizeof(ushort);
		ushort ReasonLen = (ushort)Encoding.Unicode.GetBytes(this.Reason, 0, this.Reason.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ReasonLen);
		count += sizeof(ushort);
		count += ReasonLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_UserSignIn : IPacket
{
	public string Nickname;

	public ushort Protocol { get { return (ushort)PacketID.S_UserSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
		count += NicknameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_UserSignIn);
		count += sizeof(ushort);
		ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
		count += sizeof(ushort);
		count += NicknameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_UserSignOut : IPacket
{
	public string Nickname;

	public ushort Protocol { get { return (ushort)PacketID.S_UserSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
		count += NicknameLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_UserSignOut);
		count += sizeof(ushort);
		ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
		count += sizeof(ushort);
		count += NicknameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessCommand : IPacket
{
	public string TargetNickname;
	public string Command;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessCommand; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort TargetNicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.TargetNickname = Encoding.Unicode.GetString(s.Slice(count, TargetNicknameLen));
		count += TargetNicknameLen;
		ushort CommandLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Command = Encoding.Unicode.GetString(s.Slice(count, CommandLen));
		count += CommandLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessCommand);
		count += sizeof(ushort);
		ushort TargetNicknameLen = (ushort)Encoding.Unicode.GetBytes(this.TargetNickname, 0, this.TargetNickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), TargetNicknameLen);
		count += sizeof(ushort);
		count += TargetNicknameLen;
		ushort CommandLen = (ushort)Encoding.Unicode.GetBytes(this.Command, 0, this.Command.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), CommandLen);
		count += sizeof(ushort);
		count += CommandLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_FailCommand : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_FailCommand; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ReasonLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Reason = Encoding.Unicode.GetString(s.Slice(count, ReasonLen));
		count += ReasonLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FailCommand);
		count += sizeof(ushort);
		ushort ReasonLen = (ushort)Encoding.Unicode.GetBytes(this.Reason, 0, this.Reason.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ReasonLen);
		count += sizeof(ushort);
		count += ReasonLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SendChat : IPacket
{
	public string Nickname;
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SendChat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
		count += NicknameLen;
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SendChat);
		count += sizeof(ushort);
		ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
		count += sizeof(ushort);
		count += NicknameLen;
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_ExitServer : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_ExitServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ExitServer);
		count += sizeof(ushort);
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_JoinServer : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_JoinServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort MessageLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Message = Encoding.Unicode.GetString(s.Slice(count, MessageLen));
		count += MessageLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_JoinServer);
		count += sizeof(ushort);
		ushort MessageLen = (ushort)Encoding.Unicode.GetBytes(this.Message, 0, this.Message.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), MessageLen);
		count += sizeof(ushort);
		count += MessageLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessCreateServer : IPacket
{
	public string ServerName;
	public long ServerID;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessCreateServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ServerNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.ServerName = Encoding.Unicode.GetString(s.Slice(count, ServerNameLen));
		count += ServerNameLen;
		this.ServerID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessCreateServer);
		count += sizeof(ushort);
		ushort ServerNameLen = (ushort)Encoding.Unicode.GetBytes(this.ServerName, 0, this.ServerName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ServerNameLen);
		count += sizeof(ushort);
		count += ServerNameLen;
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.ServerID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_FailCreateServer : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_FailCreateServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort ReasonLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.Reason = Encoding.Unicode.GetString(s.Slice(count, ReasonLen));
		count += ReasonLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_FailCreateServer);
		count += sizeof(ushort);
		ushort ReasonLen = (ushort)Encoding.Unicode.GetBytes(this.Reason, 0, this.Reason.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), ReasonLen);
		count += sizeof(ushort);
		count += ReasonLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_CurrentUserList : IPacket
{
	public class UserList
	{
		public string Nickname;
	
		public void Read(ReadOnlySpan<byte> s, ref ushort count)
		{
			ushort NicknameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			this.Nickname = Encoding.Unicode.GetString(s.Slice(count, NicknameLen));
			count += NicknameLen;
		}
	
		public bool Write(Span<byte> s, ref ushort count, ArraySegment<byte> segment)
		{
			bool success = true;
			ushort NicknameLen = (ushort)Encoding.Unicode.GetBytes(this.Nickname, 0, this.Nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NicknameLen);
			count += sizeof(ushort);
			count += NicknameLen;
			return success;
		}	
	}
	public List<UserList> userLists = new List<UserList>();

	public ushort Protocol { get { return (ushort)PacketID.S_CurrentUserList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.userLists.Clear();
		ushort userListLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < userListLen; i++)
		{
			UserList userList = new UserList();
			userList.Read(s, ref count);
			userLists.Add(userList);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_CurrentUserList);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.userLists.Count);
		count += sizeof(ushort);
		foreach (UserList userList in this.userLists)
			success &= userList.Write(s, ref count, segment);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

