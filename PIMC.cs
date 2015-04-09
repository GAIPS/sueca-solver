using System;

namespace SuecaSolver
{
	public class PIMC
	{

		public static void Main ()
		{
			// Deck d = new Deck();
			// SuecaGame game = new SuecaGame(d.getHand(), Suit.Clubs, 0);
			// game.SampleGame();
			// SuecaHelper sh = new SuecaHelper();
			// InformationSet i = new InformationSet();
			// sh.PIMC(i, 1);


			Deck d = new Deck();
			Card[] p0 = d.getHand();
			Card[] p1 = d.getHand();
			Card[] p2 = d.getHand();
			Card[] p3 = d.getHand();
			SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, 0);
			game.PrintPlayersHands();
			game.SampleTrick();
		}
	}
}