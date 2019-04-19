module PaddTrek.Game

type GameWorldObject =
    | EnemyShip of Enemies.EnemyShip
    | Star of Space.Star
    | Player of Player.Player
    | Starbase of Starbase.Starbase
    | EmptySpace of Space.EmptySpace

type Game = {
    size: Geography.WorldSize
    objects: GameWorldObject list
    player: Player.Player
    score: int
    gameOver: bool
}

let getAttributes gameWorldObject =
    match gameWorldObject with
        | EnemyShip es -> es.attributes
        | Star st -> st.attributes
        | Player pl -> pl.attributes
        | EmptySpace es -> es.attributes
        | Starbase sb -> sb.attributes

