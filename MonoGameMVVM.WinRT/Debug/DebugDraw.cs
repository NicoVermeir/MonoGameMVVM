using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM
{
    public static class DebugDraw
    {
        public static void DrawDebug(this Rectangle rect, GameTime gameTime, Game game)
        {
            DrawDebug(rect, gameTime, game, Microsoft.Xna.Framework.Color.Red);
        }

        public static void DrawDebug(this Rectangle rect, GameTime gameTime, Game game, Microsoft.Xna.Framework.Color color)
        {
            var screenManager = SimpleIoc.Default.GetInstance<IScreenManager>();

            var dummyTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            screenManager.SpriteBatch.Draw(dummyTexture, rect, color);
        }
    }
}
