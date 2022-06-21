using System.Text;

namespace Core.Packet;



/*
 *
 *      직접 encode, decode 하지 말고, 강의 대로 패킷 클래스에서 encode, decode하도록 변경.
 *      이 방식이 더 맞는 듯.
 *      outPacket도 마찬가지.
 *      
 */
public class InPacket : AbstractPacket
{
    public InPacket(ArraySegment<byte> segment) : base(segment)
    {
        Position = 4;
    }

    public bool DecodeBool()
    {
        var val = BitConverter.ToBoolean(Buffer.Array, Buffer.Offset + Position);
        Position += 1;
        return val;
    }
    public int DecodeInt()
    {
        var val = BitConverter.ToInt32(Buffer.Array, Buffer.Offset + Position);
        Position += 4;

        return val;
    }

    public float DecodeFloat()
    {
        var val = BitConverter.ToSingle(Buffer.Array, Buffer.Offset + Position);
        Position += 4;

        return val;
    }
    public double DecodeDouble()
    {
        var val = BitConverter.ToDouble(Buffer.Array, Buffer.Offset + Position);
        Position += 8;

        return val;
    }
    public string DecodeString()
    {
        var bytesLength = DecodeInt();
        //var val = BitConverter.ToString(Buffer.Array, Buffer.Offset + Position);
        var val = Encoding.UTF8.GetString(Buffer.Array, Buffer.Offset + Position, bytesLength);
        Position += bytesLength;
        return val;
    }
}