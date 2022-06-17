using Core.Packet;

namespace Client.Net.Handler;

public static class ChatHandler
{
    public static void OnUserChat(ChatSession session, InPacket packet)
    {
        var name = packet.DecodeString();
        var message = packet.DecodeString();
       
        Console.WriteLine($"{name} : {message}");
    }
}