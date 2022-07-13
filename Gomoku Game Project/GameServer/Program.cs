using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace GameServer
{
    class Program
    {
        static Listener _listener = new Listener();
        static int nextID; // 다음에 접속할 클라이언트 ID
        static Dictionary<int, ClientSession> connections; // 접속한 클라이언트 관리. 

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new ClientSession(); });

            Console.WriteLine("Listening ... ");

            while (true)
            {
                ;
            }
        }
    }
}
