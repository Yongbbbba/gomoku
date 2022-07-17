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
    public class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            int pos = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerEnter:
                    {
                        uint roomId = BitConverter.ToUInt32(buffer.Array, buffer.Offset + pos);
                        pos += sizeof(int);

                        // 해당 방이 존재하는지 확인
                        if (GameRoomManager.Instance.Find(roomId) != null)
                        {
                            // 이미 방에 두 명 이상인지 확인
                            if (GameRoomManager.Instance.Find(roomId).GetClientCount() >= 2)
                            {
                                // full 패킷 send
                                ArraySegment<byte> s = SendBufferHelper.Open(4096);
                                RoomFull packet = new RoomFull() { size = 0, packetId = (ushort)PacketID.RoomFull };
                                ushort sendSize = 0;
                                bool success = true;
                                sendSize += 2;
                                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + sendSize, s.Count - sendSize), packet.packetId);
                                sendSize += 2;
                                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), sendSize);
                                ArraySegment<byte> sendBuff = SendBufferHelper.Close(sendSize);
                                if (success)
                                {
                                    Send(sendBuff);
                                }
                                break;
                            }
                            else
                            {
                                GameRoomManager.Instance.Find(roomId).Enter(this);
                                // 2명이 되었으므로 게임 시작
                                Console.WriteLine($"{Room.RoodId}번방 게임 시작");
                                Room.PlayGame();
                            }
                        }
                        else
                        {
                            this.Room = GameRoomManager.Instance.Generate();
                        }
                        Console.WriteLine($"{this.SessionId}번 세션 {this.Room.RoodId}번방 입장 완료");
                    }
                    break;

                case PacketID.PlayerPut:
                    {
                        ushort x = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
                        pos += sizeof(ushort);
                        ushort y = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
                        pos += sizeof(ushort);

                        // 유저가 놓은 돌의 위치를 다른 유저에게도 전송

                        break;
                    }

                case PacketID.RoomExit:

                default:
                    break;
            }

            Console.WriteLine($"RecvPacketId : {id}, Size : {size}");
        }

        public override void OnConnected(EndPoint endPoint)
        {
			Console.WriteLine($"OnConnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            throw new NotImplementedException();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Room.Leave(this);
	        SessionManager.Instance.Remove(this);
			
			Console.WriteLine($"OnDisconnected : {endPoint}");
        }
    }
}

