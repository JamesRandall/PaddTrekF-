module PaddTrek.Game.GameObjects
open PaddTrek.Domain

let findWithSectorCoordinate gameObjects sectorCoordinate =
    let isAtCoordinate gameObject =
        (GameObjects.getAttributes gameObject).position.sector = sectorCoordinate
    
    Seq.find isAtCoordinate gameObjects
    
