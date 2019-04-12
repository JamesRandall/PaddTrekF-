module PaddTrek.Domain.Primitives

type Coordinate = {
    x: int
    y: int
}

let nullCoordinate = { x = -1; y = -1 }

type GalacticCoordinate = {
    quadrant: Coordinate
    sector: Coordinate
}

let nullGalacticCoordinate = { quadrant = nullCoordinate; sector = nullCoordinate }
    
type Range = {
    min: int
    max: int
    value: int
}

type Size = {
    width: int
    height: int
}

type GameWorldObjectAttributes = {
    name: string
    description: string
    position: GalacticCoordinate
}

type GameSize = {
    quadrantSize: Size
    sectorSize: Size
}
