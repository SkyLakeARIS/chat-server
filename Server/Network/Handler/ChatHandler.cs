using Core;
using Core.Packet;

namespace Server.Network.Handler;

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
}