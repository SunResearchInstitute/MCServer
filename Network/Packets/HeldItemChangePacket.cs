namespace MCServer.Network.Packets
{
    public class HeldItemChangePacket : Packet
    {
        public HeldItemChangePacket(Client client, byte slot) : base(client)
        {
            PacketId = 0x40;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteByte(slot);
            Data = serializer.GetBytes();
        }
    }
}