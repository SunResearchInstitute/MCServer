using System;
using System.Collections;
using System.Collections.Generic;
using MCServer.Network;

namespace MCServer
{
    public class ChunkSection
    {
        public const int Width = 16;
        public const int Height = 16;
        public const int Volume = Height * Width * Width;
        public const int PalleteBitsPerVarMax = 8;
        public const int GlobalPalleteBitsPerVar = 14;

        public static int GetBlockIndex(int x, int y, int z)
        {
            return (y << 8) | (z << 4) | x;
        }
        
        public short SolidBlockCount;
        public BitStorage Data;
        public List<int> Pallete;
        public ChunkSection()
        {
            Data = new BitStorage(4, Volume);
            Pallete = new List<int>();
            Pallete.Add(0);
        }

        public ChunkSection(BitStorage data, short solidBlockCount = 0, List<int> pallete = null)
        {
            Data = data;
            SolidBlockCount = solidBlockCount;
            Pallete = pallete;
        }

        public int GetBlock(int x, int y, int z)
        {
            int stateId = Data.Get(GetBlockIndex(x, y, z));
            if (Pallete != null)
                stateId = Pallete[stateId];
            return stateId;
        }

        public void SetBlock(int x, int y, int z, int blockStateId)
        {
            if (Pallete != null)
            {
                if (Pallete.Contains(blockStateId))
                    blockStateId = Pallete.IndexOf(blockStateId);
                else
                {
                    Pallete.Add(blockStateId);
                    blockStateId = Pallete.Count - 1;
                    byte bitsNeeded = Utils.BitsNeededToStore(blockStateId);
                    if (bitsNeeded > Data.BitsPerVar)
                    {
                        if (bitsNeeded > PalleteBitsPerVarMax)
                        {
                            BitStorage newStorage = new BitStorage(PalleteBitsPerVarMax, Volume);
                            for (int i = 0; i < Volume; i++)
                                newStorage.Set(i, Pallete[Data.Get(i)]);

                            Pallete = null; 
                            Data = newStorage;
                        }
                        else
                        {
                            BitStorage newStorage = new BitStorage(bitsNeeded, Volume);
                            for (int i = 0; i < Volume; i++)
                                newStorage.Set(i, Data.Get(i));

                            Data = newStorage;
                        }
                    }
                }
            }

            int oldBlock = GetBlock(x, y, z);
                if (oldBlock == 0 && blockStateId != 0)
                SolidBlockCount++;
            else if (oldBlock != 0 && blockStateId == 0)
                SolidBlockCount--;
            
            Data.Set(GetBlockIndex(x, y, z), blockStateId);
        }

        public void Write(MCSerializer serializer)
        {
            serializer.WriteShort(SolidBlockCount);
            serializer.WriteByte(Data.BitsPerVar);
            if (Pallete != null)
            {
                serializer.WriteVarInt(Pallete.Count);
                foreach (int val in Pallete)
                    serializer.WriteVarInt(val);
            }
            serializer.WriteVarInt(Data.Data.Length);
            foreach (long val in Data.Data)
                serializer.WriteLong(val);
        }

        public bool IsEmpty()
        {
            return SolidBlockCount == 0;
        }
    }
}