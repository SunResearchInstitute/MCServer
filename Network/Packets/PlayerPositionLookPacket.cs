using System;

namespace MCServer.Network.Packets
{
    public class PlayerPositionLookPacket : Packet
    {
        public PlayerPositionLookPacket(Client client, Entity entity) : base(client)
        {
            PacketId = 0x36;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteDouble(entity.X);
            serializer.WriteDouble(entity.Y);
            serializer.WriteDouble(entity.Z);
            serializer.WriteFloat(entity.Yaw);
            serializer.WriteFloat(entity.Pitch);
            serializer.WriteByte(0);
            Random random = new Random();
            serializer.WriteVarInt(random.Next());
            Data = serializer.GetBytes();
        }
    }
}