using System.Net;
using System.Net.Sockets;

namespace Core;

/*--------------------
	  Session
 --------------------*/

public abstract class PacketSession : Session
{
	public sealed override int OnReceived(ArraySegment<byte> buffer)
    {
        var proccessLen = 0;
        while (true)
        {
            // 패킷의 크기(헤더)를 읽을 수 있을 만큼의 바이트가 도착했는지 검사. 
            // 패킷의 크기는 short형으로 사용.
            if (buffer.Count < 2)
            {
                break;
            }

            // 패킷의 사이즈를 알 수 있으므로 패킷이 완전체로 도착했는지 확인
            var dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize) // 그래서 버퍼에 온 데이터와 패킷의 총 크기를 비교함.
            {
                break;
            }

			// 여기까지 오면 패킷을 파싱할 수 있음.
			// 패킷의 id에 따른 핸들링을 한다.
            // arraysegment는 구조체라 스택에 생성(new가 동적아님)
            OnReceivePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            proccessLen += dataSize;

            // [size(2)][packetid(2)][...]<이부분으로 이동>[size(2)][packetid(2)][...] ...
            // 처리한 패킷을 제외한 남은 패킷들만 빼서 넣어줌.
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }

        return proccessLen;
    }

    // 파싱하는 단계
    public abstract void OnReceivePacket(ArraySegment<byte> buffer);
}

public abstract class Session
{
    protected Socket _socket;
    private int _disconnected = 0; // lock

    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();// 대기중인 메세지 목록
    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); // 한방에 보내기 위한 리스트. sendQueue의 내용을 한꺼번에 가져가 전송.
    private object _lock = new object(); // Send 작업에 사용할 lock
    private RecvBuffer _recvBuffer = new RecvBuffer(65535);


    // core와 컨텐츠 서버와의 분리
    // 어떤 작업에 대한 후처리(이 서버코어 사용자가 하고 싶은 행동) 인터페이스를 제공
    public abstract int OnReceived(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnConnected(EndPoint endPoint);
    public abstract void OnDisconnected(EndPoint endPoint);

    void Clear()
    {
        lock (_lock)
        {
            _sendQueue.Clear();
            _pendingList.Clear();
        }
    }

    public void Start(Socket socket)
    {
        // 접속한 클라이언트 소켓.
        _socket = socket;

        // listener와 동일하게 비동기로 처리
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

        // Receive 흐름은 Listener와 같음
        RegisterRecv();
    }

    public void Disconnect()
    {
        // 멀티스레드 환경에서 (동시에 접근해도) 안전하게 조치
        // 서버에서 Disconnect()를 여러번 호출해도 오류가 발생하지 않음.
        // 내부에서 _Disconnect 값을 1로 변경해주는 듯
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
        {
            return;
        }

        // 멀티스레드에서 예외처리가 더 필요함.
        OnDisconnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        Clear();
    }

    #region Net_Send

    // 패킷 모아보내기를 위한 array List
    public void Send(List<ArraySegment<byte>> sendBufferList)
    {
        // 빈 리스트가 들어오면 차단. OnSendCompleted에서 예외처리를(전송바이트가 0) 하기 때문에
        // 클라 접속이 차단되는 것을 막기 위함.
        if (sendBufferList.Count == 0)
        {
            return;
        }

        // 멀티스레드 환경에서 안정적으로 돌기 위해서 lock
        lock (_lock)
        {
            // 일단 메세지 큐에 메세지를 넣음.
            foreach(ArraySegment<byte> segment in sendBufferList)
            {
                _sendQueue.Enqueue(segment);
            }

            if (_pendingList.Count == 0)
            {
                RegisterSend();
            }
        }
    }

    // 패킷을 모아보내지 않고 그냥 byte array를 전송
    public void Send(ArraySegment<byte> sendBuffer)
    {
        //_Socket.Send(sendBuffer);

        // 멀티스레드 환경에서 안정적으로 돌기 위해서 lock
        lock (_lock)
        {
            // 일단 메세지 큐에 메세지를 넣음.
            // 이는 멀티스레드 환경에 의해서 패킷 순서가 꼬이거나, 
            // 중복해서 보내지거나 하는 문제를 방지하기 위해서 
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0)
            {
                RegisterSend();
            }
        }
    }

    private void RegisterSend()
    {
        if (_disconnected == 1)
        {
            return;
        }

        while (_sendQueue.Count > 0)
        {
            var buffer = _sendQueue.Dequeue();
            // 아래 둘 중 하나만 써야 크래시가 안난다.
            // _SendArgs.SetBuffer(buffer, 0, buffer.Length); // 매번 호출하여 하나씩 처리하는 방법
            _pendingList.Add(buffer); // 리스트에 전부 넣어서 한번에 보내는 방법
        }

        // 이렇게 List로 만들어서 넣어주어야 정상적으로 동작한다고 한다.
        // _SendArgs.BufferList.Add(new ArraySegment<byte> (buffer,0,buffer.Length));  // 이렇게하면 잘 동작 안함.
        _sendArgs.BufferList = _pendingList;

        try
        {
            var pending = _socket.SendAsync(_sendArgs);

            if (!pending)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($" register send failed {e}" );
        }

    }

    private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        // RegisterSend()를 통해 접근과 Event로 Completed로 접근하는 방식이 존재하는데(중요)
        // 전자는 앞에서 락이 걸려 있기 때문에 괜찮지만
        // 후자의 경우 또 다른 스레드가 비동기로 접근할 수 있으므로 역시 lock을 건다.
        // 이런 lock을 거는 포인트는 계속 문제를 겪으면서 터득해 나갈 것
        lock (_lock)
        {
            // 보내는 크기 체크와 에러 체크
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend(_sendArgs.BytesTransferred);
                    // 락을 걸고 처리하는 동안 다른 스레드는 보내기 작업을 못할뿐
                    // SendQueue에 메세지는 넣을 수 있으므로 큐에 뭔가 들어있으면
                    // 다시 RegisterSend()로 가서 처리해줌
                    if (_sendQueue.Count > 0)
                    {
                        RegisterSend();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"메시지 송신 실패. {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }

    #endregion

    #region Net_Receive

    private void RegisterRecv()
    {
        if (_disconnected == 1) //, 스레드 세이프 연결 끊으면 실행 x
        {
            return;
        }

        // 데이터를 받기전에 여유 공간을 확보.
        _recvBuffer.Clear();
        // recv버퍼에서 현재 쓸 수 있는 공간을 받아서 그 recvArgs의 버퍼로 세팅해 준다.
        // 기존에 _recvArgs.SetBuffer(new byte[1024], 0, 1024); 코드를 삭제했기 때문.
        ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count); // buffer, writeCursor, 

        try // 스레드 세이프, 연결 끊으면 실행 x
        {
            var pending = _socket.ReceiveAsync(_recvArgs);

            if (!pending)
            {
                OnRecvCompleted(null, _recvArgs);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine($"register recv failed {e}");
        }
 
    }

    private void OnRecvCompleted(Object sender, SocketAsyncEventArgs args)
    {
        // 전송받은 메시지 길이가 0 이상이어야 하고, 에러가 없어야함.
        // args.BytesTransferred 는 전송받은 바이트 수를 의미한다.
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                // 보낼 메세지를 버퍼에 입력(write)-> 컨텐츠에서 처리 -> 처리한 만큼 버퍼 read 커서 이동 
                // write 커서 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                {
                    Disconnect();
                    return;
                }

                // 컨텐츠 쪽에서 데이터를 처리하고 처리한 값을 받아옴.
                int processLen = OnReceived(_recvBuffer.ReadSegment);
                // 비정상적인 값을 처리
                if (processLen < 0 || processLen > _recvBuffer.DataSize)
                {
                    Disconnect();
                    return;
                }

                // read 커서 이동
                if (_recvBuffer.OnRead(processLen) == false)
                {
                    Disconnect();
                    return;
                }


                // 처리가 끝나면 다시 수신 대기 상태(비동기)
                RegisterRecv();
            }
            catch (Exception e)
            {
                Console.WriteLine($"메시지 수신 실패. {e}");
            }
        }
        else
        {
            Disconnect();
        }
    }

    #endregion
}