/* Authot: buerklij
 * 
 */

using System;
using ConversionOfBrutes.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Misc
{   
    [Serializable]
	public class SpawnJob
	{
		private const int ButtonSize = 219;

		private double mTotalTime;
		private double mTimeRemaining;
		private SpawnZone mSpawn;
		private Ident mUnitIdent;
		private Fraction mUnitFraction;
		private int mCost;

		[NonSerialized]
		private SpriteBatch mSpriteBatch;
	    [NonSerialized]
		private Texture2D mButtonTexture;
		private Rectangle mSourceRect;
		private Rectangle mDestRect;
		private Rectangle mBackgroundTextureSource;
		private Rectangle mBarTextureSource;

		public SpawnJob(double time, int cost, SpawnZone spawn, Ident unit, Fraction fraction)
		{

			mTimeRemaining = time * 1000;
			mTotalTime = TimeRemaining;
			mSpawn = spawn;
			mUnitIdent = unit;
			mUnitFraction = fraction;
			mCost = cost;

			switch (unit)
			{
				case Ident.EliteAtlantic:
					mSourceRect = new Rectangle(ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Priest:
					mSourceRect = new Rectangle(0, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.PriestRanged:
					mSourceRect = new Rectangle(0, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.ShieldGuard:
					mSourceRect = new Rectangle(ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Witch:
					mSourceRect = new Rectangle(3 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.EliteBarbarian:
					mSourceRect = new Rectangle(2 * ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Beast:
					mSourceRect = new Rectangle(4 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Archer:
					mSourceRect = new Rectangle(3 * ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.ArcherMounted:
					mSourceRect = new Rectangle(4 * ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Axeman:
					mSourceRect = new Rectangle(2 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Knight:
					mSourceRect = new Rectangle(3 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				default: mSourceRect = new Rectangle();
					break;
			}

			mBackgroundTextureSource = new Rectangle(526, 934, 1, 1);
			mBarTextureSource = new Rectangle(535, 934, 1, 1);
		}

		public void Update(SpriteBatch spriteBatch, Texture2D buttons, Rectangle destRectangle)
		{
			mSpriteBatch = spriteBatch;
			mButtonTexture = buttons;
			mDestRect = destRectangle;
		}

		public void Draw()
		{
			mSpriteBatch.Draw(mButtonTexture, mDestRect, mSourceRect, Color.White);

			// Draw progress bar
			if (mTimeRemaining < mTotalTime)
			{
				float percent = (float)(mTimeRemaining / mTotalTime);
				Color progressColor = Color.Red;

				Rectangle backgroundRectangle = new Rectangle();
				backgroundRectangle.Width = (int)(mDestRect.Width * 0.95);
				backgroundRectangle.Height = (int)(mDestRect.Width / 8f);
				backgroundRectangle.X = mDestRect.X + (int)(backgroundRectangle.Width * 0.025);
				backgroundRectangle.Y = mDestRect.Y + mDestRect.Height;

				mSpriteBatch.Draw(mButtonTexture, backgroundRectangle, mBackgroundTextureSource, progressColor);

				backgroundRectangle.Width = (int)(backgroundRectangle.Width * (1-percent));

				mSpriteBatch.Draw(mButtonTexture, backgroundRectangle, mBarTextureSource, progressColor);
			}

		}

		public double TimeRemaining
		{
			get { return mTimeRemaining; }
			set { mTimeRemaining = value; }
		}

		public SpawnZone SpawnZone { get { return mSpawn;} }
		public Ident UnitIdent { get { return mUnitIdent; } }
		public Fraction UnitFraction { get { return mUnitFraction; } }
		public Rectangle DestRect { get { return mDestRect; } }
		public int Cost { get { return mCost; } }

	}
}
