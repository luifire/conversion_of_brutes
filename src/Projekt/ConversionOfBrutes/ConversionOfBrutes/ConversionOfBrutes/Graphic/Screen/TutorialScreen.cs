/**
 * Author: ???
 * 
 * Concrete Class for the Credits
 * Usage: This Class just shows the developers of the game 
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class TutorialScreen : Screen
	{
		private int mProgress;
		private Rectangle mSourceRect;
		private Texture2D mBackgroundSheet;

		/// <summary>
		/// Basic constructor for the CreditsScreen
		/// </summary>
		public TutorialScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override sealed void Initialize()
		{

			InitializeAbstr();
			mSourceRect = new Rectangle(0,0,1280,720);
			mBackgroundSheet = Main.Content.Load<Texture2D>("Screen\\tut");
			mProgress = 0;
		}

		public override void Update()
		{
			base.Update();
			if (Main.Input.MouseClicked())
			{
				Main.Input.MouseClickPosition();
				if (Main.Input.LeftMouseClicked)
				{
					if (mProgress < 3)
						mProgress++;
				}
				else if (Main.Input.RightMouseClicked)
				{
					if (mProgress > 0)
						mProgress--;
				}
				switch (mProgress)
				{
					case 0:
						mSourceRect = new Rectangle(0, 0, 1280, 720);
						break;
					case 1:
						mSourceRect = new Rectangle(1280, 0, 1280, 720);
						break;
					case 2:
						mSourceRect = new Rectangle(0, 720, 1280, 720);
						break;
					case 3:
						mSourceRect = new Rectangle(1280, 720, 1280, 720);
						break;
				}
			}
		}


		public override void Draw()
		{
			//Drawing the background in the rectangle
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mBackgroundSheet, ScaledRectangle(0, 0, 1920, 1080), mSourceRect, Color.White);
			mSpriteBatch.End();

			//Drawing each button and decide whether the mouse is sliding over it or not
			foreach (MenuButton button in mItems)
			{
				button.DrawButton();
			}
		}
	}
}
