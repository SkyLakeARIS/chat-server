using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;

namespace Core;

public class SendBufferHelper
{
    // ThreadLocal을 사용. (스레드 경쟁상태를 막기위해)
    // 멀티스레드에서 좀 더 안전함.(TLS)
    // 스레드마다 버퍼 청크를 가지고 있음.
    public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

    public static int ChunkSize { get; set; } = 4096 * 100;

    public static ArraySegment<byte> Open(int reserveSize)
    {
        // c++에서는 더 개선가능
        // 포인터로 바꿔치기( 아무것도 참조하는게 없으면)

        // 없거나, 다 썼거나 하면 새롭게 만듦.
        if (CurrentBuffer.Value == null)
        {
            CurrentBuffer.Value = new SendBuffer(ChunkSize);
        }

        if (CurrentBuffer.Value.FreeSize < reserveSize)
        {
            CurrentBuffer.Value = new SendBuffer(ChunkSize);
        }

        return CurrentBuffer.Value.Open(reserveSize);
    }

    public static ArraySegment<byte> Close(int usedSize)
    {
        return CurrentBuffer.Value.Close(usedSize);
    }

}
public class SendBuffer
{
    // send queue에 넣어져있는 데이터가 있을 수 있으므로 recv같이 재활용은 힘듦
    private byte[] _buffer;
    private int _usedSize = 0;

    public int FreeSize{ get { return _buffer.Length - _usedSize; } }

    public SendBuffer(int chunkSize)
    {
        _buffer = new byte[chunkSize];
    }
    
    public ArraySegment<byte> Open(int reserveSize)
    {
        // 사용하기 위해서 버퍼를 예약함
        // 실제 사용량과는 별개
        // 따라서 _usedSize는 변동이 없다.
        if (reserveSize > FreeSize)
        {
            return null;
        }

        return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
    }

    public ArraySegment<byte> Close(int usedSize)
    {
        // 실제로 사용한만큼만 잘라서 반환?
        ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
        _usedSize += usedSize;
        return segment;
    }


}
