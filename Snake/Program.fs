
module Snake.Program

open UI
open Game
open System.Timers

[<EntryPoint>]
let main argv =

    let ui = SnakeUI()
    use timer = new Timer(2000., AutoReset=true)
    
    ui.Keys 
    |> Observable.map (fun key -> key.ToString())
    |> Observable.merge (timer.Elapsed |> Observable.map (fun _ -> "Elapsed"))
    |> Observable.subscribe (fun key -> printfn "%s" key)
    |> ignore

    timer.Start()
    ui.Start()
    
    0
