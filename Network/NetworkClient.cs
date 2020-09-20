using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Network
{
    public class NetworkClient
    {
        public readonly NetworkStream Stream;

        public NetworkClient(NetworkStream stream)
        {
            Stream = stream;
        }
        
        private readonly byte[] _readByteBuf = new byte[1];
        private async Task<byte> ReadByteAsync()
        {
            await Stream.ReadAsync(_readByteBuf, 0, 1);
            return _readByteBuf[0];
        }

        public async Task<int> ReadVarInt()
        {
            int outVal = 0;
            byte readByte;
            int iter = 0;
            do
            {
                Task<byte> readByteTask = ReadByteAsync();
                await readByteTask;
                readByte = readByteTask.Result;
                outVal |= (readByte & 0b0111_1111) << (iter * 7);
                iter++;
            } while (iter <= 5 && (readByte & (1 << 7)) != 0);

            return outVal;
        }
        

        public async Task  WriteString(String value)
        {
            byte[] encodedLen = Utils.EncodeVarInt(value.Length);
            await Stream.WriteAsync(encodedLen);
            await Stream.WriteAsync(Encoding.ASCII.GetBytes(value));
        }
    }
}