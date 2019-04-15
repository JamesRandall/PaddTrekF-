module PaddTrek.GameObjects
open PaddTrek.Models

let getAttributes gameWorldObject =
    match gameWorldObject with
        | EnemyShip es -> es.attributes
        | Star st -> st.attributes
        | Player pl -> pl.attributes
        | EmptySpace es -> es.attributes
        | Starbase sb -> sb.attributes

let findWithSectorCoordinate gameObjects sectorCoordinate =
    let isAtCoordinate gameObject =
        (getAttributes gameObject).position.sector = sectorCoordinate
    
    let result = Seq.tryFind isAtCoordinate gameObjects
    result
