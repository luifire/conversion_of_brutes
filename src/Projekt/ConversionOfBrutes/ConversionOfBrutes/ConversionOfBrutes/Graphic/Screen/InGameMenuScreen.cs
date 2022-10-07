/**
 * Author: David Spisla 
 * 
 * Concrete Class for the InGame  Menu
 * Usage: This Class Implements the InGame Menu for the game  
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Sound;

namespace ConversionOfBrutes.Graphic.Screen
{
	class InGameMenuScreen : Screen
	{

		
		/// <summary>
		/// Basic constructor for the InGameMenuScreen
		/// </summary>
		public InGameMenuScreen()
		{
			mDrawScreensBelow = true;
			mUpdateScreensBelow = false;
			Initialize();
			mIsEscClosable = true;
		}

		public override sealed void Initialize()
		{
			
			InitializeAbstr();
			GameScreen.Stopwatch.Stop();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameMenuScreen, ScaledRectangle(720, 170, 508, 563 + 100), null);
			
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MainMenu, ScaledRectangle(870, 690, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Resume, ScaledRectangle(870, 290, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Save, ScaledRectangle(870, 390, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(870, 490, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.InGameOptions, ScaledRectangle(870, 590, 210, 100)));

			Main.Audio.PauseSound(AudioManager.Sound.InGameMusic);
			if (!Main.Audio.ResumeSound(AudioManager.Sound.PauseMusic))
			{
				Main.Audio.PlaySound(AudioManager.Sound.PauseMusic);
			}
			/*
			if (!Main.Audio.IsSoundPlaying(AudioManager.Sound.PauseMusic))
			{
				Main.Audio.PlaySound(AudioManager.Sound.PauseMusic);
			}
			else
			{
				Main.Audio.ResumeSound(AudioManager.Category.PauseMusic);
			}
			*/
		}

		protected override void HandleSoundOnExit()
		{
			Main.Audio.PauseSound(AudioManager.Sound.PauseMusic);
			if (!Main.Audio.ResumeSound(AudioManager.Sound.InGameMusic))
			{
				Main.Audio.PlaySound(AudioManager.Sound.InGameMusic);
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
							case MenuItem.MenuIdentifier.MainMenu:
								mManager.RemoveScreen();
								mManager.AddScreen(new ConfirmationScreen(true, false, false));
							
								if (GameScreen.MapEditorMode)
							       Main.HotKey.InitHotkeysForGame();
								break;
							case MenuItem.MenuIdentifier.Resume:
								HandleSoundOnExit();
								mManager.RemoveScreen();
								
								GameScreen.Stopwatch.Start();
								break;
							case MenuItem.MenuIdentifier.Save:
								if (!GameScreen.MapEditorMode)
								{
									mManager.AddScreen(new SaveLoadScreen(true, true, false));
								}
								else if(GameScreen.MapEditorMode)
								{
									mManager.AddScreen(new SaveLoadScreen(true, true, true));
								}
								
								
								break;
							case MenuItem.MenuIdentifier.Load:
								if (!GameScreen.MapEditorMode)
								{
									mManager.AddScreen(new SaveLoadScreen(true, false, false));
								}
								else if (GameScreen.MapEditorMode)
								{
									mManager.AddScreen(new SaveLoadScreen(true, false, true));
								}
								
								break;
							case MenuItem.MenuIdentifier.InGameOptions:
								//mManager.RemoveScreen();
								mManager.AddScreen(new InGameOptionsScreen());
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
