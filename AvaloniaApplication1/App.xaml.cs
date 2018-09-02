using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1
{
    public class App : Application
    {
        public override void Initialize()
        {
            Styles.AddRange(new Avalonia.Themes.Default.DefaultTheme());
            //Styles.AddRange(new Avalonia.Themes.Default.Accents.BaseLight());
        }
    }
}
