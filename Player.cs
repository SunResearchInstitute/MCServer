using System;
using MCServer.Network.Packets;

namespace MCServer
{
    public enum Gamemode
    {
        Survival,
        Creative,
        Adventure,
        Spectator
    }

    public enum PlayerEntityStatus : byte
    {
        ItemUseFinished = 9,
        EnableReducedDebugInfo = 22,
        DisableReducedDebugInfo = 23,
        OpLevel0 = 24,
        OpLevel1 = 25,
        OpLevel2 = 26,
        OpLevel3 = 27,
        OpLevel4 = 28,
        SpawnCloudParticles = 43
    }
    
    public class Player : Entity
    {
        public readonly Client client;
        public Guid UUID = new Guid();
        public String Name;
        public Chat DisplayName;
        public int Latency = 0;
        public Gamemode Gamemode = Gamemode.Survival;
        private byte _oplevel;

        public byte OpLevel
        {
            get => _oplevel;
            set
            {
                byte status;
                switch (value)
                {
                    case 1:
                        status = (byte) PlayerEntityStatus.OpLevel1;
                        break;
                    case 2:
                        status = (byte) PlayerEntityStatus.OpLevel2;
                        break;
                    case 3:
                        status = (byte) PlayerEntityStatus.OpLevel3;
                        break;
                    case 4:
                        status = (byte) PlayerEntityStatus.OpLevel4;
                        break;
                    default:
                        status = (byte) PlayerEntityStatus.OpLevel0;
                        break;
                }
                EntityStatusPacket statusPacket = new EntityStatusPacket(client, this, status);
                _oplevel = value;
            }
        }

        public Player(Client client, String name, Guid UUID)
        {
            this.client = client;
            this.UUID = UUID;
            Name = name;
            DisplayName = new Chat(name);
            OpLevel = 0;
        }
    }
}