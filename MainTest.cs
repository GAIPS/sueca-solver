using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class MainTest
	{

		public static void Main ()
		{
			Deck deck = new Deck();
			List<Card> hand = deck.GetHand(8);
			InformationSet infoSet = new InformationSet(hand, new List<Card>(), Suit.Clubs);
			PIMC pimc = new PIMC(1);
			pimc.ExecuteTestVersion(infoSet, hand);
		}
	}
}