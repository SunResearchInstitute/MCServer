using System;

namespace MCServer.Network
{
    public class ServerListPingResponse
    {
        public SLPRVersion Version { get; set; }
        public SLPRPlayers Players { get; set; }
        public SLPRDescription Description { get; set; }

        public string Favicon { get; set; }

        public ServerListPingResponse(String versionStr, int protocol, int playerCount, int maxPlayerCount, String description)
        {
            Version = new SLPRVersion();
            Players = new SLPRPlayers();
            Description = new SLPRDescription();
            Favicon = "";
            
            Version.Name = versionStr;
            Version.Protocol = protocol;

            Players.Online = playerCount;
            Players.Max = maxPlayerCount;
            Players.Sample = new SLPRPlayerData[0];
            
            Description.Text = description;
        }
    }


    public class SLPRVersion
    {
        public string Name { get; set; }
        public int Protocol { get; set; }
    }

    public class SLPRPlayers
    {
        public int Max { get; set; }
        public int Online { get; set; }
        public SLPRPlayerData[] Sample { get; set; }
    }

    public class SLPRPlayerData
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class SLPRDescription
    {
        public string Text { get; set; }
    }
}