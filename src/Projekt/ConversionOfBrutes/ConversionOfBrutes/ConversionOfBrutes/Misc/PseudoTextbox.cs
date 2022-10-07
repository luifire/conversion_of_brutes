/**
 * Author: David Spisla 
 * 
 * Concrete Class for a (Pseudo)Textbox
 * Usage: This Class Implements a Pseudo-Textbox for editing the Hotkeys 
 * or for use in the loading process (part of LoadComponent)
 * Missing: nothing
 * 
 **/

using System;
using ConversionOfBrutes.Graphic.MenuElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Misc
{

	public enum TextBoxMode
	{
		Hotkey,
		LoadingOrSave
	}

	internal class PseudoTextBox
	{
		private SpriteBatch mSpriteBatch;
		private Rectangle mRectangle;
		private MenuLabel mStartString;
		private MenuLabel mNewString;
		private Texture2D mTexture1;
		private Texture2D mTexture2;
		private TextBoxMode mMode;
		private readonly HotKeys.HotKey mKey;
		private bool mIsMouseOver;
		private bool mIsEditMode;
		private int mScreenWidth;
		private int mIndex;
		private Keys[] mHotKeys;

		/// <summary>
		/// Basic constructor
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="start"></param>
		/// <param name="rec"></param>
		public PseudoTextBox(TextBoxMode mode, HotKeys.HotKey key, MenuLabel start, Rectangle rec)
		{
			mMode = mode;
			mKey = key;
			mStartString = start;
			mNewString = start;
			mRectangle = rec;

			mTexture1 = Main.Content.Load<Texture2D>("Button\\black_square");
			mTexture2 = Main.Content.Load<Texture2D>("Button\\red_square");

			GraphicsDeviceManager graphics = Main.Graphics;
			mScreenWidth = graphics.PreferredBackBufferWidth;
			mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			mHotKeys = new[]
			{
				Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N,
			    Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.D0, Keys.D1,
				Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Up, Keys.Left, Keys.Down, Keys.Right, 
				Keys.Escape, Keys.Space
			};

		}

		/// <summary>
		/// Second constructor fur using in the LoadMap or LoadSaveGame mode
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="stringLoad"></param>
		/// <param name="rectangle"></param>
		/// <param name="index"></param>
		public PseudoTextBox(TextBoxMode mode, MenuLabel stringLoad, Rectangle rectangle, int index)
		{
			mMode = mode;
			mStartString = stringLoad;
			mNewString = stringLoad;
			mRectangle = rectangle;
			mIndex = index;

			mTexture1 = Main.Content.Load<Texture2D>("Button\\black_square");
			mTexture2 = Main.Content.Load<Texture2D>("Button\\white_square");

			GraphicsDeviceManager graphics = Main.Graphics;
			mScreenWidth = graphics.PreferredBackBufferWidth;
			mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);


		}

		/// <summary>
		/// Testing if the mouse is sliding the textbox
		/// </summary>
		public void TestMouseSlideOver()
		{
			Point point1 = new Point((int)Main.Input.GetCurrentMousePosition.X, (int)Main.Input.GetCurrentMousePosition.Y);
			mIsMouseOver = mRectangle.Contains(point1);	
		}

		
		
		/// <summary>
		/// Use this to activate the EditMode
		/// </summary>
		public bool TestEditMode()
		{

			Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
			if (mRectangle.Contains(point))
			{
				mIsEditMode = true;
				return mIsEditMode;
			}
			return false;
		}


		/// <summary>
		/// Use this in Load Mode
		/// </summary>
		public void DrawLoadMode()
		{
			if (!mIsEditMode) 
			   MouseIsOver();

			if (mIsEditMode)
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.Draw(mTexture2, mRectangle, mRectangle, Color.White);
				mSpriteBatch.End();
			}
		}

		
		/// <summary>
		/// Drawing all elements of this textbox
		/// </summary>
		public void DrawHotkeyMode()
		{

			MouseIsOver();

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.DrawString(mNewString.GetFont(),
				mNewString.Text,
				new Vector2((int) (mRectangle.X + mRectangle.Width * 0.05), (int) (mRectangle.Y + mRectangle.Height * 0.05)),
				Color.AliceBlue,
				0f,
				new Vector2(0, 0),
				2 * (mScreenWidth / 1920f),
				SpriteEffects.None,
				0f);

			mSpriteBatch.End();
		}



		/// <summary>
		/// Testing if the mouse is over
		/// </summary>
		private void MouseIsOver()
		{

			if (mIsMouseOver)
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.Draw(mTexture2, mRectangle, mRectangle, Color.White);
				mSpriteBatch.End();
				
			}
			else
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.Draw(mTexture1, mRectangle, mRectangle, Color.White);
				mSpriteBatch.End();
				
			}
		}

		/// <summary>
		/// Check if the pressed Hotkey is already used
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private bool CheckIfKeyIsUsed(Keys key)
		{

			Keys[] temp = Main.HotKey.GetArrayOfKeys;
			for (int i = 0; i < temp.Length; i++)
			{
				if (temp[i] == key)
					return true;
			}

			return false;
		}


		/// <summary>
		/// Update the Hotkeys and the Text, while using the CheckIfKeyIsUsed method  
		/// </summary>
		/// <returns></returns>
		public bool HotkeyPress()
		{
		    if (mMode == TextBoxMode.Hotkey && mIsEditMode)
			{
				KeyboardState newKeyboardState = Keyboard.GetState();
				String newText = "_";
				bool temp = true;

				for (int index = 0; index < mHotKeys.Length; index++)
				{
					Keys key = mHotKeys[index];
					if (newKeyboardState.IsKeyDown(key))
					{
						if (!CheckIfKeyIsUsed(key))
						{
							Main.HotKey.SetHotkey(mKey, key);
							newText = key.ToString();
							mIsEditMode = false;
							temp = false;
						}
					}
				}

				mNewString.Text = newText;
				return temp;
			}

			return true;
		}

		public MenuLabel StartString { get { return mStartString; } }
		public bool Editmode {
			set { mIsEditMode = value; } }

		public int Index { get { return mIndex; } }
	}

}






