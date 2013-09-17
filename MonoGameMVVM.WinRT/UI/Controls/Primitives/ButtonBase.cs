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
using System.Reflection;


namespace MonoGameMVVM
{
    public abstract class ButtonBase : ContentControl, IInputElement
    {
        public static readonly ReactiveProperty<bool> IsPressedProperty = ReactiveProperty<bool>.Register(
            "IsPressed", typeof(ButtonBase), false);

        private bool isLeftButtonDown;

        public bool IsPressed
        {
            get { return GetValue(IsPressedProperty); }

            protected internal set { SetValue(IsPressedProperty, value); }
        }

        public event EventHandler<EventArgs> Click;

        protected virtual void OnClick()
        {
            //Execute the command if there is one
            if (!string.IsNullOrEmpty(CommandName))
            {
                ExecuteCommand();

                return;
            }

            EventHandler<EventArgs> handler = Click;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void ExecuteCommand()
        {
            if (DataContext == null || CommandName.Trim() == string.Empty) return;

            MethodInfo method = DataContext.GetType().GetRuntimeMethod(CommandName, new Type[] { });

            method.Invoke(DataContext, method.GetParameters());
        }

        public void BindCommand(string commandName)
        {
            CommandName = commandName;
        }

        public void BindCommand(string commandName, object dataContext)
        {
            if (dataContext == null)
                BindCommand(commandName);
            else
            {
                CommandName = commandName;
                OverridenDataContext = dataContext.ToString();
            }
        }

        public string CommandName { get; set; }
        public string OverridenDataContext { get; set; }

        protected override void OnNextGesture(Gesture gesture)
        {
            if (!IsEnabled)
            {
                return;
            }

            switch (gesture.Type)
            {
                case GestureType.LeftButtonDown:
                    isLeftButtonDown = true;

                    if (CaptureMouse())
                    {
                        IsPressed = true;
                    }

                    break;
                case GestureType.LeftButtonUp:
                    isLeftButtonDown = false;

                    if (IsPressed)
                    {
                        OnClick();
                    }

                    ReleaseMouseCapture();
                    IsPressed = false;
                    break;
                case GestureType.Tap:

                    OnClick();

                    break;
                case GestureType.Move:
                    if (isLeftButtonDown && IsMouseCaptured)
                    {
                        IsPressed = HitTest(gesture.Point);
                    }

                    break;
            }
        }
    }
}