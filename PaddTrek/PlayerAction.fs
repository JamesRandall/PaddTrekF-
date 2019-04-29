module PaddTrek.PlayerAction
open PaddTrek.Game


type Action =
    | MoveSector of Geography.Coordinate
    | MoveQuadrant
    | ShortRangeScanner
    | LongRangeScanner
    | EnergyLevels
    
type ActionValidation = {
    isValid: bool
    message: string
}

let validate game action =
    let success = {
        isValid = true
        message = ""
    }
    
    let fail message = {
        isValid = false
        message = message
    }
    
    let validateMoveSector coords =
        let player = Game.getPlayer game
        let isEmptySpace = coords |> Geography.createGalacticCoordinate player.attributes.position.quadrant
                                  |> Map.findWithSectorCoordinate game.objects
                                  |> Game.isEmptySpace
        match isEmptySpace with
            | false -> fail "Cannot move there"
            | true -> match player.energy.value >= Player.energyToMovePlayerToSector player coords with
                        | false -> fail "Insufficient energy to move there"
                        | true -> success
    
    match action with
        | MoveSector coords -> coords |> validateMoveSector
        | _ -> success

let execute game action =
    let replacePlayer newPlayer =
        { game with Game.objects = game.objects |> Seq.map (function | Game.Player _ -> Game.Player(newPlayer) | other -> other) |> Seq.toList }
    let player = Game.getPlayer game
    
    match action with
        | MoveSector coords -> (replacePlayer (Player.moveToSector player coords), true)
        | _ -> (game, false)
