using ConversionOfBrutes.Misc;

namespace ConversionOfBrutes.Graphic.Screen
{
	class TestAreaIsClicked
	{
		public static bool Test(InputManager manager, MenuButton button)
		{
			if ((manager.MouseClickPosition().X >= button.GetRectangle().X && manager.MouseClickPosition().X <= button.GetRectangle().X + button.GetRectangle().Width) &&
						(manager.MouseClickPosition().Y >= button.GetRectangle().Y && manager.MouseClickPosition().Y <= button.GetRectangle().Y + button.GetRectangle().Height))
			{
				return true;
			}

			return false;
		}
	}
}
