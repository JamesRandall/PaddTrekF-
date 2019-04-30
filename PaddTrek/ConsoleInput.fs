module PaddTrek.ConsoleInput
open PaddTrek.Geography

type ConsoleCommand =
    | Command of PlayerAction.Action
    | Quit
    | Help
    | Error
    | Clear
    
let private createCommand commandString args =
    match commandString with
        | "M" -> Command(PlayerAction.Action.MoveSector(Geography.createCoordinateWithStrings args))
        | "G" -> Command(PlayerAction.Action.MoveQuadrant(Geography.createCoordinateWithStrings args, int((args |> Seq.toArray).[2])))
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
        
let private isNumber (arg:string) =
    fst (System.Int32.TryParse arg)    
       
let private validateMoveArgs (gameSize:WorldSize) args =
    match args |> Seq.length = 2 &&
          args |> Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg gameSize.sectorSize)) true
     with
        | false -> "Invalid short range move command - it takes the form: M x y"
        | _ -> ""
        
let private validateMoveQuadrantArgs (gameSize:WorldSize) args =
    match args |> Seq.length = 3 &&
          args |> Seq.take(2) |> Seq.fold(fun valid arg -> valid && (isValidCoordinateArg arg gameSize.quadrantSize)) true
     with
        | false -> "Invalid long range move command - it takes the form: G x y w"
        | true -> match (args |> Seq.skip(2) |> Seq.toArray).[0] |> isNumber
                   with | false -> "Warp speed must be a number" | true -> ""
                    

let acceptInput gameSize =
        let readInput () =
            Rendering.renderWaitingForInput ()
            let inputLine = System.Console.ReadLine ()
            match inputLine with
                | "" -> "", Array.empty<string>
                | _ -> (inputLine.Substring (0,1)).ToUpper(),inputLine.Substring(1).Trim().Split(' ')
                
        let commandString, args = readInput ()
        Rendering.renderInputComplete ()
        
        let errorMessage = match commandString with
                            | "M" -> validateMoveArgs gameSize args
                            | "G" -> validateMoveQuadrantArgs gameSize args
                            | _ -> ""
                            
        match errorMessage with
            | "" -> createCommand commandString args
            | _ -> Rendering.renderError errorMessage ; ConsoleCommand.Error 
