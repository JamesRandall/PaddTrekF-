module PaddTrek.CommandPipeline
open PaddTrek.Game
open PaddTrek.Geography;

type PipelineCommand = {
    game: Game.Game
    action: Game.GameAction
    args: seq<string>
    output: string
    aiActionRequired: bool
    continueToProcess: bool
}
    
(*
    not yet sure I actually like this approach here - of piping validation ahead of execution - but I wanted to
    experimented with some language features
*)
let processGameAction game args action =
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
        let isValidCoordinateArg (arg:string) (size:Geography.Size) =
            match System.Int32.TryParse arg with
            | (true, number) -> number >=0 && number < size.width
            | (false, _)  -> false

        let validate message isValid =
            match isValid with
                | true -> continueWith command
                | false -> stopWith message command

        let validateMoveSector () =
            // TODO: Validate energy requirements!
            (
                command.args |> Seq.length = 2 &&
                command.args |> Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg command.game.size.sectorSize)) true &&
                command.args |> Geography.createCoordinateWithStrings
                             |> createGalacticCoordinate player.attributes.position.quadrant
                             |> Map.findWithSectorCoordinate command.game.objects
                             |> isEmptySpace
            ) |> validate "Cannot move there"

        match command.action with
            | GameAction.MoveSector -> validateMoveSector ()
            | GameAction.ShortRangeScanner | GameAction.LongRangeScanner -> continueWith command
            | _ -> stopWith "" command
    
    let executeCommand command =
        match command.action with
        | GameAction.MoveSector ->  movePlayerToSector command.game (createCoordinateWithStrings command.args) |> continueWithNewGameState command
        | _ -> continueWith command
    
    let renderCommand command =
        if not command.continueToProcess then
            Rendering.renderError command.output
        else 
            Rendering.renderCommand command.game command.args command.action
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

