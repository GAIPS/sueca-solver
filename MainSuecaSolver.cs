using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class MainSuecaSolver
	{

		public static void Main ()
		{
			string input;
			string[] playersNames = new string[4];
			playersNames[0] = "Bot";
			int firstPlayerID, N;
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
			Console.Write("N param for PIMC: ");
			input = Console.ReadLine();
			N = Convert.ToInt32(input);

			Console.WriteLine("");

			Deck deck = new Deck();
			Random randomNumber = new Random(Guid.NewGuid().GetHashCode());
			List<List<Card>> playersHand = deck.SampleAll(10);
			List<Card> currentHand;
			Suit trump = (Suit) randomNumber.Next(0, 4);
			int cardIndex, currentPlayerID = firstPlayerID;

			ArtificialPlayer artificialPlayer = new ArtificialPlayer(playersHand[0], trump, N);
			SuecaGame game = new SuecaGame(playersHand[0].ToArray(), playersHand[1].ToArray(), playersHand[2].ToArray(), playersHand[3].ToArray(), trump, null, false);

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
				Card chosenCard;

				if (currentPlayerID != 0)
				{
					Console.Write("Pick the card you want to play by its index: ");
					input = Console.ReadLine();
					cardIndex = Convert.ToInt32(input);
					chosenCard = currentHand[cardIndex];
					artificialPlayer.AddPlay(chosenCard);
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
			game.PrintPoints(playersNames);
			Console.WriteLine("");

		}
	}
}