using System;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.Library
{
	[Serializable]
	public sealed class Area
	{
		private float mX1; // always have to be upper left (consider where (0, 0) is, this is the relativ upper left)
		private float mY1;

		private float mX2; // always have to be lower right
		private float mY2;

		private readonly Rectangle mRectangel;


		/// <summary>
		/// does not order points
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		public Area(Vector2 v1, Vector2 v2)
		{
			OrderArea(v1.X, v1.Y, v2.X, v2.Y);
			mRectangel = new Rectangle((int)mX1, (int)mY1, (int)(mX2 - mX1), (int)(mY2 - mY1));
		}

		/// <summary>
		/// Also orderes the point
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="x2"></param>
		/// <param name="y1"></param>
		/// <param name="y2"></param>
		public Area(float x1, float y1, float x2, float y2)
		{
			OrderArea(x1, y1, x2, y2);
			mRectangel = new Rectangle((int)mX1, (int)mY1, (int)(mX2 - mX1), (int)(mY2 - mY1));
		}

		private void OrderArea(float x1, float y1, float x2, float y2)
		{
			mX1 = Math.Min(x1, x2);
			mX2 = Math.Max(x1, x2);

			mY1 = Math.Min(y1, y2);
			mY2 = Math.Max(y1, y2);
		}
#if DEBUG
		public bool NanTest()
		{
			return float.IsNaN(mX1) || float.IsNaN(mX2) || float.IsNaN(mY1) || float.IsNaN(mY2);
		}
#endif

		public Vector2 UpperLeft
		{
			get { return new Vector2(mX1, mY1); }
		}

		public Vector2 LowerRight
		{
			get { return new Vector2(mX2, mY2); }
		}

		/*
		/// <summary>
		/// determins whether point is in this area
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool IsPointInArea(Vector2 point)
		{
			return point.X >= mX1 &&
			point.Y >= mY1 &&
			point.X <= mX2 &&
			point.Y <= mY2;
		}*/

		/// <summary>
		///  Calculates the Size of the area
		/// </summary>
		/// <returns></returns>
		public float AreaSize()
		{
			return (mX2 - mX1) * (mY2 - mY1);
		}

		/// <summary>
		/// checks, whether two areas overlap
		/// </summary>
		/// <param name="a2"></param>
		/// <returns></returns>
		public bool Intersects(Area a2)
		{
			return	mX1 < a2.mX2 && 
					mX2 > a2.mX1 &&
					mY1 < a2.mY2 && 
					mY2 > a2.mY1;
		}

		public Rectangle[] GetRectanglesBordersAsRectangleLines()
		{
			Rectangle[] rects = new Rectangle[4];
			int x1 = (int) mX1;
			int y1 = (int) mY1;
			int x2 = (int) mX2;
			int y2 = (int) mY2;

			rects[0] = new Rectangle(x1, y1, x2 - x1, 2);
			rects[1] = new Rectangle(x1, y2, x2 - x1, 2);
			rects[2] = new Rectangle(x1, y1, 2, y2 - y1 + 1);
			rects[3] = new Rectangle(x2, y1, 2, y2 - y1 + 1);

			return rects;
		}

		public Vector2[] GetBordersAsVectorArray(float offset = 0)
		{
			Vector2[] res = new Vector2[4];

			res[3] = new Vector2(mX1 - offset, mY1 - offset);
			res[2] = new Vector2(mX1 - offset, mY2 + offset);
			res[1] = new Vector2(mX2 + offset, mY2 + offset);
			res[0] = new Vector2(mX2 + offset, mY1 - offset);

			return res;
		}

		public override string ToString()
		{
			return "(" + Math.Floor(mX1) + "," + Math.Floor(mY1) + "," + Math.Floor(Width) + "," + Math.Floor(Height) + ")";
		}

		public Rectangle Rectangle { get { return mRectangel; } }

		private float Width { get { return mX2 - mX1; } }
		private float Height { get { return mY2 - mY1; } }
	}
}
