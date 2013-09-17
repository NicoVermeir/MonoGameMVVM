using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM
{
    public interface IScreenManager : IDrawable
    {
        List<IView> Screens { get; set; }
        SpriteBatch SpriteBatch { get; set; }
        int OrderDraw { get; set; }
    }
}
