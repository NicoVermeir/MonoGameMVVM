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
using System.Diagnostics;
using MonoGameMVVM.UI.Internal;

namespace MonoGameMVVM.UI
{
    [DebuggerDisplay("{Left}, {Top}, {Right}, {Bottom}")]
    public struct Thickness : IEquatable<Thickness>
    {
        public double Bottom;

        public double Left;

        public double Right;

        public double Top;

        public Thickness(double left, double top)
            : this(left, top, left, top)
        {
        }

        public Thickness(double uniformLength)
            : this(uniformLength, uniformLength, uniformLength, uniformLength)
        {
        }

        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public bool Equals(Thickness other)
        {
            return other.Bottom.IsCloseTo(Bottom) && other.Left.IsCloseTo(Left) &&
                   other.Right.IsCloseTo(Right) && other.Top.IsCloseTo(Top);
        }

        /// <summary>
        ///     Adds two <see cref="Thickness">Thickness</see> instances together (e.g. adds the two <see cref="Left">Left</see>
        ///     components, the two <see cref="Top">Top</see> components etc).
        /// </summary>
        /// <param name="left">The first <see cref="Thickness">Thickness</see></param>
        /// <param name="right">The second <see cref="Thickness">Thickness</see></param>
        /// <returns>
        ///     A <see cref="Thickness">Thickness</see> whose components represent the sum of the two
        ///     <see cref="Thickness">Thickness</see> instances.
        /// </returns>
        public static Thickness operator +(Thickness left, Thickness right)
        {
            return new Thickness(
                left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);
        }

        public static bool operator ==(Thickness left, Thickness right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Thickness left, Thickness right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof (Thickness))
            {
                return false;
            }

            return Equals((Thickness) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Bottom.GetHashCode();
                result = (result*397) ^ Left.GetHashCode();
                result = (result*397) ^ Right.GetHashCode();
                result = (result*397) ^ Top.GetHashCode();
                return result;
            }
        }
    }
}