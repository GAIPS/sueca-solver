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

        public const int GAMEMODE = 6;
        public const int NUMGAMES = 100;

        public static void Main()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int numGames = NUMGAMES, gameMode = GAMEMODE;
            int firstTeamWins = 0, secondTeamWins = 0, draws = 0, nullGames = 0;
            
            //Shared data between threads!
            List<int> initialBotTeamPoints = new List<int>(numGames);
            List<int> finalBotTeamPoints = new List<int>(numGames);
            List<int> initialBotTeamPointsWonGames = new List<int>();
            List<int> initialBotTeamPointsLostGames = new List<int>();
            Object allGamesLock = new Object();
            Object wonGamesLock = new Object();
            Object lostGamesLock = new Object();

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
                default:
                    break;
            }
            Console.WriteLine("#Games: " + numGames);


            Parallel.For(0, numGames,
                new ParallelOptions { MaxDegreeOfParallelism = 5 },
                () => new int[6],

                (int i, ParallelLoopState state, int[] localCount) =>
                {
                    return processGames(i, localCount, gameMode, initialBotTeamPointsWonGames, initialBotTeamPointsLostGames, initialBotTeamPoints, finalBotTeamPoints, allGamesLock, wonGamesLock, lostGamesLock);
                },

                (int[] localCount) =>
                {
                    draws += localCount[0];
                    firstTeamWins += localCount[1];
                    secondTeamWins += localCount[2];
                    nullGames += localCount[3];
                });

            //for (int i = 0; i < numGames; i++)
            //{
            //    int[] localCount = new int[4];
            //    processGames(i, localCount, gameMode);
            //    draws += localCount[0];
            //    firstTeamWins += localCount[1];
            //    secondTeamWins += localCount[2];
            //    badGames += localCount[3];
            //}


            //append information on a file!
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(@"C:\temp");
            int count = dir.GetFiles().Length;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\PiTiPf-" + count + ".txt"))
            {
                for (int i = 0; i < numGames; i++)
                {
                    file.WriteLine(initialBotTeamPoints[i] + "\t" + finalBotTeamPoints[i]);
                }
            }


            double initialBotTeamPointsWonGamesAVG = initialBotTeamPointsWonGames.Average();
            double sumOfSquaresOfDifferencesW = initialBotTeamPointsWonGames.Select(val => (val - initialBotTeamPointsWonGamesAVG) * (val - initialBotTeamPointsWonGamesAVG)).Sum();
            double sdW = Math.Sqrt(sumOfSquaresOfDifferencesW / initialBotTeamPointsWonGames.Count);
            double initialBotTeamPointsLostGamesAVG = initialBotTeamPointsLostGames.Average();
            double sumOfSquaresOfDifferencesL = initialBotTeamPointsWonGames.Select(val => (val - initialBotTeamPointsLostGamesAVG) * (val - initialBotTeamPointsLostGamesAVG)).Sum();
            double sdL = Math.Sqrt(sumOfSquaresOfDifferencesL / initialBotTeamPointsWonGames.Count);

            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("BotTeam won " + firstTeamWins + "/" + numGames);
            Console.WriteLine("OtherTeam 1 won " + secondTeamWins + "/" + numGames);
            Console.WriteLine("Draws " + draws + "/" + numGames);
            Console.WriteLine("Null Games " + nullGames);
            Console.WriteLine("Initial BotTeam Points in won games: AVG-" + initialBotTeamPointsWonGamesAVG + " SD-" + sdW);
            Console.WriteLine("Initial BotTeam Points in lost games: AVG-" + initialBotTeamPointsLostGamesAVG + " SD-" + sdL);
            
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

        static int[] processGames(int i, int[] localCount, int gameMode, List<int> initialBotTeamPointsWonGames, List<int> initialBotTeamPointsLostGames, List<int> initialBotTeamPoints, List<int> finalBotTeamPoints, Object allGamesLock, Object wonGamesLock, Object lostGamesLock)
        {
            //int seed = -1150905530;
            //int seed = -373600003;
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
            //Console.WriteLine("LOL: " + Guid.NewGuid().GetHashCode());

            //SuecaGame.PrintCards("p0", playersHands[0]);
            //SuecaGame.PrintCards("p1", playersHands[1]);
            //SuecaGame.PrintCards("p2", playersHands[2]);
            //SuecaGame.PrintCards("p3", playersHands[3]);
            //Console.WriteLine("----------------------------------------------");
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
                    players[1] = new RandomPlayer(1, playersHands[1], randomNumber);
                    playersNames[2] = "Smartest2";
                    players[2] = new SmartestPlayer(2, playersHands[2], trump, randomNumber, seed);
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
            //Console.WriteLine("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + "] ----------------- Game " + i + " -----------------");
            //Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " - " + points[0] + " points");
            //Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " - " + points[1] + " points");
            //Console.Out.Flush();

            if (points[0] == 60)
            {
                localCount[0]++;
            }
            else if (points[0] > 60)
            {
                localCount[1]++;
                localCount[4] += botTeamInitialPoints;
                lock (wonGamesLock)
                {
                    initialBotTeamPointsWonGames.Add(botTeamInitialPoints);
                }
            }
            else
            {
                localCount[2]++;
                localCount[5] += botTeamInitialPoints;
                lock (lostGamesLock)
                {
                    initialBotTeamPointsLostGames.Add(botTeamInitialPoints);
                }
            }

            lock (allGamesLock)
            {
                initialBotTeamPoints.Add(botTeamInitialPointsWithTrumps);
                //finalBotTeamPoints.Add(botTeamFinalPointsWithTrumps);
                finalBotTeamPoints.Add(points[0]);
            }
            
                return localCount;
        }
    }
}