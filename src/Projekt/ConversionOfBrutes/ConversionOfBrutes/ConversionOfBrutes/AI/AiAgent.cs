/*
 * Author: Pius Meinert 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.AI.States;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace ConversionOfBrutes.AI
{   
	[Serializable]
	sealed class AiAgent
	{
		#region SaveClass
		[Serializable]
		public sealed class AiSave
		{
			public LinkedList<AiSquad> mSquads;
			public LinkedList<Zone> mTargetedZones;
			public int mSquadSize = 5;
			public int mMaxSquadCount = 8;
			public HomeZone mHomeZone;
			public LinkedList<Unit> mAllAiUnits = new LinkedList<Unit>();
			public HashSet<Unit> mUnitsInSquads = new HashSet<Unit>();
			public int mNewSquadTimer;
		}
		#endregion

		#region Private member variables
		private LinkedList<AiSquad> mSquads = new LinkedList<AiSquad>();
		private int mSquadSize;
		private int mMaxSquadCount;
		private  LinkedList<Zone> mTargetedZones = new LinkedList<Zone>(); 

		private HomeZone mHomeZone;
		private LinkedList<Unit> mAllAiUnits = new LinkedList<Unit>(); 
		private HashSet<Unit> mUnitsInSquads = new HashSet<Unit>(); 

		private int mNewSquadTimer;
		private int mZoneCheck;
		#endregion

		public AiAgent()
		{
			// Adjust values according to difficulty
			switch (GameScreen.GameDifficulty)
			{
				case GameDifficulty.Easy:
					mSquadSize = 11;
					mMaxSquadCount = 3;
					break;
				case GameDifficulty.Normal:
					mSquadSize = 6;
					mMaxSquadCount = 8;
					break;
				case GameDifficulty.Hard:
					mSquadSize = 3;
					mMaxSquadCount = 17;
					break;
			}

			// Get HomeZone
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects.Where(obj => obj is HomeZone && obj.Fraction == Fraction.Ai))
			{
				mHomeZone = (HomeZone) obj;
				break;
			}

			UpdateUnits();
			InitializeSquads();
		}

		#region Load/Save

		public void SetSaveInformation(AiSave save)
		{
			mSquads = save.mSquads;
			foreach (AiSquad squad in mSquads)
			{
				squad.SquadMembers = SaveAndLoad.GenerateNewList(save.mSquads.First(s => s == squad).SquadMembers);
			}
			mTargetedZones = save.mTargetedZones;
			mSquadSize = save.mSquadSize;
			mMaxSquadCount = save.mMaxSquadCount;
			mHomeZone = save.mHomeZone;
			mAllAiUnits = SaveAndLoad.GenerateNewList(save.mAllAiUnits);
			mUnitsInSquads = new HashSet<Unit>(SaveAndLoad.GenerateNewList(new LinkedList<Unit>(save.mUnitsInSquads.ToList())));
			mNewSquadTimer = save.mNewSquadTimer;
		}

		public AiSave GetSaveInformation()
		{
			return new AiSave()
			{
				mSquads = mSquads,
				mTargetedZones = mTargetedZones,
				mSquadSize = mSquadSize,
				mMaxSquadCount = mMaxSquadCount,
				mHomeZone = mHomeZone,
				mAllAiUnits = mAllAiUnits,
				mUnitsInSquads = mUnitsInSquads,
				mNewSquadTimer = mNewSquadTimer
			};
		}
#endregion

		public void Update(GameTime gameTime)
		{
			UpdateUnits();

			/* Most likely not needed anymore
			if (mAllAiUnits.Count == 0)
			{	
				GameScreen.ObjectManager.CreateWorldObject(Ident.EliteBarbarian, new Vector2(1800, 1330), Fraction.Ai);
				UpdateUnits();
				InitializeSquads();
			}
			*/

			// Distribute new units to the squads
			if (mSquads.Count > 0)
			{
				DistributeNewUnits();
			}
			
			// Adjust squadSize
			if (mSquads.All(squad => squad.SquadMembers.Count >= SquadSize - 1))
			{
				mSquadSize++;
			}

			bool checkZones = mZoneCheck++ > 50;
			if (checkZones)
			{
				mZoneCheck = 0;
				foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects.Where(obj => obj is Zone && !(obj is HomeZone) && obj.Fraction == Fraction.Ai))
				{
					if (!(((Zone) obj).FractionPoints > -490))
					{
						continue;
					}
					AiSquad cand = mSquads.First();
					float distance = float.MaxValue; //Vector2.Distance(obj.Position, mSquads.First().SquadMembers.First().Position);
					foreach (AiSquad s in mSquads.Where(s => s.States.Count > 0 &&
															 s.SquadMembers.Count > 0 &&
					                                         s.States.Peek() is ConquerState && !s.IsHelping))
					{
						float d = Vector2.Distance(obj.Position, s.SquadMembers.First().Position);
						if (d < distance)
						{
							cand = s;
						}
					}
					cand.States.Push(new DefendState(cand, (Zone)obj));
					break;
				}
			}

			// Update each squad
			foreach (AiSquad squad in mSquads)
			{
				// Help squad
				if (squad.States.Count > 0 && squad.States.Peek() is RetreatState)
				{
					var squad1 = squad;
					foreach (AiSquad s in mSquads.Where(s => s != squad1 && s.States.Count > 0 &&
						s.States.Peek() is ConquerState && !s.IsHelping))
					{
						s.IsHelping = true;
						s.States.Pop();
						mTargetedZones.Remove(s.TargetZone);
						s.States.Push(new ConquerState(s, squad.TargetZone));
						break;
					}
				}
				squad.Update(gameTime);
			}

			// Create new squad
			if (GameScreen.GameLogic.AtlantisPoints - 200 < GameScreen.GameLogic.BarbPoints && mNewSquadTimer++ > 400 &&
				GameScreen.GameLogic.BarbPoints < GameScreen.VictoryThreshold - 200 &&
				mSquads.Count < mMaxSquadCount && mAllAiUnits.Count > 0 &&
				mSquads.All(squad => squad.SquadMembers.Count >= mSquadSize - 1))
			{
				mNewSquadTimer = 0;
				Unit newSquadLeader = mAllAiUnits.First();
				foreach (AiSquad squad in mSquads.Reverse())
				{
					if (squad.SquadMembers.Count > 0)
					{
						newSquadLeader = squad.SquadMembers.Last();
						squad.SquadMembers.RemoveLast();
						break;
					}
				}
				mSquads.AddLast(new AiSquad(new LinkedList<Unit>(new[] {newSquadLeader})));
			}

			if (GameScreen.AiDebug)
			{
				String s = "\n";
				for (int i = 0; i < mSquads.Count; i++)
				{
					s = s + i + ": ";
					s = s + mSquads.ElementAt(i).SquadMembers.Count + " (" + mSquads.ElementAt(i).SpawnState.UnitQueue.Count +
						", " + mSquads.ElementAt(i).SpawnTimer + ") - ";
					s = mSquads.ElementAt(i).States.Aggregate(s, (current, state) => current + state.ToString().Remove(0, 29) + "\n");
					s = s + "\n";
					foreach (Unit squadMember in mSquads.ElementAt(i).SquadMembers)
					{
						squadMember.AiDevInfo = i.ToString();
					}
				}
				s = s + GameScreen.GameDifficulty + " - squadSize: " + mSquadSize + "\n";
				GameScreen.Hud.DeveloperInfo = s;
			}
		}

		private void InitializeSquads()
		{
			int i = 0;
			if (mAllAiUnits.Count > 0)
			{
				mSquads.AddLast(new AiSquad(new LinkedList<Unit>(new[] {mAllAiUnits.First()})));
				mUnitsInSquads.Add(mAllAiUnits.First());
				mAllAiUnits.RemoveFirst();
			}
			else
			{
				mSquads.AddLast(new AiSquad(new LinkedList<Unit>()));
			}
			foreach (Unit unit in mAllAiUnits)
			{
				if (mSquads.ElementAt(i).SquadMembers.Count < mSquadSize - 2)
				{
					mSquads.ElementAt(i).SquadMembers.AddLast(unit);
					mUnitsInSquads.Add(unit);
				}
				else
				{
					mSquads.AddLast(new AiSquad(new LinkedList<Unit>(new[] {unit})));
					mUnitsInSquads.Add(unit);
					i++;
				}
			}
			UpdateUnits();
		}

		private void DistributeNewUnits()
		{
			if (mSquads.Count > mSquadSize / 2 * 5)
			{
				mSquads.Clear();
				mUnitsInSquads.Clear();
				mSquadSize = 50;
				InitializeSquads();
				return;
			}
			// Essentially only for tech demo. Create new squad if all other are full
			if (mAllAiUnits.Count > 0 && mSquads.Count(squad => squad.SquadMembers.Count >= mSquadSize) >= mSquads.Count - 1)
			{
				Unit newSquadLeader = mAllAiUnits.First();
				foreach (AiSquad squad in mSquads.Reverse())
				{
					if (squad.SquadMembers.Count > 0)
					{
						newSquadLeader = squad.SquadMembers.Last();
						squad.SquadMembers.RemoveLast();
						break;
					}
				}
				mSquads.AddLast(new AiSquad(new LinkedList<Unit>(new[] {newSquadLeader})));
			}

			float distance = float.MaxValue;
			AiSquad cand = mSquads.First();
			foreach (Unit unit in mAllAiUnits)
			{
				if (!mUnitsInSquads.Contains(unit))
				{
					mUnitsInSquads.Add(unit);
					foreach (AiSquad squad in mSquads)
					{
						if (squad.States.Count > 0 && squad.States.Peek() is RetreatState && squad.SquadMembers.Count < SquadSize)
						{
							cand = squad;
							break;
						}
						if (squad.SquadMembers.Count == 0)
						{
							cand = squad;
							break;
							//squad.SquadMembers.AddLast(unit);
							//break;
						}
						if (squad.SquadMembers.Count < mSquadSize && Vector2.Distance(squad.SquadMembers.First().Position, unit.Position) < distance)
						{
							distance = Vector2.Distance(squad.SquadMembers.First().Position, unit.Position);
							cand = squad;
						}
					}
					cand.SquadMembers.AddLast(unit);
					unit.WalkToPosition(
						cand.SquadMembers.First().CurrentPath.Count > 0
							? cand.SquadMembers.First().CurrentPath.Last()
							: cand.SquadMembers.First().Position,
						cand.SquadMembers.First().CurrentSendId);
				}
			}
		}

		private void UpdateUnits()
		{
			mAllAiUnits.Clear();
			foreach (Unit unit in GameScreen.ObjectManager.Units.Where(unit => unit.Fraction == Fraction.Ai))
			{
				mAllAiUnits.AddLast(unit);
			}
		}

		#region Getter/Setter
		/// <summary>
		/// The HomeZome (SpawnZone).
		/// </summary>
		public HomeZone HomeZone
		{
			get { return mHomeZone; }
		}

		/// <summary>
		/// All Zones that are currently being conquered by a squad.
		/// </summary>
		public LinkedList<Zone> TargetedZones
		{
			get { return mTargetedZones; }
			set { mTargetedZones = value; }
		}

		/// <summary>
		/// Maximum squad size.
		/// </summary>
		public int SquadSize
		{
			get
			{
				return mSquadSize;
			}
		}
		#endregion
	}
}
