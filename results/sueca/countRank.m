function [ result ] = countRank( hand, rank )
    result = 0;
    for i = 1:size(hand,2)
       card = hand(1,i);
       if getRank(card) == rank
            result = result + 1;
       end
    end
end

