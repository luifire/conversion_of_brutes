/**
 * Author: Julian Löffler 
 * 
 * Level Editor Class
 * Usage: This Class Implements an Editor to create Maps for the game.  
 * Missing: 
 * - Ability to Delete objects.
 * - Ability to set Gridcells as Blocked (necessary for Pathfinding & collision)
 **/


using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Misc
{
	/// <summary>
	/// Class to Spawn 3D Objects
	/// </summary>
	sealed class LevelEditor
	{
		private readonly GraphicsManager mGraphicsManager;

		// Mouse + Keyboard
		private MouseState mMouse = Mouse.GetState();
		private MouseState mOldMouseState;
		private KeyboardState mKeyboardState = Keyboard.GetState();
		private readonly WorldGridLevelEditor mGridLevelEditor;

		// List of the currently spawned WorldObjects.
		//private readonly ListOfWorldObjects mWorldObjects;
		private ObjectManager mObjectManager;
		private Ident mCurrentObject;

		/// <summary>
		/// SpawnManager constructor
		/// </summary>
		/// <param name="objectManager"></param>
		/// <param name="graphicsManager"></param>
		public LevelEditor(ObjectManager objectManager, GraphicsManager graphicsManager)
		{
			mObjectManager = objectManager;
			mGraphicsManager = graphicsManager;
			mCurrentObject = Ident.Tree;
			mGridLevelEditor = new WorldGridLevelEditor();
		}

		/// <summary>
		/// Update function
		/// </summary>
		public void Update()
		{
			mOldMouseState = mMouse;
			mMouse = Mouse.GetState();
			SetCurrentObject();
			Spawn();
		}
		/// <summary>
		/// Draw function
		/// </summary>
		public void Draw()
		{
			//foreach (var worldObject in mWorldObjects.GetWorldObjects())
			//{
			//	worldObject.Draw();
			//}
		}

		//Calculates the 2D Mouseposition to a 3D position
		private Vector2 Calc3DMousepos(MouseState ms)
		{
			return GameScreen.Camera.CalcWorldPosition(new Vector2(ms.X, ms.Y));
			/*
			var nearScreenPoint = new Vector3(ms.X, ms.Y, 0);
			var farScreenPoint = new Vector3(ms.X, ms.Y, 1);
			var nearWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(nearScreenPoint, GameScreen.Camera.ProjectionMatrix, GameScreen.Camera.ViewMatrix, Matrix.Identity);
			var farWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(farScreenPoint, GameScreen.Camera.ProjectionMatrix, GameScreen.Camera.ViewMatrix, Matrix.Identity);
			var direction = farWorldPoint - nearWorldPoint;
			var zFactor = -nearWorldPoint.Y / direction.Y;
			var zeroWorldPoint = nearWorldPoint + direction * zFactor;

			return new Vector2(zeroWorldPoint.X, zeroWorldPoint.Z);*/
		}

		// Sets the object you wanna Place
		private void SetCurrentObject()
		{
			if (Main.Input.WasButtonPressed(Keys.F1))
			{
				mCurrentObject = Ident.Castle;
			}
			if (Main.Input.WasButtonPressed(Keys.F2))
			{
				mCurrentObject = Ident.Rock;
			}
			if (Main.Input.WasButtonPressed(Keys.F3))
			{
				mCurrentObject = Ident.Tree;
			}
			if (Main.Input.WasButtonPressed(Keys.F4))
			{
				mCurrentObject = Ident.Spawnzone;
			}
			if (Main.Input.WasButtonPressed(Keys.F5))
			{
				mCurrentObject = Ident.Zone;
			}
			if (Main.Input.WasButtonPressed(Keys.F6))
			{
				mCurrentObject = Ident.ShieldGuard;
			}
		}
		/// <summary>
		/// Spawn the object you requested
		/// </summary>
		private void Spawn()
		{
			// Check if the Mouse is inside the Game-window
			if (mMouse.Y > Main.Graphics.PreferredBackBufferHeight || mMouse.Y < 0 || mMouse.X > Main.Graphics.PreferredBackBufferWidth || mMouse.X < 0)
			{
				return;
			}

			if (mMouse.LeftButton != ButtonState.Pressed || mOldMouseState.LeftButton != ButtonState.Released || mGridLevelEditor.CellIsBlocked(mMouse.X, mMouse.Y))
			{
				return;
			}

			WorldObject newObj = GameScreen.ObjectManager.CreateWorldObject(mCurrentObject,
				Calc3DMousepos(mMouse),
				Fraction.Gaia);

			//mWorldObjects.GetWorldObjects().Add(newObj);

			// Set the Grid value to Blocked(true), you can't spawn more than 1 object per X,Y coordinate
			mGridLevelEditor.SetBlocked(mMouse.X, mMouse.Y);
		}
	}
}
