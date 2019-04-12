module PaddTrek.Domain.GameObjects
open PaddTrek.Domain.Enemy
open PaddTrek.Domain.Player
open PaddTrek.Domain.Primitives

type Star = {
    attributes: GameWorldObjectAttributes
}

type GameWorldObject =
    | EnemyShip of EnemyShip
    | Star of Star
    | Player of Player
    
type Game = {
    objects: GameWorldObject list
    //player: Player
}

