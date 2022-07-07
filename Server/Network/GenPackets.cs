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
	S_UserSignIn = 14,
	S_UserSignOut = 15,
	S_SuccessSignUp = 16,
	S_FailSignUp = 17,
	S_SucessSignOut = 18,
	S_FailSignOut = 19,
	S_SuccessCommand = 20,
	S_FailCommand = 21,
	S_SendChat = 22,
	S_ExitServer = 23,
	S_JoinServer = 24,
	
}

public interface IPacket
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
	public string UserName;

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
		ushort UserNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.UserName = Encoding.Unicode.GetString(s.Slice(count, UserNameLen));
		count += UserNameLen;
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
		ushort UserNameLen = (ushort)Encoding.Unicode.GetBytes(this.UserName, 0, this.UserName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), UserNameLen);
		count += sizeof(ushort);
		count += UserNameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestSignOut : IPacket
{
	public long UserID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class C_RequestEnterServer : IPacket
{
	public long UserID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestEnterServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestEnterServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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

class C_RequestLeaveServer : IPacket
{
	public long UserID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestLeaveServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestLeaveServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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

class C_RequestCreateServer : IPacket
{
	public long UserID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestCreateServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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
	public long UserID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestDeleteServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_RequestDeleteServer);
		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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

class C_RequestJoinServer : IPacket
{
	public long UserID;
	public string ServerName;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestJoinServer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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
	public long UserID;
	public string Command;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestCommand; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
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
	public long UserID;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestServerList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
		count += sizeof(long);
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessSignIn : IPacket
{
	public long UserID;
	public string UserName;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.UserID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort UserNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.UserName = Encoding.Unicode.GetString(s.Slice(count, UserNameLen));
		count += UserNameLen;
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.UserID);
		count += sizeof(long);
		ushort UserNameLen = (ushort)Encoding.Unicode.GetBytes(this.UserName, 0, this.UserName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), UserNameLen);
		count += sizeof(ushort);
		count += UserNameLen;
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

class S_UserSignIn : IPacket
{
	public string UserName;

	public ushort Protocol { get { return (ushort)PacketID.S_UserSignIn; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort UserNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.UserName = Encoding.Unicode.GetString(s.Slice(count, UserNameLen));
		count += UserNameLen;
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
		ushort UserNameLen = (ushort)Encoding.Unicode.GetBytes(this.UserName, 0, this.UserName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), UserNameLen);
		count += sizeof(ushort);
		count += UserNameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_UserSignOut : IPacket
{
	public string UserName;

	public ushort Protocol { get { return (ushort)PacketID.S_UserSignOut; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort UserNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.UserName = Encoding.Unicode.GetString(s.Slice(count, UserNameLen));
		count += UserNameLen;
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
		ushort UserNameLen = (ushort)Encoding.Unicode.GetBytes(this.UserName, 0, this.UserName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), UserNameLen);
		count += sizeof(ushort);
		count += UserNameLen;
		success &= BitConverter.TryWriteBytes(s, count);
		if (success == false)
			return null;
		return SendBufferHelper.Close(count);
	}
}

class S_SuccessSignUp : IPacket
{
	public string Reason;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessSignUp; } }

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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessSignUp);
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

class S_SucessSignOut : IPacket
{
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SucessSignOut; } }

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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SucessSignOut);
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

class S_SuccessCommand : IPacket
{
	public string TargetUserName;
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SuccessCommand; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort TargetUserNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.TargetUserName = Encoding.Unicode.GetString(s.Slice(count, TargetUserNameLen));
		count += TargetUserNameLen;
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
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_SuccessCommand);
		count += sizeof(ushort);
		ushort TargetUserNameLen = (ushort)Encoding.Unicode.GetBytes(this.TargetUserName, 0, this.TargetUserName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), TargetUserNameLen);
		count += sizeof(ushort);
		count += TargetUserNameLen;
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
	public string NickName;
	public string Message;

	public ushort Protocol { get { return (ushort)PacketID.S_SendChat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort NickNameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.NickName = Encoding.Unicode.GetString(s.Slice(count, NickNameLen));
		count += NickNameLen;
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
		ushort NickNameLen = (ushort)Encoding.Unicode.GetBytes(this.NickName, 0, this.NickName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), NickNameLen);
		count += sizeof(ushort);
		count += NickNameLen;
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

