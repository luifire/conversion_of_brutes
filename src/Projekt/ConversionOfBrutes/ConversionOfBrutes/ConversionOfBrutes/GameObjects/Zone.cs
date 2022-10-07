using System;
using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Library;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;

// Author: buerklij

namespace ConversionOfBrutes.GameObjects
{
    [Serializable]
    public class Zone : WorldObject
    {
		private const float ConquerAreaSize = 100;

	    private Area mConquerArea;	// a Rectangle that contains the Zone's circle
	    private double mFractionPoints;
	    private bool mSeen;

		[NonSerialized]
		private Healthbar mHealthbar;

	    /// <summary>
		/// Constructor. Sets the FractionPoints according to the zone's fraction.
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="position"></param>
	    /// <param name="fraction"></param>
	    /// <param name="areaSize"></param>
		public Zone(Ident id, Vector2 position, Fraction fraction, float areaSize)
			: base(id, position, fraction, areaSize)
        {
            switch (fraction)
            {
                case Fraction.Player:
                    mFractionPoints = 500;
					mSeen = true;
					break;
                case Fraction.Ai:
                    mFractionPoints = -500;
                    break;
                case Fraction.Gaia:
                    mFractionPoints = 0;
                    break;
            }

		    if (id != Ident.HomeZone)
				GameScreen.ParticleManager.AddRingEmitter(this);
			mHealthbar = new Healthbar(this, new Vector2(120, 15), false);
			mConquerArea = new Area(Position.X - ConquerAreaSize, Position.Y - ConquerAreaSize, Position.X + ConquerAreaSize, Position.Y + ConquerAreaSize);
        }

	    public virtual void CopyFromZone(Zone oldZone)
	    {
		    mFractionPoints = oldZone.mFractionPoints;
		    mSeen = oldZone.mSeen;
	    }

		/// <summary>
		/// Used to Draw healthbars. has to happen separately from drawing 3D models
		/// </summary>
	    public virtual void DrawHealthbar()
	    {
			if (Visible)
			{
				GameScreen.GraphicsManager.DrawHealthBar(mHealthbar);
			}
	    }

        /// <summary>
        /// Got a new setter to play a sound when the zone is captured.
        /// </summary>
        public new Fraction Fraction
        {
	        get { return base.Fraction; }
	        set { 
                if (Fraction != value)
                {
	                switch (value)
	                {
		                case Fraction.Player:
				            Main.Audio.PlayUnitSound(AudioManager.Sound.PlayerZoneCaptured, PointPosition);
			                Seen = true;
			                Visible = true;
			                break;
						case Fraction.Ai:
				            Main.Audio.PlayUnitSound(AudioManager.Sound.AiZoneCaptured, PointPosition);
			                break;
	                }
	                if (!GameScreen.MapEditorMode)
	                {
		                if (this is SpawnZone)
		                {
			                while (((SpawnZone) this).SpawnJobs.Count > 0)
			                {
				                GameScreen.GameLogic.CancelSpawn(((SpawnZone) this).SpawnJobs.First.Value);
			                }
			                GameScreen.Hud.UpdateSpawnQueue(this);
		                }
	                }

	                // Need this in order to update the Hud
	                GameScreen.ObjectManager.SelectionHandler.NewSelection = true;

	                base.Fraction = value;
                } 
            }
        }

	    public void Update()
		{
			if (GameScreen.FogOfWar)
			{
				if (TimeToUpdateFow())
				{
					SetVisiblity();
				}
			}
	    }

		public Rectangle ConquerRect { get { return mConquerArea.Rectangle; } }
		public float ConquerCircleSize { get { return ConquerAreaSize;} }
	    public double FractionPoints { get { return mFractionPoints; } set { mFractionPoints = value; } }
		public Healthbar HealthBar { get { return mHealthbar;} }
		public bool Seen { get { return mSeen; } set { mSeen = value; } }
    }

    [Serializable]
    public class SpawnZone : Zone
    {
        public Vector2 UnitDestination { get; set; }
	    private LinkedList<SpawnJob> mSpawnJobs;
	    protected Vector2 mSpawnPosition;

	    public SpawnZone(Ident id, Vector2 position, Fraction fraction, float areaSize)
			: base(id, position, fraction, areaSize)
	    {
		    mSpawnPosition = position + new Vector2(0, 45);
			UnitDestination = mSpawnPosition + Vector2.One;
			mSpawnJobs = new LinkedList<SpawnJob>();
        }

		/// <summary>
		/// Draws the Waypoint ("unitDestination")
		/// Has to be separated from Draw3DStuff because else it wouldn't be drawn if the Zone is out of sight
		/// </summary>
	    public void DrawWaypoint()
	    {
			if (mIsSelected && Fraction == Fraction.Player)
				GameScreen.GraphicsManager.DrawWorldObject(Ident.Waypoint, UnitDestination);
	    }

	    public LinkedList<SpawnJob> SpawnJobs{ get { return mSpawnJobs; } }

		public override void CopyFromZone(Zone oldZone)
	    {
		    mSpawnJobs = ((SpawnZone)oldZone).SpawnJobs;
			base.CopyFromZone(oldZone);
	    }

		public Vector2 SpawnPosition { get { return mSpawnPosition; } }
    }

	[Serializable]
	public class HomeZone : SpawnZone
	{
		public HomeZone(Ident id, Vector2 position, Fraction fraction, float areaSize)
			: base(id, position, fraction, areaSize)
		{
			mSpawnPosition = position + ((fraction == Fraction.Player) ? new Vector2(0, 150) : new Vector2(0, -150));
			UnitDestination = mSpawnPosition + Vector2.One;
		}

		
		public override void DrawHealthbar()
		{
		}

	}
}
