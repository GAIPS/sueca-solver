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
        static int[] NewMethod(int i, int[] localCount, /*object deckLock,*/ int gameMode)
        {
//            Console.WriteLine("----------------- Game " + i + " ----------------- TID: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            Random randomNumber = new Random(Guid.NewGuid().GetHashCode());
            string[] playersNames = new string[4];
            ArtificialPlayer[] players = new ArtificialPlayer[4];
            List<List<int>> playersHands;
            Deck deck = new Deck();
            playersHands = deck.SampleHands(new int[]
                {
                    10,
                    10,
                    10,
                    10
                });
            int trump = randomNumber.Next(0, 4);
            //                SuecaGame.PrintHandsReport(playersHands, trump);
            // Console.WriteLine("Trump " + trump);
            SuecaGame game = new SuecaGame(10, playersHands, trump, null);
            // game.PrintPlayersHands();
            int currentPlayerID = i % 4;
            switch (gameMode)
            {
                case 1:
                    playersNames[0] = "Bot1";
                    players[0] = new SmartPlayer(playersHands[0], trump);//, deckLock);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(playersHands[1]);
                    playersNames[2] = "Random2";
                    players[2] = new RandomPlayer(playersHands[2]);
                    playersNames[3] = "Random3";
                    players[3] = new RandomPlayer(playersHands[3]);
                    break;
                case 2:
                    playersNames[0] = "Bot1";
                    players[0] = new SmartPlayer(playersHands[0], trump);//, deckLock);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(playersHands[1]);
                    playersNames[2] = "Bot2";
                    players[2] = new SmartPlayer(playersHands[2], trump);//, deckLock);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(playersHands[3]);
                    break;
                case 3:
                    playersNames[0] = "Bot1";
                    players[0] = new SmartPlayer(playersHands[0], trump);//, deckLock);
                    playersNames[1] = "Bot2";
                    players[1] = new SmartPlayer(playersHands[1], trump);//, deckLock);
                    playersNames[2] = "Bot3";
                    players[2] = new SmartPlayer(playersHands[2], trump);//, deckLock);
                    playersNames[3] = "Random1";
                    players[3] = new RandomPlayer(playersHands[3]);
                    break;
                case 4:
                    playersNames[0] = "Bot1";
                    players[0] = new SmartPlayer(playersHands[0], trump);//, deckLock);
                    playersNames[1] = "Bot2";
                    players[1] = new SmartPlayer(playersHands[1], trump);//, deckLock);
                    playersNames[2] = "Bot3";
                    players[2] = new SmartPlayer(playersHands[2], trump);//, deckLock);
                    playersNames[3] = "Bot4";
                    players[3] = new SmartPlayer(playersHands[3], trump);//, deckLock);
                    break;
                default:
                    break;
            }
//            Console.WriteLine("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + "] ------");
//            Console.Out.Flush();
            for (int j = 0; j < 40; j++)
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
            }
            int[] points = game.GetGamePoints();
            Console.WriteLine("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + "] ----------------- Game " + i + " -----------------");
            Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " - " + points[0] + " points");
            Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " - " + points[1] + " points");
            Console.Out.Flush();
            // Console.WriteLine("-----------------------------------------");
            // Console.WriteLine("");
            // Console.WriteLine("");
            if (points[0] == 60)
            {
                localCount[0]++;
            }
            else if (points[0] > 60)
            {
                localCount[1]++;
            }
            else
            {
                localCount[2]++;
            }
            return localCount;
        }

        public static void Main()
        {
            int numGames, gameMode, firstTeamWins = 0, secondTeamWins = 0, draws = 0;

            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA TEST WAR ||||||||||||||||||||");
            Console.WriteLine("");
            Console.WriteLine("[1] - 1 Bot 3 Random");
            Console.WriteLine("[2] - 2 Bot 2 Random");
            Console.WriteLine("[3] - 3 Bot 1 Random");
            Console.WriteLine("[4] - 4 Bot 0 Random");
            Console.Write("Choose an option from 1 to 4: ");
//            input = Console.ReadLine();
//            gameMode = Convert.ToInt32(input);
            gameMode = 1;
            Console.WriteLine(gameMode);
            Console.Write("How many games: ");
//            input = Console.ReadLine();
//            numGames = Convert.ToInt32(input);
            numGames = 20;
            Console.WriteLine(numGames);

            //object resultLock = new object();
            //object deckLock = new object();
            //for (int i = 0; i < numGames; i++)
            Parallel.For(0, numGames,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                () => new int[3],

                (int i, ParallelLoopState state, int[] localCount) =>
                {
                    return NewMethod(i, localCount/*, deckLock*/, gameMode);
                    //return localCount;
                },

                (int[] localCount) =>
                {
                    //lock (resultLock)
                    //{
                    Console.WriteLine("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + "] - FINISHED ITS CHUNK!!!");
                        draws += localCount[0];
                        firstTeamWins += localCount[1];
                        secondTeamWins += localCount[2];
                    //}
                });

            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("Team 0 won " + firstTeamWins + "/" + numGames);
            Console.WriteLine("Team 1 won " + secondTeamWins + "/" + numGames);
            Console.WriteLine("Draws " + draws + "/" + numGames);

        }
    }
}