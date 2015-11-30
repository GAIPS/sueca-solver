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


H = dlmread('log17.txt','\t',2,0);
numGamesH = size(H,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsH = zeros(numGamesH,1);
for i = 1:numGamesH
  TeamFinalPointsH(i,1) = H(i,23);
  if H(i,23) > 60
      countWins = countWins + 1;
  elseif H(i,23) == 60
      countDraws = countDraws + 1;
  end
end
winRate = (countWins / numGamesH) * 100
drawRate = (countDraws / numGamesH) * 100

I = dlmread('log15.txt','\t',2,0);
numGamesI = size(I,1);
TeamFinalPointsI = zeros(numGamesI,1);
for i = 1:numGamesI
  TeamFinalPointsI(i,1) = I(i,23);
end

J = dlmread('log18.txt','\t',2,0);
numGamesJ = size(J,1);
TeamFinalPointsJ = zeros(numGamesJ,1);
for i = 1:numGamesJ
  TeamFinalPointsJ(i,1) = J(i,23);
end

K = dlmread('log16.txt','\t',2,0);
numGamesK = size(K,1);
TeamFinalPointsK = zeros(numGamesK,1);
for i = 1:numGamesK
  TeamFinalPointsK(i,1) = K(i,23);
end

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
l = histfit(TeamFinalPointsH,60);
l(2).Color = [1 0.11 0.52];
delete(l(1));
m = histfit(TeamFinalPointsI,60);
m(2).Color = [0.36 0.14 0.9];
delete(m(1));
n = histfit(TeamFinalPointsJ,60);
n(2).Color = [0.15 0.95 0.95];
delete(n(1));
o = histfit(TeamFinalPointsK,60);
o(2).Color = [0.21 0.87 0.08];
delete(o(1));
axis([0 120 0 45]);
set(gca,'FontSize',12)