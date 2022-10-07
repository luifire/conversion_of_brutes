/**
 * Author: David Luibrand, David Spisla
 * 
 * ScreenManager Class
 * Usage: The Screenmanager is handling all screens of the game. The class
 * will be initialized in the Main.cs
 * Missing: nothing
 **/

using System.Collections.Generic;

namespace ConversionOfBrutes.Graphic.Screen
{
	public sealed class ScreenManager
	{
		private int mFirstScreenDrawed;
		private int mLastScreenUpdated;
		/// <summary>
		/// This is the list of active screens in the game.
		/// </summary>
		private readonly List<Screen> mScreens = new List<Screen>();

		private static ScreenManager sInstance;

		public ScreenManager(Screen start)
		{
			sInstance = this;
			AddScreen(start);
		}

		/// <summary>
		/// Only update the top of the screen stack.
		/// </summary>
		public void Update()
		{
			int screenCount = mScreens.Count;
			for (int i = mScreens.Count - 1; i >= mLastScreenUpdated; i--)
			{
				mScreens[i].Update();
				// doesn't hurt if we update one to less
				if (screenCount != mScreens.Count)
					break;
			}
		}

		/// <summary>
		/// Draw all visible screens. A screen that is not translucent will stop the iteration of screens.
		/// </summary>
		public void Draw()
		{
			for (int i = mFirstScreenDrawed; i < mScreens.Count; i++)
			{
				mScreens[i].Draw();
			}
		}

		/// <summary>
		/// Add a screen to the top of the stack, if the manager is initialized but the screen isn't initialize it.
		/// Also set it's manager to this.
		/// </summary>
		/// <param name="screen">The screen to add.</param>
		public void AddScreen(Screen screen)
		{
			screen.mManager = this;

			// in list
			mScreens.Add(screen);
			// tell him, that he was added
			screen.ScreenAdded();
			// calc new stuff
			ScreensHaveChanged();
		}

		private void ScreensHaveChanged()
		{
			mFirstScreenDrawed = 0;
			mLastScreenUpdated = 0;
			// iterate backwards to find the lowst drawer and updater
			for (int i = mScreens.Count - 1; i >= 0; i--)
			{
				if (mScreens[i].DrawScreenBelow == false && mFirstScreenDrawed == 0)
					mFirstScreenDrawed = i;

				if (mScreens[i].UpdateScreenBelow == false && mLastScreenUpdated == 0)
					mLastScreenUpdated = i;
			}
		}

		/// <summary>
		/// Remove a screen from the screen stack.
		/// </summary>
		/// <returns>The removed screen.</returns>
		public void RemoveScreen()
		{
			mScreens[mScreens.Count - 1].ScreenRemoved();
			mScreens.RemoveAt(mScreens.Count - 1);
			ScreensHaveChanged();
		}

		public static ScreenManager Instance { get { return sInstance; } }
	}
}
