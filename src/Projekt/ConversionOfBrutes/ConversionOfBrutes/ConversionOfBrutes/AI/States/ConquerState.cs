/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.States
{
	[Serializable]
	sealed class ConquerState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private readonly SpawnZone mHomeZone = GameScreen.AiAgent.HomeZone;
		private Zone mTargetZone;
		private readonly LinkedList<Zone> mZones = new LinkedList<Zone>();

		// Wait before targeting new zone
		private int mWaitTimer;

		public ConquerState(AiSquad aiSquad, Zone targetZone)
		{
			mAiSquad = aiSquad;
			mTargetZone = targetZone;
			UpdateZones();
			if (mZones.Count - GameScreen.AiAgent.TargetedZones.Count <= 1 && mAiSquad.States.Count > 0)
			{
				mAiSquad.States.Pop();
			}
			else if (mZones.Count - GameScreen.AiAgent.TargetedZones.Count > 1 && mAiSquad.SquadMembers.Count > 0)
			{
				if (targetZone == GameScreen.AiAgent.HomeZone)
				{
					mTargetZone = GetZone(mAiSquad.SquadMembers.First().Position);
				}
				ConquerZone(mAiSquad.SquadMembers, mTargetZone);
			}
			mAiSquad.TargetZone = mTargetZone;
		}

		public override void Update()
		{
			UpdateZones();
			if (mZones.Count <= 0 && mAiSquad.States.Count > 0)
			{
				mAiSquad.States.Pop();
			}
			else if (mTargetZone is HomeZone && mZones.Count <= 0)
			{
				return;
			}
			else
			{
				if (mAiSquad.SquadMembers.Count(squadMember => squadMember.DestinationReached &&
					Vector2.Distance(squadMember.Position, mTargetZone.Position) > squadMember.SightRange) >
					mAiSquad.SquadMembers.Count/2)
				{
					ConquerZone(mAiSquad.SquadMembers, mTargetZone);
				}
				if (mTargetZone.Fraction == Fraction.Ai && mAiSquad.SquadMembers.Count > 0 && mZones.Count > 0)
				{
					mWaitTimer++;
				}
				// wait fixed time (based on difficulty) before conquering new zone
				if (mWaitTimer > 350 * (2 - (int)GameScreen.GameDifficulty))
				{
					mWaitTimer = 0;
					mAiSquad.IsHelping = false;
					GameScreen.AiAgent.TargetedZones.Remove(mTargetZone);
					mTargetZone = GetZone(mAiSquad.SquadMembers.First().Position);
					GameScreen.AiAgent.TargetedZones.AddLast(mTargetZone);
					ConquerZone(mAiSquad.SquadMembers, mTargetZone);
				}
			}
			base.Update();
		}

		private void UpdateZones()
		{
			mZones.Clear();
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects.Where(obj => obj is Zone &&
			                                                                             obj.Fraction != Fraction.Ai && !(obj is HomeZone)))
			{
				mZones.AddFirst((Zone) obj);
			}
		}

		private void ConquerZone(LinkedList<Unit> group, WorldObject targetZone)
		{
			Random random = new Random();
			int x = random.Next(0, 99);
			if (x < (3 - (int)GameScreen.GameDifficulty) * 25)
			{
				GameScreen.ObjectManager.ApplyPulkWalk(new LinkedList<WorldObject>(@group), targetZone.Position);
			}
			else
			{
				GameScreen.ObjectManager.AttackMove(targetZone.Position, new LinkedList<WorldObject>(@group));
			}
		}

		private Zone GetZone(Vector2 position)
		{
			Zone cand = mZones.First();
			Random random = new Random();
			int x = random.Next(0, 99);
			if (x < 75)
			{
				float distance = Vector2.Distance(position, mZones.First().Position) +
				                 Vector2.Distance(mHomeZone.Position, mZones.First().Position);
				foreach (Zone zone in mZones)
				{
					float d2 = Vector2.Distance(position, zone.Position) + Vector2.Distance(mHomeZone.Position, zone.Position);
					if (d2 < distance && !GameScreen.AiAgent.TargetedZones.Contains(zone)) // && !mTargetZones.Contains(zone))
					{
						distance = d2;
						cand = zone;
					}
				}
			}
			else if (GameScreen.AiAgent.TargetedZones.Count < mZones.Count)
			{
				for (int i = 0; i < mZones.Count; i++)
				{
					cand = mZones.ElementAt(i);
					if (!GameScreen.AiAgent.TargetedZones.Contains(cand))
					{
						break;
					}
				}
			}
			GameScreen.AiAgent.TargetedZones.AddLast(cand);
			GameScreen.AiAgent.TargetedZones = new LinkedList<Zone>(GameScreen.AiAgent.TargetedZones.Distinct());
			return cand;
		}
	}
}
