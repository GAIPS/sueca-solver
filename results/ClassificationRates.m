A = dlmread('log12.txt','\t',2,0);
numGames = size(A,1);
AceRank = 9;
SevenRank = 8;

TeamFinalPoints = zeros(numGames,1);
teamNumTrumps = zeros(numGames,1);
teamNumAces = zeros(numGames,1);
teamNumSevens = zeros(numGames,1);

numHardGames = 0;
numHardWonDrew = 0;
numMediumGames = 0;
numMediumWonDrew = 0;
numEasyGames = 0;
numEasyWonDrew = 0;

for i = 1:numGames
   hand_0 = A(i,1:10);
   hand_2 = A(i,11:20);
   trump = A(i,21);
   firstPlayer = A(i,22);
   
   TeamFinalPoints(i,1) = A(i,23);
   teamNumTrumps(i,1) = countSuit(hand_0, trump) + countSuit(hand_2, trump);
   teamNumAces(i,1) = countRank(hand_0, AceRank) + countRank(hand_2, AceRank);
   teamNumSevens(i,1) = countRank(hand_0, SevenRank) + countRank(hand_2, SevenRank);
   
   if teamNumTrumps(i,1) <= 4 && teamNumAces(i,1) <= 1 && teamNumSevens(i,1) <= 1
       numHardGames = numHardGames + 1;
       if TeamFinalPoints(i,1) >= 60
           numHardWonDrew = numHardWonDrew + 1;
       end
   elseif teamNumTrumps(i,1) >= 6 && teamNumAces(i,1) >= 3 && teamNumSevens(i,1) >= 3
       numEasyGames = numEasyGames + 1;
       if TeamFinalPoints(i,1) >= 60
           numEasyWonDrew = numEasyWonDrew + 1;
       end
   else
       numMediumGames = numMediumGames + 1;
       if TeamFinalPoints(i,1) >= 60
           numMediumWonDrew = numMediumWonDrew + 1;
       end
   end
end


numHardGames
numMediumGames
numEasyGames
HardWonDrewRate = (numHardWonDrew / numHardGames) * 100
MediumWonDrewRate = (numMediumWonDrew / numMediumGames) * 100
EasyWonDrewRate = (numEasyWonDrew / numEasyGames) * 100

WonDrew = (numHardWonDrew + numMediumWonDrew + numEasyWonDrew) / 1000


