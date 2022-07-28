
namespace Core;


// send버퍼를 래핑.
public class SendBufferHelper
{
    // ThreadLocal을 사용. (스레드 경쟁상태를 막기위해)
    // 멀티스레드에서 좀 더 안전함.(TLS)
    // 스레드마다 버퍼 청크를 가지고 있음.
    public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

    public static int ChunkSize { get; set; } = 65535 * 100;

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

// 서버가 가지고 있는 버퍼, 서버에서 클라로 데이터를 전송할 때.
public class SendBuffer
{
    // 다른 세션에 보내려고 send queue에 넣어져있는 데이터가 있을 수 있으므로 recv같이 재활용은 힘듦
    private byte[] _buffer;
    private int _usedSize = 0;

    public int FreeSize{ get { return _buffer.Length - _usedSize; } }

    public SendBuffer(int chunkSize)
    {
        _buffer = new byte[chunkSize];
    }
    
    // 예약 사이즈로 실제 사용하기 전에 넉넉히 받아가는 느낌의 용도.
    // 실제 정확한 크기를 예측하기 힘든 패킷이 있기 때문.. (문자열, 리스트 등)
    public ArraySegment<byte> Open(int reserveSize)
    {
        if (reserveSize > FreeSize)
        {
            return null;
        }

        return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
    }

    // 실제 사용한 사이즈를 알려주면 그만큼 커서를 옮기고 사용한 부분을 반환한다.
    public ArraySegment<byte> Close(int usedSize)
    {
        // 실제로 사용한만큼만 잘라서 반환?
        ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
        _usedSize += usedSize;
        return segment;
    }

}
