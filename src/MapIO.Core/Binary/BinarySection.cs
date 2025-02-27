using MapIO.Core.Extensions;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MapIO.Core.Binary
{

    public abstract class BinarySection
    {
        protected virtual long Size { get; }

        readonly MemoryStream _source;
        readonly long _start;
        readonly bool _isLittleEndian;
        readonly Encoding _encoding;

        public BinarySection(MemoryStream source, long start, bool? isLittleEndian = null, Encoding encoding = null)
        {
            _source = source;
            _start = start;
            _isLittleEndian = isLittleEndian ?? BitConverter.IsLittleEndian;
            _encoding = encoding ?? Encoding.UTF8;
        }

        public BinarySection(BinarySection source, long offset)
        {
            _source = source._source;
            _start = source._start + offset;
            _isLittleEndian = source._isLittleEndian;
            _encoding = source._encoding;
        }

        public MemoryStream GetInternalStream() => _source;

        #region Read/Write Core
        /// <summary>
        /// Reads a sequence of bytes from the current stream starting at the specified offset and with the specified length.
        /// </summary>
        /// <param name="offset">The byte offset in the stream at which to begin reading.</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <returns>A span containing the bytes read from the stream.</returns>
        protected Span<byte> Read(long offset, int length)
        {
            Span<byte> buffer = new byte[length];
            _source.Position = _start + offset;
            _source.Read(buffer);
            return buffer;
        }

        /// <inheritdoc cref="Read(long, int)"/>
        /// <param name="count">The count of bytes read</param>
        protected Span<byte> Read(long offset, int length, out int count)
        {
            Span<byte> buffer = new byte[length];
            _source.Position = _start + offset;
            count = _source.Read(buffer);
            return buffer;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream starting at the specified offset.
        /// </summary>
        /// <param name="offset">The byte offset in the stream at which to begin writing.</param>
        /// <param name="buffer">A read-only span containing the bytes to write to the stream.</param>
        protected void Write(long offset, ReadOnlySpan<byte> buffer)
        {
            _source.Position = _start + offset;
            _source.Write(buffer);
        }
        #endregion

        #region Read Basic
        protected byte ReadAsByte(long offset)
        {
            _source.Position = _start + offset;
            return (byte)_source.ReadByte();
        }

        protected sbyte ReadAsSByte(long offset) => (sbyte)ReadAsByte(offset);

        protected int ReadAsInt32(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(buffer) : BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        protected uint ReadAsUInt32(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(buffer) : BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }

        protected short ReadAsInt16(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(buffer) : BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        protected ushort ReadAsUInt16(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(buffer) : BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }

        protected long ReadAsInt64(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(buffer) : BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        protected ulong ReadAsUInt64(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(buffer) : BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }

#if NET
        protected float ReadAsSingle(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadSingleLittleEndian(buffer) : BinaryPrimitives.ReadSingleBigEndian(buffer);
        }

        protected double ReadAsDouble(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _isLittleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(buffer) : BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
#endif

        protected bool ReadAsBoolean(long offset)
        {
            _source.Position = _start + offset;
            return _source.ReadByte() != 0;
        }

        protected string ReadAsString(long offset, int length)
        {
            Span<byte> buffer = Read(offset, length);
            return _encoding.GetString(buffer);
        }
        #endregion

        #region Read Generic
        protected T ReadAsType<T>(long offset, int length) => (T)ReadAsType(offset, length, typeof(T));

        internal object ReadAsType(long offset, int length, Type asType)
        {
            if (asType.IsAssignableTo(typeof(BinarySection))) return ReadAsBinarySectionProxy(offset, asType);
            return ReadAsType(offset, length, Type.GetTypeCode(asType));
        }

        protected object ReadAsType(long offset, int length, TypeCode asTypeCode)
        {
            switch (asTypeCode)
            {
                case TypeCode.Byte: return ReadAsByte(offset);
                case TypeCode.SByte: return ReadAsSByte(offset);
                case TypeCode.Int16: return ReadAsInt16(offset, length);
                case TypeCode.UInt16: return ReadAsUInt16(offset, length);
                case TypeCode.Int32: return ReadAsInt32(offset, length);
                case TypeCode.UInt32: return ReadAsUInt32(offset, length);
                case TypeCode.Int64: return ReadAsInt64(offset, length);
                case TypeCode.UInt64: return ReadAsUInt64(offset, length);
#if NET
                case TypeCode.Single: return ReadAsSingle(offset, length);
                case TypeCode.Double: return ReadAsDouble(offset, length);
#endif
                case TypeCode.Boolean: return ReadAsBoolean(offset);
                case TypeCode.String: return ReadAsString(offset, length);
                default: throw new NotSupportedException();
            }
        }

        internal object Read(FieldAttribute attribute) => ReadAsType(attribute.Offset, attribute.Length, attribute.Type);

        /*T ReadAsBinarySection<T>(long offset) where T : BinarySection, new() => ReadAsBinarySection(offset, typeof(T)) as T;

        object ReadAsBinarySection(long offset, Type asType)
        {
            var instance = Activator.CreateInstance(asType, new object[] { this, offset }) as BinarySection;
            asType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.GetCustomAttribute<FieldAttribute>() is not null)
                .ToList()
                .ForEach(fi =>
                {
                    var attribute = fi.GetCustomAttribute<FieldAttribute>();
                    object value = attribute.Type == TypeCode.Object && fi.FieldType.IsAssignableTo(typeof(BinarySection))
                    ? instance.ReadAsType(attribute.Offset, attribute.Length, fi.FieldType)
                    : instance.Read(attribute);
                    fi.SetValue(instance, value);
                });
            asType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(pi => pi.GetCustomAttribute<FieldAttribute>() is not null)
                .ToList()
                .ForEach(pi =>
                {
                    var attribute = pi.GetCustomAttribute<FieldAttribute>();
                    object value = attribute.Type == TypeCode.Object && pi.PropertyType.IsAssignableTo(typeof(BinarySection))
                    ? instance.ReadAsType(attribute.Offset, attribute.Length, pi.PropertyType)
                    : instance.Read(attribute);
                    pi.SetValue(instance, value);
                });
            return instance;
        }*/

        protected T ReadAsBinarySectionProxy<T>(long offset) where T : BinarySection => ReadAsBinarySectionProxy(offset, typeof(T)) as T;

        protected BinarySection ReadAsBinarySectionProxy(long offset, Type asType)
        {
            var proxy = BinarySectionInterceptor.CreateProxy(asType, this, offset);
            asType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.GetCustomAttribute<FieldAttribute>() != null)
                .ToList()
                .ForEach(fi =>
                {
                    var attribute = fi.GetCustomAttribute<FieldAttribute>();
                    var value = proxy.Read(attribute);
                    fi.SetValue(proxy, value);
                });
            return proxy;
        }
        #endregion

        #region Write Basic
        protected void WriteAsByte(long offset, byte value)
        {
            _source.Position = _start + offset;
            _source.WriteByte(value);
        }

        protected void WriteAsSByte(long offset, sbyte value) => WriteAsByte(offset, (byte)value);

        protected void WriteAsInt32(long offset, int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            if (_isLittleEndian) BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            else BinaryPrimitives.WriteInt32BigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsUInt32(long offset, uint value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(uint)];
            if (_isLittleEndian) BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
            else BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsInt16(long offset, short value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            if (_isLittleEndian) BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
            else BinaryPrimitives.WriteInt16BigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsUInt16(long offset, ushort value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ushort)];
            if (_isLittleEndian) BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
            else BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsInt64(long offset, long value)
        {

            Span<byte> buffer = stackalloc byte[sizeof(long)];
            if (_isLittleEndian) BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
            else BinaryPrimitives.WriteInt64BigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsUInt64(long offset, ulong value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ulong)];
            if (_isLittleEndian) BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
            else BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
            Write(offset, buffer);
        }

#if NET
        protected void WriteAsSingle(long offset, float value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(float)];
            if (_isLittleEndian) BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
            else BinaryPrimitives.WriteSingleBigEndian(buffer, value);
            Write(offset, buffer);
        }

        protected void WriteAsDouble(long offset, double value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(double)];
            if (_isLittleEndian) BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
            else BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
            Write(offset, buffer);
        }
#endif

        protected void WriteAsBoolean(long offset, bool value)
        {
            _source.Position = _start + offset;
            _source.WriteByte((byte)(value ? 1 : 0));
        }

        protected void WriteAsString(long offset, string value)
        {
            Span<byte> buffer = _encoding.GetBytes(value);
            Write(offset, buffer);
        }
        #endregion

        #region Write Generic
        protected void WriteAsType<T>(long offset, T value) => WriteAsType(offset, value, typeof(T));

        protected void WriteAsType(long offset, object value, Type asType) => WriteAsType(offset, value, Type.GetTypeCode(asType));

        protected void WriteAsType(long offset, object value, TypeCode asTypeCode)
        {
            switch (asTypeCode)
            {
                case TypeCode.Byte:
                    WriteAsByte(offset, Convert.ToByte(value));
                    break;
                case TypeCode.SByte:
                    WriteAsSByte(offset, Convert.ToSByte(value));
                    break;
                case TypeCode.Int16:
                    WriteAsInt16(offset, Convert.ToInt16(value));
                    break;
                case TypeCode.UInt16:
                    WriteAsUInt16(offset, Convert.ToUInt16(value));
                    break;
                case TypeCode.Int32:
                    WriteAsInt32(offset, Convert.ToInt32(value));
                    break;
                case TypeCode.UInt32:
                    WriteAsUInt32(offset, Convert.ToUInt32(value));
                    break;
                case TypeCode.Int64:
                    WriteAsInt64(offset, Convert.ToInt64(value));
                    break;
                case TypeCode.UInt64:
                    WriteAsUInt64(offset, Convert.ToUInt64(value));
                    break;
#if NET
                case TypeCode.Single:
                    WriteAsSingle(offset, (float)value);
                    break;
                case TypeCode.Double:
                    WriteAsDouble(offset, (double)value);
                    break;
#endif
                case TypeCode.Boolean:
                    WriteAsBoolean(offset, Convert.ToBoolean(value));
                    break;
                case TypeCode.String:
                    WriteAsString(offset, Convert.ToString(value));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        internal void Write(FieldAttribute attribute, object value) => WriteAsType(attribute.Offset, value, attribute.Type);
        #endregion

        #region Write Advanced
        protected void WriteAsBinarySection<T>(long offset, T section) where T : BinarySection
        {
            typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.GetCustomAttribute<FieldAttribute>() != null)
                .ToList()
                .ForEach(fi =>
                {
                    var attribute = fi.GetCustomAttribute<FieldAttribute>();
                    var value = fi.GetValue(section);
                    Write(attribute, value);
                });
            typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(pi => pi.GetCustomAttribute<FieldAttribute>() != null)
                .ToList()
                .ForEach(pi =>
                {
                    var attribute = pi.GetCustomAttribute<FieldAttribute>();
                    var value = pi.GetValue(section);
                    Write(attribute, value);
                });
        }

        #endregion
    }
}