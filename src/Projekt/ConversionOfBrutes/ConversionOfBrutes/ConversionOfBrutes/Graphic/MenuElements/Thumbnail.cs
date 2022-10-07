using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Author: Julian Bürklin

namespace ConversionOfBrutes.Graphic.MenuElements
{
	class Thumbnail
	{
		private const int ButtonSize = 219;
		private SpriteBatch mSpriteBatch;

		private WorldObject mObj;
		private Healthbar mHealthbar;
		private Healthbar mFPbar;
		private readonly bool mShowFp;
		private readonly Texture2D mSelectionButtons;
		private Rectangle mSourceRect;
		private Rectangle mDestRect;


		/// <summary>
		/// Creates a new Thumbnail for selected objects for the HUD.
		/// </summary>
		/// <param name="obj">The Object to create a Thumbnail to</param>
		/// <param name="spriteBatch">SpriteBatch that is required to draw the sprite.</param>
		/// <param name="buttons"></param>
		/// <param name="rect"></param>
		public Thumbnail(WorldObject obj, Texture2D buttons, Rectangle rect, SpriteBatch spriteBatch)
		{
			mObj = obj;
			mSelectionButtons = buttons;
			mDestRect = rect;
			if (!(obj is Zone) && obj.Fraction == Fraction.Ai)
			{
				mShowFp = true;
			}

			switch (obj.Ident)
			{
				case Ident.Archer:
					mSourceRect = new Rectangle(3*ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.ArcherMounted:
					mSourceRect = new Rectangle(4 * ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Axeman:
					mSourceRect = new Rectangle(2 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.EliteAtlantic:
					mSourceRect = new Rectangle(ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Beast:
					mSourceRect = new Rectangle(4 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.EliteBarbarian:
					mSourceRect = new Rectangle(2 * ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Knight:
					mSourceRect = new Rectangle(3 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Priest:
					mSourceRect = new Rectangle(0, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.PriestRanged:
					mSourceRect = new Rectangle(0, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Witch:
					mSourceRect = new Rectangle(3 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.ShieldGuard:
					mSourceRect = new Rectangle(ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Spawnzone:
					mSourceRect = new Rectangle(0, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.Zone:
					mSourceRect = new Rectangle(4 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.HomeZone:
					mSourceRect = new Rectangle(2 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.TechdemoAtlantic:
					mSourceRect = new Rectangle(ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
				case Ident.TechdemoBarb:
					mSourceRect = new Rectangle(2 * ButtonSize, 2 * ButtonSize, ButtonSize, ButtonSize);
					break;
			}

			mSpriteBatch = spriteBatch;

			mHealthbar = new Healthbar(mObj, new Vector2(rect.X, rect.Bottom-7), new Vector2(rect.Width, 7), false);
			if (mShowFp)
				mFPbar = new Healthbar(mObj, new Vector2(rect.X, rect.Top), new Vector2(rect.Width, 7), true);
		}

		public void Update()
		{
			mHealthbar.Update();
			if (mShowFp)
				mFPbar.Update();
		}

		public void Draw()
		{
			mSpriteBatch.Draw(mSelectionButtons, mDestRect, mSourceRect, Color.White);
			GameScreen.GraphicsManager.DrawHealthBar(mHealthbar, mSpriteBatch);
			if (mShowFp)
				GameScreen.GraphicsManager.DrawHealthBar(mFPbar, mSpriteBatch);
			
		}

		public Rectangle DestRect { get { return mDestRect;} }
		public WorldObject RepresentedObject { get { return mObj;} }
	}
}
