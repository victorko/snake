module Snake.Driver

open Snake.Game

type Key = KeyUp | KeyDown | KeyLeft | KeyRight | KeySpace

type Shape =
    | GreenSquare of int * int
    | RedCircle of int * int
    | Text of string

type GameState =
    | GameStop
    | GameContinue of Board
    | GamePause of Board

type GameEvent =
    | KeyEvent of Key
    | TickEvent

type ITimer =
    abstract member Start: unit -> unit
    abstract member Stop: unit -> unit

let gameCycle (timer: ITimer) redraw state event = 

    let drawBoard board =
        redraw (
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
            timer.Stop()
            redraw [Text ("SCORE: " + score.ToString())]
            GameStop

    let turn board key =
        processTurn (dirOfKey key) board
        |> handleStepResult

    let tick board = 
        processTick board
        |> handleStepResult

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