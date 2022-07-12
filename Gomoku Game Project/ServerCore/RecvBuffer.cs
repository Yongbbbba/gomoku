using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos = 0;
        int _writePos = 0;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        // buffer 공간확보
        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 버퍼에 데이터가 남아있지 않으면 커서가 옮기기
                _readPos = 0;
                _writePos = 0;
            }
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
            {
                return false;
            }

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
            {
                return false;
            }

            _writePos += numOfBytes;
            return true;
        }
    }
}
