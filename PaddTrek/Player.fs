module PaddTrek.Player

type PlayerShields = {
    fore: Range.Range
    port: Range.Range
    aft: Range.Range
    starboard: Range.Range
    raised: bool
}

type PlayerSystem = {
    name: string
    health: Range.Range
}

type PlayerHealth = {
    hull: PlayerSystem
    impulseEngines: PlayerSystem
    warpEngines: PlayerSystem
    energyConverter: PlayerSystem
    shieldGenerator: PlayerSystem
}

type Player = {
    attributes: Models.GameWorldObjectAttributes
    energy: Range.Range
    shields: PlayerShields
    health: PlayerHealth
}

let create position id  = 
    {
        attributes = {
            id = id
            name = "Player"
            description = "The player"
            position = position 
        }
        energy = Range.createWithMax 5000
        shields = {
            fore = Range.createWithMax 1500
            port = Range.createWithMax 1500
            aft = Range.createWithMax 1500
            starboard = Range.createWithMax 1500
            raised = true
        }
        health = {
            hull = { name = "Hull" ; health = Range.createWithMax 2000 }
            impulseEngines = { name = "Impulse Engines" ; health = Range.createWithMax 1000 }
            warpEngines = { name = "Warp Engines" ; health = Range.createWithMax 1000 }
            energyConverter = { name = "Energy Converter"; health = Range.createWithMax 750 }
            shieldGenerator = { name = "Shield Generator"; health = Range.createWithMax 750 }
        }
    }

let energyToMovePlayerToSector player coordinates =
    let energyConsumption = 10
    let distance = Geography.distanceBetweenCoordinates coordinates player.attributes.position.sector
    distance * energyConsumption

let moveToSector player coordinates =
    {
        player with
            attributes = { player.attributes with position = { player.attributes.position with sector = coordinates}}
            energy = { player.energy with value = player.energy.value - (energyToMovePlayerToSector player coordinates) }
    }

let hitByEnergyWeapon energy player =
    // temporarily to test this we're just hitting the fore shield, will worry about angle
    // and systems later!
    
    let newForeShield, actualNewForeShieldAdjustment = player.shields.fore |> Range.decrement energy 
    let newPortShield = player.shields.port
    let newAftShield = player.shields.aft
    let newStarboardShield = player.shields.starboard
    
    {
        player with
            shields = {
                    fore = newForeShield
                    port = newPortShield
                    aft = newAftShield
                    starboard = newStarboardShield
                    raised = player.shields.raised
                }
    }