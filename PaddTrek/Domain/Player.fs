module PaddTrek.Domain.Player
open PaddTrek.Domain.Primitives

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
