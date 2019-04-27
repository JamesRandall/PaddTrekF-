module Random
open System

let private randomNumberGenerator = Random()

let between min max = randomNumberGenerator.Next(min, max+1)
let upto max = randomNumberGenerator.Next(max+1)
let any = randomNumberGenerator.Next()
    
