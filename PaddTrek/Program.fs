open PaddTrek
open PaddTrek.Game
open PaddTrek.GameBuilder
open PaddTrek.ConsoleInput

[<EntryPoint>]
let main argv =
    let confirmQuit() =
        Rendering.renderMessage "Press y to confirm you want to quit the game"
        System.Console.ReadKey().Key = System.ConsoleKey.Y
        
    let playerHasNotQuit command = not(command = ConsoleCommand.Quit && confirmQuit())
    
    let processCommand gameState command =
        match command with
            | ConsoleCommand.Clear -> System.Console.Clear (); gameState
            | ConsoleCommand.Help -> Rendering.renderHelp() ; gameState
            | Command cmd -> CommandPipeline.processGameAction gameState cmd
            | _ -> gameState
    
    let newGame = createGame
    let gameSize = newGame.size
    Rendering.renderWelcomeMessage ()
    Rendering.renderShortRangeScanner newGame 
    
    let commands = seq {
        while true do
            let command = acceptInput gameSize
            yield command
    }
    
    let completedGame =
        commands
        |> Seq.takeWhile playerHasNotQuit
        |> Seq.fold processCommand newGame

    completedGame.score