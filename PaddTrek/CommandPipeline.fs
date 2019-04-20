module PaddTrek.CommandPipeline
open PaddTrek.Game

type PipelineCommand = {
    game: Game.Game
    action: PlayerAction.Action
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}
    
let private continueWith command = command

let private continueWithNewGameState command newGameState = { command with aiActionRequired = true; game = newGameState }
    
let private stopWith message command = { command with continueToProcess = false ; output = message }

let private handleAiActionIfRequired command =
        match command.aiActionRequired with
            | true -> 
                { command with game = Ai.turn command.game }
            | _ -> continueWith command
            
let private validateCommand command =
        let validationResult = PlayerAction.validate command.game command.action
        match validationResult.isValid with
            | true -> command |> continueWith
            | false -> command |> stopWith validationResult.message
    
let private executeCommand command =
    command.action |> PlayerAction.execute command.game |> continueWithNewGameState command

let private renderCommand command =
    if not command.continueToProcess then
        Rendering.renderError command.output
    else 
        Rendering.renderCommand command.game command.action
    continueWith command

let processGameAction game action =
    // implies an AI action is required
    let player = game |> getPlayer
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

