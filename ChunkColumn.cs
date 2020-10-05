using System;
using System.Collections.Generic;
using MCServer.Network;

namespace MCServer
{
    public class ChunkColumn
    {
        public const int Width = 16;
        public const int Height = 256;
        public const int SectionCount = 16;

        public readonly int X;
        public readonly int Z;
        public int SkyLightMask = 0;
        public int BlockLightMask = 0;
        public int SectionMask = 0;
        public ChunkSection[] Sections = new ChunkSection[SectionCount];
        public Biome[] Biomes = new Biome[1024];
        public BitStorage[] BlockLightSections = new BitStorage[SectionCount + 2];
        public BitStorage[] SkyLightSections = new BitStorage[SectionCount + 2];

        public void FillLayer(int y, int blockState, Biome biome)
        {
            for (int z = 0; z < 16; z++)
                for (int x = 0; x < 16; x++)
                    SetBlock(new Block(blockState, biome, x, y, z));
        }
        
        public void FillTest(int height = 64)
        {
            for (int y = 0; y < height; y++)
            {
                if (y < 4)
                    FillLayer(y, Server.MinecraftData.GetBlocksByName()["bedrock"].DefaultState, Biome.TheVoid);
                else if (y < 50)
                    FillLayer(y, Server.MinecraftData.GetBlocksByName()["stone"].DefaultState, Biome.Plains);
                else if (y < 80)
                    FillLayer(y, Server.MinecraftData.GetBlocksByName()["dirt"].DefaultState, Biome.Plains);
            }
            
            FillLayer(height, Server.MinecraftData.GetBlocksByName()["grass_block"].DefaultState, Biome.Plains);
        }
        public static int GetChunkSectionIdx(int y)
        {
            return y / 16;
        }

        public static int GetLightSectionIdx(int y)
        {
            return GetChunkSectionIdx(y) + 1;
        }

        public static int GetChunkRelHeight(int y)
        {
            return y & 0xf;
        }

        public static int GetChunkRelBlockIdx(int x, int y, int z)
        {
            return (GetChunkRelHeight(y) << 8) | (z << 4) | x;
        }

        public static int GetHeightmapIdx(int x, int z)
        {
            return (z << 4) | x;
        }

        public static int GetBiomeIdx(int x, int y, int z)
        {
            return ((y >> 2) & 63) << 4 | ((z >> 2) & 3) << 2 | ((x >> 2) & 3);
        }

        public ChunkColumn(int x, int z)
        {
            X = x;
            Z = z;
        }

        public Block GetBlock(int x, int y, int z)
        {
            int chunkSection = GetChunkSectionIdx(y);
            if (Sections[chunkSection] == null)
                return new Block(0, Biome.TheVoid, x, y, z);

            return new Block(Sections[chunkSection].GetBlock(x, GetChunkRelHeight(y), z), GetBiome(x, y, z), x, y, z,
                GetBlockLight(x, y, z), GetSkyLight(x, y, z));
        }

        public void SetBlock(Block block)
        {
            SetBlockStateId(block.X, block.Y, block.Z, block.StateId);
            SetBiome(block.X, block.Y, block.Z, block.Biome);
            SetBlockLight(block.X, block.Y, block.Z, block.Light);
            SetSkyLight(block.X, block.Y, block.Z, block.Skylight);
        }

        public Biome GetBiome(int x, int y, int z)
        {
            if (y < 0 || y > 255)
                return Biome.TheVoid;
            return Biomes[GetBiomeIdx(x, y, z)];
        }

        public void SetBiome(int x, int y, int z, Biome biome)
        {
            if (y < 0 || y > 255)
                return;
            Biomes[GetBiomeIdx(x, y, z)] = biome;
        }

        public int GetBlockLight(int x, int y, int z)
        {
            return BlockLightSections[GetLightSectionIdx(y)] != null
                ? BlockLightSections[GetLightSectionIdx(y)].Get(GetChunkRelBlockIdx(x, y, z))
                : 0;
        }
        
        public int GetSkyLight(int x, int y, int z)
        {
            return SkyLightSections[GetLightSectionIdx(y)] != null
                ? SkyLightSections[GetLightSectionIdx(y)].Get(GetChunkRelBlockIdx(x, y, z))
                : 0;
        }

        public void SetBlockStateId(int x, int y, int z, int blockStateId)
        {
            if (y < 0 || y > 255)
                return;

            int chunkSectionIdx = GetChunkSectionIdx(y);
            if (Sections[chunkSectionIdx] == null)
            {
                if (blockStateId == 0)
                    return;
                
                Sections[chunkSectionIdx] = new ChunkSection();
                SectionMask |= 1 << chunkSectionIdx;
            }
            
            Sections[chunkSectionIdx].SetBlock(x, GetChunkRelHeight(y), z, blockStateId);
        }

        public void SetBlockLight(int x, int y, int z, int blockLight)
        {
            int chunkSectionIdx = GetChunkSectionIdx(y);
            if (BlockLightSections[chunkSectionIdx] == null)
            {
                if (blockLight == 0)
                    return;
                
                BlockLightSections[chunkSectionIdx] = new BitStorage(4, ChunkSection.Volume);
                BlockLightMask |= 1 << chunkSectionIdx;
            }
            
            BlockLightSections[chunkSectionIdx].Set(GetChunkRelBlockIdx(x, y, z), blockLight);
        }

        public void SetSkyLight(int x, int y, int z, int skyLight)
        {
            int chunkSectionIdx = GetChunkSectionIdx(y);
            if (SkyLightSections[chunkSectionIdx] == null)
            {
                if (skyLight == 0)
                    return;
                
                SkyLightSections[chunkSectionIdx] = new BitStorage(4, ChunkSection.Volume);
                SkyLightMask |= 1 << chunkSectionIdx;
            }
            
            SkyLightSections[chunkSectionIdx].Set(GetChunkRelBlockIdx(x, y, z), skyLight);
        }
    }
}