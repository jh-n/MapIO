using System;

namespace MapIO.Core.Binary
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// Set the offset of the bytes to read from or write to the stream relative to the start of the section
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// Set the length of bytes to read from the stream
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Set the type of the intermediate value directly read from or written to the stream; then it will be converted to the property or field type
        /// </summary>
        public TypeCode Type { get; set; }
    }
}