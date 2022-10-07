/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.AI.States;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace ConversionOfBrutes.AI
{
	[Serializable]
	sealed class AiSquad
	{
		#region Private member variables
		private readonly Stack<BaseState> mStates = new Stack<BaseState>();
		private readonly SpawnState mSpawnState;
		private int mSpawnTimer;
		private Zone mTargetZone;

		private LinkedList<Unit> mSquadMembers;
#endregion

		public AiSquad(LinkedList<Unit> squadMembers)
		{
			IsHelping = false;

			mSquadMembers = squadMembers;

			//mStates.Push(new ConquerState(this, mAiAgent.HomeZone));
			mSpawnState = new SpawnState(this);
			States.Push(mSpawnState);
		}

		public void Update(GameTime gameTime)
		{
			UpdateSquadMembers();

			// Update SpawnTimer for SpawnState
			if (mSpawnState.UnitQueue.Count != 0)
			{
				mSpawnTimer -= Main.GameTime.ElapsedGameTime.Milliseconds;
				if (mSquadMembers.Count >= GameScreen.AiAgent.SquadSize)
				{
					mSpawnTimer = 0;
				}
			}

			// Search for survivors
			if (States.Count == 0)
			{
				States.Push(new SearchState(this));
			}

			// Stock up to full squad size 
			if (mSpawnTimer <= 0 && mSquadMembers.Count < GameScreen.AiAgent.SquadSize && States.Peek() is ConquerState)
			{
				States.Push(mSpawnState);
			}

			//Console.WriteLine(mStates.Peek());
			// React if one member has been attacked
			if (States.Peek() is ConquerState)
			{
				foreach (Unit squadMember in mSquadMembers)
				{
					if (gameTime.ElapsedGameTime.Milliseconds - squadMember.TimeOfLastAttack < 100 &&
					    squadMember.LastAttacker != null && !squadMember.LastAttacker.IsDead)
					{
						// Check how many friendly/enemy units are in the area
						List<WorldObject> surrroundingWorldObjects =
							GameScreen.Map.GetObjects(new Rectangle((int) (squadMember.Position.X - squadMember.SightRange),
								(int) (squadMember.Position.Y - squadMember.SightRange),
								2 * squadMember.SightRange,
								2 * squadMember.SightRange));
						List<Unit> surrroundingPlayerUnits = new List<Unit>();
						List<Unit> surrroundingAiUnits = new List<Unit>();
						foreach (WorldObject obj in surrroundingWorldObjects)
						{
							if (obj is Unit)
							{
								if (obj.Fraction == Fraction.Ai)
								{
									surrroundingAiUnits.Add((Unit) obj);
								}
								else if (obj.Fraction == Fraction.Player)
								{
									surrroundingPlayerUnits.Add((Unit) obj);
								}
							}
						}
						// Retreat if outnumbered
						if (surrroundingAiUnits.Count + 5 < surrroundingPlayerUnits.Count &&
							GameScreen.GameDifficulty != GameDifficulty.Easy)
						{
							if (States.Peek() is ConquerState)
							{
								States.Push(new RetreatState(this, squadMember));
							}
						}
						// Attack/defend if in majority
						else
						{
							if (States.Peek() is ConquerState)
							{
								States.Push(new AttackState(this, squadMember.LastAttacker));
							}
						}
					}
				}
			}

			// Abort searching, a zone has been captured by the enemy 
			if (States.Peek() is SearchState && 
				GameScreen.ObjectManager.MapObjects.Any(obj => obj is Zone && !(obj is HomeZone) && obj.Fraction != Fraction.Ai))
			{
				States.Pop();
				States.Push(new ConquerState(this, GameScreen.AiAgent.HomeZone));
			}

			States.Peek().Update();
		}

		private void UpdateSquadMembers()
		{
			LinkedList<Unit> squadMembersToBeRemoved = new LinkedList<Unit>();
			foreach (Unit squadMember in mSquadMembers)
			{
				if (squadMember.IsDead || squadMember.IsConverted)
				{
					// To be deleted
					squadMember.AiDevInfo = "";

					squadMembersToBeRemoved.AddLast(squadMember);
				}
			}
			foreach (Unit deadSquadMember in squadMembersToBeRemoved)
			{
				mSquadMembers.Remove(deadSquadMember);
			}
		}

		#region Getter/Setter
		/// <summary>
		/// Helping other squad, remove when help state implemented.
		/// </summary>
		public bool IsHelping { get; set; }

		public Stack<BaseState> States
		{
			get { return mStates; }
		} 

		/// <summary>
		/// All Units in this squad.
		/// </summary>
		public LinkedList<Unit> SquadMembers
		{
			get { return mSquadMembers; }
			set { mSquadMembers = value; }
		}

		/// <summary>
		/// Zone this squad wants to conquer.
		/// </summary>
		public Zone TargetZone
		{
			get { return mTargetZone; }
			set { mTargetZone = value; }
		}

		public SpawnState SpawnState { get { return mSpawnState; } }

		public int SpawnTimer
		{
			get { return mSpawnTimer; }
			set { mSpawnTimer = value; }
		}

		#endregion
	}
}
