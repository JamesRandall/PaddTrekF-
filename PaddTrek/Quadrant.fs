module PaddTrek.Game.Quadrant
open PaddTrek.GameTypes
open PaddTrek.GameObjects

let objectsInQuadrant gameObjects quadrantCoordinate =
    let isInQuadrant gameObject =
        let attributes = getAttributes gameObject
        attributes.position.quadrant = quadrantCoordinate
        
    Seq.toArray (Seq.filter isInQuadrant gameObjects)

let createQuadrant gameObjects quadrantCoordinate gameSize =
    let quadrantObjects = objectsInQuadrant gameObjects quadrantCoordinate

    let quadrantArray = Seq.toArray (
                            Seq.map (fun y -> Seq.toArray (
                                                Seq.map (fun x -> findWithSectorCoordinate quadrantObjects { x = x; y = y })
                                                    [0..gameSize.sectorSize.width-1]))
                                [0 .. gameSize.sectorSize.height-1])

    {
        map = quadrantArray
        objects = quadrantObjects
    }
    