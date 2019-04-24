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
    
let getEnemies gameObjects =
    gameObjects
    |> Seq.filter (function | EnemyShip _ -> true | _ -> false)
    |> Seq.map(function | EnemyShip es -> es | _ -> failwith "error")

let isEmptySpace gameWorldObject =
    match gameWorldObject with
        | EmptySpace _ -> true
        | _ -> false

let updateWithObjects updatedGameObjects game =
    let updateOrKeep existingObject =
        let existingObjectId = (getAttributes existingObject).id
        let foundObject = updatedGameObjects |> Seq.tryFind (fun candidate -> (getAttributes candidate).id = existingObjectId)
        match foundObject with
            | Some found -> found
            | None -> existingObject
        
    let updatedGame = { game with objects = game.objects |> Seq.map updateOrKeep |> Seq.toList }
    updatedGame