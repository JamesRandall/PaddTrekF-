module PaddTrek.Game.GameBuilder
open PaddTrek.Domain.Enemy
open PaddTrek.Domain.Primitives
open PaddTrek.Domain.GameObjects
open PaddTrek.Game.CoordinateHelpers


let createGame =
    let gameSize = { quadrantSize = { width = 8; height = 8 } ; sectorSize = { width = 8; height = 8 } }
    let existingObjects = []
    
    let newPosition () =
        nonClashingRandomGalacticPosition existingObjects gameSize
    
    let createStar =
        Star({
             attributes = {
                name = "Star"
                description = "Star"
                position = newPosition()   
            }
        })
        
    let createEnemy name description maxEnergy maxShields maxHull =
        let position = newPosition
        EnemyShip({
            attributes = {
                name = name
                description = description
                position = newPosition() 
            }
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
    
    let createEnemyScout () = createEnemy "Scout" "An enemy scout with weak shields and a weak hull" 1000 1000 1000
    
    let stars = Seq.toList (Seq.map (fun _ -> createStar) [0..400])
    let enemyScouts = Seq.toList (Seq.map (fun _ -> createEnemyScout()) [0..150])
        
    {
        objects = List.concat (seq [stars; enemyScouts])
    }

    