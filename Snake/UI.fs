namespace Snake.UI

open Avalonia
open Avalonia.Controls
open Avalonia.Media
open Avalonia.Themes.Default
open Avalonia.Input

type Key = KeyUp | KeyDown | KeyLeft | KeyRight | KeySpace

type Shape =
    | GreenSquare of int * int
    | RedCircle of int * int
    | Text of string

[<AllowNullLiteral>]
type SnakeCanvas(padding, cellSize, size) =
    inherit Canvas(
        Width = 3. * padding + (cellSize + padding) * float size,
        Height = 3. * padding + (cellSize + padding) * float size
    )

    let mutable shapes = list<Shape>.Empty
    
    member this.Redraw newShapes =
        shapes <- newShapes
        this.InvalidateVisual()

    override this.Render(context) =
        base.Render(context)

        let coord a = padding + (cellSize + padding) * float a

        let borderPen = Pen(Colors.LightGray.ToUint32(), 2.)
        let borderRect = Rect(padding, padding, coord size, coord size)
        context.DrawRectangle(borderPen, borderRect)

        let drawShape =                 
            function
            | GreenSquare (x, y) ->           
                let rect = Rect(coord x, coord y, cellSize, cellSize)
                context.FillRectangle(SolidColorBrush(Colors.Green), rect)
            | RedCircle (x, y) -> 
                let rect = Rect(coord x, coord y, cellSize, cellSize)
                context.FillRectangle(SolidColorBrush(Colors.Red), rect, float32 (cellSize / 2.))
            | Text text ->
                let formattedText = 
                    FormattedText(
                        Text = text
                    )
                context.DrawText(SolidColorBrush(Colors.Black), Point(10., 10.), formattedText)

        List.iter drawShape shapes

type App() =
    inherit Application()
    override this.Initialize() = 
        this.Styles.AddRange(DefaultTheme())   

type SnakeUI(padding, cellSize, size) = 
    
    let mutable appBuilder: AppBuilder = null
    let mutable window: Window = null
    let mutable canvas: SnakeCanvas = null

    do 
        // it's really important to create these objects in such order
        appBuilder <- AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .SetupWithoutStarting()
        canvas <- SnakeCanvas(padding, cellSize, size)
        window <- Window(
            Content=canvas,
            SizeToContent=SizeToContent.WidthAndHeight
        )
        window.KeyBindings.Clear()

    member this.Start() =
        window.Show()
        appBuilder.Instance.Run(window)   

    member this.Keys = 
        window.KeyDown //IJKL
        |> Event.choose (fun args -> 
            match args.Key with
            | Key.Space -> Some KeySpace
            | Key.I -> Some KeyUp
            | Key.K -> Some KeyDown
            | Key.L -> Some KeyRight
            | Key.J -> Some KeyLeft
            | _ -> None)

    member this.Redraw shapes = 
        canvas.Redraw shapes

        