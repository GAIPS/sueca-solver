function [ result ] = handPoints( hand )
    result = 0;
    for i = 1:size(hand,2)
       card = hand(1,i);
       result = result + getPoints(card);
    end
end

