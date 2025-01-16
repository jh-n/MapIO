using System;
using System.Collections.Generic;
using System.Linq;

namespace MapIO.TSK
{

    public static class TskHelper
    {
        public static void EnsureMapVersion(int mapVersion)
        {
            if (mapVersion < 0 || mapVersion > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(mapVersion), "Map version must be between 0 and 7.");
            }
        }

        public static IEnumerable<int> GetIndice(int mapSize)
        {
            return Enumerable.Range(0, mapSize);
        }
    }
}