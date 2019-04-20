module PaddTrek.Rendering
open PaddTrek
open System
open System
open PaddTrek.Enemies
open PaddTrek.Map
open PaddTrek.Game
open ConsoleOutput

let private defaultConsoleColor = ConsoleColor.Green

let private printLine text =
    printf "%s\n" text

let private setHeaderColors () =
    Console.ForegroundColor <- ConsoleColor.Black
    Console.BackgroundColor <- defaultConsoleColor
let private setDefaultColors () =
    Console.ForegroundColor <- defaultConsoleColor
    Console.BackgroundColor <- ConsoleColor.Black
     
let renderShortRangeScanner game =
    let quadrant = Map.createCurrentQuadrant game
    let consoleForegroundColor character =
        match character with
            | 's' | 'c' | 'd' -> ConsoleColor.Red
            | '*' -> ConsoleColor.DarkYellow
            | 'p' | 'b' -> ConsoleColor.Blue
            | '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' -> ConsoleColor.Black
            | _ -> defaultConsoleColor

    let consoleBackgroundColor character =
        match character with
            | '_' | '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' -> defaultConsoleColor
            | _ -> ConsoleColor.Black
    
    let enemyShipCharacter enemyShip =
        match enemyShip.enemyType with
            | EnemyType.Scout -> "s"
            | EnemyType.Cruiser -> "c"
            | EnemyType.Dreadnought -> "d"
        
    let getGameObjectCharacter gameWorldObject =
        match gameWorldObject with
            | GameWorldObject.EnemyShip enemy -> enemyShipCharacter enemy
            | Star _ -> "*"
            | Player _ -> "p"
            | EmptySpace _ -> "."
            | Starbase _ -> "b"
    
    let renderShortRangeScannerToString quadrant =
        let headerLine = "__" + ((Seq.map (fun i -> sprintf "%d_" i) [0..quadrant.map.[0].Length-1]) |> Seq.fold(+) "") + "\n"
        let contents = Seq.map (fun rowIndex -> sprintf "%d %s\n" rowIndex ((Seq.map (fun column ->
            sprintf "%s " (getGameObjectCharacter column))quadrant.map.[rowIndex]) |> Seq.fold(+) "")) [0..quadrant.map.Length-1]
        
        (Seq.concat([seq [headerLine]; contents])) |> Seq.fold(+) ""

    let renderInColor scannerString =
        scannerString |> Seq.iter (fun c ->
            Console.ForegroundColor <- consoleForegroundColor c
            Console.BackgroundColor <- consoleBackgroundColor c
            printf "%c" (if c = '_' then ' ' else c)
            )
    
    renderShortRangeScannerToString quadrant |> renderInColor
    
let renderLongRangeScanner game =
    let gameObjects = game.objects
    let worldSize = game.size
    let quadrantSummaries = createQuadrantSummaries gameObjects worldSize
    let processRow rowIndex =
        let outputCell colIndex =
            let cell = quadrantSummaries.[rowIndex].[colIndex]
            [
                BackgroundColor(match cell.hasPlayer with | true -> ConsoleColor.Blue | false -> ConsoleColor.Black) ;
                ForegroundColor(match cell.numberOfStars with | 0 -> defaultConsoleColor | _ -> ConsoleColor.DarkYellow) ;
                String(sprintf " %d " cell.numberOfStars) ;
                ForegroundColor(match cell.hasStarbase with | true -> ConsoleColor.Blue | false -> defaultConsoleColor) ;
                String(sprintf "%s " (if cell.hasStarbase then "S" else "0")) ;
                ForegroundColor(match cell.numberOfEnemies with | 0 -> defaultConsoleColor | _ -> ConsoleColor.Red) ;
                String(sprintf "%d  " cell.numberOfEnemies)
            ]
        
        [
             HeaderColor ;
             String(sprintf "%d" rowIndex) ;
             DefaultColor ;
             String(" ") ;
             Seq(Seq.map outputCell [0..quadrantSummaries.[rowIndex].Length-1] |> Seq.concat) ;
             NewLine
        ]

    let headerLine = [
                         HeaderColor ;
                         String("  ") ;
                         String(((Seq.map (fun i -> sprintf "   %d    " i) [0..worldSize.quadrantSize.width-1]) |> Seq.fold(+) "")) ;
                         NewLine
                     ]    
    let consoleOutput = (Seq.map processRow [0..quadrantSummaries.Length-1] |> Seq.concat) |> Seq.append [DefaultColor]
    consoleOutput |> Seq.append headerLine |> write
    
let renderWelcomeMessage () =
    [
        DefaultColor ;
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
        ("Port shields", player.shields.port)
        ("Aft shields", player.shields.aft)
        ("Starboard shields", player.shields.starboard)
    ] |> Seq.map renderRange |> Seq.concat |> write
    
let renderWaitingForInput () =
    [ NewLine ; String("> ") ] |> write

let renderInputComplete () =
    [ NewLine ] |> write

let renderMessage text =
    Console.ForegroundColor <- defaultConsoleColor
    printLine text

let renderHelp () =
    [
        DefaultColor ;
        Line("Q - quit the game") ;
        Line("S - short range scanner") ;
        Line("M x y - move within a sector") ;
        Line("E - show energy levels") ;
        Line("U - shields up") ;
        Line("D - shields down")
    ] |> write

let renderError message =
    Console.ForegroundColor <- ConsoleColor.DarkRed
    printLine message

let renderCommand game command =
    match command with
    | PlayerAction.ShortRangeScanner | PlayerAction.MoveQuadrant -> renderShortRangeScanner game
    | PlayerAction.LongRangeScanner -> renderLongRangeScanner game
    | PlayerAction.MoveSector coords -> renderShortRangeScanner game ; printf "Moved to position %d,%d" coords.x coords.y
    | PlayerAction.EnergyLevels -> renderEnergyLevels game
