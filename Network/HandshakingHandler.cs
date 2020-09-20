using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MCServer.Network
{
    public static class HandshakingHandler
    {
        public static readonly Dictionary<int, Func<Client, byte[], Task>> Actions = new Dictionary<int, Func<Client, byte[], Task>>
        {
            {0x00, Handshake},
        };

        private static async Task Handshake(Client client, byte[] data)
        {
            MCSerializer serializer = new MCSerializer(new MemoryStream(data));
            int protocolVersion = serializer.ReadVarInt();

            String host = serializer.ReadString(255);
            ushort port = serializer.ReadUShort();
            int nextStateInt = serializer.ReadVarInt();
            if (nextStateInt > 4)
            {
                client.State = ClientState.Closed;
                return;
            }

            ClientState nextState = (ClientState) nextStateInt;
            if (nextState != ClientState.Status && nextState != ClientState.Login)
            {
                client.State = ClientState.Closed;
                return;
            }
            
            client.State = nextState;
            Console.WriteLine("Protocol #" + protocolVersion + ", Host: " + host + ":" + port + ", Next State: " + nextState);
        }
    }
}