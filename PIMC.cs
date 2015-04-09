using System;

namespace SuecaSolver
{
	public class PIMC
	{

		public static void Main ()
		{
			Deck d = new Deck();
			Game game = new Game(d.getHand(), Suit.Clubs, 0);
			game.Play();
			SuecaHelper sh = new SuecaHelper();
			InformationSet i = new InformationSet();
			sh.PIMC(i, 1);
		}
	}
}