using System;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Map
{
	class MiniMap
	{
		Texture2D mMiniMap;
		private int mMiniMapUpdateCounter;
		private readonly int mMinimapWidth;
		private readonly int mMinimapHeight;
		private readonly uint[] mGreenMiniMap;

		public MiniMap(Rectangle miniMapRect)
		{
			mMinimapWidth = miniMapRect.Width;
			mMinimapHeight = miniMapRect.Height;

			mGreenMiniMap = new uint[mMinimapWidth * mMinimapHeight];

			// initialize with green
			for (int i = 0; i < mGreenMiniMap.Length; i++)
			{
				mGreenMiniMap[i] = Color.DarkOliveGreen.PackedValue;
			}
		}

		public void Update()
		{
			// Update every 20. time
			if (mMiniMapUpdateCounter++ > 5)
			{
				mMiniMapUpdateCounter = 0;
				UpdateMinimap();
			}
		}

		// Declare a delegate.
		delegate void MiniDrawer(float xPos, float yPos, Color color);

		private void UpdateMinimap()
		{
			int mapWidth = GameScreen.Map.QuadRect.Width;
			int mapHeight = GameScreen.Map.QuadRect.Height;
			int miniMapSize = mMinimapWidth * mMinimapHeight;
			uint[] miniMap = new uint[miniMapSize];

			// initialize with green
			mGreenMiniMap.CopyTo(miniMap, 0);

			// inline drawer
			MiniDrawer miniDrawer = delegate(float xPos, float yPos, Color color)
			{
				int index = mMinimapWidth * (int) yPos + (int) xPos;

				if (yPos < mMinimapHeight && xPos < mMinimapWidth)
				{ 
					if (index < 0 || index > mMinimapWidth * mMinimapHeight)
						return;

					miniMap[index] = color.PackedValue;
				}
			};


			// All WorldObjects and Selected Units
			for(int specialObjects = 0; specialObjects < 2; specialObjects++)
			{
				// Performance es gibt in ObjManager zwei Listen mit WorldObj und Units
				var allObjects = specialObjects == 0 ?  GameScreen.Map.GetEnumerator() : GameScreen.ObjectManager.SelectionHandler.SelectedObjects.GetEnumerator();
			
				while(allObjects.MoveNext())
				{
					var obj = allObjects.Current;
				
					Color color;
					int areaSize = 2;
					switch (obj.Ident)
					{
						case Ident.Tree:
							color = Color.DarkGreen;
							break;
						case Ident.MountainSmall:
							color = Color.Black;
							areaSize = 5;
							break;
						case Ident.Mountain:
							color = Color.Black;
							areaSize = 9;
							break;
						case Ident.MountainBig:
							color = Color.Black;
							areaSize = 15;
							break;
						case Ident.Pond:
							color = Color.SkyBlue;
							areaSize = 9;
							break;

						case Ident.Zone:
						case Ident.Spawnzone:
						case Ident.HomeZone:
							Zone zone = (Zone) obj;
							if (((zone.Visible || zone.Seen)) || obj.Ident == Ident.HomeZone || GameScreen.FogOfWar == false)
							{
								switch (obj.Fraction)
								{
									case Fraction.Player:
										color = Color.DarkBlue;
										break;
									case Fraction.Ai:
										color = Color.DarkRed;
										break;
									default:
										color = Color.White;
										break;
								}
							}
							else
							{
								color = Color.White;
							}

							areaSize = obj.Ident == Ident.HomeZone ? 18 : 12;
							break;
						default:
							if(obj is Unit == false)
								throw new ArgumentOutOfRangeException();
							Unit unit = (Unit) obj;
							// enemy Unit not visible
							if (unit.Visible == false && !GameScreen.TechDemo )
								continue;
							color = unit.Fraction == Fraction.Ai ? Color.Red : Color.Blue;
							break;
					}
					// selected objects get other color
					if (specialObjects == 1)
						color.PackedValue |= 0x90909090;

					float miniMapXOffset = obj.Position.X / mapWidth * mMinimapWidth;
					float miniMapYOffset = obj.Position.Y / mapHeight * mMinimapHeight;

					int halfArea = areaSize / 2;

					if (obj is Zone)
					{
						// Draws an area for each obj
						for (int x = 0; x < areaSize; x++)
						{
							miniDrawer(miniMapXOffset + x - halfArea, miniMapYOffset - halfArea, color);
							miniDrawer(miniMapXOffset + x - halfArea, miniMapYOffset + halfArea, color); // +areaSize - halfArea = halfArea
						}

						for (int y = 0; y < areaSize + 1; y++)
						{
							miniDrawer(miniMapXOffset - halfArea, miniMapYOffset + y - halfArea, color);
							miniDrawer(miniMapXOffset + halfArea, miniMapYOffset + y - halfArea, color); // +areaSize - halfArea = halfArea
						}
					}
					else
					{
						miniMapXOffset -= halfArea;
						miniMapYOffset -= halfArea;

						// Draws an area for each obj
						for (int x = 0; x < areaSize; x++)
						{
							for (int y = 0; y < areaSize; y++)
							{
								miniDrawer(miniMapXOffset + x, miniMapYOffset + y, color);
							}
						}
					}
				}
				/*
					float relativX = obj.Position.X / mapWidth;
					float relativY = obj.Position.Y / mapHeight;

					if (obj is Zone)
					{
						float halfArea = areaSize / 2f;
						// Draws an area for each obj
						for (int x = 0; x < areaSize; x++)
						{
							miniDrawer(relativX * mMinimapWidth + x - halfArea, relativY * mMinimapHeight - halfArea, color);
							miniDrawer(relativX * mMinimapWidth + x - halfArea, relativY * mMinimapHeight + halfArea, color); // +areaSize - halfArea = halfArea
						}

						for (int y = 0; y < areaSize + 1; y++)
						{
							miniDrawer(relativX * mMinimapWidth - halfArea, relativY * mMinimapHeight + y - halfArea, color);
							miniDrawer(relativX * mMinimapWidth + halfArea, relativY * mMinimapHeight + y - halfArea, color); // +areaSize - halfArea = halfArea
						}
					}
					else
					{
						// Draws an area for each obj
						for (int x = 0; x < areaSize; x++)
						{
							for (int y = 0; y < areaSize; y++)
							{
								miniDrawer(relativX * mMinimapWidth + x, relativY * mMinimapHeight + y, color);
							}
						}
						*/
			}
			
			// Draw Camera View field
			{
				Rectangle cameraRect = GameScreen.Camera.VisibleRectangle;
				
				bool noLeftBarrier = cameraRect.X < 0;
				if (noLeftBarrier)
				{
					cameraRect.Width += cameraRect.X;
					if (cameraRect.Width < 0)
						cameraRect.Width = 0;
					cameraRect.X = 0;
				}

				bool noTopBarrier = cameraRect.Y < 0;
				if (noTopBarrier)
				{
					cameraRect.Height += cameraRect.Y;
					if (cameraRect.Height < 0)
						cameraRect.Height = 0;
					cameraRect.Y = 0;
				}

				bool noRightBarrier = cameraRect.X + cameraRect.Width > mapWidth - 1;
				if (noRightBarrier)
					cameraRect.Width = cameraRect.Width - (mapWidth - (cameraRect.X + cameraRect.Width + 1));

				bool noBottomBarrier = cameraRect.Y + cameraRect.Height > mapHeight - 1;
				if (noBottomBarrier)
					cameraRect.Height = mapHeight - cameraRect.Y - 1;

				int xStart = (int)(((float)cameraRect.X / mapWidth) * mMinimapWidth);
				int yStart = (int)(((float)cameraRect.Y / mapHeight) * mMinimapHeight);

				float relWidth = (float)cameraRect.Width / mapWidth;
				float relHeight = (float)cameraRect.Height / mapHeight;

				int miniCamWidth = (int)(relWidth * mMinimapWidth);
				int miniCamHeight = (int)(relHeight * mMinimapHeight);

				// in case of to much to the right
				int negWidth = mMinimapWidth - xStart - miniCamWidth - 1;
				if (negWidth < 0)
					miniCamWidth += negWidth;

				int negHeight = mMinimapHeight - yStart - miniCamHeight - 1;
				if (negHeight < 0)
					miniCamHeight += negHeight;

				Color cameraColor = Color.YellowGreen;
				int indexYUpStart = yStart*mMinimapWidth;
				int indexYDownStart = (yStart + miniCamHeight) * mMinimapWidth;
				
				for (int x = xStart; x < miniCamWidth + xStart; x++)
				{
					if(noTopBarrier == false)
						miniMap[indexYUpStart + x] = cameraColor.PackedValue;
					if(noBottomBarrier == false)
						miniMap[indexYDownStart + x] = cameraColor.PackedValue;
				}
				
				for (int y = yStart; y <= miniCamHeight + yStart; y++)
				{
					if(noLeftBarrier == false)
						miniDrawer(xStart, y, cameraColor);
					if(noRightBarrier == false)
						miniDrawer(xStart + miniCamWidth, y, cameraColor);
				}
			}
			
			mMiniMap = new Texture2D(Main.Graphics.GraphicsDevice, mMinimapWidth, mMinimapHeight);
			mMiniMap.SetData(miniMap);
		}

		public Texture2D MiniMapTexture { get { return mMiniMap; } }
	}
}
