/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Hotkey Menu
 * Usage: This Class Implements the Hotkey Menu for the game  
 * Missing: nothing 
 * 
 **/

using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.Graphic.Screen
{
	class HotkeyScreen : Screen
	{
		private List<PseudoTextBox> mMBox;
		private bool mMIsEditing;

		/// <summary>
		/// Basic constructor for the HotkeyScreen
		/// </summary>
		public HotkeyScreen()
		{
			Initialize();
		}


		public override sealed void Initialize()
		{

			InitializeAbstr();
			mMBox = new List<PseudoTextBox>();
		    mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(855, 910, 210, 100)));

			mBackground = new MenuLabel(MenuItem.MenuIdentifier.HotkeyScreen, ScaledRectangle(0, 0, 1920, 1080), null);
			mLabels.Add(AddLabel(290, 170, "Camera move to object:"));
			mLabels.Add(AddLabel(290, 220, "Camera up:"));
			mLabels.Add(AddLabel(290, 270, "Camera left:"));
			mLabels.Add(AddLabel(290, 320, "Camera down:"));
			mLabels.Add(AddLabel(290, 370, "Camera right:"));
			mLabels.Add(AddLabel(290, 420, "Camera Zoom In:"));
			mLabels.Add(AddLabel(290, 470, "Camera Zoom Out:"));
			mLabels.Add(AddLabel(290, 520, "Move:"));
			mLabels.Add(AddLabel(290, 570, "AttackMove:"));
			mLabels.Add(AddLabel(290, 620, "Patrol:"));
			mLabels.Add(AddLabel(290, 670, "Taunt:"));
			mLabels.Add(AddLabel(290, 720, "Attack:"));
			mLabels.Add(AddLabel(290, 770, "Stop:"));
			mLabels.Add(AddLabel(290, 820, "Pause (Game Menu):"));

			mLabels.Add(AddLabel(820, 170, "Group 0:"));
			mLabels.Add(AddLabel(820, 220, "Group 1:"));
			mLabels.Add(AddLabel(820, 270, "Group 2:"));
			mLabels.Add(AddLabel(820, 320, "Group 3:"));
			mLabels.Add(AddLabel(820, 370, "Group 4:"));
			mLabels.Add(AddLabel(820, 420, "Group 5:"));
			mLabels.Add(AddLabel(820, 470, "Group 6:"));
			mLabels.Add(AddLabel(820, 520, "Group 7:"));
			mLabels.Add(AddLabel(820, 570, "Group 8:"));
			mLabels.Add(AddLabel(820, 620, "Group 9:"));

			mLabels.Add(AddLabel(1150, 170, "Spawn Shieldguard:"));
			mLabels.Add(AddLabel(1150, 220, "Spawn Priest:"));
			mLabels.Add(AddLabel(1150, 270, "Spawn Range Priest:"));
			mLabels.Add(AddLabel(1150, 320, "Spawn Elite Atlantican:"));
			mLabels.Add(AddLabel(1150, 370, "Spawn Witch:"));
			mLabels.Add(AddLabel(1150, 420, "Spawn Beast:"));

			mLabels.Add(AddLabel(1150, 910, "Note: You can only use the letters A-Z, "));
			mLabels.Add(AddLabel(1150, 960, "the digits 0-9, ESCAPE, SPACE and the arrow keys."));


			AddHotkeyBox(HotKeys.HotKey.CenterCamera, ScaledRectangle(630, 170, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraUp, ScaledRectangle(630, 220, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraLeft, ScaledRectangle(630, 270, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraDown, ScaledRectangle(630, 320, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraRight, ScaledRectangle(630, 370, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraZoomIn, ScaledRectangle(630, 420, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.CameraZoomOut, ScaledRectangle(630, 470, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Move, ScaledRectangle(630, 520, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.AttackMove, ScaledRectangle(630, 570, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Patrol, ScaledRectangle(630, 620, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Taunt, ScaledRectangle(630, 670, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Attack, ScaledRectangle(630, 720, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Stop, ScaledRectangle(630, 770, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.ShowGameMenu, ScaledRectangle(630, 820, 130, 30));


			AddHotkeyBox(HotKeys.HotKey.Group0, ScaledRectangle(960, 170, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group1, ScaledRectangle(960, 220, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group2, ScaledRectangle(960, 270, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group3, ScaledRectangle(960, 320, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group4, ScaledRectangle(960, 370, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group5, ScaledRectangle(960, 420, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group6, ScaledRectangle(960, 470, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group7, ScaledRectangle(960, 520, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group8, ScaledRectangle(960, 570, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.Group9, ScaledRectangle(960, 620, 130, 30));

			AddHotkeyBox(HotKeys.HotKey.SpawnShieldGuard, ScaledRectangle(1500, 170, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.SpawnPriest, ScaledRectangle(1500, 220, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.SpawnRangedPriest, ScaledRectangle(1500, 270, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.SpawnEliteAtlantican, ScaledRectangle(1500, 320, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.SpawnWitch, ScaledRectangle(1500, 370, 130, 30));
			AddHotkeyBox(HotKeys.HotKey.SpawnBeast, ScaledRectangle(1500, 420, 130, 30));
			
		}


		private void AddHotkeyBox(HotKeys.HotKey key, Rectangle rectangle)
		{
			mMBox.Add(new PseudoTextBox(TextBoxMode.Hotkey, key,
				new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(1100, 370, 0, 0),
					Main.HotKey.GetHotkey(key).ToString()), rectangle));
		}

		
		public override void Update()
		{
			base.Update();
			HotkeyUpdate();
		}

		public override void Draw()
		{
			
			DrawButtonsAndBackground();
			DrawListOfLabels(mLabels);
			foreach (PseudoTextBox box in mMBox)
			{
				box.DrawHotkeyMode();
			}

		}

		private void HotkeyUpdate()
		{
			//MouseSliding for the Textboxes
			foreach (PseudoTextBox box in mMBox)
			{
				box.TestMouseSlideOver();
			}

			//MouseSliding for the Buttons
			TestMouseSlideOver();


			if (Main.Input.MouseClicked())
			{

				//Activate one textbox when cliked on it and deactivate all other
				if (!mMIsEditing)
				{
					foreach (PseudoTextBox box in mMBox)
					{
						mMIsEditing = box.TestEditMode();
						if (mMIsEditing)
							break;
					}
				}

				/*
				//Testting if button was clicked
				foreach (MenuButton button in mItems)
				{
					Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
					if (button.Rectangle.Contains(point) && !mInGame)
					{
						
						mManager.RemoveScreen();
						//mManager.AddScreen(new OptionScreen());
					}

					if (button.Rectangle.Contains(point) && mInGame)
					{
						mManager.RemoveScreen();
						//mManager.AddScreen(new InGameOptionsScreen());
					}

				}
				*/
				//Testting if button was clicked

				foreach (MenuButton button in mItems)
				{
					Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
					if (button.Rectangle.Contains(point))
					{
						mManager.RemoveScreen();
					}
				}

			}

			//The activate textbox will get the input from the keyboard
			if (mMIsEditing)
			{
				foreach (PseudoTextBox box in mMBox)
				{
					mMIsEditing = box.HotkeyPress();
					if (!mMIsEditing)
						break;
				}
			}
		}
		 
	}
}


