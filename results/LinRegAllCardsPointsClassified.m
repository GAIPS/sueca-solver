A = dlmread('log7.txt','\t',2,0);
numGames = size(A,1);
AceRank = 9;
SevenRank = 8;

TeamFinalPoints = zeros(numGames,1);
teamNumTrumps = zeros(numGames,1);
teamNumAces = zeros(numGames,1);
teamNumSevens = zeros(numGames,1);
p_0 = zeros(numGames,1);
p_2 = zeros(numGames,1);
pt_0 = zeros(numGames,1);
pt_2 = zeros(numGames,1);
t_0 = zeros(numGames,1);
t_2 = zeros(numGames,1);
A_0 = zeros(numGames,1);
A_2 = zeros(numGames,1);
x7_0 = zeros(numGames,1);
x7_2 = zeros(numGames,1);
At_02 = zeros(numGames,1);
hAt_0 = zeros(numGames,1);
hAt_2 = zeros(numGames,1);
suits_0 = zeros(numGames,1);
suits_2 = zeros(numGames,1);
first_0 = zeros(numGames,1);
first_2 = zeros(numGames,1);

for i = 1:numGames
   hand_0 = A(i,1:10);
   hand_2 = A(i,11:20);
   trump = A(i,21);
   firstPlayer = A(i,22);
   
   TeamFinalPoints(i,1) = A(i,23);
   p_0(i,1) = handPoints(hand_0);
   p_2(i,1) = handPoints(hand_2);
   pt_0(i,1) = handTrumpPoints(hand_0, trump);
   pt_2(i,1) = handTrumpPoints(hand_2, trump);
   t_0(i,1) = countSuit(hand_0, trump);
   t_2(i,1) = countSuit(hand_2, trump);
   A_0(i,1) = countRank(hand_0, AceRank);
   A_2(i,1) = countRank(hand_2, AceRank);
   x7_0(i,1) = countRank(hand_0, SevenRank);
   x7_2(i,1) = countRank(hand_2, SevenRank);
   At_02(i,1) = 0;
   if hasCard(hand_0, trump, AceRank) == 1 || hasCard(hand_2, trump, AceRank) == 1
      At_02(i,1) = 1; 
   end
   hAt_0 = hasCard(hand_0, trump, AceRank);
   hAt_2 = hasCard(hand_2, trump, AceRank);
   suits_0(i,1) = countSuits(hand_0);
   suits_2(i,1) = countSuits(hand_2);
   first_0(i,1) = 0;
   if firstPlayer == 0
       first_0(i,1) = 1;
   end
   first_2(i,1) = 0;
   if firstPlayer == 2
      first_2(i,1) = 1; 
   end
   teamNumTrumps(i,1) = countSuit(hand_0, trump) + countSuit(hand_2, trump);
   teamNumAces(i,1) = countRank(hand_0, AceRank) + countRank(hand_2, AceRank);
   teamNumSevens(i,1) = countRank(hand_0, SevenRank) + countRank(hand_2, SevenRank);
end


tbl = table(p_0, p_2, pt_0, pt_2, t_0, t_2, A_0, A_2, x7_0, x7_2, At_02, suits_0, suits_2, first_0, first_2, teamNumTrumps, teamNumAces, teamNumSevens, TeamFinalPoints, 'VariableNames', {'p_0', 'p_2', 'pt_0', 'pt_2', 't_0', 't_2', 'A_0', 'A_2', 'x7_0', 'x7_2', 'At_02', 'suits_0', 'suits_2', 'first_0', 'first_2', 'teamNumTrumps', 'teamNumAces', 'teamNumSevens', 'TeamFinalPoints'});
%mdl = fitlm(tbl,'TeamFinalPoints ~ p_0 + p_2 + pt_0 + pt_2 + t_0 + t_2 + A_0 + A_2 + x7_0 + x7_2 + At_02 + teamNumTrumps + teamNumAces + teamNumSevens')
mdl = fitlm(tbl,'TeamFinalPoints ~ teamNumTrumps + teamNumAces + teamNumSevens')
%mdl = stepwiselm(tbl,'fp ~ t_0 + t_2 + A_0 + A_2 + x7_0 + x7_2')
result = table2array(mdl.Coefficients);
save('LinRegResult5.txt', 'result', '-ascii');
%h = plotAdded(mdl, 'teamNumTrumps')
j = plotAdded(mdl, 'teamNumAces')
%k = plotAdded(mdl, 'teamNumSevens')
axis([-0.5,4.5,0,120])
line([-0.5 4.5],[60 60], 'Color', 'black')
%h = plotSlice(mdl)
