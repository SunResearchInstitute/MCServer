namespace MCServer.Network.Packets
{
    public class DeclareRecipesPacket : Packet
    {
        public DeclareRecipesPacket(Client client) : base(client)
        {
            PacketId = 0x5B;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteVarInt(0);
            Data = serializer.GetBytes();
        }
    }
}