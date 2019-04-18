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
    
    let createEnemyScout () = Enemies.createEnemy Enemies.EnemyType.Scout (newPosition()) 1000 1000 1000
    let createEnemyCruiser () = Enemies.createEnemy Enemies.EnemyType.Cruiser (newPosition()) 2000 1500 1500
    let createEnemyDreadnought () = Enemies.createEnemy Enemies.EnemyType.Cruiser (newPosition()) 3000 2500 2500
    
    let stars = Seq.map (fun _ -> Star(Space.createStar(newPosition()))) [0..150]
    let enemyScouts = Seq.map (fun _ -> EnemyShip(createEnemyScout())) [0..50]
    let enemyCruisers = Seq.map (fun _ -> EnemyShip(createEnemyCruiser())) [0..30]
    let enemyDreadnoughts = Seq.map (fun _ -> EnemyShip(createEnemyDreadnought())) [0..10]

    let player = Player.create (newPosition())
        
    {
        size = worldSize
        objects = Seq.toList (
                    stars |>
                    Seq.append enemyScouts |>
                    Seq.append enemyCruisers |>
                    Seq.append enemyDreadnoughts |>
                    Seq.append [Player(player)]
                    )
        player = player
    }
