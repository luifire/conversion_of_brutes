/**
 * Author: David Spisla 
 * 
 * Concrete Class for the AchievementBar 
 * Usage: This Class implements big Bars for the Achievement process.  
 * Missing: nothing
 * 
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	sealed class Bar
	{
		private SpriteBatch mSpriteBatch;

		private Vector2 mMPosition;
		private Vector2 mMScaledDimension;

		private float mValueMax;
		private Texture2D mTexture;
		private Rectangle mBackSourceRect = new Rectangle();
		

		/// <summary>
		/// Creates a new AchievementBar for the AchieventScreen
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch that is required to draw the sprite.</param>
		/// <param name="rectangle"></param>
		public Bar(SpriteBatch spriteBatch, Rectangle rectangle)
		{

			mTexture = new Texture2D(Main.Game.GraphicsDevice, 1, 1);
			mTexture.SetData(new[] { Color.Green });
			mSpriteBatch = spriteBatch;
			mMPosition.X = rectangle.X;
			mMPosition.Y = rectangle.Y;
			mMScaledDimension.X = rectangle.Width;
			mMScaledDimension.Y = rectangle.Height;
			mValueMax = mMScaledDimension.X * 0.9f;
		}


		public void Draw(float part, float max)
		{
			//Compute the part of the green bar which should be drawn
			float temp = part / max;
			float partOfTheMaximum = mValueMax * temp;

			Color backgroundColor = new Color(0, 0, 0, 200);
			Rectangle backgroundRectangle = new Rectangle();
			backgroundRectangle.Width = (int)mMScaledDimension.X;
			backgroundRectangle.Height = (int)mMScaledDimension.Y;
			backgroundRectangle.X = (int)mMPosition.X;
			backgroundRectangle.Y = (int)mMPosition.Y;

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			//Drawing the background of the bar
			mSpriteBatch.Draw(mTexture, backgroundRectangle, mBackSourceRect, backgroundColor);

			backgroundRectangle.Width = (int)partOfTheMaximum;
			backgroundRectangle.Height = (int)(mMScaledDimension.Y * 0.5);
			backgroundRectangle.X = (int)mMPosition.X + (int)(mMScaledDimension.X * 0.05);
			backgroundRectangle.Y = (int)mMPosition.Y + (int)(mMScaledDimension.Y * 0.25);

			//Drawing the bar
			mSpriteBatch.Draw(mTexture, backgroundRectangle, mBackSourceRect, Color.Green);
		    mSpriteBatch.End();
		}
	}
}
	

