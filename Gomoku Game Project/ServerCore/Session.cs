using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
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

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

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

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
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

        #endregion
    }
}
