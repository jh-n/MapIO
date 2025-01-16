using MapIO.Core.Binary;

namespace MapIO.TSK
{
    public class LineCategoryPerDie : BinarySection
    {
        public static readonly long SIZE = 8;
        public LineCategoryPerDie(BinarySection source, long offset) : base(source, offset)
        {
        }

        //[Field(Offset = 0, Length = 2, Type = TypeCode.UInt16)]
        public virtual int Word1st
        {
            get => ReadAsUInt16(0, 2);
            set => WriteAsUInt16(0, (ushort)value);
        }
        //[Field(Offset = 2, Length = 2, Type = TypeCode.UInt16)]
        public virtual int Word2nd
        {
            get => ReadAsUInt16(2, 2);
            set => WriteAsUInt16(2, (ushort)value);
        }
        //[Field(Offset = 4, Length = 2, Type = TypeCode.UInt16)]
        public virtual int Word3rd
        {
            get => ReadAsUInt16(4, 2);
            set => WriteAsUInt16(4, (ushort)value);
        }
        //[Field(Offset = 6, Length = 2, Type = TypeCode.UInt16)]
        public virtual int Word4th
        {
            get => ReadAsUInt16(6, 2);
            set => WriteAsUInt16(6, (ushort)value);
        }
    }
}
