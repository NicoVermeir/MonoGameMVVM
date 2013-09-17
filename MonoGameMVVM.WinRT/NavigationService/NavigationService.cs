using System;
using System.Linq;

namespace MonoGameMVVM
{
    public class NavigationService : INavigationService
    {
        private readonly IScreenManager _screenManager;

        public NavigationService(IScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public bool Navigate(Type target)
        {
            try
            {
                var screen = _screenManager.Screens.FirstOrDefault(s => s.GetType() == target);

                //disable all active screens
                var activeScreens = _screenManager.Screens.Where(s => s.IsActiveScreen).ToList();
                if (activeScreens.Any())
                {
                    foreach (var activeScreen in activeScreens)
                    {
                        activeScreen.IsActiveScreen = false;
                    }
                }

                //navigate to requested screen
                if (screen != null)
                {
                    screen.IsActiveScreen = true;
                }
                else
                {
                    IView newScreen = (IView)Activator.CreateInstance(target);
                    newScreen.IsActiveScreen = true;

                    _screenManager.Screens.Add(newScreen);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}