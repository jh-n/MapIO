using MapIO.Core.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapIO.SINF
{
    public class SinfData
    {
        public SinfDataAccessor Accessor { get; }
        /// <summary>
        /// Prober Device
        /// </summary>
        public string DEVICE { get; set; }
        public string LOT { get; set; }
        /// <summary>
        /// Wafer ID
        /// </summary>
        public string WAFER { get; set; }
        /// <summary>
        /// wafer flat position (0=TOP, 90=RIGHT, 180=BOTTOM, 270=LEFT)
        /// </summary>
        public int FNLOC { get; set; }
        /// <summary>
        /// number of rows
        /// </summary>
        public int ROWCT { get; set; }
        /// <summary>
        /// number of columns
        /// </summary>
        public int COLCT { get; set; }
        /// <summary>
        /// List of Bin Codes that are good die (comma or space delimited)
        /// </summary>
        public string BCEQU { get; set; }
        /// <summary>
        /// x-coord of reference die (optional)
        /// </summary>
        public int REFPX { get; set; }
        /// <summary>
        /// y-coord of reference die (optional)
        /// </summary>
        public int REFPY { get; set; }
        /// <summary>
        /// die units of measurement (mm or mil)
        /// </summary>
        public string DUTMS { get; set; }
        public int DIECT { get; set; }
        /// <summary>
        /// step along X
        /// </summary>
        public double XDIES { get; set; }
        /// <summary>
        /// step along Y
        /// </summary>
        public double YDIES { get; set; }
        public string COMMENT { get; set; }
        public List<string> RowData { get; set; }

        public SinfData()
        {
            Accessor = new SinfDataAccessor(this);
        }

        public SinfData(StreamReader reader)
        {
            Accessor = new SinfDataAccessor(this);
            Read(reader);
        }

        public void Read(StreamReader reader)
        {
            string line;
            RowData = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                var kv = line.ReadLineAsKeyValuePairs(new char[] { ':' }).First();
                switch (kv.Key)
                {
                    case "DEVICE": DEVICE = kv.Value; break;
                    case "LOT": LOT = kv.Value; break;
                    case "WAFER": WAFER = kv.Value; break;
                    case "FNLOC": FNLOC = int.Parse(kv.Value); break;
                    case "ROWCT": ROWCT = int.Parse(kv.Value); break;
                    case "COLCT": COLCT = int.Parse(kv.Value); break;
                    case "BCEQU": BCEQU = kv.Value; break;
                    case "REFPX": REFPX = int.Parse(kv.Value); break;
                    case "REFPY": REFPY = int.Parse(kv.Value); break;
                    case "DUTMS": DUTMS = kv.Value; break;
                    case "DIECT": DIECT = int.Parse(kv.Value); break;
                    case "XDIES": XDIES = double.Parse(kv.Value); break;
                    case "YDIES": YDIES = double.Parse(kv.Value); break;
                    case "COMMENT": COMMENT = kv.Value; break;
                    case "RowData": RowData.Add(kv.Value); break;
                }
            }
        }

        public void Write(StreamWriter writer)
        {
            writer.WriteLine($"DEVICE:{DEVICE}");
            writer.WriteLine($"LOT:{LOT}");
            writer.WriteLine($"WAFER:{WAFER}");
            writer.WriteLine($"FNLOC:{FNLOC}");
            writer.WriteLine($"ROWCT:{ROWCT}");
            writer.WriteLine($"COLCT:{COLCT}");
            writer.WriteLine($"BCEQU:{BCEQU}");
            writer.WriteLine($"REFPX:{REFPX}");
            writer.WriteLine($"REFPY:{REFPY}");
            writer.WriteLine($"DUTMS:{DUTMS}");
            //writer.WriteLine($"DIECT:{DIECT}");
            writer.WriteLine($"XDIES:{XDIES:G10}");
            writer.WriteLine($"YDIES:{YDIES:G10}");
            //writer.WriteLine($"COMMENT:{COMMENT}");
            RowData.ForEach(x =>
            {
                writer.WriteLine($"RowData:{x}");
            });
            writer.Flush();
        }
    }
}