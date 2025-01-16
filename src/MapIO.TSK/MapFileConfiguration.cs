using MapIO.Core.Binary;
using System;

namespace MapIO.TSK
{
    public class MapFileConfiguration : BinarySection
    {
        public MapFileConfiguration(BinarySection source, long offset) : base(source, offset) { }

        [Field(Offset = 0, Length = 2, Type = TypeCode.UInt16)]
        public virtual int Word { get; set; }

        /// <summary>
        /// always ON
        /// </summary>
        public bool HeaderInformation
        {
            get => (Word & 0b1) == 1;
            set
            {
                Word &= 0b1111_1111_1111_1110;
                Word |= value ? 1 : 0;
            }
        }

        /// <summary>
        /// always ON
        /// </summary>
        public bool TestResultInformationPerDie
        {
            get => (Word >> 1 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1111_1101;
                Word |= value ? 0b10 : 0;
            }
        }

        public bool LineCategoryInformation
        {
            get => (Word >> 2 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1111_1011;
                Word |= value ? 0b100 : 0;
            }
        }

        public bool ExtensionHeaderInformation
        {
            get => (Word >> 3 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1111_0111;
                Word |= value ? 0b1000 : 0;
            }
        }

        public bool TestResultInformationPerExtensionDie
        {
            get => (Word >> 4 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1110_1111;
                Word |= value ? 0b1_0000 : 0;
            }
        }

        public bool ExtensionLineCategoryInformation
        {
            get => (Word >> 5 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1101_1111;
                Word |= value ? 0b10_0000 : 0;
            }
        }

        /// <summary>
        /// for special users
        /// </summary>
        public bool TextXtrFile
        {
            get => (Word >> 6 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_1011_1111;
                Word |= value ? 0b100_0000 : 0;
            }
        }

        public bool CspWaferHeaderInformation
        {
            get => (Word >> 7 & 1) == 1;
            set
            {
                Word &= 0b1111_1111_0111_1111;
                Word |= value ? 0b1000_0000 : 0;
            }
        }

        /// <summary>
        /// 2048(over 256) multi site information
        /// </summary>
        public bool MultiSiteInformation
        {
            get => (Word >> 8 & 1) == 1;
            set
            {
                Word &= 0b1111_1110_1111_1111;
                Word |= value ? 0b1_0000_0000 : 0;
            }
        }

        public bool ExtendedHeader2Information
        {
            get => (Word >> 9 & 1) == 1;
            set
            {
                Word &= 0b1111_1101_1111_1111;
                Word |= value ? 0b10_0000_0000 : 0;
            }
        }

        /// <summary>
        /// (0: Normal / 1:C.S.P map
        /// </summary>
        public int PreviousCspMapType
        {
            get => Word >> 15 & 1;
            set
            {
                Word &= 0b0111_1111_1111_1111;
                Word |= (value & 1) << 15;
            }
        }
    }
}