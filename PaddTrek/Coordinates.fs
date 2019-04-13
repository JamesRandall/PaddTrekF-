module PaddTrek.Coordinates
open System
open PaddTrek.GameTypes

let randomGalacticPosition gameSize =
    let randomNumberGenerator = Random()
    {
        quadrant = {
            x = randomNumberGenerator.Next(0, gameSize.quadrantSize.width - 1)
            y = randomNumberGenerator.Next(0, gameSize.quadrantSize.height - 1)
        }
        sector = {
            x = randomNumberGenerator.Next(0, gameSize.sectorSize.width - 1)
            y = randomNumberGenerator.Next(0, gameSize.sectorSize.height - 1)
        }
    }
    

let nonClashingRandomGalacticPosition existingCoordinates gameSize =
    let getNewPosition =
        let candidatePosition = randomGalacticPosition gameSize
        if Seq.exists (fun existingPosition -> existingPosition = candidatePosition) existingCoordinates then
            nullGalacticCoordinate
        else
            candidatePosition
    
    let mutable newPosition = nullGalacticCoordinate
    
    while newPosition = nullGalacticCoordinate do
        newPosition <- getNewPosition
        
    newPosition