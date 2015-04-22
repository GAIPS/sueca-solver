using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class PIMC
	{

		private int N;

		public PIMC(int n)
		{
			N = n;
		}

		public Card Execute(InformationSet infoSet)
		{
			for (int i = 0; i < N; i++)
			{

				List<List<Card>> players = infoSet.Sample();
				Card[] p0 = players[0].ToArray();
				Card[] p1 = players[1].ToArray();
				Card[] p2 = players[2].ToArray();
				Card[] p3 = players[3].ToArray();


				SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, infoSet.GetJustPlayed(), true);

				for (int j = 0; j < p0.Length; j++)
				{
					Card card = p0[j];
					int cardValueInTrick = game.SampleGame(card);
					// int cardValueInTrick = game.SampleTrick(card);
					infoSet.AddCardValue(j, cardValueInTrick);
				}
			}

			// Do I really have to do the average?
			infoSet.calculateAverages(N);
			infoSet.PrintInfoSet();
			return infoSet.GetHighestCard();
		}


		public Card ExecuteTestVersion(InformationSet infoSet)
		{
			List<List<Card>> players = infoSet.SampleAll(2);
			Card[] p0 = players[0].ToArray();
			Card[] p1 = players[1].ToArray();
			Card[] p2 = players[2].ToArray();
			Card[] p3 = players[3].ToArray();

			SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, infoSet.GetJustPlayed(), true);

			Card card = p0[0];
			int cardValueInTrick = game.SampleGame(card);
			// int cardValueInTrick = game.SampleTrick(card);
			Console.WriteLine("cardValueInTrick - " + card.ID + " " + cardValueInTrick);
			infoSet.AddCardValue(0, cardValueInTrick);

			// Do I really have to do the average?
			infoSet.calculateAverages(N);
			infoSet.PrintInfoSet();
			return infoSet.GetHighestCard();
		}
	}
}