namespace Core.Packet;

public abstract class AbstractPacket
{
    public ArraySegment<byte> Buffer { get; }

    public int Position { get; protected set; }

    public int Length => Buffer.Count;

    protected AbstractPacket(ArraySegment<byte> segment)
    {
        Buffer = segment;
    }
}