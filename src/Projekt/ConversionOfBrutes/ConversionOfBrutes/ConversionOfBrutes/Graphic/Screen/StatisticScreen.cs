/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Statistic Menu
 * Usage: This Class Implements the Statistic Menu for the game  
 * Missing: nothing
 * 
 **/

using System;
using System.Globalization;
using ConversionOfBrutes.Graphic.MenuElements;

namespace ConversionOfBrutes.Graphic.Screen
{   

	class StatisticScreen : Screen
	{
		/// <summary>
		/// Basic constructor for the StatisticScreen
		/// </summary>
		public StatisticScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override sealed void Initialize()
		{

			InitializeAbstr();
			TimeSpan duration = TimeSpan.FromSeconds(Main.mGameStatistic.DurationOfGame);
			String gameTime = String.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", duration.Hours, duration.Minutes, duration.Seconds);
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.StatisticScreen, ScaledRectangle(0, 0, 1920, 1080), null);

		    mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(855, 910, 210, 100)));
	
			mLabels.Add(AddLabel(770, 200, "Statistic of the last game: "));
			mLabels.Add(AddLabel(770, 240, "The recruited Units: " + Main.mGameStatistic.RecruitedUnits));
			mLabels.Add(AddLabel(770, 280, "The converted Units: " + Main.mGameStatistic.ConvertedUnits));
			mLabels.Add(AddLabel(770, 320, "The killed Units: " + Main.mGameStatistic.KilledUnits));
			mLabels.Add(AddLabel(770, 360, "The lost Units: " + Main.mGameStatistic.LostUnits));
			mLabels.Add(AddLabel(770, 400, "The overall received Winpoints: " + (int)Main.mGameStatistic.OverallReceivedWinpoints));
			mLabels.Add(AddLabel(770, 440, "The overall lost Winpoints: " + (Main.mGameStatistic.OverallLostWinpoints * -1)));
			mLabels.Add(AddLabel(770, 480, "The occupied Areas: " + Main.mGameStatistic.OccupiedAreas));
			mLabels.Add(AddLabel(770, 520, "The lost Areas:  " + (Main.mGameStatistic.LostAreas * -1)));
			mLabels.Add(AddLabel(770, 560, "The total GameTime: " + gameTime));
			mLabels.Add(AddLabel(770, 640, "Statistic of all games: "));
			mLabels.Add(AddLabel(770, 680, "Games started: " + Main.mAllOverGameStatistic.GamesStarted));
			mLabels.Add(AddLabel(770, 720, "Games won: " + Main.mAllOverGameStatistic.GamesWon));
			mLabels.Add(AddLabel(770, 760, "Games lost: " + Main.mAllOverGameStatistic.GamesLost));
			mLabels.Add(AddLabel(770, 800, "Games not finished: " + Main.mAllOverGameStatistic.GamesNotFinished));
			
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
		}
   }
}

