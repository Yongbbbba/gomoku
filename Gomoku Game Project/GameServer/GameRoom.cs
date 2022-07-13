using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace GameServer
{
    public class GameRoomManager
    {
        static GameRoomManager _roomManager = new GameRoomManager();
        public static GameRoomManager Instance { get { return _roomManager; } }

        uint _roomId = 0;
        Dictionary<uint, GameRoom> _rooms = new Dictionary<uint, GameRoom>();
        object _lock = new object();

        public GameRoom Generate()
        {
            lock(_lock)
            {
                uint roomId = ++_roomId;

                GameRoom room = new GameRoom();
                room.RoodId = roomId;
                _rooms.Add(roomId, room);

                return room;
            }
        }

        public GameRoom Find(uint id)
        {
            lock(_lock)
            {
                GameRoom room = null;
                _rooms.TryGetValue(id, out room);
                return room;
            }
        }

        public void Remove(GameRoom room)
        {
            lock (_lock)
            {
                _rooms.Remove(room.RoodId);
            }
        }
    }

    public class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>(); // gameroom에 참여한 session, 최대 2명
        public uint RoodId { get; set; }

        public void BroadCast() { }

        public void Leave(ClientSession session)
        {
            session.Room = null;
            _sessions.Remove(session);
            if (_sessions.Count == 0)
            {
                GameRoomManager.Instance.Remove(this);
            }
        }

        public bool Enter(ClientSession session)
        {
            if (GetClientCount() >= 2)
            {
                return false;
            }
            session.Room = this; 
            _sessions.Add(session);
            return true;
        }
        
        public int GetClientCount()
        {
            return _sessions.Count;
        }

        public void PlayGame()
        {
            Color color = Color.BLACK;
            for (int i = 0; i < _sessions.Count; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);
                PlayerPlay packet = new PlayerPlay() { size = 0, packetId = (ushort)PacketID.PlayerPlay, color = color };
                ushort sendSize = 0;
                bool success = true;
                sendSize += 2;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + sendSize, s.Count - sendSize), packet.packetId);
                sendSize += 2;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + sendSize, s.Count - sendSize), (int)packet.color);
                sendSize += 4;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), sendSize);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(sendSize);
                if (success)
                {

                    _sessions[i].Send(sendBuff);
                }
                color = Color.WHITE;
            }
        }
    }
}

