using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Zone = ConversionOfBrutes.GameObjects.Zone;

// Author: Julian Bürklin

namespace ConversionOfBrutes.Graphic.MenuElements
{
	public class Healthbar
	{
		private Vector2 mPosition;
		private Vector2 mDimension;
		private Vector2 mScaledDimension;

		private float mValueMax;
		private float mValueCurrent;
		private bool mFaith;
		private bool mIsThumbnailBar;
		private WorldObject mObj;
		private Rectangle mBackSourceRect;
		private Rectangle mBarSourceRect;
		private float mPercent;

		/// <summary>
		/// Creates a new Healthbar for a (selectable) WorldObject.
		/// </summary>
		/// <param name="obj">The object that is represented by the bar</param>
		/// <param name="dimension">Component dimensions.</param>
		/// <param name="showsFaithPoints">Determines whether this is a health or a faithpoint bar.</param>
		public Healthbar(WorldObject obj, Vector2 dimension, bool showsFaithPoints)
		{
			mScaledDimension = dimension;
			mDimension = dimension;
			mFaith = showsFaithPoints;
			mObj = obj;
			mIsThumbnailBar = false;

			if (mObj is Zone)
			{
				mPosition = ToScreenCoordinates(mObj.Position);
				mValueMax = 1000;
			}
			else
			{
				mValueMax = mFaith ? ((Unit)mObj).MaxFaithPoints : ((Unit)mObj).MaxHealthPoints;
			}
			InitSourceRects();
		}

		/// <summary>
		/// Creates a new healthbar as part of a thumbnail.
		/// </summary>
		/// <param name="obj">The object that is represented by the bar</param>
		/// <param name="position"></param>
		/// <param name="dimension">Component dimensions.</param>
		/// <param name="showsFaithPoints">Determines whether this is a health or a faithpoint bar.</param>
		public Healthbar(WorldObject obj, Vector2 position, Vector2 dimension, bool showsFaithPoints)
		{
			mScaledDimension = dimension;
			mFaith = showsFaithPoints;
			mObj = obj;
			mPosition = position;
			mIsThumbnailBar = true;

			if (mObj is Zone)
			{
				mValueMax = 1000;
			}
			else
			{
				mValueMax = mFaith ? ((Unit)mObj).MaxFaithPoints : ((Unit)mObj).MaxHealthPoints;
			}
			InitSourceRects();
		}

		/// <summary>
		///  Sets the source rectangles for mSelectionButtons
		/// </summary>
		private void InitSourceRects()
		{
			mBackSourceRect = new Rectangle(526, 934, 1, 1);
			mBarSourceRect = mFaith ? new Rectangle(532, 934, 1, 1) : new Rectangle(529,934,1,1);

		}

		public void Update()
		{
			if (!mIsThumbnailBar)
			{
				float scale = (1-GameScreen.Camera.Position.Y/2000);
				scale = (scale < 0.25) ? 0.25f : scale;
				mScaledDimension = mDimension * scale;
				mPosition = ToScreenCoordinates(mObj.Position);
				mPosition.X -= (int)(mScaledDimension.X / 2);

				if (mFaith)
					mPosition.Y -= 10*scale;
			}
			
			if (mObj is Zone)
			{
				mValueCurrent = (int) ((Zone) mObj).FractionPoints + 500;
			}
			else
			{
				mValueCurrent = mFaith ? ((Unit)mObj).FaithPoints : ((Unit)mObj).HealthPoints;
			}

			mPercent = mValueCurrent / mValueMax;
		}

		/// <summary>
		/// Projects a 3D world coordinate to 2D screen coordinate
		/// </summary>
		/// <param name="worldCoords"></param>
		/// <returns></returns>
		private Vector2 ToScreenCoordinates(Vector2 worldCoords)
		{
			Vector3 modelPosition;
			
			modelPosition.X = worldCoords.X;
			modelPosition.Y = ((mObj.Ident == Ident.Knight || mObj.Ident == Ident.ArcherMounted) ? 30 : 20);
			modelPosition.Z = worldCoords.Y;

			var screenPositon = Main.Graphics.GraphicsDevice.Viewport.Project(modelPosition,
				GameScreen.Camera.ProjectionMatrix,
				GameScreen.Camera.ViewMatrix,
				Matrix.Identity);
			return new Vector2(screenPositon.X, screenPositon.Y);
		}

		public bool ShowsFp { get { return mFaith;} }
		public Vector2 ScaledDimension { get { return mScaledDimension;} }
		public Vector2 Position { get { return mPosition;} }
		public float Percent { get { return mPercent;} }
		public Rectangle BackSourceRect { get { return mBackSourceRect;} }
		public Rectangle BarSourceRect { get { return mBarSourceRect;} }
		public WorldObject RepresentedObject { get { return mObj;} }
	}
}
