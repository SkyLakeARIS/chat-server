using Core;
using Core.Packet;

namespace Server.Network;

public static class ChatHandler
{
    public static void OnUserChat(ChatSession session, InPacket packet)
    {
        var message = packet.DecodeString();
        var outPacket = new OutPacket((int)ServerPacket.Chat);
        outPacket.EncodeString(session.NickName);
        outPacket.EncodeString(message);

        var sendBuff = outPacket.Close();

        Program.Server.Broadcast(sendBuff);
    }

    internal static void C_SendChatHandler(PacketSession arg1, IPacket arg2)
    {
        C_SendChat packet = arg2 as C_SendChat;
        ChatSession session = arg1 as ChatSession;
        S_SendChat respondPacket = new S_SendChat() {Message = packet.Message };

        Console.WriteLine(packet.Message);

        session.Send(respondPacket.Write());
    }

    internal static void C_RequestSignInHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestEnterServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestLeaveServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestCreateServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestDeleteServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestJoinServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void C_RequestServerListHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }
}