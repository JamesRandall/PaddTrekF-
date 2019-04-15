module PaddTrek.Rendering
open System
open PaddTrek.Models

let private defaultConsoleColor = ConsoleColor.Green

let private printLine text =
    printf "%s\n" text
     
let renderShortRangeScanner quadrant =
    let consoleColor character =
        match character with
            | 's' | 'c' | 'd' -> ConsoleColor.Red
            | '*' -> ConsoleColor.DarkYellow
            | 'p' | 'b' -> ConsoleColor.Blue
            | _ -> defaultConsoleColor
    
    let enemyShipCharacter enemyShip =
        match enemyShip.enemyType with
            | EnemyType.Scout -> "s"
            | EnemyType.Cruiser -> "c"
            | EnemyType.Dreadnought -> "d"
        
    let getGameObjectCharacter gameWorldObject =
        match gameWorldObject with
            | EnemyShip enemy -> enemyShipCharacter enemy
            | Star _ -> "*"
            | Player _ -> "p"
            | EmptySpace _ -> "."
            | Starbase _ -> "b"
    
    let renderShortRangeScannerToString quadrant =
        let headerLine = "  " + ((Seq.map (fun i -> sprintf "%d " i) [0..quadrant.map.[0].Length-1]) |> Seq.fold(+) "") + "\n"
        let contents = Seq.map (fun rowIndex -> sprintf "%d %s\n" rowIndex ((Seq.map (fun column ->
            sprintf "%s " (getGameObjectCharacter column))quadrant.map.[rowIndex]) |> Seq.fold(+) "")) [0..quadrant.map.Length-1]
        
        (Seq.concat([seq [headerLine]; contents])) |> Seq.fold(+) ""
        
    let renderInColor scannerString =
        scannerString |> Seq.iter (fun c ->
            Console.ForegroundColor <- consoleColor c
            printf "%c" c
            )
    
    renderShortRangeScannerToString quadrant |> renderInColor
    
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
