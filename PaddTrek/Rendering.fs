module PaddTrek.Rendering
open PaddTrek
open System
open PaddTrek.Enemies
open PaddTrek.Map
open PaddTrek.Game
open ConsoleOutput
open PaddTrek.Message
    
let renderScanners game =
    let worldSize = game.size.quadrantSize
    let quadrantSummaries = createQuadrantSummaries game
    let currentQuadrant = Map.createCurrentQuadrant game
    
    let processRow rowIndex =
        let outputShortRangeCell cell =
            let consoleForegroundColor character =
                match character with
                    | EnemyShip _ -> ForegroundColor(ConsoleColor.Red)
                    | Star _ -> ForegroundColor(ConsoleColor.DarkYellow)
                    | Player _ -> ForegroundColor(ConsoleColor.Blue)
                    | Starbase _ -> ForegroundColor(ConsoleColor.Blue)
                    | _ -> DefaultForegroundColor

            let enemyShipCharacter enemyShip =
                match enemyShip.enemyType with
                    | EnemyType.Scout -> String("s")
                    | EnemyType.Cruiser -> String("c")
                    | EnemyType.Dreadnought -> String("d")
                
            let getGameObjectCharacter gameWorldObject =
                match gameWorldObject with
                    | GameWorldObject.EnemyShip enemy -> enemyShipCharacter enemy
                    | Star _ -> String("*")
                    | Player _ -> String("p")
                    | EmptySpace _ -> String(".")
                    | Starbase _ -> String("b")
            
            [
                consoleForegroundColor cell ;
                getGameObjectCharacter cell ;
                String (" ")
            ]
        
        let outputLongRangeCell colIndex =
            let cell = quadrantSummaries.[rowIndex].[colIndex]
            match cell.isDiscovered with
                | true -> [
                    BackgroundColor(match cell.hasPlayer with | true -> ConsoleColor.Blue | false -> ConsoleColor.Black) ;
                    ForegroundColor(match cell.numberOfStars with | 0 -> ConsoleColor.Green | _ -> ConsoleColor.DarkYellow) ;
                    String(sprintf " %d " cell.numberOfStars) ;
                    ForegroundColor(match cell.hasStarbase with | true -> ConsoleColor.Blue | false -> ConsoleColor.Yellow) ;
                    String(sprintf "%s " (if cell.hasStarbase then "S" else "0")) ;
                    ForegroundColor(match cell.numberOfEnemies with | 0 -> ConsoleColor.Green | _ -> ConsoleColor.Red) ;
                    String(sprintf "%d  " cell.numberOfEnemies)]
                | false -> [
                    BackgroundColor(match cell.hasPlayer with | true -> ConsoleColor.Blue | false -> ConsoleColor.Black) ;
                    ForegroundColor(ConsoleColor.DarkGreen) ;
                    String(" ? ? ?  ")
                ]
                
        [
            // Short range
            HeaderColor ;
            String(sprintf "%d" rowIndex) ;
            DefaultColor ;
            String(" ") ;
            Seq(Seq.concat (Seq.map outputShortRangeCell currentQuadrant.map.[rowIndex])) ;
            String("    ") ;
            // Long range
            HeaderColor ;
            String(sprintf "%d" rowIndex) ;
            DefaultColor ;
            String(" ") ;
            Seq(Seq.map outputLongRangeCell [0..quadrantSummaries.[rowIndex].Length-1] |> Seq.concat) ;
            DefaultColor ;
            NewLine
        ]
    
    let headerLine = [
                         // short range
                         HeaderColor ;
                         String("  ") ;
                         String (((Seq.map (fun i -> sprintf "%d " i) [0..currentQuadrant.map.[0].Length-1]) |> Seq.fold(+) "")) ;
                         DefaultColor ;
                         String("    ") ;
                         // long range
                         HeaderColor ;
                         String("  ") ;
                         String(((Seq.map (fun i -> sprintf "   %d    " i) [0..worldSize.width-1]) |> Seq.fold(+) "")) ;
                         DefaultColor ;
                         NewLine
                     ]    
    
    let consoleOutput = (Seq.map processRow [0..quadrantSummaries.Length-1] |> Seq.concat) |> Seq.append [DefaultColor]
    consoleOutput |> Seq.append headerLine |> write
    
    

    
let renderWelcomeMessage () =
    [
        Line("Welcome to PaddTrek F#") ;
        Line("The galaxy is under attack and your ship is the last hope against the invaders.") ;
        Line("Press ? at any time to see the list of commands you can give your crew.") ;
        NewLine        
    ] |> write

let private renderRange (description, range:Range.Range) =
    let percentage = 100 * range.value / range.max
    [
        DefaultColor ;
        ForegroundColor(if percentage < 15 then ConsoleColor.Red elif percentage < 30 then ConsoleColor.Yellow else ConsoleColor.Green) ;
        Line(sprintf "%s %d/%d (%d%%)" description range.value range.max percentage)
    ]

let renderEnergyLevels game =
    let player = game |> Game.getPlayer
    [
        ("Main energy", player.energy)
        ("Fore shields", player.shields.fore)
        ("Starboard shields", player.shields.starboard)
        ("Aft shields", player.shields.aft)
        ("Port shields", player.shields.port)
    ] |> Seq.map renderRange |> Seq.concat |> write
    
let renderShields game =
    let player = game |> Game.getPlayer
    let shields =
        if player.shields.raised then
            [
                ForegroundColor(ConsoleColor.Green)
                Line("Shields raised")
            ]
        else
            [
                ForegroundColor(ConsoleColor.Red)
                Line("Shields lowered")
            ]
    [
        ("Fore shields", player.shields.fore)
        ("Starboard shields", player.shields.starboard)
        ("Aft shields", player.shields.aft)
        ("Port shields", player.shields.port)
        ("Shield generator", player.health.ShieldGenerator.health)
    ] |> Seq.map renderRange |> Seq.concat |> Seq.append shields |> write
    
let renderWaitingForInput () =
    [ NewLine ; String("> ") ] |> write

let renderInputComplete () =
    [ NewLine ] |> write

let renderMessage text =
    [ Line(text) ] |> write

let renderHelp () =
    [
        Line("Q - quit the game") ;
        Line("S - scanners") ;
        Line("M x y - move within a sector (short range)") ;
        Line("M x y w - move within the galaxy (long range) at the given warp speed")
        Line("E - show energy levels") ;
        Line("R - raise shields") ;
        Line("L - lower shields")
    ] |> write

let renderError message =
    [
        ForegroundColor(ConsoleColor.DarkRed) ;
        BackgroundColor(ConsoleColor.Black) ;
        Line(message)
    ] |> write

let renderCommand game command =
    match command with
    | PlayerAction.ShortRangeScanner | PlayerAction.MoveQuadrant(_,_) -> renderScanners game
    | PlayerAction.MoveSector coords -> renderScanners game ; printf "Moved to position %d,%d" coords.x coords.y
    | PlayerAction.EnergyLevels -> renderEnergyLevels game
    | PlayerAction.RaiseShields | PlayerAction.LowerShields -> renderShields game

let renderMessages messages =
    let createRenderOutputFromMessage message =
        [
            (match message.kind with
                | MessageKind.Danger -> ForegroundColor(ConsoleColor.Red)
                | MessageKind.Warning -> ForegroundColor(ConsoleColor.Yellow)
                | _ -> ForegroundColor(ConsoleColor.Green)) ;
            Line(message.text)
        ]
    
    let output = messages |> Seq.map createRenderOutputFromMessage |> Seq.concat
    output |> write
