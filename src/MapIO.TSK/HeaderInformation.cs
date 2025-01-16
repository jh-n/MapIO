using MapIO.Core.Binary;
using System;

namespace MapIO.TSK
{
    public class HeaderInformation : BinarySection
    {
        public static readonly long SIZE = 236;

        public HeaderInformation(BinarySection source, long offset) : base(source, offset)
        {
        }

        // Wafer Testing Setup Data
        [Field(Offset = 0, Length = 20, Type = TypeCode.String)]
        public virtual string OperatorName { get; set; }
        [Field(Offset = 20, Length = 16, Type = TypeCode.String)]
        public virtual string DeviceName { get; set; }
        [Field(Offset = 36, Length = 2, Type = TypeCode.UInt16)]
        public virtual int WaferSize { get; set; }
        [Field(Offset = 38, Length = 2, Type = TypeCode.UInt16)]
        public virtual int MachineNo { get; set; }
        [Field(Offset = 40, Length = 4, Type = TypeCode.Int32)]
        public virtual uint IndexSizeX { get; set; }
        [Field(Offset = 44, Length = 4, Type = TypeCode.UInt32)]
        public virtual uint IndexSizeY { get; set; }
        [Field(Offset = 48, Length = 2, Type = TypeCode.UInt16)]
        public virtual int OrientationFlatDirection { get; set; }
        [Field(Offset = 50, Length = 1, Type = TypeCode.Byte)]
        public virtual int FinalEditingMachineType { get; set; }


        /// <summary>
        /// 0: Normal <br/>
        /// 1: 250,000 Chips <br/>
        /// 2: 256 Multi-sites <br/>
        /// 3: 256 Multi-sites(without extended header information) <br/>
        /// 4: 1024 category <br/>
        /// 5: 2048 (256 over9 multi-sites <br/>
        /// 6: Extended Header 2
        /// 7: 7: 9999 category
        /// </summary>
        [Field(Offset = 51, Length = 1, Type = TypeCode.Byte)]
        public virtual int MapVersion { get; set; }

        /// <summary>
        /// Column Count
        /// </summary>
        [Field(Offset = 52, Length = 2, Type = TypeCode.UInt16)] public virtual int MapDataAreaRowSize { get; set; }
        // describe how many elements on each row in the map data area
        /// <summary>
        /// Row Count
        /// </summary>
        [Field(Offset = 54, Length = 2, Type = TypeCode.UInt16)] public virtual int MapDataAreaLineSize { get; set; }
        /// <summary>
        /// 0:6 byte, 1:1 byte, 2:2 byte, 3:3 byte
        /// </summary>
        [Field(Offset = 56, Length = 4, Type = TypeCode.Byte)] public virtual uint MapDataForm { get; set; }

        // Wafer Specific Data
        [Field(Offset = 60, Length = 21, Type = TypeCode.String)] public virtual string WaferId { get; set; }
        [Field(Offset = 81, Length = 1, Type = TypeCode.Byte)] public virtual int NumberOfProbing { get; set; }
        [Field(Offset = 82, Length = 18, Type = TypeCode.String)] public virtual string LotNo { get; set; }
        [Field(Offset = 100, Length = 2, Type = TypeCode.UInt16)] public virtual int CassetteNo { get; set; } = 1;
        [Field(Offset = 102, Length = 2, Type = TypeCode.UInt16)] public virtual int SlotNo { get; set; }


        /// <summary>
        /// 1: leftward 2: rightward
        /// </summary>
        [Field(Offset = 104, Length = 1, Type = TypeCode.Byte)] public virtual int XCoordinatesIncreaseDirection { get; set; }
        /// <summary>
        /// 1: forward 2: backward 
        /// </summary>
        [Field(Offset = 105, Length = 1, Type = TypeCode.Byte)] public virtual int YCoordinatesIncreaseDirection { get; set; }
        /// <summary>
        /// 1: Wafer center die <br/>
        /// 2: Teaching die <br/>
        /// 3: Target sense die
        /// </summary>
        [Field(Offset = 106, Length = 1, Type = TypeCode.Byte)] public virtual int ReferenceDieSettingProcedures { get; set; }

        [Field(Offset = 108, Length = 4, Type = TypeCode.UInt32)] public virtual int TargetDiePositionX { get; set; }
        [Field(Offset = 112, Length = 4, Type = TypeCode.UInt32)] public virtual int TargetDiePositionY { get; set; }
        [Field(Offset = 116, Length = 2, Type = TypeCode.UInt32)] public virtual ushort ReferenceDieCoordinatorX { get; set; }
        [Field(Offset = 118, Length = 2, Type = TypeCode.UInt32)] public virtual ushort ReferenceDieCoordinatorY { get; set; }
        [Field(Offset = 120, Length = 1, Type = TypeCode.UInt32)] public virtual ushort ProbingStartPosition { get; set; }
        /// <summary>
        /// 1: leftward 2: rightward 3: upward 4: backward
        /// </summary>
        [Field(Offset = 121, Length = 1, Type = TypeCode.Byte)] public virtual int ProbingDirection { get; set; }

        [Field(Offset = 124, Length = 4, Type = TypeCode.UInt32)] public virtual int DistanceXtoWaferCenterDieOrigin { get; set; }
        [Field(Offset = 128, Length = 4, Type = TypeCode.UInt32)] public virtual int DistanceYtoWaferCenterDieOrigin { get; set; }
        [Field(Offset = 132, Length = 4, Type = TypeCode.Int32)] public virtual int CoordinatorXofWaferCenterDie { get; set; }
        [Field(Offset = 136, Length = 4, Type = TypeCode.Int32)] public virtual int CoordinatorYofWaferCenterDie { get; set; }
        [Field(Offset = 140, Length = 4, Type = TypeCode.Int32)] public virtual int FirstDieCoordinatorX { get; set; }
        [Field(Offset = 144, Length = 4, Type = TypeCode.Int32)] public virtual int FirstDieCoordinatorY { get; set; }


        [Field(Offset = 196, Length = 4, Type = TypeCode.UInt16)] public virtual int VegaMachineNo { get; set; }
        [Field(Offset = 200, Length = 4, Type = TypeCode.UInt16)] public virtual int VegaMachineNo2 { get; set; }
        [Field(Offset = 204, Length = 4, Type = TypeCode.UInt16)] public virtual int SepcialCharacters { get; set; }
        /// <summary>
        /// 0: Normal End <br/>
        /// 1: Yield NG <br/>
        /// 2: Continuous FAIL NG <br/>
        /// 3: Manual Unload
        /// </summary>
        [Field(Offset = 208, Length = 1, Type = TypeCode.Byte)] public virtual byte TestingEndInformation { get; set; }

        [Field(Offset = 210, Length = 2, Type = TypeCode.UInt16)] public virtual int TotalTestedDice { get; set; }
        [Field(Offset = 212, Length = 2, Type = TypeCode.UInt16)] public virtual int TotalPassDice { get; set; }
        [Field(Offset = 214, Length = 2, Type = TypeCode.UInt16)] public virtual int TotalFailDice { get; set; }
        [Field(Offset = 216, Length = 4, Type = TypeCode.UInt32)] public virtual uint TestDieInformationAddress { get; set; }
        [Field(Offset = 220, Length = 4, Type = TypeCode.UInt16)] public virtual int NumberOfLineCategoryData { get; set; }
        [Field(Offset = 224, Length = 4, Type = TypeCode.UInt16)] public virtual int LineCategoryAddress { get; set; }
        /// <summary>
        /// Map file configuration is effective with map version 2 (256 multi) and later.
        /// </summary>
        [Field(Offset = 228, Length = 2, Type = TypeCode.Object)] public virtual MapFileConfiguration MapFileConfiguration { get; set; }
        [Field(Offset = 230, Length = 2, Type = TypeCode.UInt16)] public virtual int MaxMultiSite { get; set; }
        [Field(Offset = 232, Length = 2, Type = TypeCode.UInt16)] public virtual int MaxCategories { get; set; }
    }
}
