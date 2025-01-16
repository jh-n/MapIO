using MapIO.Core.Binary;
using System;

namespace MapIO.TSK
{
    public class ExtendedHeaderInformation : BinarySection
    {
        public static readonly long SIZE = 172;
        public ExtendedHeaderInformation(BinarySection source, long offset) : base(source, offset)
        {
        }

        //[Field(Offset = 0, Length = 20, ElementSize = 1)] public virtual byte[] Reserved { get; set; }
        //[Field(Offset = 20, Length = 12)] public virtual TimeData TestStartTime { get; set; } = new();
        //[Field(Offset = 32, Length = 12)] public virtual TimeData TestEndTime { get; set; } = new();
        [Field(Offset = 44, Length = 2, Type = TypeCode.UInt16)] public virtual int ContinuousFail { get; set; }
        //[Field(Offset = 46, Length = 6, ElementSize = 2)] public virtual short[] Reserved { get; set; }
        //[Field(Offset = 52, Length = 12)] public virtual TestResult TestResult { get; set; } = new();
        [Field(Offset = 64, Length = 4, Type = TypeCode.UInt32)] public virtual uint TestedFail1Dice { get; set; }
        [Field(Offset = 68, Length = 4, Type = TypeCode.UInt32)] public virtual uint TestedFail2Dice { get; set; }
        //[Field(Offset = 72, Length = 12)] public virtual TestResult TestResultOfNormalOrMultiPassProbing { get; } = new();
        //[Field(Offset = 84, Length = 12)] public virtual TestResult TestResultAtDirectMultiPassProbing { get; } = new();
        //[Field(Offset = 96, Length = 12, ElementSize = 4)] public virtual int[] Reserved { get; set; }
        //[Field(Offset = 108, Length = 64)] public virtual string Reserved { get; set; }
    }
}
