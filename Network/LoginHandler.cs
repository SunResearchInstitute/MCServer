using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MCServer.Network.Packets;

namespace MCServer.Network
{
    public class LoginHandler
    {
        public static readonly Dictionary<int, Func<Client, byte[], Task>> Actions = new Dictionary<int, Func<Client, byte[], Task>>
        {
            {0x00, LoginStart},
        };

        private static async Task LoginStart(Client client, byte[] data)
        {
            String username = Utils.ReadString(new MemoryStream(data), 16);
            Console.WriteLine("Client attempting to authenticate as " +  username);

            Guid guid = new Guid();
            LoginSuccessPacket loginSuccessPacket = new LoginSuccessPacket(client, guid, username);
            await loginSuccessPacket.SendToClient();
            client.State = ClientState.Play;
            client.Player = new Player(client, username, guid);

            JoinGamePacket joinGamePacket = new JoinGamePacket(client, client.Player.EntityId, (byte)client.Player.Gamemode, 0, 0x0000000000000000);
            await joinGamePacket.SendToClient();
            Console.WriteLine("Client authenticated as " + username);
            
            HeldItemChangePacket heldItemChangePacket = new HeldItemChangePacket(client, 0);
            await heldItemChangePacket.SendToClient();
            
            
            PlayerPositionLookPacket playerPositionLookPacket = new PlayerPositionLookPacket(client, client.Player);
            await playerPositionLookPacket.SendToClient();
            
            PlayerInfoPacket playerInfoPacket = new PlayerInfoPacket(client, PlayerInfoAction.AddPlayer);
            playerInfoPacket.AddPlayer(client.Player);
            await playerInfoPacket.SendToClient();
            
            playerInfoPacket = new PlayerInfoPacket(client, PlayerInfoAction.UpdateLatency);
            playerInfoPacket.AddPlayer(client.Player);
            await playerInfoPacket.SendToClient();
            
        }
    }
}