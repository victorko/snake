using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AvaloniaApplication1
{
    public class SnakeCanvas : UserControl
    {
        public SnakeCanvas()
        {
           
        }

        public override void Render(DrawingContext context)
        {
            //base.Render(context);

            context.DrawText(
                new SolidColorBrush(Color.Parse("red")), 
                new Point(20, 20),
                new FormattedText { Text = "HEllo!!11" });
        }
    }
}
