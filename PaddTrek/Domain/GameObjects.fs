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

type Quadrant = {
    map: GameWorldObject array array
    objects: GameWorldObject array
}

let getAttributes gameWorldObject =
    match gameWorldObject with
        | EnemyShip es -> es.attributes
        | Star st -> st.attributes
        | Player pl -> pl.attributes
