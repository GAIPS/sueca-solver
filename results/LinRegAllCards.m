A = dlmread('log3.txt','\t',2,0);
AceRank = 9;
SevenRank = 8;

for i = 1:size(A,1)
   hand_0 = A(i,1:10);
   hand_2 = A(i,11:20);
   trump = A(i,21);
   firstPlayer = A(i,22);
   fp = A(i,23);
   p_0 = handPoints(hand_0);
   p_2 = handPoints(hand_2);
   pt_0 = handTrumpPoints(hand_0, trump);
   pt_2 = handTrumpPoints(hand_2, trump);
   t_0 = countSuit(hand_0, trump);
   t_2 = countSuit(hand_2, trump);
   A_0 = countRank(hand_0, AceRank);
   A_2 = countRank(hand_2, AceRank);
   x7_0 = countRank(hand_0, SevenRank);
   x7_2 = countRank(hand_2, SevenRank);
   At_02 = 0;
   if hasCard(hand_0, trump, AceRank) == 1 || hasCard(hand_1, trump, AceRank) == 1
      At_02 = 1; 
   end
   suits_0 = countSuits(hand_0);
   suits_2 = countSuits(hand_2);
   first_0 = 0;
   if firstPlayer == 0
       first_0 = 1;
   end
   first_2 = 0;
   if firstPlayer == 2
      first_2 = 1; 
   end
end
   