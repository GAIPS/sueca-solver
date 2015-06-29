function [ result ] = countSuits( hand )
    result = 0;
    lastSuit = -1;
    for i = 1:size(hand,2)
        card = hand(1,i);
    	cardSuit = getSuit(card);
    	if cardSuit ~= lastSuit
            lastSuit = cardSuit;
            result = result + 1;
       end
    end
end

