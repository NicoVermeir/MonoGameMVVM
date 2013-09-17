using Microsoft.Xna.Framework.Graphics;

namespace MonoGameMVVM.Sample
{
    public class SecondView : ViewBase
    {
        public SecondView()
            : base(ViewModelLocator.Main, ViewModelLocator.Game)
        {
            
        }

        public override void BuildPage()
        {
            var spriteFont = Game.Content.Load<SpriteFont>("MenuDetail");
            var spriteFontAdapter = new SpriteFontAdapter(spriteFont);

            LayoutRoot.Content = new StackPanel
            {
                Background = new SolidColorBrush(Colors.Blue),
                Children =
                {
                    new TextBlock(spriteFontAdapter)
                    {
                        Background = new SolidColorBrush(Colors.Blue),
                        Text = "Second page!"
                    }
                }
            };
        }
    }
}
