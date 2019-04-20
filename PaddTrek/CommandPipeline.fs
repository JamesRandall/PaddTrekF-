module PaddTrek.CommandPipeline
open PaddTrek.Game

type PipelineCommand = {
    game: Game.Game
    action: PlayerAction.Action
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}
    
(*
    not yet sure I actually like this approach here - of piping validation ahead of execution - but I wanted to
    experimented with some language features
*)
let processGameAction game action =
    let continueWith command = command

    // implies an AI action is required
    let continueWithNewGameState command newGameState = { command with aiActionRequired = true; game = newGameState }
    
    let stopWith message command = { command with continueToProcess = false ; output = message }

    let player = game |> getPlayer
    
    let handleAiActionIfRequired command =
        match command.aiActionRequired with
            | true -> 
                { command with game = Ai.turn game }
            | _ -> continueWith command
    
    let validateCommand command =
        let validationResult = PlayerAction.validate command.game command.action
        match validationResult.isValid with
            | true -> command |> continueWith
            | false -> command |> stopWith validationResult.message
    
    let executeCommand command =
        command.action |> PlayerAction.execute command.game |> continueWithNewGameState command
    
    let renderCommand command =
        if not command.continueToProcess then
            Rendering.renderError command.output
        else 
            Rendering.renderCommand command.game command.action
        continueWith command

    let command =
        {
            game = game
            action = action
            output = ""
            aiActionRequired = false
            continueToProcess = true
        }
    let commandResult = command |> validateCommand |> executeCommand |> renderCommand |> handleAiActionIfRequired
    commandResult.game

