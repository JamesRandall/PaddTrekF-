module PaddTrek.Enemies

// Enemies

type EnemyType =
    | Scout
    | Cruiser
    | Dreadnought

type EnemyShip = {
    attributes: Models.GameWorldObjectAttributes
    enemyType: EnemyType
    energy: Range.Range
    shields: Range.Range
    hull: Range.Range
    maxPhaserPower: int
}

let createEnemy enemyType position id maxEnergy maxShields maxHull maxPhaserPower =
    {
        attributes = {
            id = id
            name = "name"
            description = "description"
            position = position 
        }
        enemyType = enemyType
        energy = Range.createWithMax maxEnergy
        shields = Range.createWithMax maxShields
        hull = Range.createWithMax maxHull
        maxPhaserPower = maxPhaserPower
    }
    
let createEnemyScout position id = createEnemy EnemyType.Scout position id 1000 1000 1000 300
let createEnemyCruiser position id = createEnemy EnemyType.Cruiser position id 2000 1500 1500 450 
let createEnemyDreadnought position id = createEnemy EnemyType.Cruiser position id 3000 2500 2500 800

let spendEnergy energy enemy =
    {
        enemy with energy = enemy.energy |> Range.decrement energy |> fst
    }
