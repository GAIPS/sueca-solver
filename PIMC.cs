using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class PIMC
	{

		public void Execute(InformationSet infoSet, int N)
		{
			for (int i = 0; i < N; i++)
			{
				List<List<Card>> players = infoSet.Sample();
				Card[] p0 = infoSet.Hand.ToArray();
				Card[] p1 = players[0].ToArray();
				Card[] p2 = players[1].ToArray();
				Card[] p3 = players[2].ToArray();

				SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, null, false);

				foreach (Card card in p0)
				{
					int cardValueInTrick = game.SampleTrick(card);
					infoSet.AddCardValue(card, cardValueInTrick);
					// Console.WriteLine("cardValueInTrick - " + card.ID + " " + cardValueInTrick);
				}
			}
			infoSet.calculateAverages(N);
		}
	}
}