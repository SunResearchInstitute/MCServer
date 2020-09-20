using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MCServer.Network.Packets
{
    public class JoinGamePacket : Packet
    {
        public JoinGamePacket(Client client, int entityID, byte gamemode, int dimension, long seedHash) : base(client)
        {
            PacketId = 0x26;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteInt(entityID);
            serializer.WriteByte(gamemode);
            serializer.WriteInt(dimension);
            serializer.WriteLong(seedHash);
            serializer.WriteByte(69);
            serializer.WriteString("default");
            serializer.WriteVarInt(16);
            serializer.WriteBool(false);
            serializer.WriteBool(true);
            Data = serializer.GetBytes();
        }
    }
}