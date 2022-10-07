/**
 * Author: David Spisla 
 * 
 * Concrete Class for a LoadComponent
 * Usage: Use thsi class for the loading process in a menu 
 * Missing: nothing 
 * 
 **/

using System;
using System.Collections.Generic;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	class LoadComponent : Screen.Screen
	{


		private readonly List<String> mMListForLoad;
		private List<PseudoTextBox> mMBoxesForLabels; 
		private Rectangle mMRectangleStart;
		private Rectangle mMRectangleBoxes;
		private String mStringIsEmpty;

		public LoadComponent(List<String> listForLoad, Rectangle startPosition, Rectangle boxes)
		{
			mMListForLoad = listForLoad;
			mMRectangleStart = startPosition;
			mMRectangleBoxes = boxes;
			Initialize();
		}



		public override sealed void Initialize()
		{
			InitializeAbstr();
			mMBoxesForLabels = new List<PseudoTextBox>();
			int i = 0;
			foreach (String s in mMListForLoad)
			{

				//Adding all strings to the List
				MenuLabel temp = new MenuLabel(MenuItem.MenuIdentifier.Label, ScaledRectangle(mMRectangleStart.X, mMRectangleStart.Y, mMRectangleStart.Width, mMRectangleStart.Height), s);
				mMRectangleStart.Y += 40;

				//Create a Box for each string
				
				mMBoxesForLabels.Add(new PseudoTextBox(TextBoxMode.LoadingOrSave, temp, ScaledRectangle(mMRectangleBoxes.X, mMRectangleBoxes.Y, mMRectangleBoxes.Width, mMRectangleBoxes.Height), i));
				mMRectangleBoxes.Y += 40;
				i++;
				
			}
			mMBoxesForLabels[0].Editmode = true;

		}

		public override void Update()
		{
			base.Update();
			//MouseSliding for the Textboxes
			foreach (PseudoTextBox box in mMBoxesForLabels)
			{
				box.TestMouseSlideOver();
			}


		}

		public int TestLoadSaveMode()
		{
			int index = -1;
			foreach (PseudoTextBox box in mMBoxesForLabels)
			{
				bool isClickedBox = box.TestEditMode();
				if (isClickedBox)
				{
					index = box.Index;
					mStringIsEmpty = box.StartString.Text;
					PseudoTextBox temp = box;
					mMBoxesForLabels.Remove(box);
					foreach (PseudoTextBox b in mMBoxesForLabels)
					{
						b.Editmode = false;
					}
					mMBoxesForLabels.Add(temp);
					break;
				}
			}
			
			return index;
		}


		public override void Draw()
		{
			foreach (PseudoTextBox box in mMBoxesForLabels)
			{
				box.DrawLoadMode();
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.DrawString(box.StartString.GetFont(), box.StartString.Text, new Vector2(box.StartString.Rectangle.X, box.StartString.Rectangle.Y), Color.AliceBlue,
				0f, new Vector2(0, 0), 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);
				mSpriteBatch.End();
				
			}
		}

		public List<PseudoTextBox> BoxesForLabels { get { return mMBoxesForLabels; } }
		public String StringIsEmpty { get { return mStringIsEmpty; } } 
	}
}
