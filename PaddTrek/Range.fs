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