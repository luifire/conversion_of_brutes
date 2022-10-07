/**
 * Author: David Spisla 
 * 
 * Concrete Class for the VictoryScreen 
 * Usage: This Class Implements the VictoryScreen for the game  
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;

namespace ConversionOfBrutes.Graphic.Screen
{
	sealed class DefeatAndVictoryScreen : Screen
	{

		private readonly bool mMIsVictory;

		/// <summary>
		/// Basic constructor for the VictoryScreen
		/// </summary>
		public DefeatAndVictoryScreen(bool isVictory)
		{
			mDrawScreensBelow = true;
			mMIsVictory = isVictory;
			Initialize();
			
		}


		public override void Initialize()
		{
			
			InitializeAbstr();
			GameScreen.Stopwatch.Stop();

			if (!GameScreen.MapEditorMode)
			   Main.mGameStatistic.DurationOfGame += GameScreen.Stopwatch.Elapsed.TotalSeconds;

			if (mMIsVictory && Main.mGameStatistic.DurationOfGame <= Main.AxFasterLight && !GameScreen.MapEditorMode)
			{
				Main.mAchievements.FasterLight = (int)Main.mGameStatistic.DurationOfGame;
			}
			
			mBackground = mMIsVictory ? new MenuLabel(MenuItem.MenuIdentifier.VictoryScreen, ScaledRectangle(1920 / 2 - (1920 / 8), 1080 / 2 - (1080 / 4), 508, 563), null)
				: new MenuLabel(MenuItem.MenuIdentifier.DefeatScreen, ScaledRectangle(1920 / 2 - (1920 / 8), 1080 / 2 - (1080 / 4), 508, 563), null);
			
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.NewGame, ScaledRectangle(870, 370, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Statistics, ScaledRectangle(870, 480, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(870, 590, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MainMenu, ScaledRectangle(870, 700, 210, 100)));

		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();
			MenuButton button = MouseClickedGetButton();
			if (button != null)
			{

						switch (button.Identifier)
						{
							case MenuItem.MenuIdentifier.NewGame:
								if (!GameScreen.MapEditorMode)
								   SaveAndLoad.SaveOverallGameStatistic(Main.mAllOverGameStatistic);
								mManager.RemoveScreen();
								mManager.RemoveScreen();
								mManager.RemoveScreen();
								SaveAndLoad.LoadNewGame("1");
								break;
							case MenuItem.MenuIdentifier.Statistics:
								SaveAndLoad.SaveGameStatistic(Main.mGameStatistic);
								mManager.AddScreen(new StatisticScreen());
								break;
							case MenuItem.MenuIdentifier.Load:
								mManager.AddScreen(new SaveLoadScreen(true, false, false));
								break;
							case MenuItem.MenuIdentifier.MainMenu:
								mManager.RemoveScreen();
								mManager.AddScreen(new ConfirmationScreen(true, true, mMIsVictory));
								break;
							
						}
					}
		}

		public override void Draw()
		{
			
			DrawButtonsAndBackground();
			Handling2DAnd3D();
			
		}
	}
}

	

