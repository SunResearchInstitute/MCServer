namespace MCServer
{
    public class Block
    {
        public int StateId;
        public readonly int X, Y, Z;
        public int Light, Skylight;
        public readonly Biome Biome = Biome.TheVoid;

        public Block(int stateId, Biome biome, int x = 0, int y = 0, int z = 0, int light = 0, int skylight = 0)
        {
            StateId = stateId;
            Biome = biome;
            X = x;
            Y = y;
            Z = z;
            Light = light;
            Skylight = skylight;
        }
    }
}