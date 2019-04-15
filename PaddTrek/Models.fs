module PaddTrek.Models

type Coordinate = {
    x: int
    y: int
}

type GalacticCoordinate = {
    quadrant: Coordinate
    sector: Coordinate
}
    
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

type Starbase = {
    attributes: GameWorldObjectAttributes
}

// Enemies

type EnemyType =
    | Scout
    | Cruiser
    | Dreadnought

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

// Game world

type GameWorldObject =
    | EnemyShip of EnemyShip
    | Star of Star
    | Player of Player
    | Starbase of Starbase
    | EmptySpace of EmptySpace

type GameSize = {
    quadrantSize: Size
    sectorSize: Size
}

type Game = {
    size: GameSize
    objects: GameWorldObject list
    player: Player
}

type Quadrant = {
    map: GameWorldObject array array
    objects: GameWorldObject array
}

type QuadrantSummary = {
    numberOfEnemies: int
    numberOfStars: int
    starbase: bool
}

