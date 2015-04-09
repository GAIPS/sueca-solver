using System;

namespace SuecaSolver
{
	public class PIMC
	{

		public static void Main ()
		{
			Deck d = new Deck();
			SuecaGame game = new SuecaGame(d.getHand(), Suit.Clubs, 0);
			game.SampleGame();
			SuecaHelper sh = new SuecaHelper();
			InformationSet i = new InformationSet();
			sh.PIMC(i, 1);
		}
	}
}