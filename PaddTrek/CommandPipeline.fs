module PaddTrek.CommandPipeline

type CommandPipelineResult = {
    game: Game.Game
    code: string
    args: seq<string>
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}
    
let processCommand game args command =
    let confirmQuit() =
        Rendering.renderMessage "Press y to confirm you want to quit the game"
        System.Console.ReadKey().Key = System.ConsoleKey.Y

    let continueWith command = command

    let continueWithAiAction command = { command with aiActionRequired = true }
    
    let stopWith command = { command with continueToProcess = false }
    
    let gameOverWith command = { command with continueToProcess = false ; game = { command.game with gameOver = true } }
            
    let handleAiActionIfRequired command =
        match command.aiActionRequired with
            | true -> 
                { command with game = Ai.turn game }
            | _ -> continueWith command
    
    let validateCommand command =
        let isValidCoordinateArg (arg:string) (size:Geography.Size) =
            match System.Int32.TryParse arg with
            | (true, number) -> number >=0 && number < size.width
            | (false, _)  -> false
    
        match command.code with
            | "M" -> if Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg command.game.size.sectorSize)) true command.args then continueWith command else stopWith command
            | "S" | "L" | "Q" -> continueWith command
            | _ -> stopWith command
    
    let executeCommand command =
        match command.code with
        | "Q" -> match confirmQuit() with
                    | true -> gameOverWith command
                    | false -> continueWith command
        | "M" -> continueWithAiAction command
        | _ -> continueWith command
    
    let renderCommand command =
        if command.continueToProcess then Rendering.renderCommand command.game command.args command.code
        continueWith command

    let command =
        {
            game = game
            code = command
            args = args
            output = ""
            aiActionRequired = false
            continueToProcess = true
        }
    command |> validateCommand |> executeCommand |> renderCommand |> handleAiActionIfRequired 

