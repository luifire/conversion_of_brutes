/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Confirmation Screen
 * Usage: Use this class to confirm an action for leaving the game or went to the MainMenu
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;

namespace ConversionOfBrutes.Graphic.Screen
{
	class ConfirmationScreen : Screen
	{

		private bool mInGame;
		private bool mIsVictoryDefeat;
		private bool mWin;
		public ConfirmationScreen(bool inGame, bool isVictoryDefeat, bool win)

		{
			Initialize();
			mDrawScreensBelow = true;
			mInGame = inGame;
			mIsVictoryDefeat = isVictoryDefeat;
			mWin = win;
		}
		
		
		
		public override sealed void Initialize()
		{
			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.BlankoScreen, ScaledRectangle(720, 370, 500, 200), null);
			mLabels.Add(AddLabel(860, 390, "Leave the Game?"));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Yes, ScaledRectangle(820, 430, 150, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.No, ScaledRectangle(970, 430, 150, 100)));

		}

		protected override void HandleSoundOnExit()
		{
			if (!mInGame)
			{
				Main.Audio.StopSound(AudioManager.Sound.MainMenuMusic);
			}
			else
			{
				Main.Audio.StopSound(AudioManager.Sound.PauseMusic);
				Main.Audio.StopSound(AudioManager.Sound.InGameMusic);
				Main.Audio.PlaySound(AudioManager.Sound.MainMenuMusic);
			}
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
					case MenuItem.MenuIdentifier.Yes:
						HandleSoundOnExit();
						if (!mInGame)
						{
							SaveAndLoad.SaveOverallGameStatistic(Main.mAllOverGameStatistic);
							SaveAndLoad.SaveAchievements(Main.mAchievements);
							Main.MainExit();
						}

						if (mInGame)
						{
							if (GameScreen.GameNotFinished)
							{
								Main.mAllOverGameStatistic.GamesNotFinished++;
							}
							
							if (!GameScreen.MapEditorMode)
							{
								Main.mGameStatistic.DurationOfGame += GameScreen.Stopwatch.Elapsed.TotalSeconds;
								SaveAndLoad.SaveGameStatistic(Main.mGameStatistic);
								SaveAndLoad.SaveOverallGameStatistic(Main.mAllOverGameStatistic);
								SaveAndLoad.SaveAchievements(Main.mAchievements);
								
							}
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							mManager.AddScreen(new MainMenuScreen());
						} 


						break;
					case MenuItem.MenuIdentifier.No:
						if (!mIsVictoryDefeat && !mInGame)
						{
							mManager.RemoveScreen();
						}

						if (!mIsVictoryDefeat && mInGame)
						{
							mManager.RemoveScreen();
							mManager.AddScreen(new InGameMenuScreen());
						}

						if (mIsVictoryDefeat && mInGame)
						{
							mManager.RemoveScreen();
							mManager.AddScreen(new DefeatAndVictoryScreen(mWin));
						}
							
						break;
				}
			}
		}

		public override void Draw()
		{
			DrawButtonsAndBackground();
			DrawListOfLabels(mLabels);
			if (mInGame)
			{
				Handling2DAnd3D();
			}
		}
	}
}
