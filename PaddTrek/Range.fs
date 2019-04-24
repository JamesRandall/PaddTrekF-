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