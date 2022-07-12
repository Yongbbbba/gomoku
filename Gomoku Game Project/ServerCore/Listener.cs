using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listenSocket.Bind(endPoint);

            _listenSocket.Listen(10); // backlog: 커넥션 큐에 pending된 큐의 최대 길이

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

            RegisterAccpet(args);
        }

        void RegisterAccpet(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
        }

    }
}
