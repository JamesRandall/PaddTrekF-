module PaddTrek.Space

type Star = {
    attributes: Models.GameWorldObjectAttributes
}

type EmptySpace = {
    attributes: Models.GameWorldObjectAttributes
}

let createEmpty position id : EmptySpace =
    {
        attributes = {
            id = id
            name = "Empty space"
            description = "Nothing much to see here!"
            position = position
        }
    }

let createStar position id : Star =
    {
         attributes = {
            id = id
            name = "Star"
            description = "Star"
            position = position   
        }
    }