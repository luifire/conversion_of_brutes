/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Main Menu
 * Usage: This Class Implements the Main Menu for the game  
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;

namespace ConversionOfBrutes.Graphic.Screen
{
	internal sealed class MainMenuScreen : Screen
	{

		/// <summary>
		/// Basic constructor for the MainMenuScreen
		/// </summary>
		public MainMenuScreen()
		{
			Initialize();
		}

		public override void Initialize()
		{
			Main.Audio.PlaySound(AudioManager.Sound.MainMenuMusic);

			/*
			Main.Audio.PauseSound(AudioManager.Category.InGameMusic);
			if (!Main.Audio.IsSoundPlaying(AudioManager.Sound.MainMenuMusic))
			{
				Main.Audio.PlaySound(AudioManager.Sound.MainMenuMusic);
			}
			else
			{
				Main.Audio.ResumeSound(AudioManager.Category.MainMenuMusic);
			}
			*/
			//if (!Main.Audio.IsSoundPlaying(AudioManager.Sound.MainMenuMusic))
			//{
			//	Main.Audio.PlaySound(AudioManager.Sound.MainMenuMusic);
			//	//Main.Audio.SetVolume(AudioManager.Category.Music, 1f);
			//}

			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.Background, ScaledRectangle(0, 0, 1920, 1080), null);
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Play, ScaledRectangle(105, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(315, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Map, ScaledRectangle(525, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Statistics, ScaledRectangle(945, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.GameOptions, ScaledRectangle(735, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Achievements, ScaledRectangle(1155, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Credits, ScaledRectangle(1365, 940, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Quit, ScaledRectangle(1575, 940, 210, 100)));
			
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
							case MenuItem.MenuIdentifier.Play:
								mManager.AddScreen(new PreGameScreen());
								break;
							case MenuItem.MenuIdentifier.Load:
								mManager.AddScreen(new SaveLoadScreen(false, false, false));
								break;
							case MenuItem.MenuIdentifier.Map:
								SaveAndLoad.LoadMapEditor("2");
								break;
							case MenuItem.MenuIdentifier.Quit:
								mManager.AddScreen(new ConfirmationScreen(false, false, false));
								break;
							case MenuItem.MenuIdentifier.Statistics:
								mManager.AddScreen(new StatisticScreen());
								break;
							case MenuItem.MenuIdentifier.Achievements:
								mManager.AddScreen(new AchievementScreen());
								break;
							case MenuItem.MenuIdentifier.GameOptions:
								mManager.AddScreen(new OptionScreen());
								break;
							case MenuItem.MenuIdentifier.Credits:
								Main.Audio.PlaySound(AudioManager.Sound.CreditsMusic);
								Main.Audio.PauseSound(AudioManager.Sound.MainMenuMusic);
								mManager.AddScreen(new CreditsScreen());
								break;
						}
					}
		}
		
		public override void Draw()
		{
			DrawButtonsAndBackground();
		}
		
	}
}
