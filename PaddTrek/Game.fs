module PaddTrek.Game

type GameWorldObject =
    | EnemyShip of Enemies.EnemyShip
    | Star of Space.Star
    | Player of Player.Player
    | Starbase of Starbase.Starbase
    | EmptySpace of Space.EmptySpace
    with
        member x.PlayerValue =
            match x with | Player p -> p | _ -> failwith "Not a player"

type Game = {
    size: Geography.WorldSize
    objects: GameWorldObject list
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

let getPlayer game =
    (game.objects |> Seq.find (function | Player _ -> true | _ -> false)).PlayerValue

let isEmptySpace gameWorldObject =
    match gameWorldObject with
        | EmptySpace _ -> true
        | _ -> false

