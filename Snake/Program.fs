
module Snake.Program

open Snake.UI
open Snake.Game
open System.Timers

type GameState =
    | GameStop
    | GameContinue of Board
    | GamePause of Board

type GameEvent =
    | KeyEvent of Key
    | TickEvent

let ui = SnakeUI(2., 20., 20)
let timer = new Timer(2000., AutoReset=true)

let startGame () = ...

let pauseGame board = ...

let  resumeGame board = ...

let turn board key = ...

let tick board = ...

let gameCycle state event = 
    match state with    
    | GameStop ->
        match event with 
        | KeyEvent KeySpace -> startGame ()
        | _ -> state
    | GameContinue board ->
        match event with
        | KeyEvent KeySpace -> pauseGame board
        | KeyEvent key -> turn board key
        | TickEvent -> tick board
    | GamePause board ->
        match event with
        | KeyEvent KeySpace -> resumeGame board
        | _ -> state

[<EntryPoint>]
let main argv =   
    ui.Keys 
    |> Observable.map (fun key -> KeyEvent key)
    |> Observable.merge (
        timer.Elapsed 
        |> Observable.map (fun _ -> TickEvent))
    |> Observable.scan gameCycle GameStop
    |> ignore
    
    ui.Start()    
    0
