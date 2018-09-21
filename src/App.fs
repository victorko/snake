module Snake.Browser

open System
open Fable.Core.JsInterop
open Fable.Import
open Snake.Driver

let padding, cellSize, size = 2., 20., 20.

let width = 3. * padding + (cellSize + padding) * float size
let height = 3. * padding + (cellSize + padding) * float size

let drawShape (ctx: Browser.CanvasRenderingContext2D) =
    let coord a = padding + (cellSize + padding) * float a
    function
    | GreenSquare (x, y) ->
        ctx.fillStyle <- !^"green"
        ctx.fillRect (coord x, coord y, cellSize, cellSize)
    | RedCircle (x, y) ->
        let centerCoord a = coord a + cellSize / 2.
        ctx.beginPath ()
        ctx.arc (centerCoord x, centerCoord y, cellSize / 2., 0., Math.PI*2., false)
        ctx.fillStyle <- !^"red"
        ctx.fill ()
        ctx.closePath ()
    | Text text ->
        ctx.font <- "24px serif"
        let metrics = ctx.measureText text
        let x, y = (width - metrics.width) / 2., (height - 24.) / 2.
        ctx.fillStyle <- !^"black"
        ctx.fillText (text, x, y)

let eventFromKey =
    function
    | 32. -> Some KeySpace
    | 37. -> Some KeyLeft
    | 38. -> Some KeyUp
    | 39. -> Some KeyRight
    | 40. -> Some KeyDown
    | _ -> None

let canvas = Browser.document.getElementsByTagName_canvas().[0]
canvas.width <- width
canvas.height <- height
let ctx = canvas.getContext_2d()

let redraw shapes =
    ctx.clearRect (0., 0., width, height)
    List.iter (drawShape ctx) shapes

let createTimer interval onTimer =
    let mutable refreshIntervalId = 0.
    { 
        new ITimer with
        member this.Start () = refreshIntervalId <- Browser.window.setInterval (onTimer, interval)
        member this.Stop () = Browser.window.clearInterval(refreshIntervalId)
    }

let init() =

    let mutable state = GameStop

    let rec onTimer () = state <- gameCycle timer redraw state TickEvent
    and timer = createTimer 500 onTimer

    let onKey keyCode =
        match eventFromKey keyCode with
        | Some evt -> state <- gameCycle timer redraw state (KeyEvent evt)
        | None -> ()
    Browser.document.addEventListener_keydown (fun e -> onKey e.keyCode)

    drawShape ctx (Text "<Space> - start/pause  ←↑↓→ - turns")

init ()