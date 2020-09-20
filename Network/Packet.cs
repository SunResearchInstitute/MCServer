using System;
using System.Threading.Tasks;

namespace MCServer.Network
{
    public class Packet
    {
        public int PacketId;
        public byte[] Data;
        public readonly Client Client;

        public Packet(Client client)
        {
            Client = client;
        }

        public Packet(Client client, int id, byte[] data)
        {
            Client = client;
            PacketId = id;
            Data = data;
        }

        public async Task ReadFromClient()
        {
            PacketId = -1;
            Data = null;
            
            Task<int> lengthTask = Client.NetworkClient.ReadVarInt();
            await lengthTask;
            int length = lengthTask.Result;
            if (length == 0)
            {
                Client.State = ClientState.Closed;
                return;
            }
            
            if (!Client.Compressed)
            {
                Task<int> packetIdTask = Client.NetworkClient.ReadVarInt();
                await packetIdTask;
                PacketId = packetIdTask.Result;

                int encodedLength = Utils.EncodeVarInt(PacketId).Length;
                if (encodedLength > length)
                {
                    Client.State = ClientState.Closed;
                    return;
                }

                int dataSize = length - encodedLength;
                Data = new byte[dataSize];
                if (dataSize != 0)
                {
                    Task<int> readTask = Client.NetworkClient.Stream.ReadAsync(Data, 0, dataSize);
                    await readTask;
                    if (readTask.Result == -1)
                        Client.State = ClientState.Closed;
                }
            }
        }

        public async Task SendToClient()
        {
            if (PacketId == -1 || Data == null)
                return;

            byte[] encodedPacketId = Utils.EncodeVarInt(PacketId);
            byte[] encodedDataLength = Utils.EncodeVarInt(encodedPacketId.Length + Data.Length);
            if (!Client.Compressed)
            {
                await Client.NetworkClient.Stream.WriteAsync(encodedDataLength);
                await Client.NetworkClient.Stream.WriteAsync(encodedPacketId);
                await Client.NetworkClient.Stream.WriteAsync(Data);
            }
        }
    }
}