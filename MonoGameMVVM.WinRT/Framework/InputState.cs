using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameMVVM.WinRT
{
    public class InputState
    {
        public const int MaxInputs = 1;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        private MouseState _currentMouseState;
        private MouseState _lastMouseState;
        private double _leftMouseClickTimer;
        private int _leftMouseClickCount;
        public TouchCollection TouchState;
        public readonly bool[] GamePadWasConnected;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[4];
            CurrentGamePadStates = new GamePadState[4];
            LastKeyboardStates = new KeyboardState[4];
            LastGamePadStates = new GamePadState[4];
            GamePadWasConnected = new bool[4];
            _currentMouseState = default(MouseState);
            _lastMouseState = default(MouseState);
        }

        //public static void InitializeTouchPanel()
        //{
        //    TouchPanel.EnabledGestures = GestureType.Tap;
        //}

        public MouseState CurrentMouseState
        {
            get
            {
                return _currentMouseState;
            }
        }

        public MouseState LastMouseState
        {
            get
            {
                return _lastMouseState;
            }
        }

        public bool IsDoubleClickOrTap(Rectangle inputBounds)
        {
            if (Gestures.Any(gestureSample => gestureSample.GestureType == Microsoft.Xna.Framework.Input.Touch.GestureType.DoubleTap && inputBounds.Contains((int)gestureSample.Position.X, (int)gestureSample.Position.Y)))
            {
                return true;
            }
            if (IsNewLeftMouseDoubleClick())
                return inputBounds.Contains(new Microsoft.Xna.Framework.Point(_currentMouseState.X, _currentMouseState.Y));

            return false;
        }

        public GameTime Time { get; protected set; }
        public Vector2 ScrollOffset { get; set; }

        public bool IsSinglePointerRelease(Rectangle inputBounds)
        {
            if (_currentMouseState.LeftButton == ButtonState.Pressed && inputBounds.Contains(_currentMouseState.X, _currentMouseState.Y))
                return false;
            bool flag = false;

            foreach (TouchLocation touchLocation in TouchState)
            {
                Microsoft.Xna.Framework.Point point = new Microsoft.Xna.Framework.Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y);
                if (inputBounds.Contains(point))
                {
                    if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                        return false;
                    if (touchLocation.State == TouchLocationState.Released)
                        flag = true;
                }
            }

            return IsNewLeftMouseClick(inputBounds) || flag;
        }

        public bool IsNewLeftMouseEvent(out ButtonState state)
        {
            state = _currentMouseState.LeftButton;
            return state != _lastMouseState.LeftButton;
        }

        public bool IsLeftMouseUp()
        {
            return _currentMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsNewLeftMouseDoubleClick()
        {
            if (_currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
                return _leftMouseClickCount == 2;

            return false;
        }

        public bool IsNewRightMouseClick()
        {
            if (_currentMouseState.RightButton == ButtonState.Released)
                return _lastMouseState.RightButton == ButtonState.Pressed;

            return false;
        }

        public bool IsNewRightMouseEvent(out ButtonState state)
        {
            state = _currentMouseState.RightButton;
            return state != _lastMouseState.RightButton;
        }

        public bool IsRightMouseDown()
        {
            return _currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsRightMouseDown(Rectangle bounds)
        {
            if (!IsPointerWithin(bounds))
                return false;

            return _currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsPointerWithin(Rectangle inputBounds)
        {
            if ((from touchLocation in TouchState let point = new Microsoft.Xna.Framework.Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y) where inputBounds.Contains(point) && (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved) select touchLocation).Any())
            {
                return true;
            }
            return inputBounds.Contains(new Microsoft.Xna.Framework.Point(_currentMouseState.X, _currentMouseState.Y));
        }

        public bool IsLeftMouseDown()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsNewThirdMouseClick()
        {
            if (_currentMouseState.MiddleButton == ButtonState.Pressed)
                return _lastMouseState.MiddleButton == ButtonState.Released;

            return false;
        }

        public bool IsNewMouseScrollUp()
        {
            return _currentMouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue;
        }

        public bool IsNewMouseScrollDown()
        {
            return _currentMouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue;
        }

        public int GetMouseScroll()
        {
            return _currentMouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue;
        }

        public bool IsNewMouseDelta(out int x, out int y) 
        {
            x = _currentMouseState.X - _lastMouseState.X;
            y = _currentMouseState.Y - _lastMouseState.Y;
            if (x == 0)
                return y != 0;

            return true;
        }
        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Time = gameTime;
            for (int i = 0; i < 4; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentKeyboardStates[i] = Keyboard.GetState();
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            double totalMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
            _leftMouseClickTimer += totalMilliseconds;

            if (_leftMouseClickTimer > 250.0)
            {
                _leftMouseClickTimer = 0.0;
                _leftMouseClickCount = 0;
            }
            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (_currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                _leftMouseClickCount++;
                _leftMouseClickTimer = 0.0;
                if (_leftMouseClickCount > 2)
                {
                    _leftMouseClickCount = 1;
                }
            }
            TouchState = TouchPanel.GetState();
            Gestures.Clear();

            foreach (TouchLocation touchLocation in TouchState)
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    Gestures.Add(new GestureSample(Microsoft.Xna.Framework.Input.Touch.GestureType.Tap, gameTime.ElapsedGameTime, touchLocation.Position, Vector2.Zero, Vector2.Zero, Vector2.Zero));
                }
            }

            //while (TouchPanel.IsGestureAvailable)
            //{
            //    Gestures.Add(TouchPanel.ReadGesture());
            //}
        }


        /// <summary>
        /// Helper for checking if a key was pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentKeyboardStates[i].IsKeyDown(key);
            }

            // Accept input from any player.
            return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
                    IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
                    IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
                    IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
        }

        /// <summary>
        /// Helper for checking if a button was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
            }

            // Accept input from any player.
            return (IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Four, out playerIndex));
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }

            // Accept input from any player.
            return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
        }

        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;
                int num = (int)playerIndex;
                return CurrentGamePadStates[num].IsButtonDown(button) && LastGamePadStates[num].IsButtonUp(button);
            }
            return IsNewButtonPress(button, PlayerIndex.One, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
        }

        public Vector2 GetMousePos()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }

        public bool IsNewLeftMouseClick(Rectangle inputBounds)
        {
            return _currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed && _leftMouseClickCount == 1;
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardStates[0].IsKeyDown(key);
        }

        public bool IsNewTap()
        {
            return Gestures.Any(gesture => gesture.GestureType == Microsoft.Xna.Framework.Input.Touch.GestureType.Tap);
        }

        public bool IsNewPointerDown(Rectangle inputBounds)
        {
            if ((from touchLocation in TouchState let point = new Microsoft.Xna.Framework.Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y) where inputBounds.Contains(point) && touchLocation.State == TouchLocationState.Pressed select touchLocation).Any())
            {
                return true;
            }
            return inputBounds.Contains(new Microsoft.Xna.Framework.Point(_currentMouseState.X, _currentMouseState.Y)) && _currentMouseState.LeftButton == ButtonState.Pressed && _currentMouseState.LeftButton != _lastMouseState.LeftButton;
        }

        public bool TryGetTouchDragPos(out Vector2 pos)
        {
            pos = Vector2.Zero;
            var gesture = Gestures.LastOrDefault(g => g.GestureType == Microsoft.Xna.Framework.Input.Touch.GestureType.HorizontalDrag);
            if (gesture.Timestamp == TimeSpan.Zero)
                return false;

            pos = gesture.Position; //Position = startpoint, Position2 = endpoint of the drag
            return true;
        }
    }
}
