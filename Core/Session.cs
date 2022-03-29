using System.Net;
using System.Net.Sockets;

namespace Core;

/*--------------------
	  Session
 --------------------*/

public abstract class PacketSession : Session
{
    // sealed란?
    // 이 클래스를 다시 상속받아서 이 함수를 오버라이드하려고 하면
    // 하지 못하도록 오류를 발생시켜줌(컴파일오류)
    // 여기서 목적은 OnReceivePacket함수로 대체하기 위해서이다.
    public sealed override int OnReceived(ArraySegment<byte> buffer)
    {
        var proccessLen = 0;
        while (true)
        {
            // 패킷 길이 + 패킷타입 헤더 체크
            if (buffer.Count < 8)
            {
                break;
            }

            // 패킷이 완전체로 도착했는지 확인
            // 헤더를 볼 수 있으니, 헤더 정보를 봐서 패킷의 크기를 읽어오는 부분.
            // 아마 이 함수는 UINT16만큼만 읽는 듯(size타입이 ushort 2bytes)
            var dataSize = BitConverter.ToInt32(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize) // 그래서 버퍼에 온 데이터와 패킷의 총 크기를 비교함.
            {
                break;
            }

            // 이부분은 패킷을 어떻게든 조립할 수 있는 부분이 됨.
            // 패킷 한 덩어리 부분을 넘겨주는 부분
            // arratsegment는 구조체라 스택에 생성(new가 동적아님)
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
    private Socket _socket;
    private int _disconnected = 0; // lock
    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); // 대기중인 메세지 목록
    private object _lock = new object(); // Send 작업에 사용할 lock
    private RecvBuffer _recvBuffer = new RecvBuffer(1024);

    public abstract int OnReceived(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnConnected(EndPoint endPoint);
    public abstract void OnDisconnected(EndPoint endPoint);


    public void Start(Socket socket)
    {
        _socket = socket;

        // listener와 동일하게 비동기로 처리
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        // recvArgs.Completed += OnRecvCompleted; 같은 코드

        //// 동기 방식에서 recvBuffer 만든것과 동일하게 데이터를 받을 버퍼를 설정해 줌.
        //_recvArgs.SetBuffer(new byte[1024], 0, 1024);


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

        OnDisconnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    #region Net_Send

    public void Send(ArraySegment<byte> sendBuffer)
    {
        //_Socket.Send(sendBuffer);

        // 멀티스레드 환경에서 안정적으로 돌기 위해서 lock
        lock (_lock)
        {
            // 일단 메세지 큐에 메세지를 넣음.
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0)
            {
                RegisterSend();
            }
        }
    }

    private void RegisterSend()
    {
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

        var pending = _socket.SendAsync(_sendArgs);

        if (!pending)
        {
            OnSendCompleted(null, _sendArgs);
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
        _recvBuffer.Clear();
        ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
        var pending = _socket.ReceiveAsync(_recvArgs);

        if (!pending)
        {
            OnRecvCompleted(null, _recvArgs);
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
                // 보낼 메세지를 버퍼에 입력(write)-> 전송(read)-> 버퍼 read 커서 이동 
                // write 커서 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                {
                    Disconnect();
                    return;
                }

                // 컨텐츠 쪽으로 데이터를 전송하여 처리한 데이터 수를 받아옴
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