using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class SuecaSolver
	{

		public static void Main (string[] args)
		{
			// Deck d = new Deck();
			// SuecaGame game = new SuecaGame(d.getHand(), Suit.Clubs, 0);
			// game.SampleGame();
			// SuecaHelper sh = new SuecaHelper();
			// InformationSet i = new InformationSet();
			// sh.PIMC(i, 1);

			if (args.Length > 0)
			{
				Console.WriteLine(args[0]);
				Deck d = new Deck();
				Card[] p0 = d.getHand();
				Card[] p1 = d.getHand();
				Card[] p2 = d.getHand();
				Card[] p3 = d.getHand();

				Move[] alreadyPlayed = new Move[1];
				alreadyPlayed[0] = new Move(3, p3[0]);
				SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, alreadyPlayed);
				game.PrintPlayersHands();
				//game.SampleTrick(SuecaGame.PossibleMoves(p0, p3[0].Suit)[0]);
			} else {
				Console.WriteLine("Choose the number of sample for PIMC.");
			}


		}
	}
}