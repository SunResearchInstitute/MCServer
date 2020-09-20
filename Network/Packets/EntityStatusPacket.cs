namespace MCServer.Network.Packets
{
    public class EntityStatusPacket : Packet
    {
        public EntityStatusPacket(Client client, Entity entity, byte status) : base(client)
        {
            PacketId = 0x1C;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteInt(entity.EntityId);
            serializer.WriteByte(status);
            Data = serializer.GetBytes();
        }
    }
}