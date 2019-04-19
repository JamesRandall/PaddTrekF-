open PaddTrek
open PaddTrek.Game
open PaddTrek.GameBuilder
open System

type ConsoleCommand =
    | Quit
    | Unknown

type SystemCommand =
    | Command of Game.GameAction
    | ConsoleCommand of ConsoleCommand

[<EntryPoint>]
let main argv =
    let game = createGame

    let acceptInput () =
        let readInput () =
            Rendering.renderWaitingForInput ()
            let inputLine = Console.ReadLine ()
            match inputLine with
                | "" -> "", Array.empty<string>
                | _ -> (inputLine.Substring (0,1)).ToUpper(),inputLine.Substring(1).Trim().Split(' ')
        let commandString, args = readInput ()
        let command = match commandString with
            | "M" -> Command(GameAction.MoveSector)
            | "G" -> Command(GameAction.MoveQuadrant)
            | "S" -> Command(GameAction.ShortRangeScanner)
            | "L" -> Command(GameAction.LongRangeScanner)
            | "Q" -> ConsoleCommand(ConsoleCommand.Quit)
            | _ -> ConsoleCommand(ConsoleCommand.Unknown)
        Rendering.renderInputComplete ()
        command, args  
        
    let confirmQuit() =
        Rendering.renderMessage "Press y to confirm you want to quit the game"
        System.Console.ReadKey().Key = System.ConsoleKey.Y

    let processConsoleCommand game cmd =
        match cmd with
            | ConsoleCommand.Quit -> { game with gameOver = confirmQuit() }
            | _ -> game

    let rec gameLoop gameState =
        let command, args = acceptInput ()
        let newGameState = match command with
                            | Command cmd -> CommandPipeline.processGameAction game args cmd
                            | ConsoleCommand ccmd -> processConsoleCommand game ccmd
        
        match newGameState.gameOver with
            | true -> gameState
            | false -> gameLoop newGameState

    Rendering.renderWelcomeMessage ()
    Rendering.renderShortRangeScanner game
    (gameLoop game).score
