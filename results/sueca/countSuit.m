function [ result ] = countSuit( hand, suit )
    result = 0;
    for i = 1:size(hand,2)
       card = hand(1,i);
       if getSuit(card) == suit
            result = result + 1;
       end
    end
end

