module PaddTrek.GameBuilder
open PaddTrek.Models
open PaddTrek.Geography
open PaddTrek.Game
open System.Collections.Generic


let createGame =
    let worldSize = { quadrantSize = { width = 8; height = 8 } ; sectorSize = { width = 8; height = 8 } }
    let existingCoordinates = HashSet<GalacticCoordinate>()
    let newPosition () =
        let createdPosition = nonClashingRandomGalacticPosition (existingCoordinates, worldSize)
        ignore(existingCoordinates.Add (createdPosition))
        createdPosition
    
    let stars = Seq.map (fun i -> Star(Space.createStar (newPosition ()) i)) [0..149]
    let enemyScouts = Seq.map (fun i -> EnemyShip(Enemies.createEnemyScout (newPosition ()) (i))) [150..199]
    let enemyCruisers = Seq.map (fun i -> EnemyShip(Enemies.createEnemyCruiser (newPosition ()) i)) [200..229]
    let enemyDreadnoughts = Seq.map (fun i -> EnemyShip(Enemies.createEnemyDreadnought (newPosition()) i)) [230..239]
    
    let player = Player.create (newPosition()) 240
    let quadrantPosition = player.attributes.position.quadrant
    
    
    let playerSurroundingQuadrants = seq {
        for y in (max (quadrantPosition.y-1) 0) .. (min (quadrantPosition.y+1) (worldSize.quadrantSize.height-1)) do
            for x in (max (quadrantPosition.x-1) 0) .. (min (quadrantPosition.x+1) (worldSize.quadrantSize.width-1)) do
                yield (x,y)
    }
    
    {
        size = worldSize
        objects = Seq.toList (
                    stars |>
                    Seq.append enemyScouts |>
                    Seq.append enemyCruisers |>
                    Seq.append enemyDreadnoughts |>
                    Seq.append [Player(player)]
                    )
        score = 0
        gameOver = false
        discoveredQuadrants = playerSurroundingQuadrants |> Seq.map (fun (x,y) -> createCoordinate x y) |> Seq.toList
    }
