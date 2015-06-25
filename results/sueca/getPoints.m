function [ value ] = getPoints( card )
    valuesByRank = [0 0 0 0 0 2 3 4 10 11];
    rank = rem(card,10) + 1;
    value = valuesByRank(rank);
end

