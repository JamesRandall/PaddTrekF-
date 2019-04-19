module PaddTrek.Geography
open System.Collections.Generic

type Size = {
    width: int
    height: int
}

type Coordinate = {
    x: int
    y: int
}

type GalacticCoordinate = {
    quadrant: Coordinate
    sector: Coordinate
}

type WorldSize = {
    quadrantSize: Size
    sectorSize: Size
}

let randomGalacticPosition worldSize =
    {
        quadrant = {
            x = Random.upto (worldSize.quadrantSize.width - 1)
            y = Random.upto (worldSize.quadrantSize.height - 1)
        }
        sector = {
            x = Random.upto (worldSize.sectorSize.width - 1)
            y = Random.upto (worldSize.sectorSize.height - 1)
        }
    }

let nonClashingRandomGalacticPosition (existingCoordinates:ISet<GalacticCoordinate>, worldSize:WorldSize) =
    let rec getNewPosition() =
        let candidatePosition = randomGalacticPosition worldSize
        if existingCoordinates.Contains (candidatePosition) then
            getNewPosition()
        else
            candidatePosition

    getNewPosition ()

let createCoordinateWithStrings args = 
    let intArgs = args |> Seq.map (fun arg -> System.Int32.Parse(arg)) |> Seq.toArray
    {
        x = intArgs.[0]
        y = intArgs.[1]
    }

let createGalacticCoordinate quadrantCoord sectorCoord =
    {
        quadrant = quadrantCoord
        sector = sectorCoord
    }

let distanceBetweenCoordinates coord1 coord2 =
    let width = abs(coord1.x - coord2.x)
    let height = abs(coord1.y - coord2.y)
    let distance = ceil(sqrt(float(width*width + height*height)))
    int(distance)