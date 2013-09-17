using Microsoft.Xna.Framework;
using MonoGameMVVM.WP8.Sample.View;
using MonoGameMVVM.WP8.Sample.ViewModel;

namespace MonoGameMVVM.WP8.Sample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialize the MVVM framework
        /// The ViewModelLocator will register all ViewModels and Services in an IOC
        /// allowing injection of all sorts
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            //MVVM setup
            ViewModelLocator locator = new ViewModelLocator();
            locator.Initialize(this);

            //navigate to first page
            var navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            navigationService.Navigate(typeof(MainView));

            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}

