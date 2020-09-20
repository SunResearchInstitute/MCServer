using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MCServer.Network.Packets
{
    public class LoginSuccessPacket : Packet
    {
        public LoginSuccessPacket(Client client, Guid uuid, String playerName) : base(client)
        {
            PacketId = 0x02;
            MCSerializer serializer = new MCSerializer();
            serializer.WriteString(uuid.ToString());
            serializer.WriteString(playerName);
            Data = serializer.GetBytes();
        }
    }
}