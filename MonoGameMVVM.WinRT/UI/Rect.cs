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


namespace MonoGameMVVM
{
    /// <summary>
    ///     A structure representing a Rectangle in 2D space
    /// </summary>
    [DebuggerDisplay("{X}, {Y} : {Width} x {Height}")]
    public struct Rect : IEquatable<Rect>
    {
        /// <summary>
        ///     The Height of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Height;

        /// <summary>
        ///     The Width of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Width;

        /// <summary>
        ///     The X coordinate of the top left corner of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double X;

        /// <summary>
        ///     The Y coordinate of the top left corner of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Y;

        static Rect()
        {
            Empty = new Rect
            {
                X = double.PositiveInfinity,
                Y = double.PositiveInfinity,
                Width = double.NegativeInfinity,
                Height = double.NegativeInfinity
            };
        }

        /// <summary>
        ///     Initializes a new <see cref="Rect">Rect</see> struct from a <see cref="Size">Size</see>.
        /// </summary>
        /// <param name="size">The <see cref="Size">Size</see> of the <see cref="Rect">Rect</see>.</param>
        public Rect(Size size)
            : this(new Point(), size)
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="Rect">Rect</see> struct from a <see cref="Point">Point</see> and a
        ///     <see cref="Size">Size</see>.
        /// </summary>
        /// <param name="position">The position of the top left corner of the <see cref="Rect">Rect</see>.</param>
        /// <param name="size">The <see cref="Size">Size</see> of the <see cref="Rect">Rect</see>.</param>
        public Rect(Point position, Size size)
            : this(position.X, position.Y, size.Width, size.Height)
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="Rect">Rect</see> struct with the specified coordinates, width and height.
        /// </summary>
        /// <param name="x">The x-coordinate of the top left corner of the <see cref="Rect">Rect</see>.</param>
        /// <param name="y">The y-coordinate of the top left corner of the <see cref="Rect">Rect</see>.</param>
        /// <param name="width">The width of the <see cref="Rect">Rect</see>.</param>
        /// <param name="height">The height of the <see cref="Rect">Rect</see>.</param>
        public Rect(double x, double y, double width, double height)
        {
            if (width < 0d || height < 0d)
            {
                throw new ArgumentException("width and height cannot be negative");
            }

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Initializes a new <see cref="Rect">Rect</see> that is just large enough to encompass the two specified
        ///     <see cref="Point">Point</see>s.
        /// </summary>
        /// <param name="point1">The first <see cref="Point">Point</see> to encompass.</param>
        /// <param name="point2">The second <see cref="Point">Point</see> to encompass.</param>
        public Rect(Point point1, Point point2)
        {
            X = Math.Min(point1.X, point2.X);
            Y = Math.Min(point1.Y, point2.Y);
            Width = (Math.Max(point1.X, point2.X) - X).EnsurePositive();
            Height = (Math.Max(point1.Y, point2.Y) - Y).EnsurePositive();
        }

        /// <summary>
        ///     An empty <see cref="Rect">Rect</see>.  Empty Rects have positive infinity coordinates and negative infinity size.
        /// </summary>
        public static Rect Empty { get; private set; }

        /// <summary>
        ///     Gets the bottom side of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Bottom
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }

                return Y + Height;
            }
        }

        /// <summary>
        ///     Determines if the <see cref="Rect">Rect</see> is empty - i.e. has positive infinity coordinates and negative
        ///     infinity size.
        /// </summary>
        public bool IsEmpty
        {
            get { return Width < 0d; }
        }

        /// <summary>
        ///     Gets the left side of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Left
        {
            get { return X; }
        }

        /// <summary>
        ///     Gets the location of the top left corner of the <see cref="Rect">Rect</see> in 2D space.
        /// </summary>
        public Point Location
        {
            get { return new Point(X, Y); }
        }

        /// <summary>
        ///     Gets the right side of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Right
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }

                return X + Width;
            }
        }

        /// <summary>
        ///     Gets the size of the <see cref="Rect">Rect</see>.
        /// </summary>
        public Size Size
        {
            get { return new Size(Width, Height); }
        }

        /// <summary>
        ///     Gets the top side of the <see cref="Rect">Rect</see>.
        /// </summary>
        public double Top
        {
            get { return Y; }
        }

        public bool Equals(Rect other)
        {
            if (other.IsEmpty)
            {
                return IsEmpty;
            }

            return other.Height.IsCloseTo(Height) && other.Width.IsCloseTo(Width) && other.X.IsCloseTo(X) &&
                   other.Y.IsCloseTo(Y);
        }

        /// <summary>
        ///     Compares two <see cref="Rect">Rect</see>s for equality.
        /// </summary>
        /// <param name="left">The left <see cref="Rect">Rect</see>.</param>
        /// <param name="right">The right <see cref="Rect">Rect</see>.</param>
        /// <returns>true if the two <see cref="Rect">Rect</see>s are equal.</returns>
        public static bool operator ==(Rect left, Rect right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two <see cref="Rect">Rect</see>s for inequality.
        /// </summary>
        /// <param name="left">The left <see cref="Rect">Rect</see>.</param>
        /// <param name="right">The right <see cref="Rect">Rect</see>.</param>
        /// <returns>true if the two <see cref="Rect">Rect</see>s are not equal.</returns>
        public static bool operator !=(Rect left, Rect right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Tests if the <see cref="Rect">Rect</see> contains the specified <see cref="Point">Point</see>.
        /// </summary>
        /// <param name="point">The <see cref="Point">Point</see> to test.</param>
        /// <returns>True if the <see cref="Rect">Rect</see> contains the specified <see cref="Point">Point</see>.</returns>
        public bool Contains(Point point)
        {
            return Contains(point.X, point.Y);
        }

        /// <summary>
        ///     Tests if the <see cref="Rect">Rect</see> contains the point described by the specified x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>True if the <see cref="Rect">Rect</see> contains the point described by the specified x and y coordinates.</returns>
        public bool Contains(double x, double y)
        {
            if (IsEmpty)
            {
                return false;
            }

            return x >= X && x - Width <= X && y >= Y && y - Height <= Y;
        }

        /// <summary>
        ///     Displaces this instance of the <see cref="Rect">Rect</see> by the specified <see cref="Vector">Vector</see>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector">Vector</see> by which to displace the <see cref="Rect">Rect</see>.</param>
        public void Displace(Vector vector)
        {
            if (!IsEmpty)
            {
                X += vector.X;
                Y += vector.Y;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof (Rect))
            {
                return false;
            }

            return Equals((Rect) obj);
        }

        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }

            unchecked
            {
                int result = Height.GetHashCode();
                result = (result*397) ^ Width.GetHashCode();
                result = (result*397) ^ X.GetHashCode();
                result = (result*397) ^ Y.GetHashCode();
                return result;
            }
        }

        /// <summary>
        ///     Changes this <see cref="Rect">Rect</see> so that it represents its intersection with the specified Rect.
        ///     If intersection does not occur, this Rect will become <see cref="Empty">Empty</see>.
        /// </summary>
        /// <param name="rect">The <see cref="Rect">Rect</see> to intersect with.</param>
        public void Intersect(Rect rect)
        {
            if (!IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                double x = Math.Max(X, rect.X);
                double y = Math.Max(Y, rect.Y);
                Width = (Math.Min(X + Width, rect.X + rect.Width) - x).EnsurePositive();
                Height = (Math.Min(Y + Height, rect.Y + rect.Height) - y).EnsurePositive();
                X = x;
                Y = y;
            }
        }

        /// <summary>
        ///     Tests whether this <see cref="Rect">Rect</see> instersects with the specified Rect.
        /// </summary>
        /// <param name="rect">The <see cref="Rect">Rect</see> to intersect with.</param>
        /// <returns>True if this <see cref="Rect">Rect</see> instersects with the specified Rect.</returns>
        public bool IntersectsWith(Rect rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (((rect.X <= (X + Width)) && ((rect.X + rect.Width) >= X)) &&
                    (rect.Y <= (Y + Height))) && ((rect.Y + rect.Height) >= Y);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1} : {2} x {3}", X, Y, Width, Height);
        }

        /// <summary>
        ///     Expands this <see cref="Rect">Rect</see> to encompass the specified Rect.
        /// </summary>
        /// <param name="rect">The <see cref="Rect">Rect</see> to encompass.</param>
        public void Union(Rect rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                double x = Math.Min(Left, rect.Left);
                double y = Math.Min(Top, rect.Top);

                Width = rect.Width == double.PositiveInfinity || Width == double.PositiveInfinity
                    ? double.PositiveInfinity
                    : (Math.Max(Right, rect.Right) - x).EnsurePositive();

                Height = rect.Height == double.PositiveInfinity || Height == double.PositiveInfinity
                    ? double.PositiveInfinity
                    : (Math.Max(Bottom, rect.Bottom) - y).EnsurePositive();

                X = x;
                Y = y;
            }
        }

        /// <summary>
        ///     Expands this <see cref="Rect">Rect</see> to encompass the specified <see cref="Point">Point</see>.
        /// </summary>
        /// <param name="point">The <see cref="Point">Point</see> to include.</param>
        public void Union(Point point)
        {
            Union(new Rect(point, point));
        }
    }
}