open PaddTrek.Game
open PaddTrek
open PaddTrek.Game.GameBuilder

[<EntryPoint>]
let main argv =
    let game = createGame
    let createQuadrantFromGame () =
        Quadrant.createQuadrant game.objects { x=1; y=1 } game.size
        
    let quadrant = createQuadrantFromGame ()
    Rendering.renderShortRangeScanner quadrant
    
    0 // return an integer exit code
