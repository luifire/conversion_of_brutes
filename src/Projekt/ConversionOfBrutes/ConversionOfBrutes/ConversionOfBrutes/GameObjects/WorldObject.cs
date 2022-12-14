// Author: David Luibrand
using System;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Library;
using ConversionOfBrutes.Map;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{
	[Serializable]
	public enum Fraction
	{
		Player,
		Ai,
		Gaia
	};

    [Serializable]
	public enum Ident
	{
		// World Obj
		Castle,
		Tree,
		Tree2,
		Spawnzone,
		Zone,
		MountainSmall,
		Mountain,
		MountainBig,
		Pond,
		HomeZone,

		// Other
		SelectionCircle,
		ZoneCircle,
		Waypoint,

		// Units
		ShieldGuard,
		EliteBarbarian,
		Priest,
		PriestRanged,
		Archer,
		ArcherMounted,
		EliteAtlantic,
		Knight,
		Axeman,
		Beast,
		Horse,
		Witch,
		TechdemoAtlantic,
		TechdemoBarb
	}

    [Serializable]
	public class WorldObject : IQuadStorable
    {
		private readonly Ident mIdent;
		private Fraction mFraction;
		protected float mModelRotation;

	    protected Vector2 mPosition;
		private Point mPointPosition;
		private Area mArea; // Area this Worldobject is in
		private bool mHasMoved = true; // so I don't have to calc a new area all the time
		private readonly float mAreaSize;
	    private readonly int mObjectNumber = sObjectCounter++; // to Identify an Object
		private static int sObjectCounter;
		protected bool mIsSelected;


		// Fog of war 
	    private bool mVisible;
	    private int mSightRange = 200;
	    private TimeSpan mLastFowUpdate;
		private TimeSpan mFowIntervall = TimeSpan.FromMilliseconds(200);

		

	    /// <summary>
	    /// Constructor
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="position"></param>
	    /// <param name="fraction"></param>
	    /// <param name="areaSize">areaSize*areaSize is the size of the area of that worldObject</param>
	    public WorldObject(Ident id, Vector2 position, Fraction fraction, float areaSize)
	    {
		    mIdent = id;
		    position.X = Math.Abs(position.X);
		    position.Y = Math.Abs(position.Y);
		    Position = position;
		    mFraction = fraction;
		    mModelRotation = 0;
		    mAreaSize = areaSize;
		    // to calculate the area due to HasMoved from QuadTree
		    mArea = Area;

		    if (GameScreen.FogOfWar)
		    {
			    //Fog of War
			    if (fraction == Fraction.Player)
			    {
				    mVisible = true;
			    }
		    }
		    else
		    {
			    mVisible = true;
		    }

			if (GameScreen.MapEditorMode)
			{
				mVisible = false;
			}

		    if (Ident == Ident.HomeZone)
		    {
			    SightRange = 400;
		    }

	    }

		#region Fog of war
		// helper function for the Fog of war to determine what is visible for the Player. 
	    protected void SetVisiblity()
		{
			LastFowUpdate = Main.GameTime.TotalGameTime;
			// excludes player objects 
			if (Fraction != Fraction.Player)
			{
				var rangeRect = SightRange * 2;
				// get all objects in the Sightrange
				Rectangle visibleRect = new Rectangle((int)Position.X - rangeRect / 2, (int)Position.Y - rangeRect / 2, rangeRect, rangeRect);
				var visibleObj = GameScreen.Map.GetObjects(visibleRect);
				bool playerInVisibleRect = false;

				foreach (var obj in visibleObj)
				{
					// only player units can reveal Ai + Gaia unts
					if (obj.Fraction == Fraction.Player)
					{
						playerInVisibleRect = true; // playerobject in sightrange
					}
					if (playerInVisibleRect)
					{
						Visible = true;
						// Set Zones to "Seen" - this provides that you'll see the REAL color of a zone if you spot it.
						if ((Ident == Ident.Zone || Ident == Ident.Spawnzone) && Fraction != Fraction.Gaia)
						{
							var zone = (Zone) this;
							if (!zone.Seen)
							{
								zone.Seen = true;
							}
						}

						break; // it is enough to know that there is one Playerobject in range.
					}
				}
				// if there is no Playerobject in sightrange anymore, hide yourself
				if (!playerInVisibleRect && Visible)
				{
					Visible = false;
				}
			}

		}

		// Timer to chekck if the fog of war can be updated.
	    protected bool TimeToUpdateFow()
	    {
		    return mLastFowUpdate + mFowIntervall < Main.GameTime.TotalGameTime;
	    }

	    #endregion
	

		public virtual void Draw3DStuff()
        {
			if (Ident == Ident.Zone || Ident == Ident.Spawnzone || Ident == Ident.HomeZone)
			{
				GameScreen.GraphicsManager.DrawWorldObject(mIdent, mPosition, Fraction, Visible,IsSelected);
			}
			else
			{
				GameScreen.GraphicsManager.DrawWorldObject(mIdent, mPosition, Fraction);
			}
        }
		public Vector2 Position
		{
			protected internal set
			{
				mPosition = value;
				mPointPosition = new Point((int)value.X, (int)value.Y);
				mHasMoved = true;
				GameScreen.Map.Move(this);
			}
			get { return mPosition; }
		}

		public Point PointPosition { get { return mPointPosition; } }

		public int ObjectNumber { get { return mObjectNumber; } }

		public Area Area { 
			get 
			{
				if (mHasMoved)
				{
					mArea = new Area(Position.X - mAreaSize, Position.Y - mAreaSize, Position.X + mAreaSize, Position.Y + mAreaSize);
#if DEBUG
					if (mArea.NanTest())
						throw new Exception("sags Lui #234");
#endif
					mHasMoved = false;
				}
				return mArea; 
			} 
		}

        public Fraction Fraction
        {
            get { return mFraction; }
	        protected set
	        {
				mFraction = value;
	        }
        }

        public Ident Ident { get { return mIdent; } }

		public override String ToString()
		{
			return "" + mObjectNumber + "." + Ident;
		}

	    private TimeSpan LastFowUpdate
	    {
			set { mLastFowUpdate = value; }
	    }

		#region DEBUG_FUNKTION
		public static Ident GetRandomUnit(bool friend, Random rand)
	    {
		    Ident id = Ident.Tree;
		    if (friend)
		    {
			    switch (rand.Next(0, 1000) % 6)
			    {
					case 0:
						id = Ident.Priest;
					    break;
					case 1:
						id = Ident.PriestRanged;
					    break;
					case 2:
						id = Ident.Priest;
						break;
					case 3:
						id = Ident.PriestRanged;
						break;
					case 4:
						id = Ident.EliteAtlantic;
					    break;
					case 5:
						id = Ident.ShieldGuard;
					    break;
			    }
		    }
		    else
		    {
			    switch (rand.Next(0, 1000) % 5)
			    {
					case 0:
						id = Ident.EliteBarbarian;
					    break;
					case 1:
						id = Ident.Archer;
					    break;
					case 2:
						id = Ident.ArcherMounted;
						break;
					case 3:
						id = Ident.Knight;
						break;
					case 4:
						id = Ident.Axeman;
					    break;
			    }
		    }
		    return id;
	    }
		#endregion

		public Rectangle Rect { get { return Area.Rectangle; } }
	    public bool HasMoved { get { return mHasMoved; } }
		//public float AreaSize { get { return mAreaSize; } }
		public bool Visible{get { return mVisible; }
			protected set { mVisible = value; }}

		public int SightRange
		{
			get { return mSightRange; }
			private set { mSightRange = value; }
		}

	    protected float ModelRotation 
	    {
		    get { return mModelRotation; }
	    }
		public bool IsSelected { get{return mIsSelected;} set { mIsSelected = value; } }
    }
}
