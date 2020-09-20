using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCServer.Network
{
    public static class Utils
    {
        public static int ReadVarInt(Stream stream)
        {
            int outVal = 0;
            byte readByte;
            int iter = 0;
            do
            {
                int readInt = stream.ReadByte();
                if (readInt == -1)
                    readInt = 0;
                readByte = (byte)readInt;
                outVal |= (readByte & 0b0111_1111) << (iter * 7);
                iter++;
            } while (iter <= 5 && (readByte & (1 << 7)) != 0);

            return outVal;
        }

        public static long ReadVarLong(Stream stream)
        {
            long outVal = 0;
            byte readByte;
            int iter = 0;
            do
            {
                int readInt = stream.ReadByte();
                if (readInt == -1)
                    readInt = 0;
                readByte = (byte)readInt;
                outVal |= (readByte & 0b0111_1111) << (iter * 7);
                iter++;
            } while (iter <= 10 && (readByte & (1 << 7)) != 0);

            return outVal;
        }

        public static String ReadString(Stream stream, int maxLength)
        {
            int length = ReadVarInt(stream);
            byte[] encoded = new byte[length > maxLength ? maxLength : length];
            stream.Read(encoded);
            return Encoding.ASCII.GetString(encoded);
        }

        public static byte[] EncodeVarInt(int value)
        {
            List<byte> bytes = new List<byte>();
            do
            {
                byte lsb = (byte)(value & 0b0111_1111);
                value >>= 7;
                if (value != 0)
                    lsb |= 1 << 7;
                bytes.Add(lsb);
            } while (value != 0);

            return bytes.ToArray();
        }
        
        public static byte[] EncodeVarLong(long value)
        {
            List<byte> bytes = new List<byte>();
            do
            {
                byte lsb = (byte)(value & 0b0111_1111);
                value >>= 7;
                if (value != 0)
                    lsb |= 1 << 7;
                bytes.Add(lsb);
            } while (value != 0);

            return bytes.ToArray();
        }

        public static byte[] EncodeString(String value)
        {
            byte[] encodedLen = EncodeVarInt(value.Length);
            byte[] outBuf = new byte[encodedLen.Length + value.Length];
            byte[] encodedStr = Encoding.ASCII.GetBytes(value);
            Buffer.BlockCopy(encodedLen, 0, outBuf, 0, encodedLen.Length);
            Buffer.BlockCopy(encodedStr, 0, outBuf, encodedLen.Length, encodedStr.Length);
            return outBuf;
        }
    }
}