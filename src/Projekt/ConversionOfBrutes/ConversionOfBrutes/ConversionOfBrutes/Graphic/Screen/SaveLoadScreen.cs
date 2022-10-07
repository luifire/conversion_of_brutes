/**
 * Author: David Spisla 
 * 
 * Screen for Saving and Loading
 * Usage: Save or Load Screen. The context is defined by the boolean variables 
 * Missing: nothing 
 * 
 **/

using System;
using System.Collections.Generic;
using System.IO;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class SaveLoadScreen : Screen
	{

		private readonly bool mInGame;
		private readonly bool mMode;
		private readonly bool mMapEditorMode;
		private LoadComponent mLoadComponent;
		private int mIndexLoadSaveGameOrMap;
		private String mStringIsEmpty;
		private MenuLabel mSlotEmpty;
		private bool mSlotIsEmpty;
		private int mCounter;
		private List<String> mList; 

		public SaveLoadScreen(bool inGame, bool mode, bool mapEditorMode)
		{
			
			mInGame = inGame;
			mMode = mode;
			mMapEditorMode = mapEditorMode;
			Initialize();
			mIsEscClosable = true;
			if (mInGame)
				mDrawScreensBelow = true;
		}


		public override sealed void Initialize()
		{

			InitializeAbstr();
			mIndexLoadSaveGameOrMap = -1;
#if DEBUG
		    const string temp1 = "..\\..\\..\\..\\ConversionOfBrutesContent\\Saves\\0.sav";
		    const string temp2 = "..\\..\\..\\..\\ConversionOfBrutesContent\\Maps\\2.map";
#else
			const string temp1 = "Saves\\0.sav";
			const string temp2 = "Maps\\2.map";
#endif
			
			

			if (!mMapEditorMode)
			{

				mList = SaveAndLoad.GetSaveList();
				
				//Loading from the MainMenu
				if (!mInGame && !mMode)
				{
					mBackground = new MenuLabel(MenuItem.MenuIdentifier.LoadGameScreen, ScaledRectangle(0, 0, 1920, 1080), null);
					mSlotEmpty = AddLabel(760, 700, "Slot is empty! Choose another one...");
					mLoadComponent = new LoadComponent(mList, new Rectangle(820, 240, 0, 0), new Rectangle(1070, 240, 30, 30));
					mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(750, 910, 210, 100)));
					mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(960, 910, 210, 100)));
					if (File.Exists(temp1))
						mIndexLoadSaveGameOrMap = 0;

				}


				// Load/ Save from InGameMenu
				if (mInGame)
				{
					
					mLoadComponent = new LoadComponent(mList, new Rectangle(850, 300, 0, 0), new Rectangle(1100, 300, 30, 30));
					mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(980, 740, 150, 80)));


					//Part for the Loading
					if (!mMode)
					{
						mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameLoadGameScreen, ScaledRectangle(720, 170, 508, 563 + 100), null);
						mSlotEmpty = AddLabel(820, 700, "Choose an existing game!");
						mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(820, 740, 150, 80)));
						if (File.Exists(temp1))
							mIndexLoadSaveGameOrMap = 0;
					}

					//Part for the Saving
					if (mMode)
					{
						mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameSaveGameScreen, ScaledRectangle(720, 170, 508, 563 + 100), null);
						mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Save, ScaledRectangle(820, 740, 150, 80)));
						mSlotEmpty = AddLabel(850, 700, "Choose a slot!");
						mIndexLoadSaveGameOrMap = 0;
					}


				}

			}
			

			//This is for Save a Map via InGameMenu
			if (mInGame && mMode && mMapEditorMode)
			{

				mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameSaveMapScreen, ScaledRectangle(720, 170, 508, 563 + 100), null);
				mList = SaveAndLoad.GetMapList();
				mLoadComponent = new LoadComponent(mList, new Rectangle(850, 300, 0, 0), new Rectangle(1070, 300, 30, 30));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Save, ScaledRectangle(820, 740, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(980, 740, 150, 80)));
				mSlotEmpty = AddLabel(800, 700, "Can't change default map!");
				mLoadComponent.BoxesForLabels[0].Editmode = false;
				mLoadComponent.BoxesForLabels[2].Editmode = true;
				if (File.Exists(temp2))
					mIndexLoadSaveGameOrMap = 2;

			}

			//This is for Load a Map via InGameMenu
			if (mInGame && !mMode && mMapEditorMode)
			{

				mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameLoadMapScreen, ScaledRectangle(720, 170, 508, 563 + 100), null);
				mList = SaveAndLoad.GetMapList();
				mSlotEmpty = AddLabel(840, 700, "Choose an existing map!");
				mLoadComponent = new LoadComponent(mList, new Rectangle(850, 300, 0, 0), new Rectangle(1070, 300, 30, 30));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Load, ScaledRectangle(820, 740, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(980, 740, 150, 80)));
				mLoadComponent.BoxesForLabels[0].Editmode = false;
				mLoadComponent.BoxesForLabels[2].Editmode = true;
				if (File.Exists(temp2))
					mIndexLoadSaveGameOrMap = 2;
			}
		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();
			mLoadComponent.Update();

			if (Main.Input.MouseClicked())
			{

				Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);

				//Get the index for Load or Save a Game or a Map via MainMenu or InGameMenu
				int temp = mLoadComponent.TestLoadSaveMode();

				if (temp != -1 )
				{
					mIndexLoadSaveGameOrMap = temp;
					mStringIsEmpty = mLoadComponent.StringIsEmpty;
   
				}

				//This is for Load or Save a Game or a Map via MainMenu or InGameMenu
				foreach (MenuButton button in mItems)
				{
					if (button.Rectangle.Contains(point))
					{
						switch (button.Identifier)
						{
							case MenuItem.MenuIdentifier.Load:
								Main.Audio.PlaySound(AudioManager.Sound.Click2);

								//This is for Load a Game
								if (!mMapEditorMode)
								{
									
									if (mStringIsEmpty != "empty" && mIndexLoadSaveGameOrMap != -1)
									{

										GameScreen.GameNotFinished = false;

										if (mInGame)
										{
											mManager.RemoveScreen();
											mManager.RemoveScreen();
											mManager.RemoveScreen();
										}
										SaveAndLoad.LoadSaveGame(mIndexLoadSaveGameOrMap);
										GameScreen.TechDemo = false;
										GameScreen.FogOfWar = true;
									}
									else
									{
										mSlotIsEmpty = true;
										mCounter = 0;
									}

									if(!mInGame)
										Main.Audio.StopSound(AudioManager.Sound.MainMenuMusic);
                           
								}

								//This is for Load a Map
								if (mMapEditorMode)
								{
									if (mStringIsEmpty != "empty" && mIndexLoadSaveGameOrMap != -1)
									{
										
										mManager.RemoveScreen();
										mManager.RemoveScreen();
										mManager.RemoveScreen();
										mManager.RemoveScreen();
										SaveAndLoad.LoadMapEditor(mIndexLoadSaveGameOrMap.ToString());
									}
									else
									{
										mSlotIsEmpty = true;
										mCounter = 0;
									}
								}
								

								mIndexLoadSaveGameOrMap = -1;
								break;
							case MenuItem.MenuIdentifier.Save:
								Main.Audio.PlaySound(AudioManager.Sound.Click2);	
							
							    //This is for Save a Game via InGameMenu
								if (mInGame && mMode && !mMapEditorMode)
								{
									if (mIndexLoadSaveGameOrMap != -1)
									{
										Main.mGameStatistic.DurationOfGame += GameScreen.Stopwatch.Elapsed.TotalSeconds;
										GameScreen.Stopwatch.Reset();
										GameScreen.GameNotFinished = false;
										SaveAndLoad.SaveGame(mIndexLoadSaveGameOrMap);
										mList = SaveAndLoad.GetSaveList();
										mLoadComponent = new LoadComponent(mList, new Rectangle(850, 300, 0, 0), new Rectangle(1100, 300, 30, 30));
										mLoadComponent.BoxesForLabels[0].Editmode = false;
									}
									else
									{
										mSlotIsEmpty = true;
										mCounter = 0;
									}
									
								}
								

								//This is for Save a Map via InGameMenu
								if (mInGame && mMode && mMapEditorMode)
								{
									if (mIndexLoadSaveGameOrMap != -1)
									{
										//Comment this condition if you want to change the map 0 or 1
										if (!(mIndexLoadSaveGameOrMap == 0 || mIndexLoadSaveGameOrMap == 1))
										{
											SaveAndLoad.SaveMap(mIndexLoadSaveGameOrMap.ToString());
										}
										else
										{
											mSlotIsEmpty = true;
											mCounter = 0;
										}

										mList = SaveAndLoad.GetMapList();
										mLoadComponent = new LoadComponent(mList, new Rectangle(850, 300, 0, 0), new Rectangle(1070, 300, 30, 30));
										mLoadComponent.BoxesForLabels[0].Editmode = false;
									}

								}

								mIndexLoadSaveGameOrMap = -1;
								break;
							case MenuItem.MenuIdentifier.Back:
								mManager.RemoveScreen();
								break;
							
						}
					}

				}
			}

		}

		public override void Draw()
		{
			
			DrawButtonsAndBackground();
			mLoadComponent.Draw();

			if (mSlotIsEmpty && mCounter <= 200)
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			    mSpriteBatch.DrawString(mSlotEmpty.GetFont(), mSlotEmpty.Text, new Vector2(mSlotEmpty.Rectangle.X, mSlotEmpty.Rectangle.Y), Color.AliceBlue,
			    0f, new Vector2(0, 0), 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);
				mSpriteBatch.End();
				mCounter++;
			}

			if (mInGame)
			{
				Handling2DAnd3D();
			}
		}
	}
}
