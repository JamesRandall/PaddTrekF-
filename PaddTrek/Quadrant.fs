module PaddTrek.Game.Quadrant
open PaddTrek.Models
open PaddTrek.GameObjects

let objectsInQuadrant gameObjects quadrantCoordinate  =
    let isInQuadrant gameObject =
        let attributes = getAttributes gameObject
        attributes.position.quadrant = quadrantCoordinate
        
    Seq.toArray (Seq.filter isInQuadrant gameObjects)

let createQuadrant gameObjects quadrantCoordinate gameSize =
    let quadrantObjects = objectsInQuadrant gameObjects quadrantCoordinate

    let createSummaryCell x y =
        let coordinate = { x = x; y = y }
        let possibleObjectAtSector = findWithSectorCoordinate quadrantObjects coordinate
        match possibleObjectAtSector with
            | Some gameObject -> gameObject
            | None _ -> EmptySpace { attributes = { name = "" ; description = "" ; position = { quadrant = quadrantCoordinate; sector = coordinate } } }

    let quadrantArray = Seq.toArray (
                            Seq.map (fun y -> Seq.toArray (
                                                Seq.map (fun x -> createSummaryCell x y)
                                                    [0..gameSize.sectorSize.width-1]))
                                [0 .. gameSize.sectorSize.height-1])

    {
        map = quadrantArray
        objects = quadrantObjects
    }

let createQuadrantSummaries gameObjects gameSize =
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
        Seq.toArray (Seq.map (fun x -> createSummaryCell { x=x; y=y} ) [0..gameSize.quadrantSize.width-1])

    let galaxyArray = Seq.toArray (
                        Seq.map createSummaryRow [0..gameSize.quadrantSize.height-1])
    
    galaxyArray