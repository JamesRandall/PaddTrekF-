module PaddTrek.Game.GameBuilder
open PaddTrek.GameTypes
open PaddTrek.Coordinates
open System.Collections.Generic


let createGame =
    let gameSize = { quadrantSize = { width = 8; height = 8 } ; sectorSize = { width = 8; height = 8 } }
    let existingCoordinates = HashSet<GalacticCoordinate>()
    let newPosition () =
        let createdPosition = nonClashingRandomGalacticPosition (existingCoordinates, gameSize)
        ignore(existingCoordinates.Add (createdPosition))
        createdPosition
    
    let createStar () =
        Star({
             attributes = {
                name = "Star"
                description = "Star"
                position = newPosition()   
            }
        })
        
    let createEnemy enemyType name description maxEnergy maxShields maxHull =
        EnemyShip({
            attributes = {
                name = name
                description = description
                position = newPosition() 
            }
            enemyType = enemyType
            energy = {
                min = 0
                max = maxEnergy
                value = maxEnergy
            }
            shields = {
                min = 0
                max = maxShields
                value = maxShields
            }
            hull = {
                min = 0
                max = maxHull
                value = maxHull
            }
        })
    
    let createEnemyScout () = createEnemy EnemyType.Scout
                                  "Scout" "An enemy scout with weak shields and a weak hull" 1000 1000 1000
    
    let stars = Seq.map (fun _ -> createStar()) [0..1000]
    let enemyScouts = Seq.map (fun _ -> createEnemyScout()) [0..150]
        
    {
        objects = Seq.toList (Seq.append stars enemyScouts)
        size = gameSize
    }
    