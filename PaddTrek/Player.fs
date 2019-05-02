module PaddTrek.Player
open PaddTrek.Message

type PlayerShields = {
    fore: Range.Range
    port: Range.Range
    aft: Range.Range
    starboard: Range.Range
    raised: bool
}

type PlayerSystemId =
    | Hull = 0
    | ImpulseEngines = 1 
    | WarpEngines = 2
    | EnergyConvertor = 3
    | ShieldGenerator = 4
    | LifeSupport = 5
    | Max = 5

type PlayerSystem = {
    name: string
    health: Range.Range
}

type PlayerHealth = {
    systems: Map<PlayerSystemId, PlayerSystem> 
} with
    member this.Hull = this.systems.Item PlayerSystemId.Hull
    member this.ImpulseEngines = this.systems.Item PlayerSystemId.ImpulseEngines
    member this.WarpEngines = this.systems.Item PlayerSystemId.WarpEngines
    member this.EnergyConvertor = this.systems.Item PlayerSystemId.EnergyConvertor
    member this.ShieldGenerator = this.systems.Item PlayerSystemId.ShieldGenerator
    member this.LifeSupport = this.systems.Item PlayerSystemId.LifeSupport
    

type Player = {
    attributes: Models.GameWorldObjectAttributes
    energy: Range.Range
    shields: PlayerShields
    health: PlayerHealth
}

let createPlayerSystem name maxHealth =
    { name = name ; health = Range.createWithMax maxHealth }

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
            systems = Map [
                    (PlayerSystemId.Hull, createPlayerSystem "Hull" 2000)
                    (PlayerSystemId.ImpulseEngines, createPlayerSystem "Impulse Engines" 1000)
                    (PlayerSystemId.WarpEngines, createPlayerSystem "Warp Engines" 1000)
                    (PlayerSystemId.EnergyConvertor, createPlayerSystem "Energy Converter" 750)
                    (PlayerSystemId.ShieldGenerator, createPlayerSystem "Shield Generator" 750)
                    (PlayerSystemId.LifeSupport, createPlayerSystem "Life Support" 750)
                ]
            }            
    }

let energyToMovePlayerToSector player coordinates =
    let energyConsumption = 10
    let distance = Geography.distanceBetweenCoordinates coordinates player.attributes.position.sector
    distance * energyConsumption
    
let energyToMovePlayerToQuadrant player coordinates warpSpeed =
    let energyConsumption = 10
    let distance = Geography.distanceBetweenCoordinates coordinates player.attributes.position.sector
    distance * energyConsumption

let moveToSector player coordinates =
    {
        player with
            attributes = { player.attributes with position = { player.attributes.position with sector = coordinates}}
            energy = { player.energy with value = player.energy.value - (energyToMovePlayerToSector player coordinates) }
    }
    
let moveToQuadrant player coordinates warpSpeed =
    // TODO: make sure we don't end up in a clashing position within a sector, if we're going to move the player out
    // of the way
    {
        player with
            attributes = { player.attributes with position = { player.attributes.position with quadrant = coordinates}}
            energy = { player.energy with value = player.energy.value - (energyToMovePlayerToQuadrant player coordinates warpSpeed) }
    }

let hitByEnergyWeapon energy fromCoordinates player =
    let playerPosition = player.attributes.position.sector
    let angleOfHit = Geography.angleBetweenTwoPoints playerPosition fromCoordinates
    
    // if you're dense like me then starboard is on the right when looking towards the front, port on the left
    let foreEnergyHit () = if angleOfHit <= 45.0 || angleOfHit >=315.0 then energy else 0    
    let starboardEnergyHit () = if angleOfHit > 45.0 && angleOfHit < 135.0 then energy else 0
    let aftEnergyHit () = if angleOfHit >=135.0 && angleOfHit <= 225.0 then energy else 0
    let portEnergyHit () = if angleOfHit > 225.0 && angleOfHit < 315.0 then energy else 0
     
    let newForeShield, actualForeShieldAdjustment = player.shields.fore |> Range.decrement (foreEnergyHit ()) 
    let newPortShield, actualPortShieldAdjustment = player.shields.port |> Range.decrement (portEnergyHit ())
    let newAftShield, actualAftShieldAdjustment = player.shields.aft |> Range.decrement (aftEnergyHit ())    
    let newStarboardShield, actualStarboardShieldAdjustment = player.shields.starboard |> Range.decrement (starboardEnergyHit ())    
        
    let totalEnergyAdjusment = actualForeShieldAdjustment +
                               actualPortShieldAdjustment +
                               actualAftShieldAdjustment +
                               actualStarboardShieldAdjustment                               
                               
    let remainingEnergyAfterShields = energy - totalEnergyAdjusment
    
    let playerWithShieldsUpdated = {
        player with
            shields = {
                    fore = newForeShield
                    port = newPortShield
                    aft = newAftShield
                    starboard = newStarboardShield
                    raised = player.shields.raised
                }
    }
    
    let messages =
        if actualForeShieldAdjustment > 0 then
            [{text = (sprintf "Fore shield hit with %d energy and reduced to %d" actualForeShieldAdjustment newForeShield.value) ;
              kind=messageKindForRange newForeShield }]
        elif actualAftShieldAdjustment > 0 then
            [{text = (sprintf "Aft shield hit with %d energy and reduced to %d" actualAftShieldAdjustment newAftShield.value) ;
              kind=messageKindForRange newAftShield}]
        elif actualPortShieldAdjustment > 0 then
            [{text = (sprintf "Port shield hit with %d energy and reduced to %d" actualPortShieldAdjustment newPortShield.value) ;
              kind=messageKindForRange newPortShield}]
        elif actualStarboardShieldAdjustment > 0 then
            [{text = (sprintf "Starboard shield hit with %d energy and reduced to %d" actualForeShieldAdjustment newStarboardShield.value) ;
              kind=messageKindForRange newStarboardShield}]
        else []
    
    let calculateHitOnPlayerSystem (remainingEnergy, previousPlayer, previousMessages) (KeyValue(systemId, system:PlayerSystem)) =
        if remainingEnergy = 0 || system.health.value = 0 then
            (remainingEnergy, previousPlayer, previousMessages)
        else
            let newEnergy = remainingEnergy - (min remainingEnergy system.health.value)
            let delta = remainingEnergy - newEnergy
            let updatedSystem = { system with health = { system.health with value = system.health.value - delta } }
            let newSystems = previousPlayer.health.systems |> Map.remove systemId |> Map.add systemId updatedSystem 
            let updatedPlayer = { previousPlayer with health = { previousPlayer.health with systems = newSystems } }
            (newEnergy, updatedPlayer, previousMessages)
        
    let randomSystems = player.health.systems |> Seq.sortBy (fun _ -> Random.any)
    let startingState = (remainingEnergyAfterShields, playerWithShieldsUpdated, messages)
    let _, updatedPlayer, messages = randomSystems |> Seq.fold calculateHitOnPlayerSystem startingState 
    
    updatedPlayer, messages

let quadrantsAroundPlayer (worldSize:Geography.WorldSize) player = seq {
        let quadrantPosition = player.attributes.position.quadrant
        for y in (max (quadrantPosition.y-1) 0) .. (min (quadrantPosition.y+1) (worldSize.quadrantSize.height-1)) do
            for x in (max (quadrantPosition.x-1) 0) .. (min (quadrantPosition.x+1) (worldSize.quadrantSize.width-1)) do
                let coord:Geography.Coordinate = { x = x ; y = y }
                yield coord
    }
