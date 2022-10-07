/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.States
{
	[Serializable]
	sealed class SpawnState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private readonly LinkedList<SpawnZone> mSpawnZones = new LinkedList<SpawnZone>();
		private readonly Stack<Ident> mUnitQueue = new Stack<Ident>();

		public SpawnState(AiSquad aiSquad)
		{
			mAiSquad = aiSquad;
			UpdateSpawnZones();
		}

		public override void Update()
		{
			UpdateSpawnZones();
			if (mAiSquad.SpawnTimer <= 0)
			{
				if (mUnitQueue.Count != 0)
				{
					mUnitQueue.Pop();
					if (mUnitQueue.Count != 0)
					{
						// Overestimate the time to prevent production of too many units.
						mAiSquad.SpawnTimer = (GameScreen.GameLogic.GetTimeCost(mUnitQueue.Peek()) + 4 + 
							(2 - (int)GameScreen.GameDifficulty)) * 1000;
					}
				}
			}
			if (GameScreen.GameLogic.BarbPoints > 200 && mAiSquad.SquadMembers.Count + mUnitQueue.Count < GameScreen.AiAgent.SquadSize &&
				GameScreen.GameLogic.BarbPoints < GameScreen.VictoryThreshold - 200 &&
				GameScreen.GameLogic.BarbContingent > 0)
			{
				SpawnZone spawnZone = GetBestSpawnZone(new Vector2(0, 0));
				if (mAiSquad.SquadMembers.Count > 0)
				{
					spawnZone = GetBestSpawnZone(mAiSquad.SquadMembers.First().Position);
				}
				Ident unitToSpawn = Ident.EliteBarbarian;
				Random random = new Random();
				int x = random.Next(0, 99);
				if (x < 10) // 10%
				{
					unitToSpawn = Ident.EliteBarbarian;
				}
				else if (x < 37) // 27%
				{
					unitToSpawn = Ident.Archer;
				}
				else if (x < 58) // 21%
				{
					unitToSpawn = Ident.ArcherMounted;
				}
				else if (x < 83) // 25%
				{
					unitToSpawn = Ident.Axeman;
				}
				else if (x < 99) // 17%
				{
					unitToSpawn = Ident.Knight;
				}
				// Adjust timer
				if (mUnitQueue.Count == 0)
				{
					mAiSquad.SpawnTimer = (GameScreen.GameLogic.GetTimeCost(unitToSpawn) + 1) * 1000;
				}
				mUnitQueue.Push(unitToSpawn);
				GameScreen.GameLogic.SpawnUnit(unitToSpawn, spawnZone, Fraction.Ai);
				// For debug purposes
				if (GameScreen.AiDebug)
				{
					GameScreen.Hud.UpdateSpawnQueue(spawnZone);
				}
			}
			else
			{
				mAiSquad.States.Pop();
				if (mAiSquad.States.Count == 0)
				{
					mAiSquad.States.Push(new ConquerState(mAiSquad, GameScreen.AiAgent.HomeZone));
				}
			}
			base.Update();
		}

		private void UpdateSpawnZones()
		{
			mSpawnZones.Clear();
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects)
			{
				if (obj is SpawnZone && obj.Fraction == Fraction.Ai)
				{
					mSpawnZones.AddLast((SpawnZone) obj);
				}
			}
		}

		private SpawnZone GetBestSpawnZone(Vector2 position)
		{
			SpawnZone cand = mSpawnZones.First();
			float distance = float.MaxValue;
			foreach (SpawnZone spawnZone in mSpawnZones)
			{
				float spawnZoneDistance = Vector2.Distance(position, spawnZone.Position);
				float spawnZoneLoad = spawnZone.SpawnJobs.Aggregate<SpawnJob, float>(0, (current, s) => (float) (current + s.TimeRemaining));
				if (spawnZoneDistance + spawnZoneLoad/10 < distance)
				{
					distance = spawnZoneDistance + spawnZoneLoad/10; 
					cand = spawnZone;
				}
			}
			return cand;
		}

		public Stack<Ident> UnitQueue { get { return mUnitQueue; } } 
	}
}
