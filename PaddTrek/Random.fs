module Random
open System

let private randomNumberGenerator = Random()

let between min max = randomNumberGenerator.Next(min, max)
let upto max = randomNumberGenerator.Next(max)

    
