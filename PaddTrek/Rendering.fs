module PaddTrek.Rendering
open System
open PaddTrek.GameTypes

let private defaultConsoleColor = ConsoleColor.Green

let renderShortRangeScanner quadrant =
    let consoleColor character =
        match character with
            | 's' | 'c' | 'd' -> ConsoleColor.Red
            | '*' -> ConsoleColor.DarkYellow
            | 'p' -> ConsoleColor.Blue
            | _ -> defaultConsoleColor
    
    let enemyShipCharacter enemyShip =
        match enemyShip.enemyType with
            | EnemyType.Scout -> "s"
            | EnemyType.Cruiser -> "c"
            | EnemyType.Dreadnought -> "d"
            | _ -> " "
        
    let getGameObjectCharacter gameWorldObject =
        match gameWorldObject with
            | EnemyShip enemy -> enemyShipCharacter enemy
            | Star _ -> "*"
            | Player _ -> "p"
            | EmptySpace _ -> "."
    
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
    
    