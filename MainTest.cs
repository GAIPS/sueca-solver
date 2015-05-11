using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class MainTest
	{

		private static bool checkSuitsHaveAllPlayers(Dictionary<int,List<int>> suitHasPlayer)
		{
			if (suitHasPlayer[0].Count == 3 &&
				suitHasPlayer[1].Count == 3 &&
				suitHasPlayer[2].Count == 3 &&
				suitHasPlayer[3].Count == 3)
			{
				return true;
			}
			return false;
		}

		public static void Main ()
		{
			int NUM_TRICKS = 7;
			Deck deck = new Deck();
			List<Card> hand = deck.GetHand(NUM_TRICKS);
			InformationSet infoSet = new InformationSet(hand, Suit.Clubs);
			PIMC pimc = new PIMC(1);
			pimc.ExecuteTestVersion(infoSet, hand, NUM_TRICKS);
		}
	}
}