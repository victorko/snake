module Snake.Browser

open Fable.Core.JsInterop
open Fable.Import
open Snake.Core.Driver
open System

let padding, cellSize, size = 2., 20., 20.
let width = 3. * padding + (cellSize + padding) * float size
let height = 3. * padding + (cellSize + padding) * float size

let coord a = padding + (cellSize + padding) * float a

let drawShape (ctx: Browser.CanvasRenderingContext2D) =
    function
    | GreenSquare (x, y) ->
        ctx.beginPath ()
        ctx.rect (coord x, coord y, cellSize, cellSize)
        ctx.fillStyle <- !^"green"
        ctx.fill ()
        ctx.closePath ()
    | RedCircle (x, y) ->
        ctx.beginPath ()
        ctx.arc (coord x + cellSize / 2., coord y + cellSize / 2., cellSize / 2., 0., Math.PI*2., false)
        ctx.fillStyle <- !^"red"
        ctx.fill ()
        ctx.closePath ()
    | Text text ->
        ctx.font <- "24px serif"
        let metrics = ctx.measureText text
        let x, y = (width - metrics.width) / 2., (height - 24.) / 2.
        ctx.fillStyle <- !^"black"
        ctx.fillText (text, x, y)

let htmlCanvas = Browser.document.getElementsByTagName_canvas().[0]
htmlCanvas.width <- width
htmlCanvas.height <- height

let ctx = htmlCanvas.getContext_2d()

let canvas = 
    {
        new ICanvas with
        member this.Redraw shapes =
            ctx.clearRect(0., 0., width, height);
            List.iter (drawShape ctx) shapes
    }

let eventFromKey =
    function
    | 32. -> Some KeySpace
    | 37. -> Some KeyLeft
    | 38. -> Some KeyUp
    | 39. -> Some KeyRight
    | 40. -> Some KeyDown
    | _ -> None

let mutable state = GameStop
let mutable refreshIntervalId = 0.

let rec timer = 
    let onTimer () = 
        state <- gameCycle timer canvas state TickEvent
    { 
        new ITimer with
        member this.Start () = refreshIntervalId <- Browser.window.setInterval (onTimer, 500)
        member this.Stop () = Browser.window.clearInterval(refreshIntervalId)
    }

let onKey keyCode =
    match eventFromKey keyCode with
    | Some evt -> state <- gameCycle timer canvas state (KeyEvent evt)
    | None -> ()

Browser.document.addEventListener_keydown (fun e -> onKey e.keyCode)

drawShape ctx (Text "<Space> - start/pause  ←↑↓→ - turns")