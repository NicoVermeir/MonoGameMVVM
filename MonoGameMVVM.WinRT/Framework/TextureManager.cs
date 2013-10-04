using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM
{
    public static class TextureManager
    {
        private readonly static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public static Texture2D LoadTexture(this Game game, string name)
        {
            if (Textures.ContainsKey(name))
                return Textures[name];

            try
            {
                Textures[name] = game.Content.Load<Texture2D>(name);
            }
            catch
            {
                Textures[name] = null;
            }

            return Textures[name];
        }
    }
}
