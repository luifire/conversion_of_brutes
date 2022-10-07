using System;
using ConversionOfBrutes.Animation;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{
	[Serializable]
	public class Horse
	{
		//Timer 
		private TimeSpan mUpdateIntervall = TimeSpan.FromMilliseconds(50); //wouldn't go higher than this.
		private TimeSpan mLastupdate;

		private AnimationManager.AnimationModels mCurrentAnimation;
		//Animation
		[NonSerialized]
		public AnimatedModel mAnimatedModel;
		public Horse()
		{
			// get Animation model
			mAnimatedModel = new AnimatedModel(GameScreen.GraphicsManager.GetAsset(Ident.Horse));
			mAnimatedModel.LoadContent(Main.Content);

			// Play idle animation
			mCurrentAnimation = AnimationManager.AnimationModels.Idle;
			GameScreen.GraphicsManager.mAnimationManager.PlayHorseAnimation(this, mCurrentAnimation);

		}
		public void StartAnimation(AnimationManager.AnimationModels animation)
		{
			if (animation != mCurrentAnimation)
			{
				GameScreen.GraphicsManager.mAnimationManager.PlayHorseAnimation(this, animation);
				mCurrentAnimation = animation;
			}
		}

		public void Draw(Vector2 position,float rotation)
		{
			GameScreen.GraphicsManager.DrawUnit(mAnimatedModel,Ident.Horse, position,rotation);
		}

		public void Update()
		{
			if (CanUpdateAnimation)
			{
				mLastupdate = Main.GameTime.TotalGameTime;
				mAnimatedModel.Update(Main.GameTime);
			}
		}
		//timer as we don't wanna update horse animations all the time
		private bool CanUpdateAnimation
		{
			get { return mLastupdate + mUpdateIntervall < Main.GameTime.TotalGameTime; }
		}
	}
}
