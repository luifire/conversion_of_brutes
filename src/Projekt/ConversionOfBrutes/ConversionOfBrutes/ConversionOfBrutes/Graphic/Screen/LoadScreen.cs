/**
 * Author: David Spisla 
 * 
 * Concrete Class for the LoadGame Screen
 * Usage: This Class Implements a loading screen.  
 * Missing: nothing
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class LoadScreen : Screen
	{
		private bool mFirstUpdate = true;

		/// <summary>
		/// Basic constructor for the LoadingScreen
		/// </summary>
		public LoadScreen()
		{
			Initialize();
			mIsEscClosable = true;
			mUpdateScreensBelow = false;
			mDrawScreensBelow = false;
		}

		public override sealed void Initialize()
		{
			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.LoadingScreen, ScaledRectangle(0, 0, 1920, 1080), null);
		}


		public override void Draw()
		{

			//Drawing the background and the loading label...
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mBackground.FirstTexture, mBackground.Rectangle, Color.White);
			mSpriteBatch.End();

		    Handling2DAnd3D();
		}

		public override void Update()
		{
			base.Update();

			// state machine to skip first update, so it can be drawn
			if (mFirstUpdate)
				mFirstUpdate = false;
			else
			{
				// has to be called here, so the stupid loading screen is shown
				SaveAndLoad.InitializeGameStep2();
			}
		}
	}
}




