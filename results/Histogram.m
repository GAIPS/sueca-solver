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


F = dlmread('log12.txt','\t',2,0);
numGamesF = size(F,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsF = zeros(numGamesF,1);
for i = 1:numGamesF
  TeamFinalPointsF(i,1) = F(i,23);
  if F(i,23) > 60
      countWins = countWins + 1;
  elseif F(i,23) == 60
      countDraws = countDraws + 1;
  end
end
winRate = (countWins / numGamesF) * 100
drawRate = (countDraws / numGamesF) * 100

G = dlmread('log13.txt','\t',2,0);
numGamesG = size(G,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsG = zeros(numGamesG,1);
for i = 1:numGamesG
  TeamFinalPointsG(i,1) = G(i,23);
  if G(i,23) > 60
      countWins = countWins + 1;
  elseif G(i,23) == 60
      countDraws = countDraws + 1;
  end
end
winRate = (countWins / numGamesG) * 100
drawRate = (countDraws / numGamesG) * 100

figure(1);
% a = histfit(TeamFinalPointsA,60);
% a(1).FaceColor = [0.2 0.4 0.2]
% delete(a(2));
% axis([0 120 0 45]);
h = histfit(TeamFinalPointsA,60)
h(2).Color = [0.2 0.4 0.2];
delete(h(1));
hold on;
j = histfit(TeamFinalPointsF,60);
j(2).Color = [1 0.84 0];
delete(j(1));
k = histfit(TeamFinalPointsG,60);
k(2).Color = [1 0.42 0];
delete(k(1));
axis([0 120 0 45]);
set(gca,'FontSize',12)