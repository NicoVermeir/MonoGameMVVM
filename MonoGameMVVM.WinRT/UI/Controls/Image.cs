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





namespace MonoGameMVVM
{
    public class Image : UIElement
    {
        public static readonly ReactiveProperty<ImageSource> SourceProperty =
            ReactiveProperty<ImageSource>.Register(
                "Source", typeof (Image), null, ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public static readonly ReactiveProperty<StretchDirection> StretchDirectionProperty =
            ReactiveProperty<StretchDirection>.Register(
                "StretchDirection",
                typeof (Image),
                StretchDirection.Both,
                ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public static readonly ReactiveProperty<Stretch> StretchProperty = ReactiveProperty<Stretch>.Register(
            "Stretch", typeof (Image), Stretch.Uniform, ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public ImageSource Source
        {
            get { return GetValue(SourceProperty); }

            set { SetValue(SourceProperty, value); }
        }

        public Stretch Stretch
        {
            get { return GetValue(StretchProperty); }

            set { SetValue(StretchProperty, value); }
        }

        public StretchDirection StretchDirection
        {
            get { return GetValue(StretchDirectionProperty); }

            set { SetValue(StretchDirectionProperty, value); }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return GetScaledImageSize(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return GetScaledImageSize(availableSize);
        }

        protected override void OnRender(IDrawingContext drawingContext)
        {
            if (Source != null)
            {
                drawingContext.DrawImage(Source, new Rect(new Point(), RenderSize));
            }
        }

        private Size GetScaledImageSize(Size givenSize)
        {
            ImageSource source = Source;
            if (source == null)
            {
                return new Size();
            }

            Size contentSize = source.Size;
            Vector scale = Viewbox.ComputeScaleFactor(givenSize, contentSize, Stretch, StretchDirection);
            return new Size(contentSize.Width*scale.X, contentSize.Height*scale.Y);
        }
    }
}