START ../../PacketGenerator/bin/Debug/net6.0/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../DummyClient/Network"
XCOPY /Y GenPackets.cs "../../Server/Network"
XCOPY /Y GenPackets.cs "../../Client/Network"
XCOPY /Y ClientPacketManager.cs "../../DummyClient/Network"
XCOPY /Y ServerPacketManager.cs "../../Server/Network"
XCOPY /Y ClientPacketManager.cs "../../Client/Network"