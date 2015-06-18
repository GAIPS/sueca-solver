using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SuecaSolver
{
    public class War
    {

        public const int GAMEMODE = 2;
        public const int NUMGAMES = 10;
        public const bool PARALLEL = true;
        public const bool SAVE_RESULTS = false;

        public static void Main()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int numGames = NUMGAMES, gameMode = GAMEMODE;
            int firstTeamWins = 0, secondTeamWins = 0, draws = 0, nullGames = 0;
            
            //Shared data between threads!
            List<int> initialBotTeamPoints = new List<int>(numGames);
            List<int> finalBotTeamPoints = new List<int>(numGames);
            Object allGamesLock = new Object();

            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA TEST WAR ||||||||||||||||||||");
            Console.WriteLine("");
            switch (gameMode)
            {
                case 1:
                    Console.WriteLine("Mode 1 (1 Smart 3 Random)");
                    break;
                case 2:
                    Console.WriteLine("Mode 2 (2 Smart 2 Random)");
                    break;
                case 3:
                    Console.WriteLine("Mode 3 (3 Smart 1 Random)");
                    break;
                case 4:
                    Console.WriteLine("Mode 4 (4 Smart)");
                    break;
                case 5:
                    Console.WriteLine("Mode 5 (2 Smart 2 RuleBased)");
                    break;
                case 6:
                    Console.WriteLine("Mode 6 (2 Smartest 2 Random)");
                    break;
                case 7:
                    Console.WriteLine("Mode 7 (2 Smartest 2 RuleBased)");
                    break;
                case 8:
                    Console.WriteLine("Mode 8 (2 RuleBased 2 Random)");
                    break;
                default:
                    break;
            }
            Console.WriteLine("#Games: " + numGames);


            if (PARALLEL)
            {
                Parallel.For(0, numGames,
                    new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    () => new int[6],

                    (int i, ParallelLoopState state, int[] localCount) =>
                    {
                        return processGames(i, localCount, gameMode, initialBotTeamPoints, finalBotTeamPoints, allGamesLock);
                    },

                    (int[] localCount) =>
                    {
                        draws += localCount[0];
                        firstTeamWins += localCount[1];
                        secondTeamWins += localCount[2];
                        nullGames += localCount[3];
                    });
            }
            else
            {
                for (int i = 0; i < numGames; i++)
                {
                    int[] localCount = new int[4];
                    processGames(i, localCount, gameMode, initialBotTeamPoints, finalBotTeamPoints, allGamesLock);
                    draws += localCount[0];
                    firstTeamWins += localCount[1];
                    secondTeamWins += localCount[2];
                    nullGames += localCount[3];
                }
            }


            if (SAVE_RESULTS)
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(@"C:\temp");
                int count = dir.GetFiles().Length;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\PiTiPf-" + count + ".txt"))
                {
                    for (int i = 0; i < numGames; i++)
                    {
                        file.WriteLine(initialBotTeamPoints[i] + "\t" + finalBotTeamPoints[i]);
                    }
                }
            }


            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("BotTeam won " + firstTeamWins + "/" + numGames);
            Console.WriteLine("OtherTeam 1 won " + secondTeamWins + "/" + numGames);
            Console.WriteLine("Draws " + draws + "/" + numGames);
            Console.WriteLine("Null Games " + nullGames);
            
            sw.Stop();
            Console.WriteLine("Total Time taken by functions is {0} seconds", sw.ElapsedMilliseconds / 1000); //seconds
            Console.WriteLine("Total Time taken by functions is {0} minutes", sw.ElapsedMilliseconds / 60000); //minutes
        }

        static bool checkHands(List<List<int>> hands, int trump)
        {
            for (int i = 0; i < hands.Count; i++)
            {
                int points = 0;
                int trumps = 0;
                for (int j = 0; j < hands[i].Count; j++)
                {
                    int card = hands[i][j];
                    points += Card.GetValue(card);
                    if (Card.GetSuit(card) == trump)
                    {
                        trumps++;
                    }
                }

                if (trumps == 0 && points < 10)
                {
                    return false;
                }
            }
            return true;
        }

        static int[] processGames(int i, int[] localCount, int gameMode, List<int> initialBotTeamPoints, List<int> finalBotTeamPoints, Object allGamesLock)
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random randomNumber = new Random(seed);
            string[] playersNames = new string[4];
            ArtificialPlayer[] players = new ArtificialPlayer[4];
            List<List<int>> playersHands;

            Deck deck = new Deck(randomNumber, seed);
            int trump = randomNumber.Next(0, 4);
            playersHands = deck.SampleHands(new int[] { 10, 10, 10, 10 });
            while (!checkHands(playersHands, trump))
            {
                playersHands = deck.SampleHands(new int[] { 10, 10, 10, 10 });
                localCount[3]++;
            }

            int botTeamInitialPoints = SuecaGame.CountPoints(playersHands[0]) + SuecaGame.CountPoints(playersHands[2]);
            int botTeamInitialPointsWithTrumps = SuecaGame.CountPointsWithTrumps(playersHands[0], trump) + SuecaGame.CountPointsWithTrumps(playersHands[2], trump);
            int botTeamFinalPointsWithTrumps = 0;
            SuecaGame game = new SuecaGame(10, playersHands, trump, null, 0, 0);
            int currentPlayerID = i % 4;

            switch (gameMode)
            {
                case 1:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1], randomNumber);
                    playersNames[2] = "Random2";
                    players[2] = new RandomPlayer(2, playersHands[2], randomNumber);
                    playersNames[3] = "Random3";
                    players[3] = new RandomPlayer(3, playersHands[3], randomNumber);
                    break;
                case 2:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1], randomNumber);
                    playersNames[2] = "Smart2";
                    players[2] = new SmartPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3], randomNumber);
                    break;
                case 3:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Smart2";
                    players[1] = new SmartPlayer(1, playersHands[1], trump, randomNumber, seed);
                    playersNames[2] = "Smart3";
                    players[2] = new SmartPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "Random1";
                    players[3] = new RandomPlayer(3, playersHands[3], randomNumber);
                    break;
                case 4:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Smart2";
                    players[1] = new SmartPlayer(1, playersHands[1], trump, randomNumber, seed);
                    playersNames[2] = "Smart3";
                    players[2] = new SmartPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "Smart4";
                    players[3] = new SmartPlayer(3, playersHands[3], trump, randomNumber, seed);
                    break;
                case 5:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump, randomNumber, seed);
                    playersNames[2] = "Smart2";
                    players[2] = new SmartPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "RuleBased2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump, randomNumber, seed);
                    break;
                case 6:
                    playersNames[0] = "Smartest1";
                    players[0] = new SmartestPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Random1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump, randomNumber, seed);
                    playersNames[2] = "Smartest2";
                    players[2] = new SmartestPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "Random2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump, randomNumber, seed);
                    break;
                case 7:
                    playersNames[0] = "Smartest1";
                    players[0] = new SmartestPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump, randomNumber, seed);
                    playersNames[2] = "Smartest2";
                    players[2] = new SmartestPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "RuleBased2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump, randomNumber, seed);
                    break;
                case 8:
                    playersNames[0] = "RuleBased1";
                    players[0] = new RuleBasedPlayer(0, playersHands[0], trump, randomNumber, seed);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1], randomNumber);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump, randomNumber, seed);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3], randomNumber);
                    break;
                default:
                    break;
            }

            
            for (int trickPoints = 0, j = 0; j < 40;)
            {
                int chosenCard = players[currentPlayerID].Play();
                game.PlayCard(currentPlayerID, chosenCard);
                for (int k = 0; k < 4; k++)
                {
                    if (k != currentPlayerID)
                    {
                        players[k].AddPlay(currentPlayerID, chosenCard);
                    }
                }
                currentPlayerID = game.GetNextPlayerId();
                
                trickPoints += Card.GetValue(chosenCard);
                if (Card.GetSuit(chosenCard) == trump)
                {
                    trickPoints += 10;
                }
                
                j++;
                if (j % 4 == 0)
                {
                    if (currentPlayerID == 0 || currentPlayerID == 2)
                    {
                        botTeamFinalPointsWithTrumps += trickPoints;
                    }
                    trickPoints = 0;
                }
            }

            int[] points = game.GetGamePoints();
            if (points[0] == 60)
            {
                localCount[0]++;
            }
            else if (points[0] > 60)
            {
                localCount[1]++;
                localCount[4] += botTeamInitialPoints;
            }
            else
            {
                localCount[2]++;
                localCount[5] += botTeamInitialPoints;
            }

            if (SAVE_RESULTS)
            {
                lock (allGamesLock)
                {
                    initialBotTeamPoints.Add(botTeamInitialPointsWithTrumps);
                    finalBotTeamPoints.Add(botTeamFinalPointsWithTrumps);
                    //finalBotTeamPoints.Add(points[0]);
                } 
            }
            
                return localCount;
        }
    }
}