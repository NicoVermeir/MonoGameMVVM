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

namespace MonoGameMVVM.UI.Internal
{
    internal static class SizeExtensions
    {
        public static Size Deflate(this Size size, Thickness thickness)
        {
            return new Size(
                (size.Width - (thickness.Left + thickness.Right)).EnsurePositive(),
                (size.Height - (thickness.Top + thickness.Bottom)).EnsurePositive());
        }

        public static Size Inflate(this Size size, Thickness thickness)
        {
            return new Size(
                (size.Width + (thickness.Left + thickness.Right)).EnsurePositive(),
                (size.Height + (thickness.Top + thickness.Bottom)).EnsurePositive());
        }

        public static bool IsCloseTo(this Size size1, Size size2)
        {
            return !size1.IsDifferentFrom(size2);
        }

        public static bool IsDifferentFrom(this Size size1, Size size2)
        {
            return size1.Width.IsDifferentFrom(size2.Width) || size1.Height.IsDifferentFrom(size2.Height);
        }
    }
}