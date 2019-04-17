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
}

let createEnemy enemyType position maxEnergy maxShields maxHull =
    {
        attributes = {
            name = "name"
            description = "description"
            position = position 
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
    }