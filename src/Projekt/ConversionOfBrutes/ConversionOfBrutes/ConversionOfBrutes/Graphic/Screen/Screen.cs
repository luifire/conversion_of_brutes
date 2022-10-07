/**
 * Author: David Spisla
 * 
 * Abstract Screen Class
 * Usage: Abstract class for all Screens handled by the Screenmanager  
 * Missing: nothing
 **/

using System;
using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Graphic.Screen
{
	public abstract class Screen
	{
		protected bool mIsEscClosable = false;

		/// <summary>
		/// A list with items for the MainMenuScreen
		/// </summary>
		protected List<MenuButton> mItems;

		/// <summary>
		/// Is need to draw a 2D Texture or label
		/// </summary>
		protected SpriteBatch mSpriteBatch;

		/// <summary>
		/// The texture for the background picture of each Screen
		/// </summary>
		protected MenuLabel mBackground;

		/// <summary>
		/// If a screen has a lot of labels you can use this list
		/// </summary>
		protected List<MenuLabel> mLabels;
		
		/// <summary>
		/// Two booleans for deciding to update or draw a screen
		/// </summary>
		protected bool mDrawScreensBelow = false;
		protected bool mUpdateScreensBelow = false;

		public bool DrawScreenBelow { get { return mDrawScreensBelow; } }
		public bool UpdateScreenBelow { get { return mUpdateScreensBelow; } }

		
		/// <summary>
		/// A reference to the ScreenManager.
		/// </summary>
		protected internal ScreenManager mManager;

		/// <summary>
		/// Every screen needs this values to calculate the values with the method ScaledRectangle
		/// </summary>
		protected int mScreenWidth;

		private int mScreenHeight;

		/// <summary>
		/// Initialize every Component of this screen.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// is called right after the screen was added
		/// </summary>
		public virtual void ScreenAdded() {}

		/// <summary>
		/// called when screen is removed
		/// </summary>
		public virtual void ScreenRemoved() { }

		/// <summary>
		/// Update every Enabled Component of this screen.
		/// </summary>
		public virtual void Update()
		{
			if (mIsEscClosable)
			{
				if (Main.Input.WasButtonPressed(Keys.Escape))
				{
					HandleSoundOnExit();
					mManager.RemoveScreen();
				}
			}
		}

		protected virtual void HandleSoundOnExit() { }


		/// <summary>
		/// Draw every Visible Component of this screen.
		/// </summary>
		public abstract void Draw();

		/// <summary>
		/// Here you can add some Labels for the Menu. Using for texts only!!!
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		protected MenuLabel AddLabel(int x, int y, String text)
		{
			return new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(x, y, 0, 0), text);
		}


		/// <summary>
		/// Creates and scales a rectangle.
		/// Input: measures of a rectangle if the screen size was 1920x1080
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns>The scaled rectangle</returns>
		protected Rectangle ScaledRectangle(int x, int y, int width, int height)
		{
			return new Rectangle((int)(x / 1920d * mScreenWidth), (int)(y / 1080d * mScreenHeight), (int)(width / 1920d * mScreenWidth), (int)(height / 1080d * mScreenHeight));
		}

		/// <summary>
		/// Testing if the mouse is sliding over a button
		/// </summary>
		protected void TestMouseSlideOver()
		{
			Point point1 = new Point((int)Main.Input.GetCurrentMousePosition.X, (int)Main.Input.GetCurrentMousePosition.Y);

			foreach (MenuButton button in mItems)
			{
				button.CheckIsMouseOver(point1);
			}
		}

		/// <summary>
		/// If the mouse is clicked run throw all buttons and return the button wich is clicked
		/// </summary>
		/// <returns></returns>
		protected MenuButton MouseClickedGetButton()
		{
			if (Main.Input.MouseClicked())
			{
				foreach (MenuButton button in mItems)
				{
					Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
					if (button.Rectangle.Contains(point))
					{
						return button;
					}
					
				}
			}
			return null;
		}
		
		/// <summary>
		/// This lines can be used for subclasses which has to drawn a background a a list of buttons
		/// </summary>
		protected void DrawButtonsAndBackground()
		{
			//Drawing the background in the rectangle
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mBackground.FirstTexture, mBackground.Rectangle, Color.White);
			mSpriteBatch.End();

			//Drawing each button and decide whether the mouse is sliding over it or not
			foreach (MenuButton button in mItems)
			{
				button.DrawButton();
			}
		}

		/// <summary>
		/// This lines are necessary if a 2D Texture is Drawn on a 3D Map. Just use in InGameMenus...
		/// </summary>
		protected void Handling2DAnd3D()
		{
			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		}

		
		/// <summary>
		/// //This method can be used for Screens which has to draw a list of labels
		/// </summary>
		/// <param name="labels"></param>
		protected void DrawListOfLabels(List<MenuLabel> labels)
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			//Draw Labels
			foreach (MenuLabel item in labels)
			{
				mSpriteBatch.DrawString(item.GetFont(), item.Text, new Vector2(item.Rectangle.X, item.Rectangle.Y), Color.AliceBlue,
				0f, new Vector2(0, 0), 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);
			}
			mSpriteBatch.End();
		}

		/// <summary>
		/// Here you can initialize the members of the super class in every screen
		/// </summary>
		protected void InitializeAbstr()
		{
			GraphicsDeviceManager graphics = Main.Graphics;
			mScreenWidth = graphics.PreferredBackBufferWidth;
			mScreenHeight = graphics.PreferredBackBufferHeight;
			mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);
			mItems = new List<MenuButton>();
			mLabels = new List<MenuLabel>();
			
		}

	}
}
