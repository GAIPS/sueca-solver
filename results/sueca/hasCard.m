function [ result ] = hasCard( hand, suit, rank )
    result = 0;
    for i = 1:size(hand,2)
       card = hand(1,i);
       if getSuit(card) == suit && getRank(card) == rank
           result = 1;
           break;
       end
    end
end

