module PaddTrek.Message

type MessageKind =
    | Information
    | Warning
    | Danger
    
type Message = {
    text: string
    kind: MessageKind
}

let messageKindForRange range =
    let percentage = range |> Range.percentage
    if percentage < 15 then
        MessageKind.Danger
    elif percentage < 30 then
        MessageKind.Warning
    else MessageKind.Information
    