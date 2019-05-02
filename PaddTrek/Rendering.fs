module PaddTrek.Rendering
open PaddTrek
open System
open PaddTrek.Enemies
open PaddTrek.Map
open PaddTrek.Game
open ConsoleOutput
open PaddTrek.Message
    
let renderShortRangeScanner game =
    let quadrant = Map.createCurrentQuadrant game
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
    
    let processCell cell =
        [
            consoleForegroundColor cell ;
            getGameObjectCharacter cell ;
            String (" ")
        ]
    let processRow rowIndex =
        [
            HeaderColor ;
            String(sprintf "%d" rowIndex) ;
            DefaultColor ;
            String(" ") ;
            Seq(Seq.concat (Seq.map processCell quadrant.map.[rowIndex])) ;
            DefaultColor ;
            NewLine
        ]
    
    let headerLine =
        [
            HeaderColor ;
            String("  ") ;
            String (((Seq.map (fun i -> sprintf "%d " i) [0..quadrant.map.[0].Length-1]) |> Seq.fold(+) "")) ;
            DefaultColor ;
            NewLine
        ]
    
    Seq.map processRow [0..quadrant.map.[0].Length-1]
        |> Seq.concat
        |> Seq.append headerLine
        |> write
    
let renderLongRangeScanner game =
    let worldSize = game.size
    let quadrantSummaries = createQuadrantSummaries game
    let processRow rowIndex =
        let outputCell colIndex =
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
             HeaderColor ;
             String(sprintf "%d" rowIndex) ;
             DefaultColor ;
             String(" ") ;
             Seq(Seq.map outputCell [0..quadrantSummaries.[rowIndex].Length-1] |> Seq.concat) ;
             DefaultColor ;
             NewLine
        ]

    let headerLine = [
                         HeaderColor ;
                         String("  ") ;
                         String(((Seq.map (fun i -> sprintf "   %d    " i) [0..worldSize.quadrantSize.width-1]) |> Seq.fold(+) "")) ;
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
    
let renderWaitingForInput () =
    [ NewLine ; String("> ") ] |> write

let renderInputComplete () =
    [ NewLine ] |> write

let renderMessage text =
    [ Line(text) ] |> write

let renderHelp () =
    [
        Line("Q - quit the game") ;
        Line("S - short range scanner") ;
        Line("M x y - move within a sector (short range)") ;
        Line("G x y - move within the galaxy (long range)")
        Line("E - show energy levels") ;
        Line("U - shields up") ;
        Line("D - shields down")
    ] |> write

let renderError message =
    [
        ForegroundColor(ConsoleColor.DarkRed) ;
        BackgroundColor(ConsoleColor.Black) ;
        Line(message)
    ] |> write

let renderCommand game command =
    match command with
    | PlayerAction.ShortRangeScanner | PlayerAction.MoveQuadrant(_,_) -> renderShortRangeScanner game
    | PlayerAction.LongRangeScanner -> renderLongRangeScanner game
    | PlayerAction.MoveSector coords -> renderShortRangeScanner game ; printf "Moved to position %d,%d" coords.x coords.y
    | PlayerAction.EnergyLevels -> renderEnergyLevels game

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
