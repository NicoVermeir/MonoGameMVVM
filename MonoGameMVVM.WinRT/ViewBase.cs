using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM
{
    public abstract class ViewBase : IView
    {
        public bool IsActiveScreen { get; set; }
        public ViewModelBase ViewModel { get; set; }
        public RootElement LayoutRoot { get; set; }
        public Game Game { get; set; }
        public Microsoft.Xna.Framework.Color GraphicsDeviceClearColor { get; set; }
        public IList<DrawableGameComponent> Components { get; set; }

        private SpriteBatchAdapter _spriteBatchAdapter;
        private SpriteBatch _spriteBatch;

        protected ViewBase(ViewModelBase viewModel, Game game)
        {
            ViewModel = viewModel;
            Game = game;
            GraphicsDeviceClearColor = Microsoft.Xna.Framework.Color.CornflowerBlue;
            Components = new List<DrawableGameComponent>();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            LoadContent();
            BuildPage();
        }

        protected void LoadContent()
        {
            _spriteBatchAdapter = new SpriteBatchAdapter(_spriteBatch);
            IPrimitivesService primitivesService = new PrimitivesService(Game.GraphicsDevice);
            IRenderer renderer = new Renderer(_spriteBatchAdapter, primitivesService);

            LayoutRoot = new RootElement(Game.GraphicsDevice.Viewport.ToRect(), renderer, new InputManager());
        }


        /// <summary>
        /// Build the page layout in this method
        /// </summary>
        public abstract void BuildPage();

        public virtual void Update(GameTime gameTime)
        {
            ViewModel.Update(gameTime);
            LayoutRoot.Update();
        }

        public virtual void Draw(GameTime gameTime)
        {
            LayoutRoot.Draw();

            foreach (var component in Components)
            {
                component.Draw(gameTime);
            }
        }
    }
}
