module PaddTrek.PlayerAction
open PaddTrek.Game


type Action =
    | MoveSector of Geography.Coordinate
    | MoveQuadrant of Geography.Coordinate * int
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
                        
    let validateMoveQuadrant (coords,warpSpeed) =
        let player = Game.getPlayer game
        match player.energy.value >= Player.energyToMovePlayerToQuadrant player coords warpSpeed with
            | false -> fail "Insufficient energy to move there"
            | true -> success
    
    match action with
        | MoveSector coords -> coords |> validateMoveSector
        | MoveQuadrant (moveQuadrant,warpSpeed) -> validateMoveQuadrant (moveQuadrant,warpSpeed) 
        | _ -> success

let execute game action =
    let replacePlayer newPlayer =
        { game with Game.objects = game.objects |> Seq.map (function | Game.Player _ -> Game.Player(newPlayer) | other -> other) |> Seq.toList }
    let player = Game.getPlayer game
    
    match action with
        | MoveSector coords -> (replacePlayer (Player.moveToSector player coords), true)
        | MoveQuadrant (coords,warpSpeed) -> ((replacePlayer (Player.moveToQuadrant player coords warpSpeed)
                                              |> Game.updateDiscoveredQuadrants), true)
        | _ -> (game, false)
