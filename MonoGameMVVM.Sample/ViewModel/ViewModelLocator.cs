using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;

namespace MonoGameMVVM.Sample
{
    public class ViewModelLocator : LocatorBase
    {
        public override void Initialize(Game game)
        {
            //Register the game
            SimpleIoc.Default.Register(() => game);

            //Services
            SimpleIoc.Default.Register<IScreenManager, ScreenManager>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();

            //Views
            SimpleIoc.Default.Register<MainView>();

            //ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static Game Game
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Game>();
            }
        }

        public static MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
    }
}
