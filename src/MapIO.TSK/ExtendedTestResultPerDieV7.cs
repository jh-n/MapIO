using MapIO.Core.Binary;

namespace MapIO.TSK
{
    public class ExtendedTestResultPerDieV7 : BinarySection, IHaveExtendedBinaryCategory, IHaveExtendedTestExecutionSite
    {
        public static readonly int SIZE = 4;

        public ExtendedTestResultPerDieV7(BinarySection source, long offset) : base(source, offset) { }

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
        /// 0 to 2047 (add 1 for actual test execution site) <br/>
        /// For memory restriction.Combination of 9999 bin and over 64 multiple Site is not permitted <br/>
        /// The Configration data screen has the restrictions of input value <br/>
        /// </summary>
        public int ExtendedTestExecutionSite
        {
            get => Word1st & 0b1_1111_1111_1111;
            set => Word1st = (Word1st & 0b1110_0000_0000_0000) | (value & 0b1_1111_1111_1111);
        }

        /// <summary>
        /// SOAK TIME Die / PMI Correction Die <br/>
        /// <code>0: No   1: Yes</code>
        /// </summary>
        public int CorrectionExecutionDie
        {
            get => Word2nd >> 15 & 1;
            set => Word2nd = (Word2nd & 0b111_1111_1111_1111) | (value & 1) << 15;
        }

        /// <summary>
        /// Rearranged Die 1/2 Index Cap <br/>
        /// <code>0: No   1: Yes</code>
        /// </summary>
        public int RearrangedDie
        {
            get => Word2nd >> 14 & 1;
            set => Word2nd = (Word2nd & 0b1011_1111_1111_1111) | (value & 1) << 14;
        }

        /// <summary>
        /// 0 to 9998 (add 1 for actual category information)
        /// </summary>
        public int ExtendedBinaryCategory
        {
            get => Word2nd & 0b11_1111_1111_1111;
            set => Word2nd = (Word2nd & 0b1100_0000_0000_0000) | (value & 0b11_1111_1111_1111);
        }
    }
}