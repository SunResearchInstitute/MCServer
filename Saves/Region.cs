using System;
using System.IO;
using System.IO.Compression;
using fNbt;
using Ionic.Zlib;
using MCServer.Network;
using CompressionMode = Ionic.Zlib.CompressionMode;

namespace MCServer.Saves
{
    public class Region
    {
        private enum CompressionScheme
        {
            Gzip = 1,
            Zlib
        }
        
        public Region(int x, int y)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            using (FileStream data = File.Open($"./world/region/r-{x}-{y}.mca", FileMode.Open))
            {
                Console.WriteLine("test");
                BinaryReader reader = new BinaryReader(data);
                int compressedChunkLength = (int)reader.ReadUInt32();
                CompressionScheme compressionScheme = (CompressionScheme) reader.ReadByte();
                Stream dataStream;
                if (compressionScheme == CompressionScheme.Zlib)
                    dataStream = new ZlibStream(data, CompressionMode.Decompress);
                else
                {
                    throw new NotImplementedException("GZIP not implemented");
                }
            
                NbtReader nbtReader = new NbtReader(dataStream);
                Console.WriteLine($"Name: {nbtReader.ReadAsTag().Name}");
            }
        }
    }
}