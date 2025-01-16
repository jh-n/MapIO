using MapIO.Core.Binary;
using System;

namespace MapIO.TSK
{
    public class TestResultPerDie : BinarySection
    {
        public static readonly long SIZE = 6;
        protected override long Size { get; } = SIZE;
        public TestResultPerDie(BinarySection source, long offset) : base(source, offset)
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


        /// <summary>
        /// 0: Not Tested / 1: Pass Die / 2: Fail 1 Die / 3: Fail 2 Die
        /// </summary>
        public int DieTestResult
        {
            get => (Word1st >> 14 & 0b11);
            set
            {
                Word1st &= 0b0011_1111_1111_1111;
                Word1st |= (value & 0b11) << 14;
            }
        }
        public int Marking
        {
            get => Word1st >> 13 & 0b1;
            set
            {
                Word1st &= 0b1101_1111_1111_1111;
                Word1st |= (value & 0b1) << 13;
            }
        }
        public int FailMarkInspection
        {
            get => Word1st >> 12 & 0b1;
            set
            {
                Word1st &= 0b1110_1111_1111_1111;
                Word1st |= (value & 0b1) << 12;
            }
        }
        /// <summary>
        /// 0: Not Re-Probed <br/>
        /// 1: Passed at re-probing <br/>
        /// 2: Failed at re-probing <br/>
        /// 3: Perform fail(for special user) <br/>
        /// </summary>
        public int ReProbingResult
        {
            get => Word1st >> 10 & 0b11;
            set
            {
                Word1st &= 0b1111_0011_1111_1111;
                Word1st |= (value & 0b11) << 10;
            }
        }
        public int NeedleMarkInspectionResult
        {
            get => Word1st >> 9 & 0b1;
            set
            {
                Word1st &= 0b1111_1101_1111_1111;
                Word1st |= (value & 0b1) << 9;
            }
        }
        public int DieCoordinatorValueX
        {
            get => Word1st & 0b1_1111_1111;
            set
            {
                Word1st &= 0b1111_1110_0000_0000;
                Word1st |= value & 0b1_1111_1111;
            }
        }
        /// <summary>
        /// 0: Skip Die <br/>
        /// 1: Probing Die <br/>
        /// 2: Compulsory Marking Die 
        /// </summary>
        public int DieProperty
        {
            get => (Word2nd >> 14 & 0b11);
            set
            {
                Word2nd &= 0b0011_1111_1111_1111;
                Word2nd |= (value & 0b11) << 14;
            }
        }
        public int NeedleMarkInspectionExecutionDieSelection
        {
            get => Word2nd >> 13 & 0b1;
            set
            {
                Word2nd &= 0b1101_1111_1111_1111;
                Word2nd |= (value & 0b1) << 13;
            }
        }
        public int SamplingDie
        {
            get => Word2nd >> 12 & 0b1;
            set
            {
                Word2nd &= 0b1110_1111_1111_1111;
                Word2nd |= (value & 0b1) << 12;
            }
        }
        public int CodeBitOfCorrdinatorValueX
        {
            get => Word2nd >> 11 & 0b1;
            set
            {
                Word2nd &= 0b1111_0111_1111_1111;
                Word2nd |= (value & 0b1) << 11;
            }
        }
        public int CodeBitOfCorrdinatorValueY
        {
            get => Word2nd >> 10 & 0b1;
            set
            {
                Word2nd &= 0b1111_1011_1111_1111;
                Word2nd |= (value & 0b1) << 10;
            }
        }
        /// <summary>
        /// Dummy Data (except wafer)
        /// </summary>
        public int DummyData
        {
            get => Word2nd >> 9 & 0b1;
            set
            {
                Word2nd &= 0b1111_1101_1111_1111;
                Word2nd |= (value & 0b1) << 9;
            }
        }
        public int DieCoordinatorValueY
        {
            get => Word2nd & 0b1_1111_1111;
            set
            {
                Word2nd &= 0b1111_1110_0000_0000;
                Word2nd |= value;
            }
        }
        public int MeasurementFinishFlag
        {
            get => Word3rd >> 15 & 0b1;
            set
            {
                Word3rd &= 0b0111_1111_1111_1111;
                Word3rd |= (value & 0b1) << 15;
            }
        }
        public int RejectChipFlag
        {
            get => Word3rd >> 14 & 0b1;
            set
            {
                Word3rd &= 0b1011_1111_1111_1111;
                Word3rd |= (value & 0b1) << 14;
            }
        }
        /// <summary>
        /// Test Execution Site No. (0 to 63)
        /// </summary>
        public int TestExecutionSiteNo
        {
            get => Word3rd >> 8 & 0b11_1111;
            set
            {
                Word3rd &= 0b1100_0000_1111_1111;
                Word3rd |= (value & 0b11_1111) << 8;
            }
        }
        public int BlockAreaJudegmentFuntion
        {
            get => Word3rd >> 6 & 0b11;
            set
            {
                Word3rd &= 0b1111_1111_0011_1111;
                Word3rd |= (value & 0b11) << 6;
            }
        }
        public int CategoryData
        {
            get => Word3rd & 0b11_1111;
            set
            {
                Word3rd &= 0b1111_1111_1100_0000;
                Word3rd |= value & 0b11_1111;
            }
        }

        public DieState State
        {
            get
            {
                switch (DieProperty)
                {
                    case 0: return DieState.Skip;
                    case 1:
                        switch (DieTestResult)
                        {
                            case 1: return DieState.Pass;
                            case 2: return DieState.Fail;
                            default: throw new NotImplementedException();
                        }
                    case 2: return DieState.Marking;
                    default: throw new NotImplementedException();
                }
            }
            set
            {
                switch (value)
                {
                    case DieState.Pass:
                        DieTestResult = 1;
                        DieProperty = 1;
                        DummyData = 0;
                        break;
                    case DieState.Fail:
                        DieTestResult = 2;
                        DieProperty = 1;
                        DummyData = 0;
                        ReProbingResult = 2;
                        break;
                    case DieState.Marking:
                        DieTestResult = 0;
                        DieProperty = 2;
                        DummyData = 0;
                        break;
                    //case DieState.Test:
                    //    DieTestResult = 0;
                    //    DieProperty = 1;
                    //    break;
                    case DieState.Skip:
                        DieTestResult = 0;
                        DieProperty = 0;
                        DummyData = 1;
                        FailMarkInspection = 1;
                        break;
                }
            }
        }
    }
}
