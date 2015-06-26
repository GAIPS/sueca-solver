function [ result ] = countSuits( hand )
    result = 0;
    lastSuit = -1;
    for i = 1:size(hand,2)
       cardSuit = getSuit(card);
       if cardSuit ~= lastSuit
           lastSuit = cardSuit;
           result = result + 1;
       end
    end
end

