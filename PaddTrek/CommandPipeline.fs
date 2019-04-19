module PaddTrek.CommandPipeline
open PaddTrek.Game

type PipelineCommand = {
    game: Game.Game
    action: Game.GameAction
    args: seq<string>
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}
    
let processGameAction game args action =
    let continueWith command = command

    let continueWithAiAction command = { command with aiActionRequired = true }
    
    let stopWith command = { command with continueToProcess = false }
    
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
    
        match command.action with
            | GameAction.MoveSector -> if Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg command.game.size.sectorSize)) true command.args then continueWith command else stopWith command
            | GameAction.ShortRangeScanner | GameAction.LongRangeScanner -> continueWith command
            | _ -> stopWith command
    
    let executeCommand command =
        match command.action with
        | GameAction.MoveSector -> continueWithAiAction command
        | _ -> continueWith command
    
    let renderCommand command =
        if command.continueToProcess then Rendering.renderCommand command.game command.args command.action
        continueWith command

    let command =
        {
            game = game
            action = action
            args = args
            output = ""
            aiActionRequired = false
            continueToProcess = true
        }
    let commandResult = command |> validateCommand |> executeCommand |> renderCommand |> handleAiActionIfRequired
    commandResult.game

