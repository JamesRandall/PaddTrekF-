open PaddTrek
open PaddTrek.Game
open PaddTrek.GameBuilder
open System

[<EntryPoint>]
let main argv =
    let game = createGame

    let acceptInput () =
        let readInput () =
            Rendering.renderWaitingForInput ()
            let inputLine = Console.ReadLine ()
            match inputLine with
                | "" -> "", Array.empty<string>
                | _ -> (inputLine.Substring (0,1)).ToUpper(),inputLine.Substring(1).Split(' ')
        let command, args = readInput ()
        Rendering.renderInputComplete ()
        command, args    

    let rec gameLoop gameState =
        let command, args = acceptInput ()
        let commandResult = CommandPipeline.processCommand game args command
        match commandResult.game.gameOver with
            | true -> gameState
            | false -> gameLoop commandResult.game

    Rendering.renderWelcomeMessage ()
    Rendering.renderShortRangeScanner game
    (gameLoop game).score
