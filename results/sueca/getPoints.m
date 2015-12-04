function [ value ] = getPoints( card )
    valuesByRank = [0 0 0 0 0 2 3 4 10 11];
    index = getRank(card) + 1;
    value = valuesByRank(index);
end

