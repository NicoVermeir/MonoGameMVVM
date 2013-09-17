using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM
{
    public class ScreenManager : DrawableGameComponent, IScreenManager
    {
        public List<IView> Screens { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        public int OrderDraw
        {
            get
            {
                return DrawOrder;
            }
            set
            {
                DrawOrder = value;
            }
        }

        public ScreenManager(Game game)
            : base(game)
        {
            game.Components.Add(this);
            Screens = new List<IView>();

            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            var activeScreens = Screens.Where(s => s.IsActiveScreen).ToList();

            if (activeScreens.Any())
            {
                foreach (var activeScreen in activeScreens)
                {
                    activeScreen.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var activeScreens = Screens.Where(s => s.IsActiveScreen).ToList();

            if (activeScreens.Any())
            {
                foreach (var activeScreen in activeScreens)
                {
                    activeScreen.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }
    }
}