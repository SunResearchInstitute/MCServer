using System;
using System.IO;
using System.Linq;

namespace MCServer.Network
{
    public class MCSerializer
    {
        private readonly MemoryStream stream = new MemoryStream();
        private readonly BinaryReader reader;
        private readonly BinaryWriter writer;
        
        public MCSerializer()
        {
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public MCSerializer(MemoryStream stream)
        {
            this.stream = stream;
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }
        
        public void WriteByte(byte value)
        {
            writer.Write(value);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public void WriteUShort(ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public ushort ReadUShort()
        {
            byte[] buf = new byte[sizeof(ushort)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToUInt16(buf, 0);
        }

        public void WriteShort(short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public short ReadShort()
        {
            byte[] buf = new byte[sizeof(short)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToInt16(buf, 0);
        }
        
        public void WriteUInt(uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public uint ReadUInt()
        {
            byte[] buf = new byte[sizeof(uint)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToUInt32(buf, 0);
        }

        public void WriteInt(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public int ReadInt()
        {
            byte[] buf = new byte[sizeof(int)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToInt32(buf, 0);
        }
        
        public void WriteULong(ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public ulong ReadULong()
        {
            byte[] buf = new byte[sizeof(ulong)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToUInt64(buf, 0);
        }

        public void WriteLong(long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public long ReadLong()
        {
            byte[] buf = new byte[sizeof(long)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToInt64(buf, 0);
        }

        public void WriteString(String value)
        {
            writer.Write(Utils.EncodeString(value));
        }

        public String ReadString(int maxLength = Int32.MaxValue)
        {
            return Utils.ReadString(stream, maxLength);
        }

        public void WriteVarInt(int value)
        {
            writer.Write(Utils.EncodeVarInt(value));
        }

        public int ReadVarInt()
        {
            return Utils.ReadVarInt(stream);
        }

        public void WriteVarLong(long value)
        {
            writer.Write(Utils.EncodeVarLong(value));
        }

        public long ReadVarLong()
        {
            return Utils.ReadVarLong(stream);
        }

        public void WriteBool(bool value)
        {
            writer.Write(value);
        }

        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }

        public void WriteBytes(Span<byte> value)
        {
            writer.Write(value);
        }
        
        public Span<byte> ReadBytes(long length = -1)
        {
            if (length == -1)
                length =  stream.Length - stream.Position;
            Span<byte> outBuf = new byte[length];
            reader.Read(outBuf);
            return outBuf;
        }

        public void WriteDouble(double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public double ReadDouble()
        {
            byte[] buf = new byte[sizeof(double)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToDouble(buf, 0);
        }

        public void WriteFloat(float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            writer.Write(buffer);
        }

        public float ReadFloat()
        {
            byte[] buf = new byte[sizeof(float)];
            reader.Read(buf);
            Array.Reverse(buf);
            return BitConverter.ToSingle(buf, 0);
        }
        
        public byte[] GetBytes()
        {
            return stream.ToArray();
        }
    }
}