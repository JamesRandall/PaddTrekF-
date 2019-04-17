module PaddTrek.Range

type Range = {
    min: int
    max: int
    value: int
}

let withMinMaxValue min max value =
    {
        min = min
        max = max
        value = value
    }

let withMinMax min max =
    {
        min = min
        max = max
        value = max
    }

