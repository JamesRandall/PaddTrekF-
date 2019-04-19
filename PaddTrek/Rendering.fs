module PaddTrek.Rendering
open System
open PaddTrek.Models
open PaddTrek.Enemies
open PaddTrek.Map
open PaddTrek.Game

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
            Console.BackgroundColor <- if cell.hasPlayer then ConsoleColor.Blue else ConsoleColor.Black

            Console.ForegroundColor <- if cell.numberOfStars = 0 then defaultConsoleColor else ConsoleColor.DarkYellow
            printf " %d " cell.numberOfStars
            Console.ForegroundColor <- if cell.hasStarbase then ConsoleColor.Blue else defaultConsoleColor
            printf "%s " (if cell.hasStarbase then "S" else "0")
            Console.ForegroundColor <- if cell.numberOfEnemies = 0 then defaultConsoleColor else ConsoleColor.Red
            printf "%d  " cell.numberOfEnemies

        setHeaderColors ()
        printf "%d" rowIndex
        setDefaultColors ()
        printf " "
        Seq.iter outputCell [0..quadrantSummaries.[rowIndex].Length-1]
        printf "\n"

    let headerLine = "  " + ((Seq.map (fun i -> sprintf "   %d    " i) [0..worldSize.quadrantSize.width-1]) |> Seq.fold(+) "") + "\n"

    setHeaderColors ()
    printf "%s" headerLine
    setDefaultColors ()
    Seq.iter processRow [0..quadrantSummaries.Length-1]
    
let renderWelcomeMessage () =
    Console.ForegroundColor <- defaultConsoleColor
    printLine "Welcome to PaddTrek F#"
    printLine "The galaxy is under attack and your ship is the last hope against the invaders."
    printLine "Press ? at any time to see the list of commands you can give your crew."
    printf "\n"

let renderWaitingForInput () =
    printf "\n> "

let renderInputComplete () =
    printf "\n"

let renderMessage text =
    Console.ForegroundColor <- defaultConsoleColor
    printLine text

let renderHelp () =
    printLine "Q - quit the game"
    printLine "S - short range scanner"
    printLine "L - long range scanner"
    printLine "M x y - move within a sector"


let renderCommand game args command =
    match command with
    | GameAction.ShortRangeScanner | GameAction.MoveSector | GameAction.MoveQuadrant -> renderShortRangeScanner game
    | GameAction.LongRangeScanner -> renderLongRangeScanner game
    // | "M" -> printf "Moved to position %s,%s" (Seq.toArray args).[0] (Seq.toArray args).[1]
    | _ -> renderMessage "Sorry I did not understand that command"
