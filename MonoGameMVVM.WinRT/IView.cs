using Microsoft.Xna.Framework;

namespace MonoGameMVVM
{
    public interface IView
    {
        bool IsActiveScreen { get; set; }
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}