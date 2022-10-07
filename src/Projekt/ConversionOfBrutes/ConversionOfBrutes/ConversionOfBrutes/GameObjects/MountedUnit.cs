using System;
using ConversionOfBrutes.Animation;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{
	[Serializable]
	class MountedUnit : AttackUnit
	{		
		private Horse mHorse;

		public MountedUnit(Ident id, Vector2 position, Fraction fraction, float attackSpeed, int healthPoints, int faithPoints, int attackDamage, int attackRange, int speed) :
			base(id, position, fraction, attackSpeed, healthPoints,faithPoints, attackDamage, attackRange, speed)
		{
			mHorse = new Horse();
		}

		public override void Update()
		{
			base.Update();
			mHorse.StartAnimation(IsMoving ? AnimationManager.AnimationModels.Walking : AnimationManager.AnimationModels.Idle);
			if(Visible) mHorse.Update();
		}

		public override void Draw3DStuff()
		{
			base.Draw3DStuff();
			if(Visible)mHorse.Draw(Position,ModelRotation);
		}
	}
}
