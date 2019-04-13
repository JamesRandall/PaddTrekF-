module PaddTrek.GameObjects
open PaddTrek.GameTypes

let getAttributes gameWorldObject =
    match gameWorldObject with
        | EnemyShip es -> es.attributes
        | Star st -> st.attributes
        | Player pl -> pl.attributes
        | EmptySpace es -> es.attributes

let findWithSectorCoordinate gameObjects sectorCoordinate =
    let isAtCoordinate gameObject =
        (getAttributes gameObject).position.sector = sectorCoordinate
    
    let result = Seq.tryFind isAtCoordinate gameObjects
    if result = None then
        EmptySpace emptySpace
    else
        result.Value
