A = dlmread('log3.txt','\t',2,0);


for i = 1:size(A,1)
   hand_0 = A(i,1:10);
   hand_2 = A(i,11:20);
   trump = A(i,21);
   first = A(i,22);
   fp = A(i,23);
   p_0 = handPoints(hand_0);
   p_2 = handPoints(hand_2);
   pt_0 = handTrumpPoints(hand_0, trump);
   pt_2 = handTrumpPoints(hand_2, trump);
end
   