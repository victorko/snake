
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
let timer = new Timer(500., AutoReset=true)

let drawBoard board =
    ui.Redraw (
        List.map GreenSquare board.snake @  
        List.map RedCircle board.food)

let startGame () =
    let board = newBoard 20 3
    drawBoard board
    timer.Start()
    GameContinue board

let pauseGame board =
    timer.Stop()
    GamePause board

let  resumeGame board = 
    timer.Start()
    GameContinue board

let dirOfKey =
    function 
    | KeyUp -> Up
    | KeyDown -> Down
    | KeyLeft -> Left
    | KeyRight -> Right
    | _ -> failwith "not direction key"

let handleStepResult stepResult = 
    match stepResult with
    | Continue board -> 
        drawBoard board
        GameContinue board
    | Stop score ->
        ui.Redraw [Text ("SCORE: " + score.ToString())]
        GameStop

let turn board key =
    processTurn (dirOfKey key) board
    |> handleStepResult

let tick board = 
    processTick board
    |> handleStepResult

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
    |> Event.map KeyEvent
    |> Event.merge (
        timer.Elapsed 
        |> Event.map (fun _ -> TickEvent))
    |> Event.scan gameCycle GameStop
    |> ignore 
    
    ui.Redraw([Text "CONTROL KEYS:\n<Space> - start/pause\n<I J K L> - turns "])
    ui.Start()    
    0
