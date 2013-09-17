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
    public class ScrollViewer : ContentControl, IInputElement
    {
        private bool isInsertingScrollContentPresenter;

        private IScrollInfo scrollInfo;

        public bool CanHorizontallyScroll
        {
            get { return scrollInfo != null ? scrollInfo.CanHorizontallyScroll : false; }

            set
            {
                if (scrollInfo != null)
                {
                    scrollInfo.CanHorizontallyScroll = value;
                }
            }
        }

        public bool CanVerticallyScroll
        {
            get { return scrollInfo != null ? scrollInfo.CanVerticallyScroll : false; }

            set
            {
                if (scrollInfo != null)
                {
                    scrollInfo.CanVerticallyScroll = value;
                }
            }
        }

        public Size Extent
        {
            get { return scrollInfo != null ? scrollInfo.Extent : new Size(); }
        }

        public Size Viewport
        {
            get { return scrollInfo != null ? scrollInfo.Viewport : new Size(); }
        }

        protected override void OnContentChanged(IElement oldContent, IElement newContent)
        {
            var oldScrollContentPresenter = oldContent as ScrollContentPresenter;
            if (oldScrollContentPresenter != null)
            {
                oldScrollContentPresenter.Content.VisualParent = null;
            }

            var newScrollInfo = newContent as IScrollInfo;
            if (newScrollInfo != null)
            {
                scrollInfo = newScrollInfo;

                if (oldContent != null && isInsertingScrollContentPresenter)
                {
                    ((ScrollContentPresenter) newContent).Content = oldContent;
                }

                isInsertingScrollContentPresenter = false;
            }
            else
            {
                isInsertingScrollContentPresenter = true;
                Content = new ScrollContentPresenter();
            }
        }

        protected override void OnNextGesture(Gesture gesture)
        {
            switch (gesture.Type)
            {
                case GestureType.LeftButtonDown:
                    CaptureMouse();
                    break;
                case GestureType.FreeDrag:
                    if (scrollInfo != null && IsMouseCaptured)
                    {
                        scrollInfo.SetHorizontalOffset(scrollInfo.Offset.X - gesture.Delta.X);
                        scrollInfo.SetVerticalOffset(scrollInfo.Offset.Y - gesture.Delta.Y);
                    }

                    break;
                case GestureType.LeftButtonUp:
                    if (IsMouseCaptured)
                    {
                        ReleaseMouseCapture();
                    }

                    break;
            }
        }
    }
}