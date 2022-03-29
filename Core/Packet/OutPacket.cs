using System.Text;
using Google.Protobuf.WellKnownTypes;

namespace Core.Packet;

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