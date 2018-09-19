module Snake.Core.Game

open System

type Pos = int * int
type Dir = Up | Down | Left | Right

type Board = {
    dir: Dir
    snake: Pos list
    food: Pos list
    size: int
    score: int
}

type StepResult =
    | Continue of Board
    | Stop of score: int
 
let newHead dir ps =
    match dir, List.head ps with
    | Up, (x, y) -> x, y - 1
    | Down, (x, y) -> x, y + 1
    | Left, (x, y) -> x - 1, y
    | Right, (x, y) -> x + 1, y

let eaten head board =
    List.tryFind (fun f -> head = f) board.food

let random = Random()

let rec newFood size snake =
    let p = random.Next(size), random.Next(size)
    if List.contains p snake 
    then newFood size snake
    else p

let updateFood foodPiece board =
    let food' = 
        newFood board.size board.snake :: board.food
        |> List.filter (fun f -> f <> foodPiece)
    { board with food = food' }

let increaseScore board =
    { board with score = board.score + 1}

let growSnake h board =
    { board with snake = h :: board.snake }

let moveSnake head' board =
    let tail' = board.snake |> List.rev |> List.tail |> List.rev
    { board with snake = head' :: tail' }

let continueGame = Continue

let isCrash board =    
    let borderCrash = 
        let outOfBorder (x, y) = x < 0 || x >= board.size || y < 0 || y >= board.size
        List.exists outOfBorder board.snake
    let autoCrash = 
        (board.snake |> List.distinct |> List.length) < (board.snake |> List.length)   
    borderCrash || autoCrash

let stopOrContinueGame board =
    if isCrash board then
        Stop board.score
    else
        Continue board

let updateDir dir board = {board with dir = dir}

let processStep dir board = 
    let head' = newHead dir board.snake
    match (eaten head' board) with
    | Some foodPiece ->
        board
        |> updateFood foodPiece
        |> increaseScore
        |> growSnake head'
        |> updateDir dir
        |> continueGame
    | None ->
        board
        |> moveSnake head'
        |> updateDir dir
        |> stopOrContinueGame

let newBoard size foodSize = 
    let snake = [for i in (size-4) ..(size-1) -> (size / 2, i)]
    {
        dir = Up
        snake = snake
        food = List.init foodSize (fun _ -> newFood size snake)
        size = size
        score = 0
    }

let processTurn turn board =
    let canTurn =
        match turn with
        | Up | Down -> board.dir = Right || board.dir = Left
        | Right | Left -> board.dir = Up || board.dir = Down
    if canTurn then 
        processStep turn board
    else 
        Continue board

let processTick board =
    processStep board.dir board