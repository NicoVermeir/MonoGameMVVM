using Microsoft.Xna.Framework.Graphics;
using MonoGameMVVM.WP8.Sample.ViewModel;

namespace MonoGameMVVM.WP8.Sample.View
{
    public class MainView : ViewBase
    {
        public MainView()
            : base(ViewModelLocator.Main, ViewModelLocator.Game)
        {

        }

        public override void BuildPage()
        {
            var spriteFont = Game.Content.Load<SpriteFont>("MenuDetail");
            var spriteFontAdapter = new SpriteFontAdapter(spriteFont);
            var addButtonImageTexture = new TextureImage(new Texture2DAdapter(Game.Content.Load<Texture2D>("continueButton")));


            //button with command that navigates to SecondView
            Button btn = new Button
            {
                Content = new Image { Source = addButtonImageTexture },
                DataContext = ViewModel
            };
            btn.BindCommand("NextPageCommand");


            //button with command that increments counter on viewmodel with 1
            Button upUp = new Button
            {
                Content = new Image { Source = addButtonImageTexture },
                DataContext = ViewModel
            };
            upUp.BindCommand("CounterUpCommand");


            //bound textblock, shows the counter property on the viewmodel
            TextBlock boundTextBlock = new TextBlock(spriteFontAdapter)
            {
                Background = new SolidColorBrush(Colors.White)
            };
            //<DataContext, propertyType, cast to>(DataContext, Property)
            boundTextBlock.Bind(TextBlock.TextProperty, BindingFactory.CreateOneWay<MainViewModel, int, string>((MainViewModel)ViewModel, vm => vm.Counter));

            LayoutRoot.Content = new StackPanel
            {
                //Background = new SolidColorBrush(Colors.Red),
                Children =
                {
                    boundTextBlock,
                    upUp,
                    new TextBlock(spriteFontAdapter)
                    {
                        Background = new SolidColorBrush(Colors.Blue),
                        Text = "Click the button!",
                        Margin = new Thickness(0, 100, 0, 0)
                    },

                    btn
                }
            };
        }
    }
}
