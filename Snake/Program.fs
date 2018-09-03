
module Snake.Program

open UI
open Game
open System.Timers

[<EntryPoint>]
let main argv =

    let ui = SnakeUI(2., 20., 20)
    use timer = new Timer(2000., AutoReset=true)
    
    ui.Keys 
    |> Observable.map (fun key -> key.ToString())
    |> Observable.merge (timer.Elapsed |> Observable.map (fun _ -> "Elapsed"))
    |> Observable.subscribe (fun key -> 
        printfn "%s" key
        ui.Redraw [
            GreenSquare (2,2)
            GreenSquare (2,3)
            GreenSquare (2,4)
            GreenSquare (3,4)

            RedCircle (3,6)
        ])
    |> ignore

    timer.Start()
    ui.Start()
    
    0
