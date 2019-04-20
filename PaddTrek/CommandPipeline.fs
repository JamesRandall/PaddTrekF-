module PaddTrek.CommandPipeline
open PaddTrek.Game
open PaddTrek.Geography;

type PipelineCommand = {
    game: Game.Game
    action: Game.GameAction
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
        

        let validate message isValid =
            match isValid with
                | true -> continueWith command
                | false -> stopWith message command
                
        let validateEnergyRequirements energyRequired cmd =
            cmd |> 
                if cmd.continueToProcess then
                    if player.energy.value > energyRequired then
                        continueWith
                    else
                        stopWith "Insufficient energy to do that"
                else
                    continueWith
                
        let validateMoveSector args =
            (
                args |> createGalacticCoordinate player.attributes.position.quadrant
                     |> Map.findWithSectorCoordinate command.game.objects
                     |> isEmptySpace
            )
            |> validate "Cannot move there"
            |> validateEnergyRequirements (Player.energyToMovePlayerToSector player args)

        match command.action with
            | MoveSector args -> validateMoveSector args
            | GameAction.ShortRangeScanner | GameAction.LongRangeScanner -> continueWith command
            | _ -> stopWith "" command
    
    let executeCommand command =
        match command.action with
        | MoveSector args ->  movePlayerToSector command.game args |> continueWithNewGameState command
        | _ -> continueWith command
    
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

