namespace Core;


// args에 버퍼를 만들지 않고, 따로 분리하여 관리함(session에서 전부 처리-> 별도로 로직 분리)
// session, 즉 클라마다 하나씩 버퍼를 가지고 있는 형태다.
public class RecvBuffer
{
    private ArraySegment<byte> _buffer;
    private int _readPos;
    private int _writePos;

    public RecvBuffer(int bufferSize)
    {
        _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
    }

    public int DataSize
    {
        get { return _writePos - _readPos; }
    }

    public int FreeSize
    {
        get { return _buffer.Count - _writePos; }
    }

    // 어디부터 데이터를 읽으면 되는가 
    public ArraySegment<byte> ReadSegment
    {
        // offset + readPos는 read 커서가 읽어서 옮겨진 경우를 포하므로
        // read 커서가 0이면 0~DataSize 만큼, read커서가 5이면 5~DataSize만큼,
        // offset은 0으로 고정된 느낌.
        get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }

    }

    // 데이터의 유효범위
    public ArraySegment<byte> WriteSegment
    {
        get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }

    }


    // 현재 방식은 순환식이 아니므로, 주기적으로 커서를 앞으로 당겨줘야 계속해서 사용이 가능하다.
    public void Clear()
    {
        int dataSize = DataSize;
        // 데이터가 없는 경우. (r==w)
        if (dataSize == 0)
        {
            _readPos = 0;
            _writePos = 0;
        }
        else // 데이터가 있으면 앞으로 복사.
        {
            Array.Copy(_buffer.Array, _buffer.Offset+_readPos, _buffer.Array, _buffer.Offset, dataSize);
            _readPos = _buffer.Offset;
            _writePos = _buffer.Offset + dataSize;
            // or
            // _readPos = 0;
            // _writePos = dataSize;

        }
    }


    // 컨텐츠 단에서 데이터를 읽은 후(성공하면) 호출하여 처리하는 부분.
    // 컨텐츠 단에서 데이터를 읽은 만큼 커서를 이동시킨다.
    public bool OnRead(int numOfBytes)
    {
        // 비정상 값.
        if (numOfBytes > DataSize)
        {
            return false;
        }

        _readPos += numOfBytes;
        return true;
    }

    // 클라이언트에서 데이터를 보냈을 때, 보낸만큼 w커서를 이동시킨다.
    public bool OnWrite(int numOfBytes)
    {
        // 비정상 값, 버퍼 공간 부족
        if (numOfBytes > FreeSize)
        {
            return false;
        }

        _writePos += numOfBytes;
        return true;
    }
}