using ConversionOfBrutes.Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

// Author: luibrand
// Controls Input (somehow)

namespace ConversionOfBrutes.Misc
{
	public sealed class InputManager
	{
		public enum MouseButton
		{
			None,
			Left,
			Right
		};

		// wird sich wahrscheinlich noch ändern
		private MouseState mMouseState;
		private Vector2 mMousePosition;
		private MouseState mLastMouseState;
		private KeyboardState mKeyboardState;
		private KeyboardState mLastKeyboardState;

		private bool mMouseHold;
		private bool mAreaSelected;
		private bool mMouseClicked; // actual click
		private MouseButton mUsedMouseButton = MouseButton.None;

		private Vector2 mMousePressedStartPos;
		private Point mMousePressedStartPoint;
		private Vector2 mMousePressedEndPos;

		public void Update(KeyboardState keyboardState, MouseState mouseState)
		{
			if (Main.Game.IsActive)
			{
				// save current info
				mLastMouseState = MouseState;
				MouseState = mouseState;

				mLastKeyboardState = mKeyboardState;
				mKeyboardState = keyboardState;

				HandleMouse();	
			}
			else
			{
				mLastMouseState = new MouseState();
				MouseState = new MouseState();
				mLastKeyboardState = new KeyboardState();
				mKeyboardState = new KeyboardState();
			}
		}


		private void HandleMouse()
		{
			bool bLeftButtonPressed = MouseState.LeftButton == ButtonState.Pressed;
			bool bRightButtonPressed = MouseState.RightButton == ButtonState.Pressed;

			//--slightly dumb programmed
			// Both Mouse Buttons Pressed => nothing is pressed
			if (bLeftButtonPressed && bRightButtonPressed)
			{
				mMouseHold = false;
				mMouseClicked = false;
				mUsedMouseButton = MouseButton.None;
			}
			// new Press started
			else if (mMouseHold == false && (bLeftButtonPressed || bRightButtonPressed))
			{
				mMouseHold = true;

				// save start info
				mMousePressedStartPos = GetCurrentMousePosition;
				mMousePressedStartPoint = GetCurrentMousePositionAsPoint();

				mAreaSelected = false;
				mMouseClicked = false;

				mUsedMouseButton = bLeftButtonPressed ? MouseButton.Left : MouseButton.Right;
			}
			// Mouse was pressed, now released
			else if (mMouseHold && (bLeftButtonPressed || bRightButtonPressed) == false)
			{
				mMouseHold = false;
				//TimeSpan span = gameTime.TotalGameTime - mMousePressedStart;
				mMousePressedEndPos = GetCurrentMousePosition;

				mAreaSelected = true;

				Area area = GetSelectedArea();

				// timespan triggered an area
				if (area.AreaSize() > 7)
				{
					mAreaSelected = true;
				}
				else
				{
					mMouseClicked = true;
					// uses startPos to determin clicked position
				}
			}
		}

		/// <summary>
		/// each button can only be fetched once
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool WasButtonPressed(Keys key)
		{
			return mKeyboardState.IsKeyUp(key) && mLastKeyboardState.IsKeyDown(key);
		
		}

		public bool CtrlDownAndButtonPressed(Keys key)
		{
			return WasButtonPressed(key) && mLastKeyboardState.IsKeyDown(Keys.LeftControl) ||
					WasButtonPressed(Keys.LeftControl) && mLastKeyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// States if button is pressed at the moment
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyDown(Keys key)
		{
			return mKeyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// Tells, whether an area has been selected
		/// </summary>
		/// <returns></returns>
		public bool IsAreaSelected()
		{
			return mAreaSelected;
		}

		/// <summary>
		/// Returns Selected Area, can only happen onces
		/// </summary>
		/// <returns></returns>
		public Area GetSelectedArea()
		{
			mAreaSelected = false;
			return new Area(mMousePressedStartPos, mMousePressedEndPos);
		}

		public MouseButton UsedMouseButton { get { return mUsedMouseButton; } }

		/// <summary>
		/// is mouse clicked?
		/// </summary>
		/// <returns></returns>
		public bool MouseClicked()
		{
			return mMouseClicked;
		}

		/// <summary>
		/// Last Clickposition, indipentend of MouseButton
		/// </summary>
		/// <returns></returns>
		public Vector2 MouseClickPosition()
		{
			mMouseClicked = false;
			return mMousePressedStartPos;
		}

		/// <summary>
		/// Use this, when MouseClick was used, so the windows underneath cant get that click
		/// </summary>
		public void MouseClickNotUsed()
		{
			mMouseClicked = true;
		}

		/// <summary>
		/// Use if you didn't use the area selection
		/// </summary>
		public void AreaSelectNotUsed()
		{
			mAreaSelected = true;
		}

		public Vector2 MouseClickStartPosition { get { return mMousePressedStartPos; } }
		public Point MouseClickStartPositionAsPoint { get { return mMousePressedStartPoint; } }

		public bool LeftMouseClicked
		{
			get { return MouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed; }
		}
		public bool RightMouseClicked
		{
			get { return MouseState.RightButton == ButtonState.Released && mLastMouseState.RightButton == ButtonState.Pressed; }
		}
		public bool MiddleMouseClicked
		{
			get { return MouseState.MiddleButton == ButtonState.Released && mLastMouseState.MiddleButton == ButtonState.Pressed; }
		}

		public MouseState MouseState
		{
			get { return mMouseState; }
			private set 
			{
				mMouseState = value;
				mMousePosition = new Vector2(value.X, value.Y);
			}
		}
		private MouseState LastMouseState
		{
			get { return mLastMouseState; }
		}

		public KeyboardState KeyboardState
		{
			get { return mKeyboardState; }
		}

		public Vector2 GetCurrentMousePosition
		{
			//return new Vector2(MouseState.X, MouseState.Y);
			get { return mMousePosition; }
		}

		public Point GetCurrentMousePositionAsPoint()
		{
			return new Point(MouseState.X, MouseState.Y);
		}
	}
}
