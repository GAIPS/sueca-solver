A = dlmread('log7.txt','\t',2,0);
numGamesA = size(A,1);
TeamFinalPointsA = zeros(numGamesA,1);
countWins = 0;
countDraws = 0;
for i = 1:numGamesA
  TeamFinalPointsA(i,1) = A(i,23);
  if A(i,23) > 60
      countWins = countWins + 1;
  elseif A(i,23) == 60
      countDraws = countDraws + 1;
  end
end

winRate = (countWins / numGamesA) * 100
drawRate = (countDraws / numGamesA) * 100


B = dlmread('log8.txt','\t',2,0);
numGamesB = size(B,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsB = zeros(numGamesB,1);
for i = 1:numGamesB
  TeamFinalPointsB(i,1) = B(i,23);
  if B(i,23) > 60
      countWins = countWins + 1;
  elseif B(i,23) == 60
      countDraws = countDraws + 1;
  end
end
winRate = (countWins / numGamesB) * 100
drawRate = (countDraws / numGamesB) * 100

C = dlmread('log9.txt','\t',2,0);
numGamesC = size(C,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsC = zeros(numGamesC,1);
for i = 1:numGamesC
  TeamFinalPointsC(i,1) = C(i,23);
  if C(i,23) > 60
      countWins = countWins + 1;
  elseif C(i,23) == 60
      countDraws = countDraws + 1;
  end
end
winRate = (countWins / numGamesC) * 100
drawRate = (countDraws / numGamesC) * 100

figure(1);
a = histfit(TeamFinalPointsA,60);
a(1).FaceColor = [0.2 0.4 0.2]
delete(a(2));
axis([0 120 0 45]);

figure(2);
b = histfit(TeamFinalPointsB,60);
b(1).FaceColor = [0.2 0.5 1]
delete(b(2));
axis([0 120 0 45]);

figure(3);
c = histfit(TeamFinalPointsC,60);
c(1).FaceColor = [1 0 0]
delete(c(2));
axis([0 120 0 45]);

figure(4);
h = histfit(TeamFinalPointsA,60)
h(2).Color = [0.2 0.4 0.2];
delete(h(1));
hold on;
g = histfit(TeamFinalPointsB,60);
g(2).Color = [0.2 0.5 1];
delete(g(1));
i = histfit(TeamFinalPointsC,60);
i(2).Color = [1 0 0];
delete(i(1));
axis([0 120 0 45]);