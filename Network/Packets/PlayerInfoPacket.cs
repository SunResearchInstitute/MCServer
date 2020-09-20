using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCServer.Network.Packets
{
    public enum PlayerInfoAction
    {
        AddPlayer,
        UpdateGamemode,
        UpdateLatency,
        UpdateDisplayName,
        RemovePlayer
    }
    
    public class PlayerInfoPacket : Packet
    {
        private readonly List<Player> players = new List<Player>();
        private readonly PlayerInfoAction action;
        public PlayerInfoPacket(Client client, PlayerInfoAction action) : base(client)
        {
            PacketId = 0x34;
            this.action = action;
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public new async Task SendToClient()
        {
            MCSerializer serializer = new MCSerializer();
            serializer.WriteVarInt((int)action);
            serializer.WriteVarInt(players.Count);
            foreach (Player player in players)
            {
                serializer.WriteBytes(player.UUID.ToByteArray());
                switch (action)
                {
                    case PlayerInfoAction.AddPlayer:
                    {
                        serializer.WriteString(player.Name);
                        serializer.WriteVarInt(0);
                        serializer.WriteVarInt((int) player.Gamemode);
                        serializer.WriteVarInt(player.Latency);
                        serializer.WriteBool(false);
                        //serializer.WriteString(player.DisplayName.ToString());
                        break;
                    }
                    case PlayerInfoAction.UpdateGamemode:
                    {
                        serializer.WriteVarInt((int) player.Gamemode);
                        break;
                    }
                    case PlayerInfoAction.UpdateLatency:
                    {
                        serializer.WriteVarInt(player.Latency);
                        break;
                    }
                    case PlayerInfoAction.UpdateDisplayName:
                    {
                        if (player.DisplayName.Message != player.Name)
                        {
                            serializer.WriteBool(true);
                            serializer.WriteString(player.DisplayName.ToString());
                        }
                        else
                        {
                            serializer.WriteBool(false);
                        }

                        break;
                    }
                }
            }

            Data = serializer.GetBytes();
        }
    }
}