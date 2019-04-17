module PaddTrek.Space

type Star = {
    attributes: Models.GameWorldObjectAttributes
}

type EmptySpace = {
    attributes: Models.GameWorldObjectAttributes
}

let createEmpty position : EmptySpace =
    {
        attributes = {
            name = "Empty space"
            description = "Nothing much to see here!"
            position = position
        }
    }

let createStar position : Star =
    {
         attributes = {
            name = "Star"
            description = "Star"
            position = position   
        }
    }