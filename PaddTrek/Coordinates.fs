module PaddTrek.Coordinates
open System
open PaddTrek.Models
open System.Collections.Generic

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

let nonClashingRandomGalacticPosition (existingCoordinates:ISet<GalacticCoordinate>, gameSize:GameSize) =
    let rec getNewPosition() =
        let candidatePosition = randomGalacticPosition gameSize
        if existingCoordinates.Contains (candidatePosition) then
            getNewPosition()
        else
            candidatePosition

    getNewPosition ()
