using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConversionOfBrutes.GameLogics;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class LoadingScreen : Screen
	{

		private SpriteBatch mSpriteBatch;
		private MenuLabel mBackground;
		private MenuLabel mLoading;

		/// <summary>
		/// Basic constructor for the LoadingScreen
		/// </summary>
		public LoadingScreen()
		{
			Initialize();
		}

		public override void Initialize()
		{

			GraphicsDeviceManager graphics = Main.Graphics;
			mScreenWidth = graphics.PreferredBackBufferWidth;
			mScreenHeight = graphics.PreferredBackBufferHeight;


			mTranslucent = false;
			mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);


			mBackground = new MenuLabel(MenuItem.MenuIdentifier.Background, ScaledRectangle(0, 0, 1920, 1080), null);
			mLoading = new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(800, 750, 0, 0), "Game is loading...");

			

			mIsLoading = true;
		}

		public override void Update(GameTime gameTime){}


		public override void Draw()
		{

			//Drawing the background and the loading Label
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mBackground.Texture, mBackground.Rectangle, Color.White);

			mSpriteBatch.DrawString(mLoading.GetFont(), mLoading.Text, new Vector2(mLoading.Rectangle.X, mLoading.Rectangle.Y), Color.AliceBlue,
				0f, new Vector2(0, 0), 4f, SpriteEffects.None, 0f);

			mSpriteBatch.End();

			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

		}
	
   }
}




