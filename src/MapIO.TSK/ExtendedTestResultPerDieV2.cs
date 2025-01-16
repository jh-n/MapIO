using MapIO.Core.Binary;

namespace MapIO.TSK
{

    public class ExtendedTestResultPerDieV2 : BinarySection, IHaveExtendedBinaryCategory, IHaveExtendedTestExecutionSite
    {
        public static readonly int SIZE = 4;

        public ExtendedTestResultPerDieV2(BinarySection source, long offset) : base(source, offset) { }

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


        /// <summary>
        /// 0 to 255 (add 1 for actual test execution site.)
        /// </summary>
        public int ExtendedTestExecutionSite
        {
            get => Word1st >> 8 & 0b1111_1111;
            set => Word1st = Word1st & 0b1111_1111 | (value & 0b1111_1111) << 8;
        }

        public int ExtendedBinaryCategory
        {
            get => Word1st & 0b1111_1111;
            set => Word1st = Word1st & 0b1111_1111_0000_0000 | value & 0b1111_1111;
        }

    }
}