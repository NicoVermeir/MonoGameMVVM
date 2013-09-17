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

using System.Collections.Generic;
using MonoGameMVVM.UI;


namespace MonoGameMVVM
{
    public class Border : UIElement
    {
        public static readonly ReactiveProperty<Brush> BackgroundProperty =
            ReactiveProperty<Brush>.Register(
                "Background", typeof (Border), ReactivePropertyChangedCallbacks.InvalidateArrange);

        public static readonly ReactiveProperty<Brush> BorderBrushProperty =
            ReactiveProperty<Brush>.Register(
                "BorderBrush", typeof (Border), null, ReactivePropertyChangedCallbacks.InvalidateArrange);

        public static readonly ReactiveProperty<Thickness> BorderThicknessProperty =
            ReactiveProperty<Thickness>.Register(
                "BorderThickness", typeof (Border), new Thickness(), ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public static readonly ReactiveProperty<IElement> ChildProperty = ReactiveProperty<IElement>.Register(
            "Child", typeof (Border), null, ChildPropertyChangedCallback);

        public static readonly ReactiveProperty<Thickness> PaddingProperty =
            ReactiveProperty<Thickness>.Register(
                "Padding", typeof (Border), new Thickness(), ReactivePropertyChangedCallbacks.InvalidateMeasure);

        private readonly IList<Rect> borders = new List<Rect>();

        public Brush Background
        {
            get { return GetValue(BackgroundProperty); }

            set { SetValue(BackgroundProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return GetValue(BorderBrushProperty); }

            set { SetValue(BorderBrushProperty, value); }
        }

        public Thickness BorderThickness
        {
            get { return GetValue(BorderThicknessProperty); }

            set { SetValue(BorderThicknessProperty, value); }
        }

        public IElement Child
        {
            get { return GetValue(ChildProperty); }

            set { SetValue(ChildProperty, value); }
        }

        public Thickness Padding
        {
            get { return GetValue(PaddingProperty); }

            set { SetValue(PaddingProperty, value); }
        }

        public override IEnumerable<IElement> GetVisualChildren()
        {
            IElement child = Child;
            if (child != null)
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IElement child = Child;

            if (child != null)
            {
                var finalRect = new Rect(new Point(), finalSize);

                finalRect = finalRect.Deflate(BorderThickness);
                finalRect = finalRect.Deflate(Padding);
                child.Arrange(finalRect);
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness borderThicknessAndPadding = BorderThickness + Padding;

            IElement child = Child;
            if (child != null)
            {
                child.Measure(availableSize.Deflate(borderThicknessAndPadding));

                return child.DesiredSize.Inflate(borderThicknessAndPadding);
            }

            return borderThicknessAndPadding.Collapse();
        }

        protected override void OnRender(IDrawingContext drawingContext)
        {
            if (BorderThickness != new Thickness() && BorderBrush != null)
            {
                GenerateBorders();

                foreach (Rect border in borders)
                {
                    drawingContext.DrawRectangle(border, BorderBrush);
                }
            }

            if (Background != null)
            {
                drawingContext.DrawRectangle(
                    new Rect(0, 0, ActualWidth, ActualHeight).Deflate(BorderThickness), Background);
            }
        }

        private static void ChildPropertyChangedCallback(
            IReactiveObject source, ReactivePropertyChangeEventArgs<IElement> change)
        {
            var border = (Border) source;
            border.InvalidateMeasure();

            IElement oldChild = change.OldValue;
            if (oldChild != null)
            {
                oldChild.VisualParent = null;
            }

            IElement newChild = change.NewValue;
            if (newChild != null)
            {
                newChild.VisualParent = border;
            }
        }

        private void GenerateBorders()
        {
            borders.Clear();

            if (BorderThickness.Left > 0)
            {
                borders.Add(new Rect(0, 0, BorderThickness.Left, ActualHeight));
            }

            if (BorderThickness.Top > 0)
            {
                borders.Add(
                    new Rect(
                        BorderThickness.Left,
                        0,
                        ActualWidth - BorderThickness.Left,
                        BorderThickness.Top));
            }

            if (BorderThickness.Right > 0)
            {
                borders.Add(
                    new Rect(
                        ActualWidth - BorderThickness.Right,
                        BorderThickness.Top,
                        BorderThickness.Right,
                        ActualHeight - BorderThickness.Top));
            }

            if (BorderThickness.Bottom > 0)
            {
                borders.Add(
                    new Rect(
                        BorderThickness.Left,
                        ActualHeight - BorderThickness.Bottom,
                        ActualWidth - (BorderThickness.Left + BorderThickness.Right),
                        BorderThickness.Bottom));
            }
        }
    }
}