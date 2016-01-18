A = dlmread('u1-Half\u1-Half.txt','\t',2,0);
numGames = size(A,1);

TeamFinalPoints = zeros(numGames,1);
numWonGames = 0;
numDrawnGames = 0;

for i = 1:numGames
   TeamFinalPoints(i,1) = A(i,30);
   if TeamFinalPoints(i,1) > 60
       numWonGames = numWonGames + 1;
   elseif TeamFinalPoints(i,1) == 60
       numDrawnGames = numDrawnGames + 1;
   end
end


WonRate = (numWonGames / numGames) * 100
DrawnRate = (numDrawnGames / numGames) * 100
FGR = WonRate + DrawnRate

PointsMean = mean(TeamFinalPoints)
PointsSD = std(TeamFinalPoints)

