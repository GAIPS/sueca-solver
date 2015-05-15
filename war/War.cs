using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MainWar
    {

        public static void Main()
        {
            string input;
            string[] playersNames = new string[4];
            int numGames, gameMode, firstTeamWins = 0, secondTeamWins = 0, draws = 0;
            ArtificialPlayer[] players = new ArtificialPlayer[4];

            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA TEST WAR ||||||||||||||||||||");
            Console.WriteLine("");
            Console.WriteLine("[1] - 1 Bot 3 Random");
            Console.WriteLine("[2] - 2 Bot 2 Random");
            Console.WriteLine("[3] - 3 Bot 1 Random");
            Console.WriteLine("[4] - 4 Bot 0 Random");
            Console.Write("Choose an option from 1 to 4: ");
            input = Console.ReadLine();
            gameMode = Convert.ToInt32(input);
            Console.Write("How many games: ");
            input = Console.ReadLine();
            numGames = Convert.ToInt32(input);

            for (int i = 0; i < numGames; i++)
            {
                // Console.WriteLine("----------------- Game " + i + " -----------------");
                Deck deck = new Deck();
                Random randomNumber = new Random(Guid.NewGuid().GetHashCode());
                List<List<int>> playersHands = deck.SampleAll(10);
                int trump = randomNumber.Next(0, 4);
                // Console.WriteLine("Trump " + trump);
                SuecaGame game = new SuecaGame(playersHands[0], playersHands[1], playersHands[2], playersHands[3], trump, null, false);
                // game.PrintPlayersHands();
                int currentPlayerID = 0;

                switch (gameMode)
                {
                    case 1:
                        playersNames[0] = "Bot1";
                        players[0] = new SmartPlayer(playersHands[0], trump);
                        playersNames[1] = "Random1";
                        players[1] = new RandomPlayer(playersHands[1]);
                        playersNames[2] = "Random2";
                        players[2] = new RandomPlayer(playersHands[2]);
                        playersNames[3] = "Random3";
                        players[3] = new RandomPlayer(playersHands[3]);
                        break;
                    case 2:
                        playersNames[0] = "Bot1";
                        players[0] = new SmartPlayer(playersHands[0], trump);
                        playersNames[1] = "Random1";
                        players[1] = new RandomPlayer(playersHands[1]);
                        playersNames[2] = "Bot2";
                        players[2] = new SmartPlayer(playersHands[2], trump);
                        playersNames[3] = "Random2";
                        players[3] = new RandomPlayer(playersHands[3]);
                        break;
                    case 3:
                        playersNames[0] = "Bot1";
                        players[0] = new SmartPlayer(playersHands[0], trump);
                        playersNames[1] = "Bot2";
                        players[1] = new SmartPlayer(playersHands[1], trump);
                        playersNames[2] = "Bot3";
                        players[2] = new SmartPlayer(playersHands[2], trump);
                        playersNames[3] = "Random1";
                        players[3] = new RandomPlayer(playersHands[3]);
                        break;
                    case 4:
                        playersNames[0] = "Bot1";
                        players[0] = new SmartPlayer(playersHands[0], trump);
                        playersNames[1] = "Bot2";
                        players[1] = new SmartPlayer(playersHands[1], trump);
                        playersNames[2] = "Bot3";
                        players[2] = new SmartPlayer(playersHands[2], trump);
                        playersNames[3] = "Bot4";
                        players[3] = new SmartPlayer(playersHands[3], trump);
                        break;
                }

                for (int j = 0; j < 40; j++)
                {
                    int chosenCard = players[currentPlayerID].Play();
                    game.PlayCard(currentPlayerID, chosenCard);
                    // Console.WriteLine("Play " + j + " of player " + currentPlayerID + " with " + chosenCard);

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
                // Console.WriteLine("------");
                // Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " - " + points[0] + " points");
                // Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " - " + points[1] + " points");
                // Console.WriteLine("-----------------------------------------");
                // Console.WriteLine("");
                // Console.WriteLine("");
                if (points[0] == 60)
                {
                    draws++;
                }
                else if (points[0] > 60)
                {
                    firstTeamWins++;
                }
                else
                {
                    secondTeamWins++;
                }
            }

            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " won " + firstTeamWins + "/" + numGames);
            Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " won " + secondTeamWins + "/" + numGames);
            Console.WriteLine("Draws " + draws + "/" + numGames);

        }
    }
}