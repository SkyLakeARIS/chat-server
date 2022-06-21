using System.Text;
using Google.Protobuf.WellKnownTypes;

namespace Core.Packet;


/*
 *
 *      직접 encode, decode 하지 말고, 강의 대로 패킷 클래스에서 encode, decode하도록 변경.
 *      이 방식이 더 맞는 듯.
 *      inPacket도 마찬가지.
 *      
 */

public class OutPacket : AbstractPacket
{
    public OutPacket(ArraySegment<byte> segment) : base(segment)
    {
        if (segment.Array == null)
        {
            throw new NullReferenceException("Array 가 Null이였습니다.");
        }

        // 왜 4?
        Position = 4;
    }

    public OutPacket(int type, int size = 4096) : this(SendBufferHelper.Open(size))
    {
        // 패킷의 헤더
        EncodeInt( type);
    }

    public void EncodeBool(bool val)
    {
        var buffer1 = BitConverter.GetBytes(val);

        Array.Copy(buffer1, 0, Buffer.Array, Buffer.Offset + Position, buffer1.Length);
        Position += 1;
    }

    public void EncodeInt(int val)
    {
        var buffer1 = BitConverter.GetBytes(val);

        Array.Copy(buffer1, 0, Buffer.Array, Buffer.Offset + Position, buffer1.Length);
        Position += 4;
    }

    public void EncodeFloat(float val)
    {
        var bytes = BitConverter.GetBytes(val);

        Array.Copy(bytes, 0, Buffer.Array, Buffer.Offset + Position, bytes.Length);
        Position += 4;
    }

    public void EncodeDouble(double val)
    {
        var bytes = BitConverter.GetBytes(val);

        Array.Copy(bytes, 0, Buffer.Array, Buffer.Offset + Position, bytes.Length);
        Position += 8;
    }

    public void EncodeString(string val)
    {
        var bytes = Encoding.UTF8.GetBytes(val);
        // BitConverter.GetBytes(val);
        // 마땅한 방법이 없어서 앞에 문자열 길이 명시
        EncodeInt(bytes.Length);
        Array.Copy(bytes, 0, Buffer.Array, Buffer.Offset + Position, bytes.Length);

        Position += bytes.Length;
    }

    public ArraySegment<byte> Close()
    {
        var buffer1 = BitConverter.GetBytes(Position);

        Array.Copy(buffer1, 0, Buffer.Array, Buffer.Offset, 4);
        return SendBufferHelper.Close(Position);
    }
}