module PaddTrek.Ai
open PaddTrek.Enemies

let private enemyTurn game (enemy:EnemyShip) =
    let maximumAvailablePhaserPower = min enemy.energy.value enemy.maxPhaserPower
    let player = game |> Game.getPlayer
    let phaserPower = Random.between (maximumAvailablePhaserPower/2) maximumAvailablePhaserPower
    
    game |> Game.updateWithObjects [
        Game.EnemyShip(enemy |> Enemies.spendEnergy phaserPower) ;
        Game.Player(player |> Player.hitByEnergyWeapon phaserPower enemy.attributes.position.sector)
    ] 
 

let turn game =
    let currentQuadrant = Map.createCurrentQuadrant game

    let updatedGame = currentQuadrant.objects
                      |> Game.getEnemies
                      |> Seq.fold enemyTurn game
    updatedGame
    
