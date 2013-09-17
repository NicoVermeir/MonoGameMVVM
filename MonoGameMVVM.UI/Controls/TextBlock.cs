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
using System.Text;
using System.Text.RegularExpressions;
using MonoGameMVVM.UI.Graphics;
using MonoGameMVVM.UI.Media;

namespace MonoGameMVVM.UI.Controls
{
    public class TextBlock : UIElement
    {
        public static readonly ReactiveProperty<Brush> BackgroundProperty =
            ReactiveProperty<Brush>.Register("Background", typeof (TextBlock));

        public static readonly ReactiveProperty<Brush> ForegroundProperty =
            ReactiveProperty<Brush>.Register("Foreground", typeof (TextBlock));

        public static readonly ReactiveProperty<Thickness> PaddingProperty =
            ReactiveProperty<Thickness>.Register(
                "Padding", typeof (TextBlock), ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public static readonly ReactiveProperty<string> TextProperty = ReactiveProperty<string>.Register(
            "Text", typeof (TextBlock), string.Empty, ReactivePropertyChangedCallbacks.InvalidateMeasure);

        public static readonly ReactiveProperty<TextWrapping> WrappingProperty =
            ReactiveProperty<TextWrapping>.Register(
                "Wrapping", typeof (TextBlock), TextWrapping.NoWrap, ReactivePropertyChangedCallbacks.InvalidateMeasure);

        private static readonly Regex WhiteSpaceRegEx = new Regex(@"\s+", RegexOptions.CultureInvariant);

        private readonly ISpriteFont spriteFont;

        private string formattedText;

        public TextBlock(ISpriteFont spriteFont)
        {
            if (spriteFont == null)
            {
                throw new ArgumentNullException("spriteFont");
            }

            this.spriteFont = spriteFont;
        }

        public Brush Background
        {
            get { return GetValue(BackgroundProperty); }

            set { SetValue(BackgroundProperty, value); }
        }

        public Brush Foreground
        {
            get { return GetValue(ForegroundProperty); }

            set { SetValue(ForegroundProperty, value); }
        }

        public Thickness Padding
        {
            get { return GetValue(PaddingProperty); }

            set { SetValue(PaddingProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        public TextWrapping Wrapping
        {
            get { return GetValue(WrappingProperty); }

            set { SetValue(WrappingProperty, value); }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            formattedText = Text;
            Size measureString = spriteFont.MeasureString(formattedText);

            if (Wrapping == TextWrapping.Wrap && measureString.Width > availableSize.Width)
            {
                formattedText = WrapText(spriteFont, formattedText, availableSize.Width);
                measureString = spriteFont.MeasureString(formattedText);
            }

            return new Size(
                measureString.Width + Padding.Left + Padding.Right,
                measureString.Height + Padding.Top + Padding.Bottom);
        }

        protected override void OnRender(IDrawingContext drawingContext)
        {
            if (Background != null)
            {
                drawingContext.DrawRectangle(new Rect(0, 0, ActualWidth, ActualHeight), Background);
            }

            drawingContext.DrawText(
                spriteFont,
                formattedText,
                new Point(Padding.Left, Padding.Top),
                Foreground ?? new SolidColorBrush(Colors.Black));
        }

        private static string WrapText(ISpriteFont font, string text, double maxLineWidth)
        {
            const string Space = " ";
            var stringBuilder = new StringBuilder();
            string[] words = WhiteSpaceRegEx.Split(text);

            double lineWidth = 0;
            double spaceWidth = font.MeasureString(Space).Width;

            foreach (string word in words)
            {
                Size size = font.MeasureString(word);

                if (lineWidth + size.Width < maxLineWidth)
                {
                    stringBuilder.AppendFormat("{0}{1}", lineWidth == 0 ? string.Empty : Space, word);
                    lineWidth += size.Width + spaceWidth;
                }
                else
                {
                    stringBuilder.AppendFormat("\n{0}", word);
                    lineWidth = size.Width + spaceWidth;
                }
            }

            return stringBuilder.ToString();
        }
    }
}