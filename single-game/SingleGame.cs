using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MainSuecaSolver
    {

        public static void Main()
        {
            string input;
            string[] playersNames = new string[4];
            playersNames[0] = "Bot";
            int firstPlayerID;
            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA GAME ||||||||||||||||||||");
            Console.WriteLine("");

            Console.WriteLine("Player 0: Bot");
            Console.Write("Player 1: ");
            playersNames[1] = Console.ReadLine();
            Console.Write("Player 2: ");
            playersNames[2] = Console.ReadLine();
            Console.Write("Player 3: ");
            playersNames[3] = Console.ReadLine();
            Console.Write("First player ID: ");
            input = Console.ReadLine();
            firstPlayerID = Convert.ToInt32(input);
            Console.WriteLine("");

            Deck deck = new Deck();
            Random randomNumber = new Random(Guid.NewGuid().GetHashCode());
            List<List<int>> playersHand = deck.SampleAll(10);
            List<int> currentHand;
            int trump = randomNumber.Next(0, 4);
            int cardIndex, currentPlayerID = firstPlayerID;

            SmartPlayer artificialPlayer = new SmartPlayer(playersHand[0], trump);
            SuecaGame game = new SuecaGame(10, playersHand[0], playersHand[1], playersHand[2], playersHand[3], trump, null);
            game.PrintPlayersHands();

            for (int i = 0; i < 40; i++)
            {
                currentHand = playersHand[currentPlayerID];
                Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||");
                Console.WriteLine("                 Trick " + (i / 4));
                Console.WriteLine("                 Player " + currentPlayerID + " - " + playersNames[currentPlayerID]);
                Console.WriteLine("                 Trump is " + trump);
                Console.WriteLine("");

                game.PrintLastTrick();
                game.PrintCurrentTrick();
                SuecaGame.PrintHand(currentHand);
                int chosenCard;

                if (currentPlayerID != 0)
                {
                    Console.Write("Pick the card you want to play by its index: ");
                    input = Console.ReadLine();
                    cardIndex = Convert.ToInt32(input);
                    chosenCard = currentHand[cardIndex];
                    artificialPlayer.AddPlay(currentPlayerID, chosenCard);
                }
                else
                {
                    chosenCard = artificialPlayer.Play();
                }

                game.PlayCard(currentPlayerID, chosenCard);
                currentHand.Remove(chosenCard);
                currentPlayerID = game.GetNextPlayerId();
            }

            Console.WriteLine("|||||||||||||||||||||||| END |||||||||||||||||||||||");
            Console.WriteLine("");
            int[] points = game.GetGamePoints();
            Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " - " + points[0] + " points");
            Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " - " + points[1] + " points");
            // game.PrintPoints(playersNames);
            Console.WriteLine("");
            Console.ReadLine();

        }
    }
}