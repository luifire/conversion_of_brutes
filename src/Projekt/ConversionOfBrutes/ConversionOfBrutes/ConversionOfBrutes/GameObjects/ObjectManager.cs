using System;
using System.Collections.Generic;
using ConversionOfBrutes.AI.Pathfinding;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using RVO;

//Author: luibrand, loefflju, buerklin

namespace ConversionOfBrutes.GameObjects
{   
	
	/// <summary>
	/// Interaktion zwischen Spieler und Spiel
	/// </summary>
	public sealed class ObjectManager
	{
		private LinkedList<WorldObject> mMapObjects;
		private LinkedList<Unit> mUnits;
		private LinkedList<Tuple<Unit, TimeSpan>> mDeadUnits;
		private LinkedList<Zone> mZones; 
		private LinkedList<WorldObject> mSelectableObjects;
		private SelectionHandler mSelectionHandler;
		private AStarPathfinder mPathfinder;
		private int mCountUnitsAtlantis;
		private int mCountUnitsBarbarian;

		private int mSendCounter = 50000; // gives an ident to all sent units, starts at 50000 because of taunt


		public ObjectManager()
		{
			mCountUnitsAtlantis = 0;
			mCountUnitsBarbarian = 0;
			mMapObjects = new LinkedList<WorldObject>();
			mUnits = new LinkedList<Unit>();
			mDeadUnits = new LinkedList<Tuple<Unit, TimeSpan>>();
			mZones = new LinkedList<Zone>();
			mSelectableObjects = new LinkedList<WorldObject>();
			mSelectionHandler = new SelectionHandler();
			mPathfinder = new AStarPathfinder();

			Simulator.Instance.setTimeStep(0.1f);
			Simulator.Instance.setAgentDefaults(20.0f, 10, 5.0f, 10.0f, 8.0f, 20.0f, Vector2.Zero);
		}

		public void CastTaunt()
		{
			LinkedList<WorldObject> selectedObjects = mSelectionHandler.SelectedObjects;
			foreach (var o in selectedObjects)
			{
				if (o.Ident == Ident.ShieldGuard && o.Fraction == Fraction.Player)
				{
					var unit = (ShieldGuard)o;
					unit.Taunt();
				}
			}
		}

		public int GetFreeSendId()
		{
			return mSendCounter++;
		}

		public void StopSelectedUnits()
		{
			LinkedList<WorldObject> selectedObjects = mSelectionHandler.SelectedObjects;
			foreach (var o in selectedObjects)
			{
				if (!(o is Unit)) continue;

				if (o.Fraction == Fraction.Player)
				{
					var unit = (Unit)o;
					unit.Stop();
				}
			}
		}

		/// <summary>
		/// Lets all selected Units attack the given unit
		/// </summary>
		/// <param name="attackedUnit"></param>
		public void AttackUnit(Unit attackedUnit)
		{
			var selectedObjects = mSelectionHandler.SelectedObjects;
			if (selectedObjects.Count > 0 && selectedObjects.First.Value.Fraction == Fraction.Player &&
			    attackedUnit.Fraction == Fraction.Ai)
			{
				foreach (var selectedObj in selectedObjects)
				{
					var unit = (Unit) selectedObj;
					unit.Attack(attackedUnit);
				}
			}
		}

		/// <summary>
		/// Sends the selected units to attackMove to the destination
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="selectedUnits"></param>
		public void AttackMove(Vector2 destination, LinkedList<WorldObject> selectedUnits)
		{
			ApplyPulkWalk(selectedUnits, destination);
			foreach (var selectedObj in selectedUnits)
			{
				var unit = (Unit)selectedObj;
				unit.AttackMove(destination);
			}
		}


		/// <summary>
		/// Sends the selected units patrolling between their current position and destination
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="selectedUnits"></param>
		public void StartPatrol(Vector2 destination, LinkedList<WorldObject> selectedUnits)
		{
			ApplyPulkWalk(selectedUnits, destination);
			foreach (var selectedObj in selectedUnits)
			{
				var unit = (Unit)selectedObj;
				unit.StartPatrol(destination);
			}
		}

		/// <summary>
		/// Remove a Unit
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public void UnitDied(Unit unit)
		{
			// TODO naja eigentlich sollte die halt erstmal rumliegen und der Obj Manager die dann irgendwann aufräumen
			mUnits.Remove(unit);
			mSelectableObjects.Remove(unit);
			GameScreen.Map.Remove(unit);
			mSelectionHandler.UnitDied(unit);
			Simulator.Instance.removeAgent(unit.mCollisionId);
			mDeadUnits.AddLast(new Tuple<Unit, TimeSpan>(unit, Main.GameTime.TotalGameTime));
			
			//For the GameStatistic
			if (GameScreen.MapEditorMode == false)
			{ 
				if (unit.Fraction == Fraction.Player)
				{
					Main.mGameStatistic.LostUnits++;
					mCountUnitsAtlantis--;
				}
				else
				{
					Main.mGameStatistic.KilledUnits++;

					if (Main.mAchievements.OverAllKilled < Main.AxUnstobbable)
						Main.mAchievements.OverAllKilled++;

					mCountUnitsBarbarian--;
				}

				//For the Achievement
				if (unit.Fraction == Fraction.Ai && unit.LastAttacker.Ident == Ident.EliteAtlantic && Main.mAchievements.Rampage < 10)
				{
					Main.mAchievements.Rampage++;
				}
			}
		}

		/// <summary>
		/// Generates World Objects
		/// </summary>
		/// <param name="id"></param>
		/// <param name="position"></param>
		/// <param name="fraction"></param>
		/// <returns></returns>
		public WorldObject CreateWorldObject(Ident id, Vector2 position, Fraction fraction)
		{
			WorldObject newObject = null;
            var isMapObject = true;

            // First, check if the new object is a map object
            switch (id)
            {
                case Ident.Castle:
					newObject = new WorldObject(id, position, fraction, 35);
		            break;
                case Ident.Tree:
					newObject = new WorldObject(id, position, fraction, 12.5f);
		            break;
                case Ident.Tree2:
					newObject = new WorldObject(id, position, fraction, 12.5f);
		            break;
                case Ident.MountainSmall:
                    newObject = new WorldObject(id, position, fraction, 25f);
					break;
                case Ident.Mountain:
                    newObject = new WorldObject(id, position, fraction, 41);
					break;
                case Ident.MountainBig:
                    newObject = new WorldObject(id, position, fraction, 95);
					break;
                case Ident.Pond:
                    newObject = new WorldObject(id, position, fraction, 41);
					break;
                case Ident.Zone:
                    newObject = new Zone(id, position, fraction, 43);
                    break;
                case Ident.Spawnzone:
                    newObject = new SpawnZone(id, position, fraction, 40);
                    break;
				case Ident.HomeZone:
					newObject = new HomeZone(id, position, fraction, 135);
					break;
                default:
                    isMapObject = false;
                    break;
            }

		    if (isMapObject) 
		    {
			    if (newObject is Zone)
			    {
					mSelectableObjects.AddLast(newObject);
					mZones.AddLast((Zone)newObject);

					Simulator.Instance.addObstacle(newObject.Area.GetBordersAsVectorArray(-3));
			    }

		        mMapObjects.AddFirst(newObject);
				// TODO anpassen
				if (id == Ident.Castle)
				{
					//Simulator.Instance.addObstacle(newObject.Area.GetBordersAsVectorArray(-3));
					//Simulator.Instance.processObstacles();
				}
		    }
            // The new object is a unit
		    else
		    {
			    const int defaultRange = 18;
                switch (id)
                {
					//Atlantic
					case Ident.ShieldGuard:
						newObject = new ShieldGuard(position, fraction, 1, 400, 10, defaultRange, 30, 100);           
						break;
					case Ident.Priest:
						newObject = new Priest(Ident.Priest, position, fraction, 1, 100, 20, defaultRange, 50);           
						break;
					case Ident.PriestRanged:
						newObject = new Priest(Ident.PriestRanged, position, fraction, 1, 70, 10, 100, 40);
						break;
					case Ident.EliteAtlantic:
		                newObject = new AttackUnit(Ident.EliteAtlantic, position, fraction, 0.7f, 200, 0, 25, defaultRange, 60);
						break;
					case Ident.Witch:
						newObject = new Priest(Ident.Witch, position, fraction, 1, 120, 40, 120, 50);
						break;
					case Ident.Beast:
						newObject = new AttackUnit(Ident.Beast, position, fraction, 0.4f, 500, 500, 60, defaultRange, 20);
						break;
					

					//Barbarian
					case Ident.EliteBarbarian:
						newObject = new AttackUnit(Ident.EliteBarbarian, position, fraction, 0.7f, 300, 300, 40, defaultRange, 30);
						break;
					case Ident.Archer:
						newObject = new AttackUnit(Ident.Archer, position, fraction, 1, 100, 100, 10, 150, 40);
						break;
					case Ident.ArcherMounted:
						newObject = new MountedUnit(Ident.ArcherMounted, position, fraction, 1, 180, 180, 15, 120, 80);
						break;
					case Ident.Axeman:
						newObject = new AttackUnit(Ident.Axeman, position, fraction, 1, 130, 130, 20, defaultRange, 50);
						break;
					case Ident.Knight:
						newObject = new MountedUnit(Ident.Knight, position, fraction, 1, 200, 200, 25, defaultRange, 80);
						break;

					//techdemo units
					case Ident.TechdemoAtlantic:
						newObject = new Priest(Ident.TechdemoAtlantic, position, fraction, 1, 120, 40, 60, 50);
						break;
					case Ident.TechdemoBarb:
						newObject = new AttackUnit(Ident.TechdemoBarb, position, fraction, 0.7f, 200, 200, 40, defaultRange, 30);
						break;

                    default:
                        throw new ArgumentOutOfRangeException("id", id, null);
                }
				((Unit)newObject).mCollisionId = Simulator.Instance.addAgent(position);

			    if (newObject.Fraction == Fraction.Player)
			    {
				    mCountUnitsAtlantis++;
			    }
				else if (newObject.Fraction == Fraction.Ai)
			    {
					mCountUnitsBarbarian++;
			    }

                mUnits.AddFirst((Unit)newObject);
				mSelectableObjects.AddFirst((Unit)newObject);
		    }
			
			GameScreen.Map.Add(newObject);

			return newObject;
		}

		public void SendSelectedUnits(Vector2 destination)
		{
			var selectedObjects = mSelectionHandler.SelectedObjects;
			if (selectedObjects.Count == 0)
				return;
			if (selectedObjects.First.Value.Fraction == Fraction.Ai)
				return;

			if (selectedObjects.First.Value is Zone)
				((SpawnZone) selectedObjects.First.Value).UnitDestination = destination;
			else
				ApplyPulkWalk(selectedObjects, destination);
				
		}

		public void ApplyPulkWalk(LinkedList<WorldObject> selectedUnits, Vector2 destPosition)
		{
			Dictionary<uint, Unit> pulkWay = new Dictionary<uint, Unit>();

			// has to be done, because they don't check for Pulk, when moving
			foreach (var obj in selectedUnits)
			{
				Unit u = (Unit) obj;
				if (u.IsMoving)
					u.PulkUpdate();
			}

			foreach (var obj in selectedUnits)
			{
				Unit unit = (Unit)obj;
				unit.Stop();
				// no path for this pulk yet => create path
				if (pulkWay.ContainsKey(unit.PulkId) == false)
				{
					unit.WalkToPosition(destPosition, mSendCounter);
					pulkWay.Add(unit.PulkId, unit);
				}
				// Path already created, use this path + offset
				else
				{
					Unit mainSendUnit = pulkWay[unit.PulkId];
					//Vector2 offset = unit.Position - mainSendUnit.Position;

					LinkedList<Vector2> newPath = new LinkedList<Vector2>();

					foreach (var waypoint in mainSendUnit.CurrentPath)
					{
						Vector2 a = waypoint; // + offset;
						newPath.AddLast(a);
					}
					// Last Point is always the same (probably)
					//newPath.RemoveLast();
					//newPath.AddLast(mainSendUnit.CurrentPath.Last.Value);

					unit.WalkToPosition(newPath, mainSendUnit.CurrentSendId);
				}
			}
			mSendCounter++;
		}

		public void SpawnUnits(bool justFriends, int n, bool dense = false)
		{
			Random rand = new Random(Main.GameTime.ElapsedGameTime.Milliseconds);
			Rectangle rect = GameScreen.Map.QuadRect;
			for (int i = 0; i < n; i++)
			{
				while (true)
				{
					Vector2 pos = new Vector2(rand.Next(rect.X, rect.X + rect.Width - 1), rand.Next(rect.Y, rect.Y + rect.Height - 1));
					if (dense)
					{
						pos.X %= 300;
						pos.X += GameScreen.Map.QuadRect.Width / 2f;
						pos.Y %= 300;
						pos.Y += GameScreen.Map.QuadRect.Height / 2f;
					}
					var objs = GameScreen.Map.GetObjects(new Rectangle((int) pos.X, (int) pos.Y, 5, 5));
					if (objs.Count == 0)
					{
						CreateWorldObject(WorldObject.GetRandomUnit(justFriends, rand), pos, justFriends ? Fraction.Player : Fraction.Ai);
						break;
					}
				}
			}
			GameScreen.Hud.DeveloperInfo = "Unit Count: " + mUnits.Count;
		}

		private void UpdateDeadUnits()
		{
			var deadUnit = mDeadUnits.First;
			while (deadUnit != null)
			{
				var nextNode = deadUnit.Next;

				deadUnit.Value.Item1.UpdateAnimationModell();
				TimeSpan ts = Main.GameTime.TotalGameTime - deadUnit.Value.Item2;

				if (ts.TotalSeconds > 15)
					mDeadUnits.Remove(deadUnit);

				deadUnit = nextNode;
			}
		}

		public void Update()
		{
			if (!GameScreen.MapEditorMode)
			{
				mSelectionHandler.HandleSelection();
			}

			// because Update might delete a node in mUnits
			LinkedListNode<Unit> unit = mUnits.First;
			while (unit != null)
			{
				unit.Value.Update();
				unit = unit.Next;
			}

			UpdateDeadUnits();

			foreach (var zone in mZones)
			{
				zone.Update();
			}

			// has to happen Before updating all units
			CollisionHandler.Update(mUnits);

			if (!GameScreen.MapEditorMode && !GameScreen.AiDebug)
				GameScreen.Hud.DeveloperInfo = "Unit Count: " + mUnits.Count;
		}

		public void Draw()
		{
			var visibleObj = GameScreen.Map.GetObjects(GameScreen.Camera.ActualVisibleRectangle);

			foreach (var obj in visibleObj)
			{
				obj.Draw3DStuff();
			}
			// Dead Units
			foreach (var deadUnit in mDeadUnits)
			{
				deadUnit.Item1.Draw3DStuff();
			}

			if (!GameScreen.MapEditorMode)
			{
				mSelectionHandler.Draw();
			}

			// Need this because Waypoints should also be drawn if the Zone itself is not in the visible rectangle
			foreach (var obj in SelectionHandler.SelectedObjects)
			{
				if (obj is SpawnZone)
				{
					var zone = ((SpawnZone) obj);
					var dest = zone.UnitDestination;
					if (GameScreen.Camera.ActualVisibleRectangle.Contains(new Point((int)dest.X, (int)dest.Y)))
					{
						zone.DrawWaypoint();
					}
				}
			}

			// Draw Healthbars
			// Has to happen after 3D drawing
			foreach (var obj in visibleObj)
			{
				if (obj is Unit)
				{
					((Unit) obj).Draw2DStuff();
				}
				else if (obj is Zone)
				{
					((Zone)obj).DrawHealthbar();
				}
			}
		}

		public SelectionHandler SelectionHandler { get { return mSelectionHandler; } }
		public LinkedList<WorldObject> MapObjects { get { return mMapObjects; } }
		public LinkedList<Unit> Units { get { return mUnits; } }
		public LinkedList<Zone> Zones { get { return mZones; } }
		public LinkedList<WorldObject> SelectableObjects { get { return mSelectableObjects; } }
		public AStarPathfinder Pathfinder { get { return mPathfinder; } }
		public int CountsUnitBarbarian { get { return mCountUnitsBarbarian; } }
		public int CountsUnitAtlantis { get { return mCountUnitsAtlantis; } }
	}
}