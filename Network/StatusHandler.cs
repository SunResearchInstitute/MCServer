using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCServer.Network
{
    public static class StatusHandler
    {
        public static readonly Dictionary<int, Func<Client, byte[], Task>> Actions = new Dictionary<int, Func<Client, byte[], Task>>
        {
            {0x00, Request},
            {0x01, PingPong},
        };

        private static async Task Request(Client client, byte[] data)
        {
            ServerListPingResponse response = new ServerListPingResponse("1.15.2", 578, 69, 420, "uwu desc");
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            String json = JsonSerializer.Serialize(response, options);
            Packet packet = new Packet(client)
            {
                PacketId = 0,
                Data = Utils.EncodeString(json)
            };
            await packet.SendToClient();
        }

        private static async Task PingPong(Client client, byte[] data)
        {
            Packet packet = new Packet(client)
            {
                PacketId = 1,
                Data = data
            };
            await packet.SendToClient();
        }
    }
}