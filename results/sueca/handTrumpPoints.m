function [ result ] = handTrumpPoints( hand, trump )
    result = 0;
    for i = 1:size(hand,2)
       card = hand(1,i);
       if getSuit(card) == trump
            result = result + getPoints(card);
       end
    end
end

