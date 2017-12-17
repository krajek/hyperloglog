using System;

namespace HLLCardinalityEstimator
{
    public interface IHyperLogLog : IHyperLogLogCore
    {
        void AddByte(byte value);
        void AddInt16(Int16 value);
        void AddInt32(Int32 value);
        void AddInt64(Int64 value);
        void AddUTF8String(string value);
        void AddGuid(Guid value);
        void AddDateTime(DateTime value);
        void AddBytes(byte[] valueBytes);
    }
}