using MapIO.Core;
using MapIO.Core.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapIO.TSK
{
    /// <summary>
    /// MapVersion: 0
    /// <list type="bullet">
    /// <item><see cref="HeaderInformation"/></item>
    /// <item><see cref="TestResultPerDie"/> list</item>
    /// <item><see cref="LineCategoryPerDie"/> list</item>
    /// </list>
    /// MapVersion: 1
    /// <list type="bullet">
    /// <item><see cref="HeaderInformation"/></item>
    /// <item><see cref="ExtendedHeaderInformation"/></item>
    /// <item><see cref="TestResultPerDieV1"/></item>
    /// <item><see cref="ExtendedTestResultPerDieV1"/></item>
    /// </list>
    /// MapVersion: 2~7
    /// <list type="bullet">
    /// <item><see cref="HeaderInformation"/></item>
    /// <item><see cref="TestResultPerDie"/> list</item>
    /// <item>Optional <see cref="LineCategoryPerDie"/> list</item>
    /// <item>Optional <see cref="ExtendedHeaderInformation"/></item>
    /// <item>Optional <see cref="ExtendedTestResultPerDieV2"/> / <see cref="ExtendedTestResultPerDieV3"/> / <see cref="ExtendedTestResultPerDieV4"/> / <see cref="ExtendedTestResultPerDieV5"/> / <see cref="ExtendedTestResultPerDieV7"/> list</item>
    /// </list>
    /// </summary>
    public class TskData : BinarySection, IDisposable
    {
        public TskDataAccessor Accessor { get; }

        public TskData(MemoryStream source) : base(source, 0, false)
        {
            Accessor = new TskDataAccessor(this);

            HeaderInformationSection = ReadAsBinarySectionProxy<HeaderInformation>(0);

            TestResultPerDieList = new Lazi<List<TestResultPerDie>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                IEnumerable<long> offsets;
                if (mapVersion == 0 || mapVersion > 1) offsets = Enumerable.Range(0, mapSize).Select(i => HeaderInformation.SIZE + i * TestResultPerDie.SIZE);
                else return null;
                //return offsets.Select(offset => ReadAsBinarySectionProxy<TestResultPerDie>(offset)).ToList();
                return offsets.Select(offset => new TestResultPerDie(this, offset)).ToList();
            });

            LineCategoryPerDieList = new Lazi<List<LineCategoryPerDie>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                IEnumerable<long> offsets;
                var indice = Enumerable.Range(0, mapSize);
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (mapVersion == 0) offsets = indice.Select(i => offset + i * LineCategoryPerDie.SIZE);
                if (mapVersion > 1)
                {
                    if (config.LineCategoryInformation) offsets = indice.Select(i => offset + i * LineCategoryPerDie.SIZE);
                    else return null;
                }
                else return null;
                //return offsets.Select(offset => ReadAsBinarySectionProxy<LineCategoryPerDie>(offset)).ToList();
                return offsets.Select(x => new LineCategoryPerDie(this, x)).ToList();
            });

            ExtendedHeaderInformationSection = new Lazi<ExtendedHeaderInformation>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion == 1) return ReadAsBinarySectionProxy<ExtendedHeaderInformation>(HeaderInformation.SIZE);
                else if (mapVersion > 1)
                {
                    var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                    if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                    return ReadAsBinarySectionProxy<ExtendedHeaderInformation>(offset);
                }
                else return null;
            });

            TestResultPerDieV1List = new Lazi<List<TestResultPerDieV1>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var indice = TskHelper.GetIndice(mapSize);
                if (mapVersion == 1)
                {
                    var offset = HeaderInformation.SIZE + ExtendedHeaderInformation.SIZE;
                    //return indice.Select(i => ReadAsBinarySectionProxy<TestResultPerDieV1>(offset + i * TestResultPerDieV1.SIZE)).ToList();
                    return indice.Select(i => new TestResultPerDieV1(this, offset + i * TestResultPerDieV1.SIZE)).ToList();
                }
                else return null;
            });

            ExtendedTestResultPerDieV1List = new Lazi<List<ExtendedTestResultPerDieV1>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                IEnumerable<long> offsets;
                var indice = TskHelper.GetIndice(mapSize);
                if (mapVersion == 1)
                {
                    var offset = HeaderInformation.SIZE + ExtendedHeaderInformation.SIZE + mapSize * TestResultPerDieV1.SIZE;
                    offsets = indice.Select(i => offset + i * ExtendedTestResultPerDieV1.SIZE);
                }
                else return null;
                //return offsets.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV1>(offset)).ToList();
                return offsets.Select(offset => new ExtendedTestResultPerDieV1(this, offset)).ToList();
            });

            ExtendedTestResultPerDieV2List = new Lazi<List<ExtendedTestResultPerDieV2>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion != 2 || !config.TestResultInformationPerExtensionDie) return null;
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                if (config.ExtensionHeaderInformation) offset += ExtendedHeaderInformation.SIZE;
                return TskHelper.GetIndice(mapSize)
                      .Select(i => offset + i * ExtendedTestResultPerDieV2.SIZE)
                      //.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV2>(offset))
                      .Select(x => new ExtendedTestResultPerDieV2(this, x))
                      .ToList();
            });

            ExtendedTestResultPerDieV3List = new Lazi<List<ExtendedTestResultPerDieV3>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion != 3 || !config.TestResultInformationPerExtensionDie) return null;
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                if (config.ExtensionHeaderInformation) offset += ExtendedHeaderInformation.SIZE;
                return TskHelper.GetIndice(mapSize)
                      .Select(i => offset + i * ExtendedTestResultPerDieV3.SIZE)
                      //.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV3>(offset))
                      .Select(x => new ExtendedTestResultPerDieV3(this, x))
                      .ToList();
            });

            ExtendedTestResultPerDieV4List = new Lazi<List<ExtendedTestResultPerDieV4>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion != 4 || !config.TestResultInformationPerExtensionDie) return null;
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                if (config.ExtensionHeaderInformation) offset += ExtendedHeaderInformation.SIZE;
                return TskHelper.GetIndice(mapSize)
                      .Select(i => offset + i * ExtendedTestResultPerDieV4.SIZE)
                      //.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV4>(offset))
                      .Select(x => new ExtendedTestResultPerDieV4(this, x))
                      .ToList();
            });

            ExtendedTestResultPerDieV5List = new Lazi<List<ExtendedTestResultPerDieV5>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion != 5 || !config.TestResultInformationPerExtensionDie) return null;
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                if (config.ExtensionHeaderInformation) offset += ExtendedHeaderInformation.SIZE;
                return TskHelper.GetIndice(mapSize)
                      .Select(i => offset + i * ExtendedTestResultPerDieV5.SIZE)
                      //.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV5>(offset))
                      .Select(x => new ExtendedTestResultPerDieV5(this, x))
                      .ToList();
            });

            ExtendedTestResultPerDieV7List = new Lazi<List<ExtendedTestResultPerDieV7>>(() =>
            {
                var mapSize = Accessor.MapSize;
                var mapVersion = Accessor.MapVersion;
                TskHelper.EnsureMapVersion(mapVersion);
                var config = Accessor.Configuration;
                if (mapVersion != 7 || !config.TestResultInformationPerExtensionDie) return null;
                var offset = HeaderInformation.SIZE + mapSize * TestResultPerDie.SIZE;
                if (config.LineCategoryInformation) offset += mapSize * LineCategoryPerDie.SIZE;
                if (config.ExtensionHeaderInformation) offset += ExtendedHeaderInformation.SIZE;
                return TskHelper.GetIndice(mapSize)
                      .Select(i => offset + i * ExtendedTestResultPerDieV7.SIZE)
                      //.Select(offset => ReadAsBinarySectionProxy<ExtendedTestResultPerDieV7>(offset))
                      .Select(x => new ExtendedTestResultPerDieV7(this, x))
                      .ToList();
            });
        }

        public HeaderInformation HeaderInformationSection { get; }

        public Lazi<List<TestResultPerDie>> TestResultPerDieList { get; }
        public Lazi<List<LineCategoryPerDie>> LineCategoryPerDieList { get; }

        public Lazi<ExtendedHeaderInformation> ExtendedHeaderInformationSection { get; }
        public Lazi<List<TestResultPerDieV1>> TestResultPerDieV1List { get; }
        public Lazi<List<ExtendedTestResultPerDieV1>> ExtendedTestResultPerDieV1List { get; }

        public Lazi<List<ExtendedTestResultPerDieV2>> ExtendedTestResultPerDieV2List { get; }
        public Lazi<List<ExtendedTestResultPerDieV3>> ExtendedTestResultPerDieV3List { get; }
        public Lazi<List<ExtendedTestResultPerDieV4>> ExtendedTestResultPerDieV4List { get; }
        public Lazi<List<ExtendedTestResultPerDieV5>> ExtendedTestResultPerDieV5List { get; }
        public Lazi<List<ExtendedTestResultPerDieV7>> ExtendedTestResultPerDieV7List { get; }

        public void Dispose()
        {
            GetInternalStream().Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
