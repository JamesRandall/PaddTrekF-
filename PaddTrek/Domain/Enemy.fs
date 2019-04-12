module PaddTrek.Domain.Enemy
open PaddTrek.Domain.Primitives

type EnemyShip = {
    attributes: GameWorldObjectAttributes
    energy: Range
    shields: Range
    hull: Range
}
