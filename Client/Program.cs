// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Net;
using Core;
using Core.Packet;

namespace Client;



public class Packet
{
    public int packetid;
    public int test;
}

internal static class Program
{

		/*--------------------
			     Client
		 --------------------*/
    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ip = ipHost.AddressList[0];
        // 목적지
        var endPoint = new IPEndPoint(ip, 9999);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => new ChatSession(), 1);


        while (true)
        {
            var message = Console.ReadLine();

            if (!string.IsNullOrEmpty(message) && ChatSession.Instance != null)
            {
                var packet = new OutPacket((int) ClientPacket.Chat);
                packet.EncodeString(message);

                ChatSession.Instance.Send(packet.Close());
            }
        }

    }
}