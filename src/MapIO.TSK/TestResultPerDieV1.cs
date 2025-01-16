using MapIO.Core.Binary;
using System;

namespace MapIO.TSK
{
    /// <summary>
    /// 2 bytes
    /// </summary>
    public class TestResultPerDieV1 : BinarySection
    {
        public static readonly long SIZE = 2;
        public TestResultPerDieV1(BinarySection source, long offset) : base(source, offset) { }

        //[Field(Offset = 0, Length = 1, Type = TypeCode.Byte)]
        public virtual int Byte1st
        {
            get => ReadAsByte(0);
            set => WriteAsByte(0, (byte)value);
        }
        //[Field(Offset = 1, Length = 1, Type = TypeCode.Byte)]
        public virtual int Byte2nd
        {
            get => ReadAsByte(1);
            set => WriteAsByte(1, (byte)value);
        }

        /// <summary>
        /// 0: Not Tested / 1: Passed Die / 2: Failed 1 Die / 3: Failed 2 Die
        /// </summary>
        public int DieTestResult
        {
            get => Byte1st >> 6 & 0b11;
            set
            {
                Byte1st &= 0b0011_1111;
                Byte1st |= (value & 0b11) << 6;
            }
        }
        /// <summary>
        /// Category Data (0 to 63) add 1 for actual data value
        /// </summary>
        public int CategoryData
        {
            get => Byte1st & 0b11_1111;
            set
            {
                Byte1st &= 0b1100_0000;
                Byte1st |= value & 0b11_1111;
            }
        }

        /// <summary>
        /// 0: No / 1: Yes (After marking: 1)
        /// </summary>
        public int Marking
        {
            get => Byte2nd >> 7 & 0b1;
            set
            {
                Byte2nd &= 0b0111_1111;
                Byte2nd |= (value & 0b1) << 7;
            }
        }
        public int Spare
        {
            get => Byte2nd >> 6 & 0b1;
            set
            {
                Byte2nd &= 0b1011_1111;
                Byte2nd |= (value & 0b1) << 6;
            }
        }
        /// <summary>
        /// Test Execution Site Number (0 to 63) add 1 for actual data value
        /// </summary>
        public int TestExecutionSiteNo
        {
            get => Byte2nd & 0b11_1111;
            set
            {
                Byte2nd &= 0b1100_0000;
                Byte2nd |= value & 0b11_1111;
            }
        }


        public DieState State
        {
            get => default;
            set
            {
                switch (value)
                {
                    case DieState.Pass:
                        DieTestResult = 1;
                        break;
                    case DieState.Fail:
                        DieTestResult = 2;
                        break;
                    case DieState.Skip:
                        DieTestResult = 0;
                        break;
                    default: throw new NotImplementedException($"Unsupported DieState {Enum.GetName(typeof(DieState), value)}.");
                }
            }
        }
    }
}
