using MapIO.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapIO.TSK
{
    public class TskDataAccessor
    {
        readonly TskData _source;
        public TskDataAccessor(TskData tskData)
        {
            _source = tskData;
        }

        public int MapVersion => _source.HeaderInformationSection.MapVersion;
        public int RowCount
        {
            get => _source.HeaderInformationSection.MapDataAreaLineSize;
            set => _source.HeaderInformationSection.MapDataAreaLineSize = value;
        }
        public int ColCount
        {
            get => _source.HeaderInformationSection.MapDataAreaRowSize;
            set => _source.HeaderInformationSection.MapDataAreaRowSize = value;
        }
        public int MapSize => RowCount * ColCount;
        public MapFileConfiguration Configuration => _source.HeaderInformationSection.MapFileConfiguration;

        public List<DieState> DieStates
        {
            get
            {
                if (MapVersion == 1) return _source.TestResultPerDieV1List.Value.Select(x => x.State).ToList();
                return _source.TestResultPerDieList.Value.Select(x => x.State).ToList();
            }
            set
            {
                var mapVersion = MapVersion;
                if (mapVersion == 1) _source.TestResultPerDieV1List.Value.Zip(value).ForEach(x => x.First.State = x.Second);
                else _source.TestResultPerDieList.Value.Zip(value).ForEach(x => x.First.State = x.Second);
            }
        }

        public int FirstDieCoordX
        {
            get => _source.HeaderInformationSection.FirstDieCoordinatorX;
            set
            {
                _source.HeaderInformationSection.FirstDieCoordinatorX = value;
                if (MapVersion != 1)
                {
                    var die = _source.TestResultPerDieList.Value.First();
                    die.DieCoordinatorValueX = value;
                }
            }
        }

        public int FirstDieCoordY
        {
            get => _source.HeaderInformationSection.FirstDieCoordinatorY;
            set
            {
                _source.HeaderInformationSection.FirstDieCoordinatorY = value;
                if (MapVersion != 1)
                {
                    var die = _source.TestResultPerDieList.Value.First();
                    die.DieCoordinatorValueY = value;
                }
            }
        }

        public List<int> Categories
        {
            get
            {
                var mapVersion = MapVersion;
                if (mapVersion == 0) return _source.TestResultPerDieList.Value.Select(x => x.CategoryData).ToList();
                if (mapVersion == 1) return _source.TestResultPerDieV1List.Value.Select(x => x.CategoryData).ToList();

                if (!Configuration.TestResultInformationPerExtensionDie) return _source.TestResultPerDieList.Value.Select(x => x.CategoryData).ToList();

                if (mapVersion == 2) return _source.ExtendedTestResultPerDieV2List.Value.Select(x => x.ExtendedBinaryCategory).ToList();
                if (mapVersion == 3) return _source.ExtendedTestResultPerDieV3List.Value.Select(x => x.ExtendedBinaryCategory).ToList();
                if (mapVersion == 4) return _source.ExtendedTestResultPerDieV4List.Value.Select(x => x.ExtendedBinaryCategory).ToList();
                if (mapVersion == 5) return _source.ExtendedTestResultPerDieV5List.Value.Select(x => x.ExtendedBinaryCategory).ToList();
                if (mapVersion == 7) return _source.ExtendedTestResultPerDieV7List.Value.Select(x => x.ExtendedBinaryCategory).ToList();

                throw new NotImplementedException($"Unsupported map version {MapVersion}");
            }
            set
            {
                var mapVersion = MapVersion;
                if (mapVersion == 1) _source.TestResultPerDieV1List.Value.Zip(value).ForEach(x => x.First.CategoryData = x.Second);
                else
                {
                    _source.TestResultPerDieList.Value.Zip(value).ForEach(x => x.First.CategoryData = x.Second);
                    if (Configuration.TestResultInformationPerExtensionDie)
                    {
                        if (mapVersion == 2) _source.ExtendedTestResultPerDieV2List.Value.Zip(value).ForEach(x => x.First.ExtendedBinaryCategory = x.Second);
                        else if (mapVersion == 3) _source.ExtendedTestResultPerDieV3List.Value.Zip(value).ForEach(x => x.First.ExtendedBinaryCategory = x.Second);
                        else if (mapVersion == 4) _source.ExtendedTestResultPerDieV4List.Value.Zip(value).ForEach(x => x.First.ExtendedBinaryCategory = x.Second);
                        else if (mapVersion == 5) _source.ExtendedTestResultPerDieV5List.Value.Zip(value).ForEach(x => x.First.ExtendedBinaryCategory = x.Second);
                        else if (mapVersion == 7) _source.ExtendedTestResultPerDieV7List.Value.Zip(value).ForEach(x => x.First.ExtendedBinaryCategory = x.Second);
                    }
                }
            }
        }

        public DirectionX CoordDirectionX
        {
            get
            {
                switch (_source.HeaderInformationSection.XCoordinatesIncreaseDirection)
                {
                    case 1: return DirectionX.Leftward;
                    case 2: return DirectionX.Rightward;
                    default: throw new NotImplementedException();
                };
            }
            set => _source.HeaderInformationSection.XCoordinatesIncreaseDirection = (int)value;
        }

        public DirectionY CoordDirectionY
        {
            get
            {
                switch (_source.HeaderInformationSection.YCoordinatesIncreaseDirection)
                {
                    case 1: return DirectionY.Forward;
                    case 2: return DirectionY.Backward;
                    default: throw new NotImplementedException();
                }
            }
            set => _source.HeaderInformationSection.YCoordinatesIncreaseDirection = (int)value;
        }

        public int GetCategory(int index)
        {
            throw new NotImplementedException();
        }

        public int GetCategory(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
