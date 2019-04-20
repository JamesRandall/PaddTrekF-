module PaddTrek.ConsoleOutput
open System

type Output =
    | String of string
    | Line of string
    | NewLine
    | DefaultColor
    | HeaderColor
    | ForegroundColor of ConsoleColor
    | BackgroundColor of ConsoleColor
    | Seq of seq<Output>

    
let write content =
    let rec outputContentItem item =
        match item with
            | String s -> printf "%s" s
            | Line s -> printf "%s\n" s
            | NewLine -> printf "\n"
            | DefaultColor -> Console.ForegroundColor <- ConsoleColor.Green ; Console.BackgroundColor <- ConsoleColor.Black
            | HeaderColor -> Console.ForegroundColor <- ConsoleColor.Black ; Console.BackgroundColor <- ConsoleColor.Green
            | ForegroundColor fc -> Console.ForegroundColor <- fc
            | BackgroundColor bc -> Console.BackgroundColor <- bc
            | Seq collection -> collection |> Seq.iter outputContentItem
    Seq.iter (fun i -> outputContentItem(i)) content
