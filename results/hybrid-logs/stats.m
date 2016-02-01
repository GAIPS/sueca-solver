A = dlmread('N1M1/N1M1.txt','\t',2,0);
numGames = size(A,1);

TeamFinalPoints = zeros(numGames,1);
Player0TimeInTrick1 = zeros(numGames,1);
Player0TimeInTrick2 = zeros(numGames,1);
Player0TimeInTrick3 = zeros(numGames,1);
Player0TimeInTrick4 = zeros(numGames,1);
Player0TimeInTrick5 = zeros(numGames,1);
Player0TimeInTrick6 = zeros(numGames,1);
Player0TimeInTrick7 = zeros(numGames,1);
Player0TimeInTrick8 = zeros(numGames,1);
Player0TimeInTrick9 = zeros(numGames,1);
Player0TimeInTrick10 = zeros(numGames,1);
numWonGames = 0;
numDrawnGames = 0;

for i = 1:numGames
   TeamFinalPoints(i,1) = A(i,23);
   if TeamFinalPoints(i,1) > 60
       numWonGames = numWonGames + 1;
   elseif TeamFinalPoints(i,1) == 60
       numDrawnGames = numDrawnGames + 1;
   end
   Player0TimeInTrick1(i,1) = A(i,24);
   Player0TimeInTrick2(i,1) = A(i,25);
   Player0TimeInTrick3(i,1) = A(i,26);
   Player0TimeInTrick4(i,1) = A(i,27);
   Player0TimeInTrick5(i,1) = A(i,28);
   Player0TimeInTrick6(i,1) = A(i,29);
   Player0TimeInTrick7(i,1) = A(i,30);
   Player0TimeInTrick8(i,1) = A(i,31);
   Player0TimeInTrick9(i,1) = A(i,32);
   Player0TimeInTrick10(i,1) = A(i,33);
end


WonRate = (numWonGames / numGames) * 100
DrawnRate = (numDrawnGames / numGames) * 100
FGR = WonRate + DrawnRate

PointsMean = mean(TeamFinalPoints)
PointsSD = std(TeamFinalPoints)
t1M = mean(Player0TimeInTrick1)
t1SD = std(Player0TimeInTrick1)