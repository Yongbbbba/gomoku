using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSIze = 2; // size of ushort

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int procesLen = 0;

            while (true)
            {
                // 헤더(패킷 사이즈)는 파싱할 수 있을만큼 도착했는지 확인
                if (buffer.Count < HeaderSIze)
                {
                    break;
                }

                // 패킷이 완전체로 도착했는지 확인 
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset); // offset에서 16비트만큼 uint로 읽어내기
                if (buffer.Count < dataSize) // 다 도착하지 않은 것
                {
                    break;
                }

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                procesLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return procesLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnecting = 0; // 다른 스레드에서 해당 세션을 disconnect했을 경우 중복호출 체크를 위한 공유 변수

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        object _lock = new object();

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        //public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnConnected(Socket socket);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        //public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnDisconnected(Socket socket);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnecting, 1) == 1) // 이미 다른 스레드에서 disconnect
            {
                return;
            }

            OnDisconnected(_socket);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public Socket GetSocket()
        {
            return _socket;
        }

        #region 네트워크 통신 파트
        
        void RegisterRecv()
        {
            _recvBuffer.Clean(); // buffer 공간 확보
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
            {
                OnRecvCompleted(null, _recvArgs);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // write 커서 이동
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠단 처리
                    int processLen = OnRecv(_recvBuffer.ReadSegment); // 컨텐츠단에서 처리된 데이터양만큼 리턴
                    
                    // read 커서 이동
                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed :{e}");
                }
            }

            else
            {
                Disconnect();
            }
        }

        void RegisterSend()
        {
            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);

            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        // CPU를 점유한 스레드가 쓸데없이 문맥교환하지 않고 일감 다 처리해버리기
                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        #endregion
    }
}
