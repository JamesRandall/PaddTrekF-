#load "Domain/Primitives.fs"
#load "Domain/Player.fs"
#load "Domain/Enemy.fs"
#load "Domain/GameObjects.fs"
#load "Game/CoordinateHelpers.fs"
#load "Game/GameBuilder.fs"

open PaddTrek.Game.GameBuilder

let game = createGame

printfn "%d" game.objects.Length
