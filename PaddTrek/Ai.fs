module PaddTrek.Ai
open PaddTrek.Enemies

let private enemyTurn (game, messages) (enemy:EnemyShip) =
    let maximumAvailablePhaserPower = min enemy.energy.value enemy.maxPhaserPower
    let player = game |> Game.getPlayer
    let phaserPower = Random.between (maximumAvailablePhaserPower/2) maximumAvailablePhaserPower
    
    let updatedEnemy = enemy |> Enemies.spendEnergy phaserPower
    let updatedPlayer, hitMessages = player |> Player.hitByEnergyWeapon phaserPower enemy.attributes.position.sector 
    
    (game |> Game.updateWithObjects [ Game.EnemyShip(updatedEnemy) ; Game.Player(updatedPlayer) ], messages |> List.append hitMessages)
 

let turn game =
    let currentQuadrant = Map.createCurrentQuadrant game

    let gameAndMessages = currentQuadrant.objects
                                    |> Game.getEnemies
                                    |> Seq.fold enemyTurn (game, [])
    gameAndMessages
    
