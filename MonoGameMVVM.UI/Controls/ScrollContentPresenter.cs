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
using MonoGameMVVM.UI.Controls.Primitives;
using MonoGameMVVM.UI.Internal;
using MonoGameMVVM.UI.Internal.Controls;

namespace MonoGameMVVM.UI.Controls
{
    public class ScrollContentPresenter : ContentControl, IScrollInfo
    {
        private bool isClippingRequired;

        private ScrollData scrollData;

        public ScrollContentPresenter()
        {
            scrollData.CanHorizontallyScroll = true;
            scrollData.CanVerticallyScroll = true;
        }

        public bool CanHorizontallyScroll
        {
            get { return scrollData.CanHorizontallyScroll; }

            set { scrollData.CanHorizontallyScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return scrollData.CanVerticallyScroll; }

            set { scrollData.CanVerticallyScroll = value; }
        }

        public Size Extent
        {
            get { return scrollData.Extent; }
        }

        public Vector Offset
        {
            get { return scrollData.Offset; }
        }

        public Size Viewport
        {
            get { return scrollData.Viewport; }
        }

        public void SetHorizontalOffset(double offset)
        {
            if (!CanHorizontallyScroll)
            {
                return;
            }

            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            offset = Math.Max(0d, offset);

            if (scrollData.Offset.X.IsDifferentFrom(offset))
            {
                scrollData.Offset.X = offset;
                InvalidateArrange();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (!CanVerticallyScroll)
            {
                return;
            }

            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            offset = Math.Max(0d, offset);

            if (scrollData.Offset.Y.IsDifferentFrom(offset))
            {
                scrollData.Offset.Y = offset;
                InvalidateArrange();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IElement content = Content;

            UpdateScrollData(finalSize, scrollData.Extent);

            if (content != null)
            {
                var finalRect = new Rect(
                    -scrollData.Offset.X,
                    -scrollData.Offset.Y,
                    content.DesiredSize.Width,
                    content.DesiredSize.Height);

                isClippingRequired = finalSize.Width.IsLessThan(finalRect.Width) ||
                                     finalSize.Height.IsLessThan(finalRect.Height);

                finalRect.Width = Math.Max(finalRect.Width, finalSize.Width);
                finalRect.Height = Math.Max(finalRect.Height, finalSize.Height);

                content.Arrange(finalRect);
            }

            return finalSize;
        }

        protected override Rect GetClippingRect(Size finalSize)
        {
            return isClippingRequired ? new Rect(RenderSize) : Rect.Empty;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            IElement content = Content;
            var desiredSize = new Size();
            var extent = new Size();

            if (content != null)
            {
                Size availableSizeForContent = availableSize;
                if (scrollData.CanHorizontallyScroll)
                {
                    availableSizeForContent.Width = double.PositiveInfinity;
                }

                if (scrollData.CanVerticallyScroll)
                {
                    availableSizeForContent.Height = double.PositiveInfinity;
                }

                content.Measure(availableSizeForContent);
                desiredSize = content.DesiredSize;

                extent = content.DesiredSize;
            }

            UpdateScrollData(availableSize, extent);
            desiredSize.Width = Math.Min(availableSize.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(availableSize.Height, desiredSize.Height);

            return desiredSize;
        }

        private void UpdateScrollData(Size viewport, Size extent)
        {
            scrollData.Viewport = viewport;
            scrollData.Extent = extent;

            double x = scrollData.Offset.X.Coerce(
                0d, scrollData.Extent.Width - scrollData.Viewport.Width);
            double y = scrollData.Offset.Y.Coerce(
                0d, scrollData.Extent.Height - scrollData.Viewport.Height);

            scrollData.Offset = new Vector(x, y);
        }
    }
}