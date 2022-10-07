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
	sealed class SearchState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private readonly LinkedList<Zone> mZones = new LinkedList<Zone>();
		private readonly Zone mTargetZone;
		public SearchState(AiSquad aiSquad)
		{
			mAiSquad = aiSquad;
			mTargetZone = GameScreen.AiAgent.HomeZone;
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects)
			{
				if (obj is Zone &&
				    obj.Fraction == Fraction.Ai)
				{
					mZones.AddFirst((Zone) obj);
				}
			}
			Random random = new Random();
			int x = random.Next(0, mZones.Count);
			mTargetZone = mZones.ElementAt(x);
			foreach (Unit squadMember in mAiSquad.SquadMembers)
			{
				squadMember.Stop();	
			}
			GameScreen.ObjectManager.AttackMove(mTargetZone.Position,
				new LinkedList<WorldObject>(mAiSquad.SquadMembers));
		}

		public override void Update()
		{
			float distanceToRetreatZone = 0;
			foreach (Unit squadMember in mAiSquad.SquadMembers)
			{
				if (!squadMember.IsDead && !squadMember.IsConverted &&
					!(squadMember.DestinationReached && Vector2.Distance(squadMember.Position, mTargetZone.Position) > squadMember.SightRange))
					//!squadMember.DestinationReached && !squadMember.Attacking)
				{
					distanceToRetreatZone = distanceToRetreatZone + Vector2.Distance(squadMember.Position, mTargetZone.Position);
				}
			}
			int distance = (mTargetZone is HomeZone) ? 500 : 250;
			if (mAiSquad.SquadMembers.Count == 0 || distanceToRetreatZone < mAiSquad.SquadMembers.Count * distance)
			{
				mAiSquad.States.Pop();
			}
		}
	}
}
