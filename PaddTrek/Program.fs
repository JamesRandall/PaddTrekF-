open PaddTrek
open PaddTrek.Game
open PaddTrek.GameBuilder
open PaddTrek.ConsoleInput

[<EntryPoint>]
let main argv =
    let confirmQuit() =
        Rendering.renderMessage "Press y to confirm you want to quit the game"
        System.Console.ReadKey().Key = System.ConsoleKey.Y

    let rec gameLoop gameState =
        let command = acceptInput gameState
        let newGameState = match command with
                            | ConsoleCommand.Quit -> { gameState with gameOver = confirmQuit() }
                            | ConsoleCommand.Help -> Rendering.renderHelp() ; gameState;
                            | Command cmd -> CommandPipeline.processGameAction gameState cmd
                            | _ -> gameState
                            
        match newGameState.gameOver with
            | true -> gameState
            | false -> gameLoop newGameState

    let newGame = createGame
    Rendering.renderWelcomeMessage ()
    Rendering.renderShortRangeScanner newGame
    (gameLoop newGame).score
