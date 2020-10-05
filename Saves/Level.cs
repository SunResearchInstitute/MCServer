using System;

namespace MCServer.Saves
{
    public class Level
    {
        public bool Hardcore;
        public bool SpawnVillages;
        public bool Raining;
        public bool Thundering;
        public int GameType;
        public int GeneratorVersion = 1;
        public int RainTicksRemaining;
        public int SpawnX, SpawnY, SpawnZ;
        public int ThunderTime;
        public int Version;
        public long LastPlayer;
        public long SizeOnDisk;
        public long Time;
        public string GeneratorName;
        public string LevelName;
        
        
    }
}