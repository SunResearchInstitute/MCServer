using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Network
{
    public class PlayHandler
    {
        public static readonly Dictionary<int, Func<Client, byte[], Task>> Actions = new Dictionary<int, Func<Client, byte[], Task>>
        {
            {0x05, ClientSettings},
            {0x0B, PluginMessage},
        };

        public static async Task ClientSettings(Client client, byte[] data)
        {
            MCSerializer serializer = new MCSerializer(new MemoryStream(data));
            client.Settings.Locale = serializer.ReadString(16);
            client.Settings.ViewDistance = serializer.ReadByte();
            client.Settings.ChatMode = (ChatMode) serializer.ReadVarInt();
            client.Settings.ChatColors = serializer.ReadBool();
            client.Settings.DisplayedSkinParts = serializer.ReadByte();
            client.Settings.MainHand = (PlayerHand) serializer.ReadVarInt();
        }

        public static async Task PluginMessage(Client client, byte[] data)
        {
            MCSerializer serializer = new MCSerializer(new MemoryStream(data));
            String channel = serializer.ReadString(16);
            if (!channel.Contains(":"))
                channel = "minecraft:" + channel;
            String message = Encoding.ASCII.GetString(serializer.ReadBytes());
            
            Console.WriteLine("Recv Message: [" + channel + "] " + message);
        }
    }
}