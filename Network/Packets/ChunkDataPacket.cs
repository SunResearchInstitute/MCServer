using System;
using fNbt;

namespace MCServer.Network.Packets
{
    public class ChunkDataPacket : Packet
    {
        public ChunkDataPacket(Client client, ChunkColumn column) : base(client)
        {
            PacketId = 0x22;
            MCSerializer serializer = new MCSerializer();


            serializer.WriteInt(column.X);
            serializer.WriteInt(column.Z);
            serializer.WriteBool(true);
            serializer.WriteVarInt(column.SectionMask);
            NbtWriter heightmapWriter = new NbtWriter(serializer.Stream, "root");
            BitStorage heightmapStorage = new BitStorage(9, 256);
            for (int y = 0; y < 256; y++)
                for (int z = 0; z < 16; z++)
                    for (int x = 0; x < 16; x++)
                        if (column.GetBlock(x, y, z).StateId != 0)
                            heightmapStorage.Set(ChunkColumn.GetHeightmapIdx(x, z), y);
            
            heightmapWriter.WriteLongArray("MOTION_BLOCKING", heightmapStorage.Data);
            heightmapWriter.EndCompound();
            heightmapWriter.Finish();
            
            for (int y = 0; y < 256; y += 4)
                for (int z = 0; z < 16; z += 4)
                    for (int x = 0; x < 16; x += 4)
                        serializer.WriteInt((int)column.GetBiome(x, y, z));

            MCSerializer dataSerializer = new MCSerializer();
            int primaryBitMask = column.SectionMask;

            for (int y = 0; y < 16; y++)
            {
                if ((primaryBitMask & 0b1) == 1)
                    column.Sections[y].Write(dataSerializer);
                
                primaryBitMask >>= 1;
            }
            
            byte[] encodedChunkData = dataSerializer.GetBytes();
            serializer.WriteVarInt(encodedChunkData.Length);
            serializer.WriteBytes(encodedChunkData);
            
            // TODO: Block Entities
            serializer.WriteVarInt(0);
            Data = serializer.GetBytes();
        }
    }
}