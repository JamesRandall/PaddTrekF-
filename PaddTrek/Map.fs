module PaddTrek.Map
open PaddTrek.Models
open PaddTrek.Geography

type Quadrant = {
    map: Game.GameWorldObject list list
    objects: Game.GameWorldObject list
}

type QuadrantSummary = {
    numberOfEnemies: int
    numberOfStars: int
    hasStarbase: bool
    hasPlayer: bool
}

let findWithSectorCoordinate gameObjects sectorCoordinate =
    let isAtCoordinate gameObject =
        (Game.getAttributes gameObject).position = sectorCoordinate
    
    let result = Seq.tryFind isAtCoordinate gameObjects
    // TODO: Think about IDs some more
    Option.defaultValue (Game.EmptySpace(Space.createEmpty sectorCoordinate -1)) result

let objectsInQuadrant gameObjects quadrantCoordinate  =
    let isInQuadrant gameObject =
        let attributes = Game.getAttributes gameObject
        attributes.position.quadrant = quadrantCoordinate
        
    Seq.filter isInQuadrant gameObjects

let createQuadrant gameObjects quadrantCoordinate worldSize =
    let quadrantObjects = Seq.toList (objectsInQuadrant gameObjects quadrantCoordinate)

    let createSummaryCell x y =
        let sectorCoordinate = createCoordinate x y
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

let createCurrentQuadrant (game:Game.Game) =
    createQuadrant game.objects (Game.getPlayer game).attributes.position.quadrant game.size

let private summarise gameObjects =
    {
        numberOfEnemies = gameObjects |> Seq.sumBy (function | Game.EnemyShip _ -> 1 | _ -> 0)
        numberOfStars = gameObjects |> Seq.sumBy (function | Game.Star _ -> 1 | _ -> 0)
        hasStarbase = gameObjects |> Seq.exists (function | Game.Starbase _ -> true | _ -> false)
        hasPlayer = gameObjects |> Seq.exists (function | Game.Player _ -> true | _ -> false)
    }
    
let createQuadrantSummary game coords =
    let objectsInQuadrant = objectsInQuadrant game.objects coords
    summarise objectsInQuadrant

let createQuadrantSummaries gameObjects worldSize =
    let objectsInQuadrantForGameObjects = objectsInQuadrant gameObjects

    let createSummaryRow y =
        Seq.toArray (Seq.map (fun x -> summarise (objectsInQuadrantForGameObjects { x=x; y=y}) ) [0..worldSize.quadrantSize.width-1])

    let galaxyArray = Seq.toArray (
                        Seq.map createSummaryRow [0..worldSize.quadrantSize.height-1])
    
    galaxyArray

