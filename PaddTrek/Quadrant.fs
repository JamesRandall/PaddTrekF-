module PaddTrek.Game.Quadrant
open PaddTrek
open PaddTrek.GameTypes
open PaddTrek.GameObjects

let objectsInQuadrant gameObjects quadrantCoordinate =
    let isInQuadrant gameObject =
        let attributes = getAttributes gameObject
        attributes.position.quadrant = quadrantCoordinate
    let result = Seq.filter isInQuadrant gameObjects
    result

let createQuadrant gameObjects quadrantCoordinate gameSize =
    let quadrantObjects = objectsInQuadrant gameObjects quadrantCoordinate
    let y = 0
    let quadrantArray = [| for y in 0 .. gameSize.sectorSize.height -> [|
                            for x in 0 .. gameSize.sectorSize.width -> findWithSectorCoordinate quadrantObjects { x = x; y = y }
        |]
    |]
    
    {
        map = quadrantArray
        objects = Seq.toArray quadrantObjects
    }
    