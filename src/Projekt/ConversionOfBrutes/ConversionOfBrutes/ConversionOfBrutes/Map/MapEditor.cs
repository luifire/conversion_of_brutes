/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Map
{
	/// <summary>
	/// Load, save and change maps to play on. In addition it manages the NavMeshEditor. 
	/// </summary>
	sealed class MapEditor
	{
		#region Private member variables
		private readonly NavigationMeshEditor mNavigationMeshEditor;
		private bool mEditMap = true;

		private readonly String mMapMode;
		private readonly String mNavMeshMode;

		private Ident mCurrentIdent = Ident.Zone;
		private Fraction mFraction = Fraction.Gaia;
		private readonly LinkedList<Tuple<Ident, Rectangle>> mObjectSizes = new LinkedList<Tuple<Ident, Rectangle>>();
#endregion

		public MapEditor()
		{
			mNavigationMeshEditor = new NavigationMeshEditor(this);
			Main.HotKey.ChangeToMapEditorMapping();

			// Get rectangle of every map object
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.Zone, GetRectangle(Ident.Zone)));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.Spawnzone, GetRectangle(Ident.Spawnzone)));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.Tree, Rectangle.Empty));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.MountainSmall, Rectangle.Empty));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.Mountain, Rectangle.Empty));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.MountainBig, Rectangle.Empty));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.Pond, Rectangle.Empty));
			mObjectSizes.AddLast(new Tuple<Ident, Rectangle>(Ident.HomeZone, GetRectangle(Ident.HomeZone)));

			#region DeveloperInfo
			mMapMode = "MapEditor" + "\n";
			foreach (HotKeys.HotKey hotKey in Enum.GetValues(typeof (HotKeys.HotKey)))
			{
				if (Main.HotKey.GetHotkey(hotKey) == Keys.F24)
				{
					continue;
				}
				if ((int) hotKey < 8)
				{
					continue;
				}
				switch (hotKey)
				{
					case HotKeys.HotKey.SwitchMode:
						mMapMode = mMapMode + "Switch to NavigationMeshEditing: " + Main.HotKey.GetHotkey(hotKey)+ "\n";
						mMapMode = mMapMode + "Place object on map: LeftClick\n";
						break;
					case HotKeys.HotKey.EditorMove:
						mMapMode = mMapMode + "Hold (with LeftMouseDown) to move object: " + Main.HotKey.GetHotkey(hotKey)+ "\n";
						break;
					case HotKeys.HotKey.EditorDelete:
						mMapMode = mMapMode + "Hold (with LeftMouseClick) to delete object: " + Main.HotKey.GetHotkey(hotKey)+ "\n";
						break;
					case HotKeys.HotKey.EditorSafety:
						mMapMode = mMapMode + "Hold and press delete to delete all objetcs (irreversible!): " +
						           Main.HotKey.GetHotkey(HotKeys.HotKey.EditorSafety) + "\n";
						break;
					case HotKeys.HotKey.EditorAlt:
						break;
					case HotKeys.HotKey.HideObjects:
						break;
					default:
						mMapMode = mMapMode + hotKey + ": " + Main.HotKey.GetHotkey(hotKey)+ "\n";
						break;
				}
			}
			mMapMode = mMapMode + "Only one HomeZone for the player/AI \n";

			mNavMeshMode = "NavigationMeshEditor" + "\n";
			foreach (HotKeys.HotKey hotKey in Enum.GetValues(typeof (HotKeys.HotKey)))
			{
				if (Main.HotKey.GetHotkey(hotKey) == Keys.F24)
				{
					continue;
				}
				switch (hotKey)
				{
					case HotKeys.HotKey.SwitchMode:
						mNavMeshMode = mNavMeshMode + "Switch to MapEditing: " + Main.HotKey.GetHotkey(hotKey) + "\n";
						mNavMeshMode = mNavMeshMode + "Add waypoint-node: LeftClick\n";
						mNavMeshMode = mNavMeshMode + "Select waypoint-node: RightClick\n";
						mNavMeshMode = mNavMeshMode + "Add edge from selected to nearest node: MiddleClick\n";
						break;
					case HotKeys.HotKey.EditorMove:
						mNavMeshMode = mNavMeshMode + "Hold (with LeftMouseDown) to move node: " + Main.HotKey.GetHotkey(hotKey) + "\n";
						break;
					case HotKeys.HotKey.EditorDelete:
						mNavMeshMode = mNavMeshMode + "Hold (with LeftMouseClick) to delete node: " + Main.HotKey.GetHotkey(hotKey) + "\n";
						break;
					case HotKeys.HotKey.EditorAlt:
						mNavMeshMode = mNavMeshMode + "Hold to do all of the above for Borders (forbiden areas): " +
						           Main.HotKey.GetHotkey(hotKey) + "\n";
						break;
					case HotKeys.HotKey.EditorSafety:
						mNavMeshMode = mNavMeshMode + "Hold and press delete to delete all Nodes (+BorderButton for Borders): " +
						               Main.HotKey.GetHotkey(hotKey) + "\n";
						break;
					case HotKeys.HotKey.HideObjects:
						mNavMeshMode = mNavMeshMode + "Press to show/hide all objects: " + Main.HotKey.GetHotkey(hotKey) + "\n";
						break;
				}
				//mNavMeshMode = mNavMeshMode + hotKey + ": " + GameScreen.HotKey.GetHotkey(hotKey) + "\n";
			}
			mNavMeshMode = mNavMeshMode + "Hold (with MiddleMouseClick) to embed Object in NavigationGraph: " + Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove) + "\n";
			if (GameScreen.Hud != null)
			{
				GameScreen.Hud.DeveloperInfo = mMapMode;
			}
			
#endregion
		}

		private Rectangle GetRectangle(Ident id)
		{
			WorldObject obj = GameScreen.ObjectManager.CreateWorldObject(id, new Vector2(200, 200), Fraction.Gaia);
			Rectangle objRect = obj.Rect;
			if (obj is Zone)
			{
				GameScreen.ObjectManager.SelectableObjects.Remove(obj);
				GameScreen.ParticleManager.RemoveRingEmitter((Zone)obj);
			}
			GameScreen.Map.Remove(obj);
			GameScreen.ObjectManager.MapObjects.Remove(obj);
			return objRect;
		}

		private void HandleMapEditorInput()
		{
			SetNewIdent();
			if (Main.Input.LeftMouseClicked)
			{
				// Add Object
				if (Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)) &&
				    Main.Input.KeyboardState.IsKeyUp(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)))
				{
					AddObject(NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition));
				}
				// Remove Object
				else if (Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)) &&
				         GameScreen.ObjectManager.MapObjects.Count > 0)
				{
					RemoveObject(GetNearestObject(NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition)));
				}
			}
			else if (Main.Input.MouseState.LeftButton == ButtonState.Pressed && GameScreen.ObjectManager.MapObjects.Count > 0 &&
			         Main.Input.KeyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorMove)))
			{
				MoveObject(GetNearestObject(NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition)));
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorDelete)) && 
				Main.Input.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.EditorSafety)))
			{
				RemoveAll();
			}
		}

		private void AddObject(Vector2 position)
		{
			// Only one HomeZone
			if ((GameScreen.ObjectManager.MapObjects.Any(obj => obj is HomeZone && obj.Fraction == Fraction.Player) &&
			     mFraction == Fraction.Player) ||
			    (GameScreen.ObjectManager.MapObjects.Any(obj => obj is HomeZone && obj.Fraction == Fraction.Ai) &&
			     mFraction == Fraction.Ai))
			{
				return;
			}

			if (!IsPositionValid(position, mObjectSizes.First(t => t.Item1 == mCurrentIdent).Item2, null))
			{
				return;
			}
			GameScreen.ObjectManager.CreateWorldObject(mCurrentIdent, position, mFraction);
		}

		private void RemoveObject(WorldObject obj)
		{
			if (!IsNearEnough(obj.Position))
			{
				return;
			}
			if (obj is Zone)
			{
				GameScreen.ObjectManager.SelectableObjects.Remove(obj);
				GameScreen.ParticleManager.RemoveRingEmitter((Zone)obj);
			}
			GameScreen.Map.Remove(obj);
			GameScreen.ObjectManager.MapObjects.Remove(obj);

		}

		private void RemoveAll()
		{
			GameScreen.Map.Clear();
			List<WorldObject> dummy = GameScreen.ObjectManager.MapObjects.ToList();
			foreach (WorldObject obj in dummy)
			{
				if (obj is Zone)
				{
					GameScreen.ObjectManager.SelectableObjects.Remove(obj);
					GameScreen.ParticleManager.RemoveRingEmitter((Zone)obj);
				}
				GameScreen.Map.Remove(obj);
				GameScreen.ObjectManager.MapObjects.Remove(obj);
			}
		}

		private void MoveObject(WorldObject obj)
		{
			if (!IsNearEnough(obj.Position) ||
				!IsPositionValid(NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition),
				mObjectSizes.First(t => t.Item1 == obj.Ident).Item2, obj))
			{
				return;
			}
			obj.Position = NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition);
		}

		private void SetNewIdent()
		{
			// Please do not look at this code below ^^"
			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Zone)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.Zone;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnZone)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.Spawnzone;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Tree)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.Tree;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.MountainSmall)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.MountainSmall;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Mountain)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.Mountain;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.MountainBig)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.MountainBig;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.Pond)))
			{
				mFraction = Fraction.Gaia;
				mCurrentIdent = Ident.Pond;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.HomeZonePlayer)))
			{
				if (GameScreen.ObjectManager.MapObjects.Any(obj => obj is HomeZone && obj.Fraction == Fraction.Player))
				{
					return;
				}
				mFraction = Fraction.Player;
				mCurrentIdent = Ident.HomeZone;
			}
			else if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.HomeZoneAi)))
			{
				if (GameScreen.ObjectManager.MapObjects.Any(obj => obj is HomeZone && obj.Fraction == Fraction.Ai))
				{
					return;
				}
				mFraction = Fraction.Ai;
				mCurrentIdent = Ident.HomeZone;
			}
		}

		internal WorldObject GetNearestObject(Vector2 position)
		{
			float closestDistance = float.MaxValue;
			WorldObject nearestObject = GameScreen.ObjectManager.MapObjects.First();	
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects)
			{
				if (Vector2.Distance(position, obj.Position) < closestDistance)
				{
					closestDistance = Vector2.Distance(position, obj.Position);
					nearestObject = obj;
				}	
			}
			return nearestObject;
		}

		private bool IsPositionValid(Vector2 position, Rectangle rect, WorldObject obj)
		{
			Point topLeft = new Point((int) position.X - rect.Width/2, (int) position.Y - rect.Height / 2);
			Point topRight = new Point((int) position.X + rect.Width/2, (int) position.Y - rect.Height / 2);
			Point bottomLeft = new Point((int) position.X - rect.Width/2, (int) position.Y + rect.Height / 2);
			Point bottomRight = new Point((int) position.X + rect.Width/2, (int) position.Y + rect.Height / 2);

			if (GameScreen.Map.QuadRect.Contains(topLeft) &&
				GameScreen.Map.QuadRect.Contains(bottomRight))
			{
				return GameScreen.ObjectManager.MapObjects.Where(o => o != obj).All(o => //!o.Rect.Intersects(obj.Rect) &&
																						 !o.Rect.Contains(topLeft) &&
				                                                                         !o.Rect.Contains(topRight) &&
				                                                                         !o.Rect.Contains(bottomLeft) &&
				                                                                         !o.Rect.Contains(bottomRight));
			}
			return false;
		}
		private bool IsNearEnough(Vector2 position)
		{
			return !(Vector2.Distance(NavigationMeshEditor.Calc3DPos(Main.Input.GetCurrentMousePosition), position) > 30);
		}

		/// <summary>
		/// Manage the input to create desired WorldObjects (MapObjects). 
		/// </summary>
		public void Update()
		{
			// Developer Info
			if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.SwitchMode)))
			{
				mEditMap = !mEditMap;
			}

			if (mEditMap)
			{
				GameScreen.Hud.DeveloperInfo = mMapMode;
				HandleMapEditorInput();
			}
			else
			{
				GameScreen.Hud.DeveloperInfo = mNavMeshMode;
				mNavigationMeshEditor.Update();
			}
		}

		/// <summary>
		/// Just invokes the draw method for the NavMeshEditor 
		/// </summary>
		public void Draw()
		{
			if (!mEditMap)
			{
				mNavigationMeshEditor.Draw();
			}
		}
	}
}
