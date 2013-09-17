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
using System.Reactive.Subjects;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameMVVM
{
    public class InputManager : IInputManager
    {
        private readonly Subject<Gesture> _gestures = new Subject<Gesture>();

        private MouseState _previousState;

        public InputManager()
        {
            TouchPanel.EnabledGestures |= Microsoft.Xna.Framework.Input.Touch.GestureType.FreeDrag | Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }

        public IObservable<Gesture> Gestures
        {
            get { return _gestures; }
        }

        public void Update()
        {
            MouseState currentState = Mouse.GetState();
            if (_previousState.LeftButton == ButtonState.Released && currentState.LeftButton == ButtonState.Pressed)
            {
                _gestures.OnNext(
                    new Gesture(GestureType.LeftButtonDown, new Point(currentState.X, currentState.Y)));
            }
            else if (_previousState.LeftButton == ButtonState.Pressed &&
                     currentState.LeftButton == ButtonState.Released)
            {
                _gestures.OnNext(
                    new Gesture(GestureType.LeftButtonUp, new Point(currentState.X, currentState.Y)));
            }

            if (currentState.X != _previousState.X || currentState.Y != _previousState.Y)
            {
                _gestures.OnNext(new Gesture(GestureType.Move, new Point(currentState.X, currentState.Y)));

#if !WINDOWS_PHONE
                if (_previousState.LeftButton == ButtonState.Pressed &&
                    currentState.LeftButton == ButtonState.Pressed)
                {
                    _gestures.OnNext(
                        new Gesture(
                            GestureType.FreeDrag,
                            new Point(currentState.X, currentState.Y),
                            new Vector(currentState.X - _previousState.X, currentState.Y - _previousState.Y)));
                }
#endif
            }

            _previousState = currentState;

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                switch (gesture.GestureType)
                {
                    case Microsoft.Xna.Framework.Input.Touch.GestureType.FreeDrag:
                        _gestures.OnNext(
                            new Gesture(
                                GestureType.FreeDrag,
                                new Point(gesture.Position.X, gesture.Position.Y),
                                new Vector(gesture.Delta.X, gesture.Delta.Y)));
                        break;
                    case Microsoft.Xna.Framework.Input.Touch.GestureType.Tap:
                        _gestures.OnNext(
                            new Gesture(
                                GestureType.Tap,
                                new Point(gesture.Position.X, gesture.Position.Y)));
                        break;
                }
            }
        }
    }
}