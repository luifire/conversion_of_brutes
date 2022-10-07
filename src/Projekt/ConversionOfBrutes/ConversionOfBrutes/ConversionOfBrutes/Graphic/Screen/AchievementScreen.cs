/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Achievement Menu
 * Usage: This Class Implements the Achievement Menu for the game  
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;

namespace ConversionOfBrutes.Graphic.Screen
{
	sealed class AchievementScreen : Screen
	{

		private Bar mMBarRampage;
		private Bar mMBarFasterLight;
		private Bar mMBarWololo;
		private Bar mMBarUnstobbable;
		private Bar mMBarPause15Times;


		/// <summary>
		/// Basic constructor for the AchievementScreen
		/// </summary>
		public AchievementScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override void Initialize()
		{

			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.AchievementScreen, ScaledRectangle(0, 0, 1920, 1080), null);
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(855, 910, 210, 100)));
			mLabels.Add(AddLabel(700, 200, "Rampage!: Kill 10 enemies with an elite warrior"));
			mLabels.Add(AddLabel(700, 320, "Faster than light: Win a game less than 10 minutes"));  
			mLabels.Add(AddLabel(700, 470, "Wololo: Convert 1000 enemies")); 
			mLabels.Add(AddLabel(700, 620, "Unstoppable: Kill 1000 enemies")); 
			mLabels.Add(AddLabel(700, 770, "Pause a game at least 15 times with your Mouse")); 
			
			mMBarRampage = new Bar(mSpriteBatch, ScaledRectangle(700, 240, 500, 50));
			mMBarFasterLight = new Bar(mSpriteBatch, ScaledRectangle(700, 370, 500, 50));
			mMBarWololo = new Bar(mSpriteBatch, ScaledRectangle(700, 520, 500, 50));
			mMBarUnstobbable = new Bar(mSpriteBatch, ScaledRectangle(700, 670, 500, 50));
			mMBarPause15Times = new Bar(mSpriteBatch, ScaledRectangle(700, 820, 500, 50));
		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();
			
			MenuButton button = MouseClickedGetButton();
			
			if (button != null)
			{
				mManager.RemoveScreen();
			}		
		}


		public override void Draw()
		{

			DrawButtonsAndBackground();
			DrawListOfLabels(mLabels);

			mMBarRampage.Draw(Main.mAchievements.Rampage, Main.AxRampage);

			if (Main.mAchievements.FasterLight <= Main.AxFasterLight && Main.mAchievements.FasterLight != 0)
			{
				mMBarFasterLight.Draw(1f, 1f);
			}
			else
			{
				mMBarFasterLight.Draw(0f, 1f);
			}

			mMBarWololo.Draw(Main.mAchievements.OverAllConverted, Main.AxWololo);
			mMBarUnstobbable.Draw(Main.mAchievements.OverAllKilled, Main.AxUnstobbable);
			mMBarPause15Times.Draw(Main.mAchievements.PauseOver15Times, Main.Pause15Times);

		}
   }
}


