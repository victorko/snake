module Snake.Game

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
    
let random = Random()

let rec newFood size snake =
    let p = random.Next(size), random.Next(size)
    if List.contains p snake 
    then newFood size snake
    else p
    
let createBoard size foodNum =
    let snake = [for i in 1..3 -> size / 2, size - i]   
    {
        dir = Up
        snake = snake
        food = List.init foodNum (fun _ -> newFood size snake)
        size = size
        score = 0
    }
    
let getNewHead dir ps =
    ps
    |> List.head
    |> match dir with
        | Up -> fun (x, y) -> x, y - 1
        | Down -> fun (x, y) -> x, y + 1
        | Left -> fun (x, y) -> x - 1, y
        | Right -> fun (x, y) -> x + 1, y

let getEaten dir snake food =
    let head' = getNewHead dir snake
    List.tryFind (fun f -> head' = f) food    

let moveSnake dir eaten snake =
    match eaten with
    | Some f ->
        f :: snake
    | None ->
        let head' = getNewHead dir snake
        let tail' = snake |> List.rev |> List.tail |> List.rev
        head' :: tail'

let updateFood dir eaten size snake food =
    match eaten with
    | Some f ->
        List.filter (fun f' -> f' <> f) (newFood size snake :: food)             
    | None ->
        let head' = getNewHead dir snake
        let tail' = snake |> List.rev |> List.tail |> List.rev
        head' :: tail'

let updateScore eaten score = 
    match eaten with
    | Some _ -> score + 1            
    | None -> score

let isCrash size ps =
    let autoCrash p = List.sumBy (fun p' -> if p' = p then 1 else 0) ps > 1
    let borderCrach (x, y) = x < 0 || x >= size || y < 0 || y >= size 
    List.exists (fun p -> autoCrash p || borderCrach p) ps

let processNewDir dir board =
    let eaten = getEaten dir board.snake board.food
    let snake' = moveSnake dir eaten board.snake
    if isCrash board.size snake' then 
        Stop board.score
    else 
        Continue {
            dir = dir
            snake = snake'
            food = updateFood dir eaten board.size snake' board.food
            size = board.size
            score = updateScore eaten board.score
        }
    
let processTurn turn board =
    let canTurn =
        match turn with
        | Up | Down -> board.dir = Right || board.dir = Left
        | Right | Left -> board.dir = Up || board.dir = Down
    if canTurn then 
        processNewDir turn board
    else 
        Continue board

let processTick board =
    processNewDir board.dir board