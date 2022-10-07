/**
 * Author: Julian Löffler
 * 
 * Attack Units
 * Missing: 
 * - 
 **/

using System;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{   
	[Serializable]
	public class AttackUnit : Unit
	{
		//private int mFaithPoints;
		private double mRegenerationTimer;

		[NonSerialized]
		private Healthbar mFaithbar;

		public AttackUnit(Ident id, Vector2 position, Fraction fraction, float attackSpeed, int healthPoints, int faithPoints, int attackDamage, int attackRange, int speed) :
			base(id, position, fraction, attackSpeed, healthPoints, attackDamage, attackRange, speed, id == Ident.ArcherMounted ? 8f : 5.5f)
		{
			mFaithPoints = faithPoints;
			mMaxFaithPoints = faithPoints;
			mFaithbar = new Healthbar(this, new Vector2(60,10), true);
			mRegenerationTimer = 0;
		}
		

		public override void Update()
		{
			base.Update();
			mFaithbar.Update();
		}

		public override void Draw2DStuff()
		{
			if (Visible)
			{
				base.Draw2DStuff();
				// Faithbar
				if (Fraction == Fraction.Ai)
					GameScreen.GraphicsManager.DrawHealthBar(mFaithbar);
			}
		}

		/// <summary>
		/// Regenerates Faithpoints
		/// </summary>
		public void Regenerate()
		{
			mRegenerationTimer += Main.GameTime.ElapsedGameTime.Milliseconds;
			if (mRegenerationTimer >= 1000)
			{
				mRegenerationTimer = 0;
				mFaithPoints += 5;
				if (mFaithPoints > mMaxFaithPoints)
					mFaithPoints = mMaxFaithPoints;
			}
		}
		
	}


}


