#region License

/* The MIT License
 *
 * Copyright (c) 2011 Red Badger Consulting
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

#endregion

using System;

namespace MonoGameMVVM
{
    /// <summary>
    ///     Represents a <see cref="Brush">Brush</see> of the specified <see cref="Color">Color</see> which can be used to
    ///     paint an area with a solid color.
    /// </summary>
    public class SolidColorBrush : Brush, IConvertible
    {
        /// <summary>
        ///     <see cref="ReactiveProperty{T}">ReactiveProperty</see> representing the <see cref="Color">Color</see> property.
        /// </summary>
        public static readonly ReactiveProperty<Color> ColorProperty = ReactiveProperty<Color>.Register(
            "Color", typeof (SolidColorBrush), Colors.White);

        /// <summary>
        ///     Initializes a new instance of the <see cref="SolidColorBrush">SolidColorBrush</see> class.
        /// </summary>
        /// <param name="color">
        ///     The <see cref="Color">Color</see> with which to create this
        ///     <see cref="SolidColorBrush">SolidColorBrush</see>.
        /// </param>
        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        /// <summary>
        ///     The <see cref="Media.Color">Color</see> of the SolidColorBrush.
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }

            set { SetValue(ColorProperty, value); }
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public override string ToString()
        {
            return Color.ToString();
        }
    }

    public interface IConvertible
    {
        ulong ToUInt64(IFormatProvider provider);
        uint ToUInt32(IFormatProvider provider);
        ushort ToUInt16(IFormatProvider provider);
        object ToType(Type conversionType, IFormatProvider provider);
        string ToString(IFormatProvider provider);
        float ToSingle(IFormatProvider provider);
        sbyte ToSByte(IFormatProvider provider);
        long ToInt64(IFormatProvider provider);
        int ToInt32(IFormatProvider provider);
        short ToInt16(IFormatProvider provider);
        double ToDouble(IFormatProvider provider);
        decimal ToDecimal(IFormatProvider provider);
        DateTime ToDateTime(IFormatProvider provider);
        char ToChar(IFormatProvider provider);
        byte ToByte(IFormatProvider provider);
        bool ToBoolean(IFormatProvider provider);
    }
}