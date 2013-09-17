using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;

namespace MonoGameMVVM
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public abstract class LocatorBase
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        protected LocatorBase()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        /// <summary>
        /// Register Views like this
        /// SimpleIoc.Default.Register<ViewName>();
        /// </summary>
        public abstract void Initialize(Game game);

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}