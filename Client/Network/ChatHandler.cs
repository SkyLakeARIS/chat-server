using Core;
using Core.Packet;

namespace Client.Network;

public static class ChatHandler
{
    public static void OnUserChat(ChatSession session, InPacket packet)
    {
        var name = packet.DecodeString();
        var message = packet.DecodeString();

        Console.WriteLine($"{name} : {message}");
    }

    internal static void S_SuccessSignInHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_FailSignInHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SuccessSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_FailSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SucessSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_FailSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SuccessCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_FailCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SendChatHandler(PacketSession arg1, IPacket arg2)
    {
        S_SendChat packet = arg2 as S_SendChat;
        ChatSession session = arg1 as ChatSession;
        Console.SetCursorPosition(session.x, session.y);
        session.y++;
        Console.WriteLine(packet.Message);
    }

    internal static void S_ExitServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_JoinServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }
}