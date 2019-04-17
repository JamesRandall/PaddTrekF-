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
    
    let stars = Seq.map (fun _ -> Star(Space.createStar(newPosition()))) [0..300]
    let enemyScouts = Seq.map (fun _ -> EnemyShip(createEnemyScout())) [0..150]

    let player = Player.create (newPosition())
        
    {
        size = worldSize
        objects = Seq.toList (Seq.append stars enemyScouts |> Seq.append [Player(player)] )
        player = player
    }
