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

B = dlmread('log8.txt','\t',2,0);
numGamesB = size(B,1);
TeamFinalPointsB = zeros(numGamesB,1);
for i = 1:numGamesB
  TeamFinalPointsB(i,1) = B(i,23);
end

C = dlmread('log9.txt','\t',2,0);
numGamesC = size(C,1);
TeamFinalPointsC = zeros(numGamesC,1);
for i = 1:numGamesC
  TeamFinalPointsC(i,1) = C(i,23);
end
% winRate = (countWins / numGamesA) * 100
% drawRate = (countDraws / numGamesA) * 100


F = dlmread('log12.txt','\t',2,0);
numGamesF = size(F,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsD = zeros(numGamesF,1);
for i = 1:numGamesF
  TeamFinalPointsD(i,1) = F(i,23);
  if F(i,23) > 60
      countWins = countWins + 1;
  elseif F(i,23) == 60
      countDraws = countDraws + 1;
  end
end
% winRate = (countWins / numGamesF) * 100
% drawRate = (countDraws / numGamesF) * 100

G = dlmread('log13.txt','\t',2,0);
numGamesG = size(G,1);
countWins = 0;
countDraws = 0;
TeamFinalPointsE = zeros(numGamesG,1);
for i = 1:numGamesG
  TeamFinalPointsE(i,1) = G(i,23);
  if G(i,23) > 60
      countWins = countWins + 1;
  elseif G(i,23) == 60
      countDraws = countDraws + 1;
  end
end
% winRate = (countWins / numGamesG) * 100
% drawRate = (countDraws / numGamesG) * 100


H = dlmread('log17.txt','\t',2,0);
numGamesH = size(H,1);
countWinsH = 0;
countDrawsH = 0;
TeamFinalPointsF = zeros(numGamesH,1);
for i = 1:numGamesH
  TeamFinalPointsF(i,1) = H(i,23);
  if H(i,23) > 60
      countWinsH = countWinsH + 1;
  elseif H(i,23) == 60
      countDrawsH = countDrawsH + 1;
  end
end
% winRateH = (countWinsH / numGamesH) * 100
% drawRateH = (countDrawsH / numGamesH) * 100

I = dlmread('log15.txt','\t',2,0);
numGamesI = size(I,1);
countWinsI = 0;
countDrawsI = 0;
TeamFinalPointsG = zeros(numGamesI,1);
for i = 1:numGamesI
  TeamFinalPointsG(i,1) = I(i,23);
  if I(i,23) > 60
      countWinsI = countWinsI + 1;
  elseif I(i,23) == 60
      countDrawsI = countDrawsI + 1;
  end
end
% winRateI = (countWinsI / numGamesI) * 100
% drawRateI = (countDrawsI / numGamesI) * 100

J = dlmread('log18.txt','\t',2,0);
numGamesJ = size(J,1);
countWinsJ = 0;
countDrawsJ = 0;
TeamFinalPointsH = zeros(numGamesJ,1);
for i = 1:numGamesJ
  TeamFinalPointsH(i,1) = J(i,23);
  if J(i,23) > 60
      countWinsJ = countWinsJ + 1;
  elseif J(i,23) == 60
      countDrawsJ = countDrawsJ + 1;
  end
end
% winRateJ = (countWinsJ / numGamesJ) * 100
% drawRateJ = (countDrawsJ / numGamesJ) * 100

K = dlmread('log16.txt','\t',2,0);
numGamesK = size(K,1);
countWinsK = 0;
countDrawsK = 0;
TeamFinalPointsI = zeros(numGamesK,1);
for i = 1:numGamesK
  TeamFinalPointsI(i,1) = K(i,23);
  if K(i,23) > 60
      countWinsK = countWinsK + 1;
  elseif K(i,23) == 60
      countDrawsK = countDrawsK + 1;
  end
end
% winRateK = (countWinsK / numGamesK) * 100
% drawRateK = (countDrawsK / numGamesK) * 100

figure(1);
% a = histfit(TeamFinalPointsA,60);
% a(1).FaceColor = [0.2 0.4 0.2]
% delete(a(2));
% axis([0 120 0 45]);
% h = histfit(TeamFinalPointsF,60)
% h(2).Color = [1 0.84 0];
% delete(h(1));
% hold on;
% j = histfit(TeamFinalPointsF,60);
% j(2).Color = [1 0.84 0];
% delete(j(1));
% k = histfit(TeamFinalPointsG,60);
% k(2).Color = [1 0.42 0];
% delete(k(1));
% l = histfit(TeamFinalPointsB,8);
% l(2).Color = [1 0 0];
% delete(l(1));
% hold on;
% n = histfit(TeamFinalPointsC,8);
% n(2).Color = [0.2 0.5 1];
% delete(n(1));
% axis([0 120 0 300]);
% xlabel('Game final points');
% ylabel('Number of games');
% set(gca,'FontSize',12);
% figure(2);
m = histfit(TeamFinalPointsA,8);
m(1).FaceColor = [0.2 0.4 0.2];
m(2).Color = [0 0 0];
% delete(m(2));
% hold on;
% o = histfit(TeamFinalPointsG,8);
% o(2).Color = [0.36 0.14 0.9];
% delete(o(1));
axis([0 120 0 300]);
xlabel('Game final points');
ylabel('Number of games');
set(gca,'XTick', [0 15 30 45 60 75 90 105 120]);
set(gca,'FontSize',12);