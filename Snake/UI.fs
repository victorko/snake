module Snake.UI

open Avalonia
open Avalonia.Controls
open Avalonia.Media
open Avalonia.Themes.Default

type SnakeCanvas() as this =
    inherit Canvas(
        Width=568.,
        Height=568.
    )

    let mutable squares = [
        (8., 8.)
    ]

    do this.Tapped.Add (fun args -> 
        let len = squares |> List.length |> float
        squares <- squares @ (squares |> List.map (fun (x, y) -> (x + 14.* len, y + 14. * len))) 
        this.InvalidateVisual())

    override this.Render(context) =
        base.Render(context)

        let borderPen = Pen(Colors.LightGray.ToUint32(), 2.)
        let squareBrush = SolidColorBrush(Colors.Green)

        context.DrawRectangle(borderPen, Rect(4., 4., 560., 560.))

        let drawSquare (x, y) = context.FillRectangle(squareBrush, Rect(x, y, 10., 10.))
        List.iter drawSquare squares

type App() =
    inherit Application()
    override this.Initialize() = 
        this.Styles.AddRange(DefaultTheme())   

type SnakeUI() = 
    
    let mutable appBuilder: AppBuilder = null
    let mutable window: Window = null

    do 
        appBuilder <- AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .SetupWithoutStarting()

        window <- Window(
            Content=SnakeCanvas(),
            SizeToContent=SizeToContent.WidthAndHeight
        )

    member this.Start() =
        window.Show()
        appBuilder.Instance.Run(window)

    member this.Keys = 
        window.KeyDown
        |> Observable.map (fun args -> args.Key)

    //member this.Redraw 
        