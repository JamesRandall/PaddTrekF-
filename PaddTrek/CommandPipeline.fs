module PaddTrek.CommandPipeline

type PipelineCommand = {
    game: Game.Game
    action: PlayerAction.Action
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}

let private createPipelineCommand game action =
    {
        game = game
        action = action
        output = ""
        aiActionRequired = false
        continueToProcess = true
    }
    
let private continueWith command = command

let private continueWithNewGameState command (newGameState, aiActionRequired) = { command with aiActionRequired = aiActionRequired; game = newGameState }
    
let private stopWith message command = { command with continueToProcess = false ; output = message }

let private handleAiActionIfRequired command =
        match command.aiActionRequired with
            | true ->
                let updatedGame, aiMessages = Ai.turn command.game
                aiMessages |> Rendering.renderMessages
                { command with game = updatedGame }
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
    let command = createPipelineCommand game action
    let commandResult = command |> validateCommand |> executeCommand |> renderCommand |> handleAiActionIfRequired
    commandResult.game

