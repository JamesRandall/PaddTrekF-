open PaddTrek.Game
open PaddTrek
open PaddTrek.Game.GameBuilder
open System
open PaddTrek.Ai

[<EntryPoint>]
let main argv =
    let game = createGame

    let confirmQuit() =
        Rendering.renderMessage "Press y to confirm you want to quit the game"
        Console.ReadKey().Key = ConsoleKey.Y

    let createQuadrantFromGame () =
        Quadrant.createQuadrant game.objects game.player.attributes.position.quadrant game.size

    let acceptInput () =
        let readInput () =
            Rendering.renderWaitingForInput ()
            let inputLine = Console.ReadLine ()
            if inputLine.Length = 0 then
                "", Array.empty<string>
            else
                let command = inputLine.Substring (0,1)
                let args = inputLine.Substring(1).Split(' ')
                command, args
        let command, args = readInput ()
        Rendering.renderInputComplete ()
        command, args

    let aiActionRequired func =
        func ()
        true

    let noAiActionRequired func =
        func ()
        false

    let renderShortRangeScanner () =
        Rendering.renderShortRangeScanner (createQuadrantFromGame ())
    
    let gameLoop () =
        let handleAiActionIfRequired actionRequired =
            if actionRequired then
                aiTurn game
            else
                false
        Rendering.renderWelcomeMessage ()
        renderShortRangeScanner ()

        let mutable quit = false
        while not quit do
            let command, args = acceptInput ()
            let gameOver = (match command with
                                | "Q" -> noAiActionRequired (fun _ -> quit <- confirmQuit())
                                | "S" -> noAiActionRequired (fun _ -> renderShortRangeScanner ())
                                | "?" -> noAiActionRequired (fun _ -> Rendering.renderHelp ())
                                | _ -> noAiActionRequired (fun _ -> Rendering.renderMessage "Sorry I did not understand that command"))
                        |> handleAiActionIfRequired
            
            quit <- quit || gameOver

    gameLoop ()

    0 // return an integer exit code
