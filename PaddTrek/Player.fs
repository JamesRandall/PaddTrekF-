module PaddTrek.Player

type PlayerShields = {
    fore: Range.Range
    port: Range.Range
    aft: Range.Range
    starboard: Range.Range
}

type PlayerSystem = {
    name: string
    health: Range.Range
}

type PlayerHealth = {
    hull: PlayerSystem
    impulseEngines: PlayerSystem
    warpEngines: PlayerSystem
}

type Player = {
    attributes: Models.GameWorldObjectAttributes
    energy: Range.Range
    shields: PlayerShields
    health: PlayerHealth
}

let create position  = 
    {
        attributes = {
            name = "Player"
            description = "The player"
            position = position 
        }
        energy = { min = 0 ; max = 1000 ; value = 1000 }
        shields = {
            fore = { min = 0 ; max = 1000 ; value = 1000 }
            port = { min = 0 ; max = 1000 ; value = 1000 }
            aft = { min = 0 ; max = 1000 ; value = 1000 }
            starboard = { min = 0 ; max = 1000 ; value = 1000 }
        }
        health = {
            hull = { name = "Hull" ; health = { min = 0 ; max = 1000 ; value = 1000 } }
            impulseEngines = { name = "Impulse Engines" ; health = { min = 0 ; max = 1000 ; value = 1000 } }
            warpEngines = { name = "Warp Engines" ; health = { min = 0 ; max = 1000 ; value = 1000 } }
        }
    }