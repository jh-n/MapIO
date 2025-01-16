using MapIO.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace MapIO.SINF
{
    public class SinfDataAccessor
    {
        readonly SinfData _source;
        public SinfDataAccessor(SinfData source)
        {
            _source = source;
        }

        int RawBinCodeLength => _source.RowData.IsNullOrEmpty() ? 3 : _source.RowData.First().IndexOf(' ');

        public BinCodeType BinCodeType
        {
            get
            {
                switch (RawBinCodeLength)
                {
                    case 2: return BinCodeType.Hex;
                    case 3: return BinCodeType.Decimal;
                    default: throw new Exception($"Unsupported bin length {RawBinCodeLength}");
                }
            }
        }

        public List<int?> BinCodes
        {
            get
            {
                var rawCategory = _source.RowData.Select(row => row.Split(' ')).SelectMany(x => x);
                switch (BinCodeType)
                {
                    case BinCodeType.Decimal: return rawCategory.Select(x => int.TryParse(x, out var bin) ? bin : default(int?)).ToList();
                    case BinCodeType.Hex: return rawCategory.Select(x => int.TryParse(x, NumberStyles.HexNumber, null, out var bin) ? bin : default(int?)).ToList();
                    default: throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Definition of bin codes that are considered as pass dies.
        /// </summary>
        public HashSet<int> PassBinCodes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_source.BCEQU)) return DefaultPassBinCodes;
                switch (BinCodeType)
                {
                    case BinCodeType.Hex:
                        return _source.BCEQU
#if NETSTANDARD
                            .Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .WhereNot(string.IsNullOrWhiteSpace)
#else
                            .Split(new char[] { ' ', ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
#endif
                            .Select(x => int.Parse(x, NumberStyles.HexNumber))
                            .ToHashSet();
                    case BinCodeType.Decimal:
                        return _source.BCEQU
#if NETSTANDARD
                            .Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .WhereNot(string.IsNullOrWhiteSpace)
#else
                            .Split(new char[] { ' ', ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
#endif
                            .Select(int.Parse)
                            .ToHashSet();
                    default: throw new Exception("Unsupported bin type.");
                }
            }
            set
            {
                switch (BinCodeType)
                {
                    case BinCodeType.Hex:
                        _source.BCEQU = string.Join(" ", value.Select(x => x.ToString("X").PadLeft(2, '0')));
                        break;
                    case BinCodeType.Decimal:
                        _source.BCEQU = string.Join(" ", value.Select(x => x.ToString().PadLeft(3, '0')));
                        break;
                    default: throw new Exception("Unsupported data size.");
                };
            }
        }

        public static HashSet<int> DefaultPassBinCodes => new HashSet<int>() { 1 };

        public void SetBinCodes(BinCodeType binCodeType, ICollection<int?> binCodes, int colCount)
        {
            if (binCodes.Count % colCount != 0) throw new Exception("Bin codes count must be a multiple of column count.");
            var rawSkip = '_'.Repeat((int)binCodeType);
            switch (binCodeType)
            {
                case BinCodeType.Decimal:
                    _source.RowData = binCodes.Select(x => x.HasValue ? x.Value.ToString().PadLeft(3, '0') : rawSkip).Page(colCount).Select(x => string.Join(" ", x)).ToList();
                    break;
                case BinCodeType.Hex:
                    _source.RowData = binCodes.Select(x => x.HasValue ? x.Value.ToString("X").PadLeft(2, '0') : rawSkip).Page(colCount).Select(x => string.Join(" ", x)).ToList();
                    break;
                default: throw new NotImplementedException();
            };
            _source.COLCT = colCount;
            _source.ROWCT = binCodes.Count / colCount;
        }
    }
}