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
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    public enum PacketID
    {

    }


    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        // TODO : 로직 판정을 이곳에서 하기 
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OndisConnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transffered bytes : {numOfBytes}");
        }
    }
}
