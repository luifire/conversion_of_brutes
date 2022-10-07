using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Library;
using Microsoft.Xna.Framework.Input;
using Zone = ConversionOfBrutes.GameObjects.Zone;


//Author: luibrand, buerklij

namespace ConversionOfBrutes.Misc
{   
	[Serializable]
	public sealed class SelectionHandler
	{
		[Serializable]
		public class SelectionSave
		{
			public LinkedList<WorldObject>[] mUnitGrouping;
			public LinkedList<WorldObject> mSelectedObjects;
		}

		private const int SelectionRangeX = 2;
		private const int SelectionRangeY = 6;

		private bool mNewSelection;
		private readonly LinkedList<WorldObject>[] mUnitGrouping;

		private LinkedList<WorldObject> mSelectedObjects = new LinkedList<WorldObject>(); // also includes a selected (spawn) zone

		private bool mAttackButton;
		private bool mPatrolButton;
		private bool mMoveButton;
		private bool mAttackMoveButton;

		public SelectionHandler()
		{
			mUnitGrouping = new LinkedList<WorldObject>[10];
			for (int i = 0; i < mUnitGrouping.Length; i++)
				mUnitGrouping[i] = new LinkedList<WorldObject>();
		}

		/// <summary>
		/// invoked after everything is loaded
		/// </summary>
		/// <param name="save"></param>
		public void SetSaveInformation(SelectionSave save)
		{
			SelectedObjects = SaveAndLoad.GenerateNewList(save.mSelectedObjects);

			for (int i = 0; i < mUnitGrouping.Length; i++)
				mUnitGrouping[i] = SaveAndLoad.GenerateNewList(save.mUnitGrouping[i]);
		}

		/// <summary>
		/// invoked to get save information
		/// </summary>
		/// <returns></returns>
		public SelectionSave GetSaveInformation()
		{
			return new SelectionSave()
			{
				mSelectedObjects = mSelectedObjects,
				mUnitGrouping = mUnitGrouping
			};
		}

		/// <summary>
		/// Draws the Selection line
		/// </summary>
		public void Draw()
		{
			// Draw Selection Area
			if (Main.Input.MouseState.LeftButton == ButtonState.Pressed &&
				GameScreen.VisibleRectangle.Contains(Main.Input.MouseClickStartPositionAsPoint))
			{
				Area a = new Area(Main.Input.MouseClickStartPosition, Main.Input.GetCurrentMousePosition);
				if(a.AreaSize() > 7)
					GameScreen.GraphicsManager.DrawArea(a);
			}
		}

		public void HandleSelection()
		{
			Vector2 destPosition;
			InputManager input = Main.Input;
			bool areaSelectedOrMouseClicked = input.MouseClicked() || input.IsAreaSelected();

			if (areaSelectedOrMouseClicked == false)
				return;

			LinkedList<WorldObject> currentlyPickedObj = new LinkedList<WorldObject>();

			// one click
			if (input.MouseClicked())
			{
				// this has to happen here
				destPosition = GameScreen.Camera.CalcWorldPosition(input.MouseClickPosition()); // in case of moving, this is where they should go

				var objectsInClickArea = GameScreen.Map.GetObjects(new Rectangle((int)destPosition.X - SelectionRangeX,
					(int)destPosition.Y - SelectionRangeY,
					SelectionRangeX * 2,
					SelectionRangeY * 2));

				WorldObject tmpPicked = null;
				// pick one object
				foreach (var obj in objectsInClickArea)
				{
					if (obj is Unit && obj.Visible)
					{ 
						currentlyPickedObj.AddFirst(obj);
						break;
					}
					// TODO: Das ist nicht wirklich schön, aber es muss irgendwie sichergestellt werden, dass nur eine spawnzone ausgewählt wird, wenn keine unit vorhanden ist
					if (tmpPicked == null && (obj is Zone))
					{
						tmpPicked = obj;
					}

				}
				if (currentlyPickedObj.Count == 0 && tmpPicked != null)
				{
					currentlyPickedObj.AddFirst(tmpPicked);
				}
			}
			// area selecte
			else // if (input.IsAreaSelected())
			{
				// this has to happen here too
				destPosition = GameScreen.Camera.CalcWorldPosition(input.MouseClickPosition()); // in case of moving, this is where they should go

				var currentlySelectedFriends = new LinkedList<WorldObject>();
				var currentlySelectedEnemies = new LinkedList<WorldObject>();
				var currentlySelectedGaia = new LinkedList<WorldObject>();

				Area area1 = input.GetSelectedArea();
				Area selectedArea = new Area(GameScreen.Camera.CalcWorldPosition(area1.UpperLeft), GameScreen.Camera.CalcWorldPosition(area1.LowerRight));

				var objInSelectedArea = GameScreen.Map.GetObjects(selectedArea.Rectangle);

				// pick units
				foreach (var obj in objInSelectedArea)
				{
					if (obj.Visible)
					{
						if (selectedArea.Intersects(obj.Area))
						{
							switch (obj.Fraction)
							{
								case Fraction.Player:
									currentlySelectedFriends.AddLast(obj);
									break;
								case Fraction.Ai:
									currentlySelectedEnemies.AddLast(obj);
									break;
								case Fraction.Gaia:
									if (obj is Zone)
										currentlySelectedGaia.AddLast(obj);
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
					}
				}

				// if friends are in the picked units => just pick friends
				//currentlyPickedObj = (currentlySelectedFriends.Count == 0) ? currentlySelectedEnemies : currentlySelectedFriends;
				if (currentlySelectedFriends.Count > 0)
				{
					currentlyPickedObj = currentlySelectedFriends;
				}
				else if (currentlySelectedEnemies.Count > 0)
				{
					currentlyPickedObj = currentlySelectedEnemies;
				}
				else
				{
					currentlyPickedObj = currentlySelectedGaia;
				}

				if (currentlyPickedObj.Count > 1)
				{
					if (currentlyPickedObj.All(ob => (ob is Zone)))
					{
						WorldObject zone = currentlyPickedObj.First.Value;
						currentlyPickedObj.Clear();
						currentlyPickedObj.AddFirst(zone);
					}
					else
					{
						LinkedListNode<WorldObject> obj = currentlyPickedObj.First;
						while (obj != null)
						{
							if (obj.Value is Zone)
							{
								LinkedListNode<WorldObject> dmy = obj.Next;
								currentlyPickedObj.Remove(obj);
								obj = dmy;
							}
							else
								obj = obj.Next;
							
						}
					}
				}
			}

			//=== Apply Action
			if (input.UsedMouseButton == InputManager.MouseButton.Left)
			{
				// First check if an ActionButton in Hud was pressed
				// if yes, do that, then return
				if (CheckHudButtons(destPosition, currentlyPickedObj))
					return;

				// If LeftCtrl was pressed add or remove objects
				if (Main.Input.IsKeyDown(Keys.LeftControl))
				{
					if (currentlyPickedObj.Count == 0 || currentlyPickedObj.First.Value is Zone || currentlyPickedObj.First.Value.Fraction == Fraction.Ai || SelectedObjects.First.Value is Zone)
						return;

					if (currentlyPickedObj.Any(ob => !(ob.IsSelected)))
					{
						var dmy = SelectedObjects;

						foreach (var obj in currentlyPickedObj)
						{
							if (!obj.IsSelected)
							{
								dmy.AddLast(obj);
							}
						}

						SelectedObjects = dmy;
					}
					else
					{
						foreach (var obj in currentlyPickedObj)
						{
							RemoveSelectedObject(obj);
						}
					}
				}
				// otherwise just select unit
				else
				{
					/*
					// Trim to 54
					while (currentlyPickedObj.Count > 54)
					{
						currentlyPickedObj.RemoveLast();
					}
					*/
					// In case you selected a zone which is covered by fow, don't select it
					if (currentlyPickedObj.Count > 0 && currentlyPickedObj.First.Value is Zone && !currentlyPickedObj.First.Value.Visible)
						currentlyPickedObj.RemoveFirst();

					SelectedObjects = currentlyPickedObj;
				}
			}
			// attack button
			else if (input.UsedMouseButton == InputManager.MouseButton.Right)
			{
				// no one selected or selected units are enemies => pick event
				if ((mSelectedObjects.Count == 0) || (mSelectedObjects.First.Value.Fraction == Fraction.Ai))
				{
					SelectedObjects = currentlyPickedObj;
				}
				// selected objects are friends
				else
				{
					// Units are selected
					if (!(mSelectedObjects.First.Value is Zone))
					{
						// no unit picked or (spawn-)zone picked => move there
						if (currentlyPickedObj.Count == 0 || (currentlyPickedObj.First.Value is Zone))
						{
							GameScreen.ObjectManager.ApplyPulkWalk(mSelectedObjects, destPosition);
						}
						else if (currentlyPickedObj.First.Value.Fraction == Fraction.Ai)
						{
							// everyone just attacks one (focus fire)
							GameScreen.ObjectManager.AttackUnit((Unit)currentlyPickedObj.First.Value);
						}
					}
					else if (mSelectedObjects.First.Value.Ident == Ident.Spawnzone || mSelectedObjects.First.Value.Ident == Ident.HomeZone) // Spawnzone is selected: set marker for Spawn
					{
						var zone = (SpawnZone) mSelectedObjects.First.Value;
						zone.UnitDestination = destPosition;
					}

				}
			}
		}

		/// <summary>
		/// Executes the appropriate action if an ActionButton was pressed
		/// </summary>
		/// <param name="destPosition"></param>
		/// <param name="currentlyPickedObj"></param>
		/// <returns>True iff a button has been pressed</returns>
		public bool CheckHudButtons(Vector2 destPosition, LinkedList<WorldObject> currentlyPickedObj)
		{
			if (mAttackButton)
			{
				if (currentlyPickedObj != null && currentlyPickedObj.Count == 1 && currentlyPickedObj.First.Value.Fraction == Fraction.Ai &&
				    currentlyPickedObj.First.Value is Unit)
				{
					GameScreen.ObjectManager.AttackUnit((Unit) currentlyPickedObj.First.Value);
				}
				mAttackButton = false;
			}
			else if (mMoveButton)
			{
				GameScreen.ObjectManager.ApplyPulkWalk(mSelectedObjects, destPosition);
				mMoveButton = false;
			}
			else if (mAttackMoveButton)
			{
				GameScreen.ObjectManager.AttackMove(destPosition, mSelectedObjects);
				mAttackMoveButton = false;
			}
			else if (mPatrolButton)
			{
				GameScreen.ObjectManager.StartPatrol(destPosition, mSelectedObjects);
				mPatrolButton = false;
			}
			else
			{
				return false;
			}
			return true;
		}

		public void UnitDied(Unit unit)
		{
			mSelectedObjects.Remove(unit);
			mNewSelection = true;
		}

		/// <summary>
		/// Safely removes an object
		/// </summary>
		/// <param name="obj"></param>
		public void RemoveSelectedObject(WorldObject obj)
		{
			obj.IsSelected = false;
			SelectedObjects.Remove(obj);
			mNewSelection = true;
		}

		public LinkedList<WorldObject> SelectedObjects { get { return mSelectedObjects; }
			set
			{
				foreach (var obj in mSelectedObjects)
				{
					obj.IsSelected = false;
				}

				mSelectedObjects = value;

				foreach (var obj in mSelectedObjects)
				{
					obj.IsSelected = true;
				}

				mNewSelection = true;
			}
		}

		public bool NewSelection {
			get
			{
				if (mNewSelection)
				{
					mNewSelection = false;
					return true;
				}
				return mNewSelection;
			}
			set { mNewSelection = value; }
		}

		public void GroupObjects(int groupNumber)
		{
			if (mSelectedObjects.Count > 0 && mSelectedObjects.First.Value.Fraction == Fraction.Player)
			{
				// delete from other groupings
				foreach (var group in mUnitGrouping)
				{
					if (group.Count > 0 && group != mSelectedObjects)
					{
						foreach (var currentlySelected in mSelectedObjects)
						{
							group.Remove(currentlySelected);
						}
					}
				}
				mUnitGrouping[groupNumber] = mSelectedObjects;
			}
			else
				mUnitGrouping[groupNumber] = new LinkedList<WorldObject>();
			/*foreach (var o in mSelectedObjects)
			{
				if (o.Fraction == Fraction.Player)
				{
					unitGroup.AddFirst(o);
				}
			}
			mUnitGrouping[groupNumber] = unitGroup;*/
		}

		public void SelectGroup(int groupNumber)
		{
			if (groupNumber < mUnitGrouping.Length)
			{
				var newSelection = new LinkedList<WorldObject>();
				foreach (var obj in mUnitGrouping[groupNumber])
				{
					if (obj is Unit && ((Unit) obj).IsDead)
						continue;
					newSelection.AddLast(obj);
				}

				mUnitGrouping[groupNumber] = newSelection;
				SelectedObjects = newSelection;
			}
			
		}

		private void ResetButtons()
		{
			mAttackButton = false;
			mAttackMoveButton = false;
			mMoveButton = false;
			mPatrolButton = false;
		}

		public bool AttackButton { get{return mAttackButton; }
			set
			{
				ResetButtons();
				mAttackButton = value;
			}
		}
		public bool MoveButton { get { return mMoveButton; }
			set
			{
				ResetButtons();
				mMoveButton = value;
			} 
		}
		public bool AttackMoveButton { get { return mAttackMoveButton; }
			set
			{
				ResetButtons();
				mAttackMoveButton = value;
			} 
		}
		public bool PatrolButton { get { return mPatrolButton; }
			set
			{
				ResetButtons();
				mPatrolButton = value;
			} 
		}
	}
}
