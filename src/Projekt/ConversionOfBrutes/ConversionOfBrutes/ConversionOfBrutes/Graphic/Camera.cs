/** 
 * Author: Julian Löffler, Luibrand
 * 
 * Camera Class
 * Usage: This Class Implements the Camera of the Game
 **/

using System;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Graphic
{
    /// <summary>
    /// Camera Class
    /// </summary>
    public sealed class Camera
    {
	    private const float DefaultHeight = 650f;
	    private const float MaxHeight = 1000f;
		private const float MinHeight = 160f;
	    private const int ActualVisibleRectangleOffset = 50;
	    private const int FogOfWarVisibleRectangleOffset = 280;
		
        // basic Camera Stuff
        private Vector3 mPosition;
		private Matrix mViewMatrix;
		private Matrix mProjectionMatrix;

        // Free Camera Stuff
        private float mPitch;
        private float mSpeed;
        private Matrix mCameraRotation;
        private int mPreviousScrollValue;
		private Rectangle mVisibleRectangle;
		private Rectangle mActualVisibleRectangle;
		private Rectangle mFogOfWarVisibleRectangle;
	    private bool mHasMoved = true;
	    private Vector2 mMaxCameraPosition; // always position.* < maxCameraPosition.* 
		private Vector2 mMinCameraPosition; // always position.* < maxCameraPosition.* 


		internal Camera(Vector2 position)
        {
			ResetCamera(position);
        }

		private void ResetCamera(Vector2 position)
        {
            // basic Camera
			mPosition = new Vector3(position.X, DefaultHeight, position.Y);
            mViewMatrix = Matrix.Identity;
			var aspectRatio = Main.Graphics.GraphicsDevice.Viewport.AspectRatio;
            mProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, .5f, 4000f);
			

            // Free camera
            mPitch = -1.0f;
            mSpeed = 15f;
            mCameraRotation = Matrix.Identity;

			UpdateViewMatrix();
			CalcVisibleRectange();
			CalcAreaToMoveIn();
        }

        internal void Update()
        {
            HandleInput();
	        if (mHasMoved)
	        {
				mHasMoved = false;
				UpdateViewMatrix();
		        CalcVisibleRectange();
	        }
        }

        private void MoveCamera(Vector3 addedVector)
        {
	        float speed = mSpeed * mPosition.Y / DefaultHeight;
			Vector3 newPosition = mPosition + speed * addedVector;

	        if (Math.Abs(addedVector.Y) > 0.0001)
		        CalcAreaToMoveIn();

			// not so pretty but whats happening here, is that we go up
			// by also going a little bit backwards, so we have to avoide moving along the seeling
	        if (newPosition.Y > MaxHeight)
	        {
		        newPosition.Y = MaxHeight;
		        return;
	        }
			if (newPosition.Y < MinHeight)
			{
				newPosition.Y = MinHeight;
				return;
			}

			if(newPosition.X < mMinCameraPosition.X)
				newPosition.X = mMinCameraPosition.X;

	        if (newPosition.Z < mMinCameraPosition.Y)
		        newPosition.Z = mMinCameraPosition.Y;

	        if (newPosition.X > mMaxCameraPosition.X)
		        newPosition.X = mMaxCameraPosition.X;
			

	        if (newPosition.Z > mMaxCameraPosition.Y)
		        newPosition.Z = mMaxCameraPosition.Y;
			
			Position = newPosition;
			//GameScreen.Hud.DeveloperInfo = mPosition.ToString() + " - " + mMaxCameraPosition.ToString();
        }

	    private void CalcAreaToMoveIn()
	    {
		    Rectangle mapRect = GameScreen.Map.QuadRect;

		    Vector3 res = GetCameraPositionDependingOnWorldPosition(new Vector2(mapRect.X + mapRect.Width, mapRect.Y + mapRect.Height));
			mMaxCameraPosition = new Vector2(res.X, res.Z);

			res = GetCameraPositionDependingOnWorldPosition(new Vector2(mapRect.X, mapRect.Y));
			mMinCameraPosition = new Vector2(res.X, res.Z);
	    }

        private void HandleInput()
        {
            var keyboardState = Keyboard.GetState();
            var mouse = Mouse.GetState();
	        int scrollFaktor = 3;

			// do nothing while button is pressed
	        if (mouse.LeftButton == ButtonState.Pressed)
		        return;

            // Forward
            if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraUp)) || (mouse.Y <= 0 && mouse.Y >=-10))
            {
				MoveCamera(Matrix.Identity.Forward);
            }
            // Backward
			if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraDown)) || (mouse.Y >= Main.Graphics.PreferredBackBufferHeight - 10 && mouse.Y <= Main.Graphics.PreferredBackBufferHeight + 10))
            {
				MoveCamera(Matrix.Identity.Backward);
            }

            // Left
			if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraLeft)) || (mouse.X <= 0 && mouse.X >= -10))
            {
				MoveCamera(-mCameraRotation.Right);
				
            }

            // Right
			if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraRight)) || mouse.X >= Main.Graphics.PreferredBackBufferWidth - 10 && mouse.X <= Main.Graphics.PreferredBackBufferWidth + 10)
            {
                MoveCamera(mCameraRotation.Right);
            }

            // up by key
			if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraZoomOut)))
            {
                MoveCamera(mCameraRotation.Backward);
            }
			// up by scroll
			if (mouse.ScrollWheelValue < mPreviousScrollValue)
			{
				MoveCamera(mCameraRotation.Backward * scrollFaktor);
			}

            //down
			if (keyboardState.IsKeyDown(Main.HotKey.GetHotkey(HotKeys.HotKey.CameraZoomIn)))
            {
				MoveCamera(-mCameraRotation.Backward);
            }
			if (mouse.ScrollWheelValue > mPreviousScrollValue)
			{
				MoveCamera(-mCameraRotation.Backward * scrollFaktor);
			}

            mPreviousScrollValue = mouse.ScrollWheelValue;

			// goto selection
	        if (Main.Input.WasButtonPressed(Main.HotKey.GetHotkey(HotKeys.HotKey.CenterCamera)))
	        {
				var obj = GameScreen.ObjectManager.SelectionHandler.SelectedObjects.First;

				if(obj != null) 
					GotoWorldPosition(obj.Value.Position);
	        }
        }

        private void UpdateViewMatrix()
        {
			// performance was passiert hier überhaupt?, mCameraRotation ist identiy, forward, up und right sollten normiert sein?
            mCameraRotation.Forward.Normalize();
            mCameraRotation.Up.Normalize();
            mCameraRotation.Right.Normalize();

            mCameraRotation *= Matrix.CreateFromAxisAngle(mCameraRotation.Right, mPitch);
            mCameraRotation *= Matrix.CreateFromAxisAngle(mCameraRotation.Up, 0);
            mCameraRotation *= Matrix.CreateFromAxisAngle(mCameraRotation.Forward, 0);

            mPitch = 0.0f;

            mViewMatrix = Matrix.CreateLookAt(mPosition, mPosition + mCameraRotation.Forward, mCameraRotation.Up);
        }

		// Calculates the 2D Mouseposition to a 3D position
		public Vector2 CalcWorldPosition(Vector2 position)
		{
			var nearScreenPoint = new Vector3(position.X, position.Y, 0);
			var farScreenPoint = new Vector3(position.X, position.Y, 1);

			var nearWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(nearScreenPoint, mProjectionMatrix, mViewMatrix, Matrix.Identity);
			var farWorldPoint = Main.Graphics.GraphicsDevice.Viewport.Unproject(farScreenPoint, mProjectionMatrix, mViewMatrix, Matrix.Identity);
			var direction = farWorldPoint - nearWorldPoint;
			var zFactor = -nearWorldPoint.Y / direction.Y;
			var zeroWorldPoint = nearWorldPoint + direction * zFactor;

			return new Vector2(zeroWorldPoint.X, zeroWorldPoint.Z);
		}

	    public void CalcVisibleRectange()
	    {
			Rectangle rect = GameScreen.VisibleRectangle;

			Vector2 upper = CalcWorldPosition(new Vector2(rect.X, rect.Y));
			// has to be done because of our trapez field of view 
			Vector2 upperRight = CalcWorldPosition(new Vector2(rect.X + rect.Width, rect.Y));
			Vector2 lower = CalcWorldPosition(new Vector2(rect.X + rect.Width, rect.Y + rect.Height));

			mVisibleRectangle = new Rectangle((int)upper.X, (int)upper.Y, (int) (upperRight.X - upper.X), (int) (lower.Y - upper.Y));

			mActualVisibleRectangle = new Rectangle((int)upper.X - ActualVisibleRectangleOffset / 2, (int)upper.Y - ActualVisibleRectangleOffset / 2, 
				(int)(upperRight.X - upper.X) + ActualVisibleRectangleOffset, (int)(lower.Y - upper.Y) + ActualVisibleRectangleOffset);

			mFogOfWarVisibleRectangle = new Rectangle((int)upper.X - FogOfWarVisibleRectangleOffset / 2, (int)upper.Y - FogOfWarVisibleRectangleOffset / 2, 
				(int)(upperRight.X - upper.X) + FogOfWarVisibleRectangleOffset, (int)(lower.Y - upper.Y) + FogOfWarVisibleRectangleOffset);
	    }

		private Vector3 GetCameraPositionDependingOnWorldPosition(Vector2 worldPosition)
		{
			float factor = mPosition.Y / mCameraRotation.Forward.Y;
			return new Vector3(worldPosition.X, 0.0f, worldPosition.Y) + factor * mCameraRotation.Forward;
		}

	    public void GotoWorldPosition(Vector2 worldPosition)
	    {
			Position = GetCameraPositionDependingOnWorldPosition(worldPosition);
	    }

		// VisibleRectangle what is actually drawn
		public Rectangle VisibleRectangle { get { return mVisibleRectangle; } }
		// additional size, so you can see roofs and stuff of objects not in VisibleRectangle
		public Rectangle ActualVisibleRectangle { get { return mActualVisibleRectangle; } }
		public Rectangle FogOfWarVisibleRectangle { get { return mFogOfWarVisibleRectangle; } }
		public Matrix ViewMatrix { get { return mViewMatrix; } }
		public Matrix ProjectionMatrix { get { return mProjectionMatrix; } }

	    public Vector3 Position
	    {
		    set
		    {
				mPosition = value;
			    mHasMoved = true;
		    }
			get { return mPosition;}
	    }
		
    }
}
