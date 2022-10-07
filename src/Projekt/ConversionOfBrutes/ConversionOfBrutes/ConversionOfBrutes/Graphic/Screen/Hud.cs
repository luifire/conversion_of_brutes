/**
 * Author: David Spisla, buerklij + derjenige, der die Klasse erstellt hat
 * 
 * Concrete Class for the HUD
 * Usage: This Class Implements the HUD which is used in the GameScreen class 
 * Missing: 
 * - functionality of all buttons
 **/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConversionOfBrutes.GameLogics;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Map;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Graphic.Screen
{
	internal sealed class Hud : Screen
	{
		private readonly DeveloperInformation mDeveloperInformation = new DeveloperInformation();

		private Texture2D mHud;
		private Rectangle mHudRectangle;
		private Rectangle mUpperHudRectangle;
		private Rectangle mLowerHudRectangle;
		private Rectangle mMiniMapRectangle;
		private List<ActionButton> mActionButtons;
		private Rectangle mToolTipBgSourceRect;
		private MenuLabel mToolTip;
		private bool mTauntToolTip;

		private MenuLabel mWinPoints;
		private MenuLabel mContingent;

		private MenuLabel mFps;
		private MenuLabel mDeveloperInfo;
		private MenuLabel mShowGameTime;
		private TimeSpan mDuration;
		private List<Thumbnail> mThumbNails;
		private LinkedList<SpawnJob> mSpawnQueue;
		private MenuLabel mThumbNailInfo;
		private Texture2D mSelectionButtons;

		private MiniMap mMiniMap;

		public Hud()
		{
			mDrawScreensBelow = true;
			mUpdateScreensBelow = true;
			Initialize();
		}


		public override void Initialize()
		{
			InitializeAbstr();
			mHud = Main.Content.Load<Texture2D>("HUD\\Hud");
			mHudRectangle = ScaledRectangle(0, 0, 1920, 1080);
			mUpperHudRectangle = ScaledRectangle(0, 0, 1920, 57);
			mLowerHudRectangle = ScaledRectangle(0, 840, 1920, 1080 - 841);

			if(Math.Abs(Main.Graphics.PreferredBackBufferHeight / (float)Main.Graphics.PreferredBackBufferWidth - 0.75f) < float.Epsilon)
				mMiniMapRectangle = ScaledRectangle(18, 845, 290, 230);
			else
				mMiniMapRectangle = ScaledRectangle(18, 845, 230, 230);

			MenuButton inGameMenuButton = new MenuButton(MenuItem.MenuIdentifier.InGameMenu, ScaledRectangle(0, 0, 120, 50));
			mWinPoints = AddLabel(800, 15, "Atlantis: | :Barbarians");
			mContingent = AddLabel(1600, 15, "");
			mDuration = new TimeSpan();
			mShowGameTime = AddLabel(170, 15, "");
			mFps = AddLabel(400, 15, "");
			mDeveloperInfo = AddLabel(400, 30, "");

			mThumbNails = new List<Thumbnail>();
			mSpawnQueue = new LinkedList<SpawnJob>();
			mActionButtons = new List<ActionButton>();
			mSelectionButtons = Main.Content.Load<Texture2D>("Button//SelectionButtons");

			mToolTipBgSourceRect = new Rectangle(526, 934, 1, 1);

			mItems.Add(inGameMenuButton);

			mMiniMap = new MiniMap(mMiniMapRectangle);

			if (GameScreen.MapEditorMode)
				mDeveloperInformation.ShowDeveloperInfo = true;
#if DEBUG
			mDeveloperInformation.ShowFps = true;
#endif

		}

		public override void Update()
		{
			base.Update();
			Point point1 = new Point((int)Main.Input.GetCurrentMousePosition.X, (int)Main.Input.GetCurrentMousePosition.Y);

			foreach (MenuButton button in mItems)
			{
				button.CheckIsMouseOver(point1);
			}
			
			var selectedObjects = GameScreen.ObjectManager.SelectionHandler.SelectedObjects;


			mDuration = TimeSpan.FromSeconds(GameScreen.Stopwatch.Elapsed.TotalSeconds).Add(TimeSpan.FromSeconds(Main.mGameStatistic.DurationOfGame));
			mShowGameTime.Text = String.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", mDuration.Hours, mDuration.Minutes, mDuration.Seconds);

			mWinPoints.Text = "Atlantis:" + (int) GameScreen.GameLogic.AtlantisPoints + " | " +
			                  (int) GameScreen.GameLogic.BarbPoints + ":Barbarians";
			mContingent.Text = "Contingent: " + GameScreen.GameLogic.AtlantisContingent + " | " +
				GameScreen.GameLogic.BarbContingent;
			if (mDeveloperInformation.ShowFps)
				mFps.Text = "FPS: " + GameScreen.Fps;
			HandleSelectedObjects(selectedObjects);
			HandleMouse(selectedObjects);
			HandleKeyboard(selectedObjects);

			mMiniMap.Update();
		}


		public override void Draw()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			mSpriteBatch.Draw(mHud, mHudRectangle, Color.White);

			mSpriteBatch.DrawString(mWinPoints.GetFont(),
				mWinPoints.Text,
				new Vector2(mWinPoints.Rectangle.X, mWinPoints.Rectangle.Y),
				Color.AliceBlue, 0f, Vector2.Zero, 2*(mScreenWidth / 1920f), SpriteEffects.None, 0f);

			mSpriteBatch.DrawString(mContingent.GetFont(),
				mContingent.Text,
				new Vector2(mContingent.Rectangle.X, mContingent.Rectangle.Y),
				Color.AliceBlue, 0f, Vector2.Zero, 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);

			mSpriteBatch.DrawString(mShowGameTime.GetFont(),
				mShowGameTime.Text,
				new Vector2(mShowGameTime.Rectangle.X, mShowGameTime.Rectangle.Y),
				Color.AliceBlue, 0f, Vector2.Zero, 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);

			if (mDeveloperInformation.ShowFps)
				mSpriteBatch.DrawString(mFps.GetFont(), mFps.Text, new Vector2(mFps.Rectangle.X, mFps.Rectangle.Y), Color.AliceBlue);

			if (mDeveloperInformation.ShowDeveloperInfo)
			{ 
				mSpriteBatch.DrawString(mDeveloperInfo.GetFont(),
				mDeveloperInfo.Text,
				new Vector2(mDeveloperInfo.Rectangle.X, mDeveloperInfo.Rectangle.Y),
				Color.AliceBlue);
			}

			foreach (MenuButton item in mItems)
			{
				mSpriteBatch.Draw(item.FirstTexture, item.Rectangle, Color.White);
			}

			// Draw thumbnails of all selected objects
			foreach (var thumbNail in mThumbNails)
			{
				if (mThumbNails.Count == 1)
				{
					Vector2 pos = new Vector2(mThumbNailInfo.Rectangle.X, mThumbNailInfo.Rectangle.Y);
					mSpriteBatch.DrawString(mThumbNailInfo.GetFont(),
						mThumbNailInfo.Text,
						pos,
						Color.Black,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f);
				}
				thumbNail.Draw();

			}

			// Draw SpawnQueue
			foreach (var job in mSpawnQueue)
			{
				job.Draw();
			}

			// Place the Action buttons in the HUD
			foreach (var button in mActionButtons)
			{
				bool activated = button.Action == ActionButton.ActionIdent.Attack && GameScreen.ObjectManager.SelectionHandler.AttackButton
					|| button.Action == ActionButton.ActionIdent.Move && GameScreen.ObjectManager.SelectionHandler.MoveButton
					|| button.Action == ActionButton.ActionIdent.AttackMove && GameScreen.ObjectManager.SelectionHandler.AttackMoveButton
					|| button.Action == ActionButton.ActionIdent.Patrol && GameScreen.ObjectManager.SelectionHandler.PatrolButton;
				button.Draw(activated);

				// Draw ToolTips
				if (button.HoverTime > 500)
				{
					mToolTip = new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(1640, 730, 325, 120), button.ToolTip);
					Rectangle backGroundRect = new Rectangle(mToolTip.Rectangle.X - 7,
						mToolTip.Rectangle.Y - 7,
						mToolTip.Rectangle.Width,
						mToolTip.Rectangle.Height);
					mSpriteBatch.Draw(mSelectionButtons, backGroundRect, mToolTipBgSourceRect, new Color(0, 0, 0, 200));

					mSpriteBatch.DrawString(mToolTip.GetFont(),
						mToolTip.Text,
						new Vector2(mToolTip.Rectangle.X, mToolTip.Rectangle.Y),
						Color.AliceBlue,
						0f,
						Vector2.Zero,
						1.5f * (mScreenWidth / 1920f),
						SpriteEffects.None,
						0f);

					if (button.Action == ActionButton.ActionIdent.Taunt)
					{
						mTauntToolTip = true;
					}
				}
				else
				{
					mTauntToolTip = false;
				}
			}

			// Minimap
			Texture2D miniMap = mMiniMap.MiniMapTexture;
			if (miniMap != null)
			{
				Rectangle rec = mMiniMapRectangle;
				mSpriteBatch.Draw(miniMap, rec, Color.White);
			}

			mSpriteBatch.End();

			foreach (MenuButton button in mItems)
			{
				button.DrawButton();
			}

			Main.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
			Main.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Main.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		}

		private void HandleMouse(LinkedList<WorldObject> selectedObjects)
		{
			bool mouseClicked = Main.Input.MouseClicked();
			bool areaSelected = Main.Input.IsAreaSelected();
			Point mousePos = Main.Input.GetCurrentMousePositionAsPoint();

			if (mouseClicked || areaSelected)
			{
				// has to happen, so nothing will happen ingame
				Main.Input.GetSelectedArea();

				Point clickedPoint = new Point((int) Main.Input.MouseClickPosition().X, (int) Main.Input.MouseClickPosition().Y);

				if (mLowerHudRectangle.Contains(clickedPoint) || mUpperHudRectangle.Contains(clickedPoint))
				{
					foreach (MenuButton item in mItems)
					{
						if (item.Rectangle.Contains(clickedPoint))
						{
							switch (item.Identifier)
							{
								case MenuItem.MenuIdentifier.InGameMenu:
									mManager.AddScreen(new InGameMenuScreen());
									if (Main.mAchievements.PauseOver15Times <= Main.Pause15Times && !GameScreen.MapEditorMode)
									{
										float tolerance = 0.009f;
										Main.mGameStatistic.PauseOver15Times++;

										if (Math.Abs(Main.mGameStatistic.PauseOver15Times - Main.Pause15Times) < tolerance)
										{
											Main.mAchievements.PauseOver15Times = Main.mGameStatistic.PauseOver15Times;
										}

									}
									break;
							}
						}
					}

					foreach (var button in mActionButtons)
					{
						if (button.DestRect.Contains(clickedPoint))
						{
							var buttonAction = button.Action;
							switch (buttonAction)
							{
								case ActionButton.ActionIdent.Attack:
									GameScreen.ObjectManager.SelectionHandler.AttackButton = true;
									break;
								case ActionButton.ActionIdent.AttackMove:
									GameScreen.ObjectManager.SelectionHandler.AttackMoveButton = true;
									break;
								case ActionButton.ActionIdent.Move:
									GameScreen.ObjectManager.SelectionHandler.MoveButton = true;
									break;
								case ActionButton.ActionIdent.Patrol:
									GameScreen.ObjectManager.SelectionHandler.PatrolButton = true;
									break;
								case ActionButton.ActionIdent.Stop:
									GameScreen.ObjectManager.StopSelectedUnits();
									break;
								case ActionButton.ActionIdent.Taunt:
									GameScreen.ObjectManager.CastTaunt();
									break;
								case ActionButton.ActionIdent.SpawnPriest:
									GameScreen.GameLogic.SpawnUnit(Ident.Priest, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
									    Main.mGameStatistic.RecruitedUnits++;
									break;
								case ActionButton.ActionIdent.SpawnShieldguard:
									GameScreen.GameLogic.SpawnUnit(Ident.ShieldGuard, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
									   Main.mGameStatistic.RecruitedUnits++;
									break;
								case ActionButton.ActionIdent.SpawnRangedPriest:
									GameScreen.GameLogic.SpawnUnit(Ident.PriestRanged, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
									   Main.mGameStatistic.RecruitedUnits++;
									break;
								case ActionButton.ActionIdent.SpawnEliteAtlantican:
									GameScreen.GameLogic.SpawnUnit(Ident.EliteAtlantic, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
									   Main.mGameStatistic.RecruitedUnits++;
									break;
								case ActionButton.ActionIdent.SpawnWitch:
									GameScreen.GameLogic.SpawnUnit(Ident.Witch, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
										Main.mGameStatistic.RecruitedUnits++;
									break;
								case ActionButton.ActionIdent.SpawnBeast:
									GameScreen.GameLogic.SpawnUnit(Ident.Beast, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
									UpdateSpawnQueue(selectedObjects.First.Value);
									if (!GameScreen.MapEditorMode)
										Main.mGameStatistic.RecruitedUnits++;
									break;

							}
						}
					}

					foreach (var thumbNail in mThumbNails)
					{
						if (thumbNail.DestRect.Contains(clickedPoint))
						{
							if (Main.Input.IsKeyDown(Keys.LeftControl))
							{
								GameScreen.ObjectManager.SelectionHandler.RemoveSelectedObject(thumbNail.RepresentedObject);
							}
							else
							{
								LinkedList<WorldObject> dmy = new LinkedList<WorldObject>();
								dmy.AddFirst(thumbNail.RepresentedObject);
								GameScreen.ObjectManager.SelectionHandler.SelectedObjects = dmy;
								
							}
						}
					}

					foreach (var job in mSpawnQueue)
					{
						if (job.DestRect.Contains(clickedPoint))
						{
							GameScreen.GameLogic.CancelSpawn(job);
							break;
						}
					}

					// moves camera to clicked point
					if (mMiniMapRectangle.Contains(clickedPoint) && Main.Input.RightMouseClicked)
					{
						MiniMapClickEvent(clickedPoint);
					}
				}
				else
				{
					if (mouseClicked)
					{
						Main.Input.MouseClickNotUsed();
					}
					else
					{
						Main.Input.AreaSelectNotUsed();
					}
					
				}
			}
			else if (Main.Input.MouseState.LeftButton == ButtonState.Pressed && 
				mMiniMapRectangle.Contains(Main.Input.MouseClickStartPositionAsPoint) && 
				mMiniMapRectangle.Contains(mousePos))
			{
				MiniMapClickEvent(mousePos);
			}

			foreach (var button in mActionButtons)
			{
				button.HoverTime = (button.DestRect.Contains(mousePos))
					? button.HoverTime += Main.GameTime.ElapsedGameTime.Milliseconds
					: 0;
			}
		}

		private void HandleKeyboard(LinkedList<WorldObject> selectedObjects)
		{
			// Cast Taunt (Shieldguard)
			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Taunt)))
			{
				GameScreen.ObjectManager.CastTaunt();
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Move)))
			{
				GameScreen.ObjectManager.SelectionHandler.MoveButton = true;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.AttackMove)))
			{
				GameScreen.ObjectManager.SelectionHandler.AttackMoveButton = true;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Patrol)))
			{
				GameScreen.ObjectManager.SelectionHandler.PatrolButton = true;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Attack)))
			{
				GameScreen.ObjectManager.SelectionHandler.AttackButton = true;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Stop)))
			{
				GameScreen.ObjectManager.StopSelectedUnits();
			}
				

			// Spawn Unit (When Spawnzone is selected)
			if (selectedObjects.Count > 0 && selectedObjects.First.Value is SpawnZone)
			{
				Ident spawnUnit = Ident.Mountain;
				
				if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnShieldGuard)))
				{
					spawnUnit = Ident.ShieldGuard;
				}
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnPriest)))
				{
					spawnUnit = Ident.Priest;
				}
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnRangedPriest)))
				{
					spawnUnit = Ident.PriestRanged;
				}
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnEliteAtlantican)))
				{
					spawnUnit = Ident.EliteAtlantic;
				}
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnWitch)))
				{
					spawnUnit = Ident.Witch;
				}
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnBeast)))
				{
					spawnUnit = Ident.Beast;
				}
				// you cannot spawn a mountain 
				if (spawnUnit != Ident.Mountain)
				{ 
					GameScreen.GameLogic.SpawnUnit(spawnUnit, (SpawnZone)selectedObjects.First.Value, Fraction.Player);
					UpdateSpawnQueue(selectedObjects.First.Value);
				}
			}

			for (int i = 0; i < HotKeys.HotKey.Group9 - HotKeys.HotKey.Group0; i++)
			{
				var currentGroup = HotKeys.HotKey.Group0 + i;
				// Group units 
				if (Main.Input.CtrlDownAndButtonPressed(Main.HotKey.GetHotkey(currentGroup)))
					GameScreen.ObjectManager.SelectionHandler.GroupObjects(i);
				// Select Groups 0-9
				else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(currentGroup)))
					GameScreen.ObjectManager.SelectionHandler.SelectGroup(i);
			}

			// Ingame Menu
			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.ShowGameMenu)))
			{
				mManager.AddScreen(new InGameMenuScreen());
			}
			else if (Main.Input.WasButtonPressed(Keys.F1))
			{
				mDeveloperInformation.ShowFps = !mDeveloperInformation.ShowFps;
			}
			else if (Main.Input.WasButtonPressed(Keys.F2))
			{
				mDeveloperInformation.ShowUnitInfo = !mDeveloperInformation.ShowUnitInfo;
			}
			else if (Main.Input.WasButtonPressed(Keys.F3))
			{
				mDeveloperInformation.ShowDeveloperInfo = !mDeveloperInformation.ShowDeveloperInfo;
			}
			else if (Main.Input.WasButtonPressed(Keys.F16))
			{
				GameScreen.ObjectManager.SpawnUnits(true, 50);
			}
			else if (Main.Input.WasButtonPressed(Keys.F17))
			{
				GameScreen.ObjectManager.SpawnUnits(false, 50);
			}
			else if (Main.Input.WasButtonPressed(Keys.F18))
			{
				GameScreen.ObjectManager.SpawnUnits(true, 50, true);
			}
			else if (Main.Input.WasButtonPressed(Keys.F7))
			{
				GameScreen.GraphicsManager.ToggleHealthBar();
			}
		}

		/// <summary>
		/// moves camera to the clicked position
		/// </summary>
		/// <param name="mousePos"></param>
		private void MiniMapClickEvent(Point mousePos)
		{
			double miniMapX = (double) (mousePos.X - mMiniMapRectangle.X) / mMiniMapRectangle.Width;
			double miniMapY = (double) (mousePos.Y - mMiniMapRectangle.Y) / mMiniMapRectangle.Height;

			Rectangle mapRect = GameScreen.Map.QuadRect;

			Vector2 mapPosition = new Vector2((float) (miniMapX * mapRect.Width), (float) (miniMapY * mapRect.Height));

			if (Main.Input.UsedMouseButton == InputManager.MouseButton.Left)
			{
				if (GameScreen.ObjectManager.SelectionHandler.CheckHudButtons(mapPosition, null))
					return;
				GameScreen.Camera.GotoWorldPosition(mapPosition);
			}
			// rechte Maustaste
			else
			{
				GameScreen.ObjectManager.SendSelectedUnits(mapPosition);
			}
		}

		private void HandleSelectedObjects(LinkedList<WorldObject> selectedObjects)
		{
			if (GameScreen.ObjectManager.SelectionHandler.NewSelection)
			{
				int cntUnits = 0;
				mThumbNails.Clear();
				mActionButtons.Clear();
				mSpawnQueue = new LinkedList<SpawnJob>();

				
				foreach (var obj in selectedObjects)
				{
					//Create thumbnails
					Rectangle thumbRect;
					thumbRect = selectedObjects.Count == 1
						? ScaledRectangle(490, 875, 180, 180)
						: ScaledRectangle(490 + (cntUnits % 18) * 60, 875 + (cntUnits / 18) * 60, 60, 60);

					mThumbNails.Add(new Thumbnail(obj, mSelectionButtons, thumbRect, mSpriteBatch));

					if (cntUnits++ >= 53)
						break;
				}

				// decide whitch Action buttons should be shown
				if (selectedObjects.Count != 0 && selectedObjects.First.Value.Fraction == Fraction.Player)
				{
					if (selectedObjects.Any(ob => (ob is SpawnZone)))
					{
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnPriest));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnShieldguard));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnRangedPriest));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnEliteAtlantican));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnBeast));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.SpawnWitch));
						
					}
					else if (!(selectedObjects.First.Value is Zone))
					{
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.Attack));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.Move));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.AttackMove));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.Patrol));
						mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.Stop));

						if (selectedObjects.Any(ob => ob.Ident == Ident.ShieldGuard))
						{
							mActionButtons.Add(new ActionButton(mSelectionButtons, mSpriteBatch, ActionButton.ActionIdent.Taunt));
						}
					}
				}
				int num = 0;
				foreach (var button in mActionButtons)
				{
					Rectangle destRectangle = ScaledRectangle(1700 + (num % 3) * 60, 880 + (num / 3) * 60, 60, 60);
					button.DestRect = destRectangle;
					num++;
				}

			}

			if (selectedObjects.Count == 1)
			{
				WorldObject obj = selectedObjects.First.Value;
				// Load additional info for HUD
				String info = "";
				switch (obj.Fraction)
				{
					case Fraction.Ai:
						info = "Enemy ";
						break;
					case Fraction.Gaia:
						info = "Neutral ";
						break;
					case Fraction.Player:
						info = "Friendly ";
						break;
				}

				info += "\n";

				switch (obj.Ident)
				{
					case Ident.Spawnzone:
						info += "Spawn Zone\n" + "Captured to " + (int)((((Zone)obj).FractionPoints) / 5d) + "%";
						UpdateSpawnQueue(obj);
						break;
					case Ident.Zone:
						info += "Zone\n" + "Captured to " + (int)((((Zone)obj).FractionPoints) / 5d) + "%";
						break;
					case Ident.HomeZone:
						info += "Home Zone\n";
						UpdateSpawnQueue(obj);
						break;
					// Atlantican Units
					case Ident.ShieldGuard:
						info += "Shieldguard";
						break;
					case Ident.Priest:
						info += "Priest";
						break;
					case Ident.PriestRanged:
						info += "Ranged Priest";
						break;
					case Ident.EliteAtlantic:
						info += "Elite Atlantican";
						break;
					case Ident.Witch:
						info += "Witch";
						break;
					case Ident.Beast:
						info += "Beast";
						break;
					case Ident.TechdemoAtlantic:
						info += "Elite Atlantican";
						break;
					// Barbarian units
					case Ident.EliteBarbarian:
						info += "Elite Barbarian";
						break;
					case Ident.Axeman:
						info += "Axeman";
						break;
					case Ident.Knight:
						info += "Knight";
						break;
					case Ident.Archer:
						info += "Archer";
						break;
					case Ident.ArcherMounted:
						info += "Mounted Archer";
						break;
					case Ident.TechdemoBarb:
						info += "Axeman";
						break;
				}
				if (!(obj is Zone))
				{
					info += "\nDamage: " + ((Unit) obj).Damage + "\n" + "attackRange: " + ((Unit) obj).AttackRange;
					info += "\n" + "Healthpoints: " + ((Unit) obj).HealthPoints + " / " + ((Unit) obj).MaxHealthPoints;
					if (obj.Fraction == Fraction.Ai)
						info += "\n" + "Faithpoints: " + ((Unit) obj).FaithPoints + " / " + ((Unit) obj).MaxFaithPoints;

					if (GameScreen.AiDebug)
					{
						info += "\n";
						info += "\n" + "mAttacking: " + ((Unit) obj).Attacking;
						info += "      " + "mAutoAttack: " + ((Unit) obj).AutoAttack;
						info += "\n" + "mDestReached: " + ((Unit) obj).DestinationReached;
						info += "      " + "Position: " + ((Unit) obj).Position;
						if (((Unit)obj).TargetUnit != null)
							info += "      " + "InAttackRange: " + ((Unit) obj).IsInAttackRange(((Unit)obj).TargetUnit);
						info += "      " + "AttackMove: " + ((Unit) obj).IsInAttackMove;
					}
				}

				mThumbNailInfo = new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(700, 900, 0, 0), info);
			}

			foreach (var thumbnail in mThumbNails)
			{
				thumbnail.Update();
			}

		}

		/// <summary>
		/// updates the spawn queue of the given Spawnzone
		/// </summary>
		/// <param name="obj">spawnZone</param>
		public void UpdateSpawnQueue(WorldObject obj)
		{
			if (obj.Fraction != Fraction.Player && !GameScreen.AiDebug)
				return;

			mSpawnQueue = ((SpawnZone)obj).SpawnJobs;

			if (mSpawnQueue.Count > 0)
			{
				mSpawnQueue.First.Value.Update(mSpriteBatch, mSelectionButtons, ScaledRectangle(870, 900, 90, 90));
			}

			int cnt = -1;
			foreach (var job in ((SpawnZone)obj).SpawnJobs)
			{
				if (cnt == -1)
				{
					cnt++;
					continue;
				}
					
				job.Update(mSpriteBatch, mSelectionButtons, ScaledRectangle(960+(cnt%10)*60, 900+(cnt/10)*60, 60 , 60));
				cnt++;
			}
		}

		public Rectangle UpperHudRectangle
		{ get { return mUpperHudRectangle; } }

		public Rectangle LowerHudRectangle
		{ get { return mLowerHudRectangle; } }

		public String DeveloperInfo
		{ set { mDeveloperInfo.Text = value; } }

		public bool TauntToolTip { get { return mTauntToolTip; } }

		public DeveloperInformation DeveloperInformation { get { return mDeveloperInformation; } }

	}
}