using MapIO.Core.Binary;

namespace MapIO.TSK
{
    public class ExtendedTestResultPerDieV1 : BinarySection
    {
        public static readonly long SIZE = 1;

        public ExtendedTestResultPerDieV1(BinarySection source, long offset) : base(source, offset)
        {
        }

        //[Field(Offset = 0, Length = 1, Type = TypeCode.Byte)]
        public virtual int Byte1st
        {
            get => ReadAsByte(0);
            set => WriteAsByte(0, (byte)value);
        }


        public int Spare
        {
            get => Byte1st >> 3 & 0b1_1111;
            set => Byte1st = (Byte1st & 0b0000_0111) | (value & 0b1_1111) << 3;
        }
        public int Used4SysWork
        {
            get => Byte1st >> 2 & 0b1;
            set => Byte1st = (Byte1st & 0b1111_1011) | (value & 1) << 2;
        }
        public int ReProbingResult
        {
            get => Byte1st & 0b11;
            set => Byte1st = (Byte1st & 0b1111_1100) | (value & 0b11);
        }
    }
}
