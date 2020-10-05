using System;
using System.Collections;
using System.Collections.Generic;

namespace MCServer.Network
{
    public class BitStorage : IEnumerable<Int32>
    {
        public readonly byte BitsPerVar;
        public readonly int Length;
        public readonly long[] Data;
        private readonly long mask;

        public BitStorage(byte bitsPerVar, int length)
        {
            if (bitsPerVar < 1 || bitsPerVar > 32)
                throw new Exception("Invalid bits per item: " + bitsPerVar);

            Length = length;
            BitsPerVar = bitsPerVar;
            Data = new long[Utils.RoundUp(bitsPerVar * length, 64) / 64];
            mask = (1 << bitsPerVar) - 1;
        }

        public int Get(int idx)
        {
            if (idx >= Length)
                throw new IndexOutOfRangeException();

            int totalBitsOffset = BitsPerVar * idx;
            int longOffset = totalBitsOffset / 64;
            int finalLongOffset = ((idx + 1) * BitsPerVar - 1) / 64;
            int shiftOperand = totalBitsOffset ^ longOffset * 64;
            if (longOffset == finalLongOffset) {
                return (int)((long)((ulong)Data[longOffset] >> shiftOperand) & mask);
            }
            int secondLongShiftOperand = 64 - shiftOperand;
            return (int)(((long)((ulong)Data[longOffset] >> shiftOperand) | Data[finalLongOffset] << secondLongShiftOperand) & mask);
        }

        public void Set(int idx, int value)
        {
            if (idx >= Length)
                throw new IndexOutOfRangeException();
            
            if (value >= mask)
                throw new Exception("Value bigger or queal to mask!");

            int totalBitsOffset = BitsPerVar * idx;
            int longOffset = totalBitsOffset / 64;
            int finalLongOffset = ((idx + 1) * BitsPerVar - 1) / 64;
            int shiftOperand = totalBitsOffset ^ longOffset * 64;
            
            Data[longOffset] = Data[longOffset] & (long)((ulong)mask << shiftOperand ^ 0xFFFFFFFFFFFFFFFFL) | (value & mask) << shiftOperand;
            if (longOffset != finalLongOffset) {
                int secondLongShiftOperand =  64 - shiftOperand;
                int n8 = BitsPerVar - secondLongShiftOperand;
                Data[finalLongOffset] = (long)((ulong)Data[finalLongOffset] >> n8) << n8 | (value & mask) >> secondLongShiftOperand;
            }
        }
        
        public IEnumerator<int> GetEnumerator()
        {
            for (int idx = 0; idx < Length; idx++)
                yield return Get(idx);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}