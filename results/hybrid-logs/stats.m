A = dlmread('TeamN5M5/TeamN5M5.txt','\t',2,0);
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
% t1M = mean(Player0TimeInTrick1)
% t1SD = std(Player0TimeInTrick1)
% t2M = mean(Player0TimeInTrick2)
% t2SD = std(Player0TimeInTrick2)
% t3M = mean(Player0TimeInTrick3)
% t3SD = std(Player0TimeInTrick3)
% t4M = mean(Player0TimeInTrick4)
% t4SD = std(Player0TimeInTrick4)
% t5M = mean(Player0TimeInTrick5)
% t5SD = std(Player0TimeInTrick5)
% t6M = mean(Player0TimeInTrick6)
% t6SD = std(Player0TimeInTrick6)
% t7M = mean(Player0TimeInTrick7)
% t7SD = std(Player0TimeInTrick7)
% t8M = mean(Player0TimeInTrick8)
% t8SD = std(Player0TimeInTrick8)
% t9M = mean(Player0TimeInTrick9)
% t9SD = std(Player0TimeInTrick9)
% t10M = mean(Player0TimeInTrick10)
% t10SD = std(Player0TimeInTrick10)