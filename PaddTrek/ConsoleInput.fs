module PaddTrek.ConsoleInput

type ConsoleCommand =
    | Command of PlayerAction.Action
    | Quit
    | Help
    | Error
    | Clear
    
let private createCommand commandString args =
    match commandString with
        | "M" -> Command(PlayerAction.Action.MoveSector(Geography.createCoordinateWithStrings args))
        | "G" -> Command(PlayerAction.Action.MoveQuadrant)
        | "S" -> Command(PlayerAction.Action.ShortRangeScanner)
        | "L" -> Command(PlayerAction.Action.LongRangeScanner)
        | "E" -> Command(PlayerAction.Action.EnergyLevels)
        | "Q" -> ConsoleCommand.Quit
        | "?" -> ConsoleCommand.Help
        | "X" -> ConsoleCommand.Clear
        | _ -> ConsoleCommand.Error

let private isValidCoordinateArg (arg:string) (size:Geography.Size) =
    match System.Int32.TryParse arg with
        | (true, number) -> number >=0 && number < size.width
        | (false, _)  -> false
        
let private validateMoveArgs (game:Game.Game) args =
    match args |> Seq.length = 2 &&
          args |> Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg game.size.sectorSize)) true
     with
        | false -> "Invalid move command - it takes the form: m x y"
        | _ -> ""

let acceptInput game =
        let readInput () =
            Rendering.renderWaitingForInput ()
            let inputLine = System.Console.ReadLine ()
            match inputLine with
                | "" -> "", Array.empty<string>
                | _ -> (inputLine.Substring (0,1)).ToUpper(),inputLine.Substring(1).Trim().Split(' ')
                
        let commandString, args = readInput ()
        Rendering.renderInputComplete ()
        
        let errorMessage = match commandString with
                            | "M" -> validateMoveArgs game args
                            | _ -> ""
                            
        match errorMessage with
            | "" -> createCommand commandString args
            | _ -> Rendering.renderError errorMessage ; ConsoleCommand.Error 
