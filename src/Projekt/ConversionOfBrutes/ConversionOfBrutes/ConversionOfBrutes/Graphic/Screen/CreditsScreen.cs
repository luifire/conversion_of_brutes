/**
 * Author: ???
 * 
 * Concrete Class for the Credits
 * Usage: This Class just shows the developers of the game 
 * Missing: nothing 
 * 
 **/

using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Sound;

namespace ConversionOfBrutes.Graphic.Screen
{
	class CreditsScreen: Screen
	{   
		

		/// <summary>
		/// Basic constructor for the CreditsScreen
		/// </summary>
		public CreditsScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override sealed void Initialize()
		{
			
			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.CreditsScreen, ScaledRectangle(0, 0, 1920, 1080), null);
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(855, 910, 210, 100)));
			mLabels.Add(AddLabel(855, 300, "David Luibrand"));
			mLabels.Add(AddLabel(855, 350, "David Spisla"));
			mLabels.Add(AddLabel(855, 400, "Julian Buerklin"));
			mLabels.Add(AddLabel(855, 450, "Julian Loeffler"));
			mLabels.Add(AddLabel(855, 500, "Pius Meinert"));
			mLabels.Add(AddLabel(855, 650, "Tutor: Ivo Enke"));
			
		
		}

		protected override void HandleSoundOnExit()
		{
			Main.Audio.StopSound(AudioManager.Sound.CreditsMusic);
			Main.Audio.ResumeSound(AudioManager.Sound.MainMenuMusic);
		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();

			TestMouseSlideOver();

			MenuButton button = MouseClickedGetButton();

			if (button != null)
			{
				HandleSoundOnExit();
				mManager.RemoveScreen();
			}	
		}


		public override void Draw()
		{

			DrawButtonsAndBackground();
			DrawListOfLabels(mLabels);
		}
	}
}
