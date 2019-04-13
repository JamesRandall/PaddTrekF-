module PaddTrek.Rendering
open System
open PaddTrek.GameTypes

let private defaultConsoleColor = ConsoleColor.Green

let private setDefaultConsoleColor =
    Console.ForegroundColor <- defaultConsoleColor
    
let getGameObjectCharacter gameWorldObject =
    match gameWorldObject with
        | EnemyShip es -> "e"
        | Star st -> "*"
        | Player pl -> "p"
        | EmptySpace es -> "."

let renderShortRangeScanner quadrant =
    setDefaultConsoleColor
    printf "  "
    for colIndex in [0..quadrant.map.[0].Length-1] do
        printf "%d " colIndex
    printf "\n"
    for rowIndex in [0..quadrant.map.Length-1] do
        let row = quadrant.map.[rowIndex]
        printf "%d " rowIndex
        for column in row do
            printf "%s " (getGameObjectCharacter column)
        printf "\n"