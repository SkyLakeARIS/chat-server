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

       // Program.Server.Broadcast(sendBuff);
    }

    internal static void C_SendChatHandler(PacketSession arg1, IPacket arg2)
    {
        C_SendChat packet = arg2 as C_SendChat;
        ChatSession session = arg1 as ChatSession;

        // 채팅 서버에 속해있지 않으면 패킷을 무시한다.
        if (session.chatServer == null)
        {
            return;
        }
        // 중계 역할로, 클라에서 보낸 메세지를 세션 정보와 함께 채팅서버 클래스로 보내서 브로드캐스트 한다.
        // jobQueue를 사용하여 직접 호출이 아니라 델리게이트를 push하는 방식으로 변경. 즉, 함수 호출이라는 액션.
        // 주문서와 비슷한 역할을 한다.

        // 단 이렇게 람다를 사용하면 해당 개체가 사라지면 에러다. Program.Server.Push(() => { Program.Server.Broadcast(session, packet.Message); })
        // 따라서 아래처럼 server를 추출한 뒤에 사용한다. 
        // 이유는 onDisconnect는 접속이 끊기면 바로 실행되는데, JobQueue는 상대적으로 미래에 실행되기 때문이다.
        // 즉, onDisconnect에서 server(채팅방)을 null로 밀어버려서 나중에 action이 실행될 때 null참조가 된다.
        Server server = session.chatServer;
        server.Push(() => server.Broadcast(session, packet.Message));
        //Program.Server.Broadcast(session, packet.Message);
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