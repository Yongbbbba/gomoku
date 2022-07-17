using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerEnter : Packet
    {
        public uint roomID;
    }

    class RoomFull : Packet
    {

    }

    class PlayerPlay : Packet
    {
        public Horse horse;
    }

    class RoomExit : Packet
    {

    }

    class PlayerPut : Packet
    {
        public ushort x;
        public ushort y;
    }

    class Fail : Packet // put, exit, play, enter 등이 실패하면 그냥 fail 패킷 보내기
    {
    }

    public enum PacketID
    {
        PlayerEnter = 1,
        RoomFull = 2,
        PlayerPlay = 3,
        RoomExit = 4,
        PlayerPut = 5,
        Fail = 6,
    }

    public enum Horse
    {
        none = 0,
        BLACK = 1,
        WHITE = 2,
    }
}
