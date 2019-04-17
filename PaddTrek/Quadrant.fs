module PaddTrek.Map
open PaddTrek.Models
open PaddTrek.Game
open PaddTrek.Geography

type Quadrant = {
    map: GameWorldObject list list
    objects: GameWorldObject list
}

type QuadrantSummary = {
    numberOfEnemies: int
    numberOfStars: int
    starbase: bool
}

let findWithSectorCoordinate gameObjects sectorCoordinate =
    let isAtCoordinate gameObject =
        (getAttributes gameObject).position = sectorCoordinate
    
    let result = Seq.tryFind isAtCoordinate gameObjects
    Option.defaultValue (EmptySpace(Space.createEmpty sectorCoordinate)) result

let objectsInQuadrant gameObjects quadrantCoordinate  =
    let isInQuadrant gameObject =
        let attributes = getAttributes gameObject
        attributes.position.quadrant = quadrantCoordinate
        
    Seq.filter isInQuadrant gameObjects

let createQuadrant gameObjects quadrantCoordinate worldSize =
    let quadrantObjects = Seq.toList (objectsInQuadrant gameObjects quadrantCoordinate)

    let createSummaryCell x y =
        let sectorCoordinate = { x = x; y = y }
        let coordinate = { quadrant = quadrantCoordinate; sector = sectorCoordinate }
        findWithSectorCoordinate quadrantObjects coordinate

    let quadrantArray = Seq.toList (
                            Seq.map (fun y -> Seq.toList (
                                                Seq.map (fun x -> createSummaryCell x y)
                                                    [0..worldSize.sectorSize.width-1]))
                                [0 .. worldSize.sectorSize.height-1])

    {
        map = quadrantArray
        objects = quadrantObjects
    }

let createQuadrantSummaries gameObjects worldSize =
    let objectsInQuadrantForGameObjects = objectsInQuadrant gameObjects

    let createSummaryCell coordinates =
        let objectsForSummary = objectsInQuadrantForGameObjects coordinates
        {
            // which way would be considered "more" F#
            numberOfEnemies = Seq.fold (fun sum -> function | EnemyShip _ -> sum + 1 | _ -> sum) 0 objectsForSummary
            numberOfStars = Seq.sumBy (fun gameObject -> match gameObject with | Star _ -> 1 | _ -> 0) objectsForSummary
            // are these equivelant - need to test in repl
            starbase = Seq.exists (function | Starbase _ -> true | _ -> false) objectsForSummary
            //starbase = Seq.exists (fun gameObject -> match gameObject with | Starbase _ -> true | _ -> false) objectsForSummary
        }

    let createSummaryRow y =
        Seq.toArray (Seq.map (fun x -> createSummaryCell { x=x; y=y} ) [0..worldSize.quadrantSize.width-1])

    let galaxyArray = Seq.toArray (
                        Seq.map createSummaryRow [0..worldSize.quadrantSize.height-1])
    
    galaxyArray
