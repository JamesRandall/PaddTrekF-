module PaddTrek.GameTypes

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

// Player

type PlayerShields = {
    fore: Range
    port: Range
    aft: Range
    starboard: Range
}

type PlayerSystem = {
    name: string
    health: Range
}

type PlayerHealth = {
    hull: PlayerSystem
    impulseEngines: PlayerSystem
    warpEngines: PlayerSystem
}

type Player = {
    attributes: GameWorldObjectAttributes
    energy: Range
    shields: PlayerShields
    health: PlayerHealth
}

// Enemies

type EnemyType =
    | Scout = 0
    | Cruiser = 1
    | Dreadnought = 2

type EnemyShip = {
    attributes: GameWorldObjectAttributes
    enemyType: EnemyType
    energy: Range
    shields: Range
    hull: Range
}

// Galactic objects

type Star = {
    attributes: GameWorldObjectAttributes
}

type EmptySpace = {
    attributes: GameWorldObjectAttributes
}

let emptySpace : EmptySpace = {
    attributes = {
        name = ""
        description = ""
        position = nullGalacticCoordinate
    }
}

// Game world

type GameWorldObject =
    | EnemyShip of EnemyShip
    | Star of Star
    | Player of Player
    | EmptySpace of EmptySpace

type GameSize = {
    quadrantSize: Size
    sectorSize: Size
}

type Game = {
    objects: GameWorldObject list
    size: GameSize
    //player: Player
}

type Quadrant = {
    map: GameWorldObject array array
    objects: GameWorldObject array
}

