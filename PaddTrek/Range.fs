module PaddTrek.Range

type Range = {
    min: int
    max: int
    value: int
}

let createWithMinMaxValue min max value =
    {
        min = min
        max = max
        value = value
    }

let createWithMinMax min max =
    {
        min = min
        max = max
        value = max
    }

let createWithMax max =
    {
        min = 0
        max = max
        value = max
    }
    
let decrement byAmount range =
    let delta = min byAmount range.value
    let updatedRange = { range with value = (range.value-delta) }
    (updatedRange, delta)
    
let percentage range =
    100 * range.value / range.max

// 100% effectiveness returns 1, 0% returns 0 for use in a multiplier
let effectivenessMultiplier range =
    let efficiency = range |> percentage
    // TODO - solve the equation to resolve the right multiplier
    let result = 2.0 ** (float(efficiency) * 0.0666666666)
    // we just cap this to the range 0.0 to 1.0
    max (min result 1.0) 0.0 
    
