using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class SuecaSolver
	{

		public static void Main (string[] args)
		{
			PIMC pimc = new PIMC();

			if (args.Length > 0)
			{
				int N = Convert.ToInt32(args[0]);
				Deck deck = new Deck();
				List<Card> hand = deck.GetHand(10);
				InformationSet infoSet = new InformationSet(hand, new List<Card>());
				pimc.Execute(infoSet, N);
				infoSet.PrintInfoSet();

				// Card[] p0 = new Card[] {new Card(Rank.King, Suit.Hearts), new Card(Rank.Three, Suit.Diamonds), new Card(Rank.Six, Suit.Clubs)};
				// Card[] p1 = new Card[] {new Card(Rank.Seven, Suit.Diamonds), new Card(Rank.Ace, Suit.Spades), new Card(Rank.King, Suit.Diamonds)};
				// Card[] p2 = new Card[] {new Card(Rank.Queen, Suit.Clubs), new Card(Rank.King, Suit.Clubs), new Card(Rank.Seven, Suit.Spades)};
				// Card[] p3 = new Card[] {new Card(Rank.Six, Suit.Spades), new Card(Rank.Jack, Suit.Spades), new Card(Rank.Four, Suit.Hearts)};
				// SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, null, true);
				// int val = game.SampleGame(new Card(Rank.Six, Suit.Clubs));
				// Console.WriteLine("The value is " + val);

			} else {
				Console.WriteLine("Choose the number of sample for PIMC.");
			}


		}
	}
}