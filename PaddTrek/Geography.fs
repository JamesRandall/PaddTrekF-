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

let createCoordinate x y = {
    x = x
    y = y
}

let randomGalacticPosition worldSize =
    {
        quadrant = createCoordinate (Random.upto (worldSize.quadrantSize.width - 1)) (Random.upto (worldSize.quadrantSize.height - 1))
        sector = createCoordinate (Random.upto (worldSize.sectorSize.width - 1)) (Random.upto (worldSize.sectorSize.height - 1))
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
    createCoordinate intArgs.[0] intArgs.[1]

let createGalacticCoordinate quadrantCoord sectorCoord =
    {
        quadrant = quadrantCoord
        sector = sectorCoord
    }

let distanceBetweenCoordinates coord1 coord2 =
    let width = (max coord1.x coord2.x) - (min coord1.x coord2.x)
    let height = (max coord1.y coord2.y) - (min coord1.y coord2.y)
    
    let distance = ceil(sqrt(float(width*width + height*height)))
    int(distance)

 
let angleBetweenTwoPoints firstPoint secondPoint =
    let xDiff = float(secondPoint.x - firstPoint.x)
    let yDiff = float(firstPoint.y - secondPoint.y)
    // if firstPoint is the "center" around which the angle revolves then this will
    // give us angles of 0 to 180 from 12 o'clock to 6 o'clock (clockwise) and from
    // > -180 to < 0 from 6 o'clock (just after) round to 12 o'clock (just before)
    let angle = (atan2 xDiff yDiff) * 180.0 / System.Math.PI
    // this moves it to 0 to 360 clockwise from 12 o'clock
    if angle < 0.0 then
        angle + 360.0
    else
        angle
    