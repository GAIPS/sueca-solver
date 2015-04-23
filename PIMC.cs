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
			infoSet.CleanCardValues();

			for (int i = 0; i < N; i++)
			{
				List<List<Card>> players = infoSet.Sample();
				List<Card> p0 = players[0];
				List<Card> p1 = players[1];
				List<Card> p2 = players[2];
				List<Card> p3 = players[3];


				SuecaGame game = new SuecaGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed(), false);
				List<Card> possibleMoves = SuecaGame.PossibleMoves(p0, infoSet.GetLeadSuit());

				for (int j = 0; j < possibleMoves.Count; j++)
				{
					Card card = possibleMoves[j];
					int cardValueInTrick = game.SampleGame(card);
					// int cardValueInTrick = game.SampleTrick(card);
					Console.WriteLine("cardValueInTrick - " + card + " " + cardValueInTrick);
					infoSet.AddCardValue(j, cardValueInTrick);
				}
			}

			// Do I really have to do the average?
			infoSet.calculateAverages(N);
			infoSet.PrintInfoSet();
			return infoSet.GetHighestCard();
		}


		public void ExecuteTestVersion(InformationSet infoSet, List<Card> hand)
		{
			List<List<Card>> players = infoSet.SampleThree(8);
			List<Card> p0 = hand;
			List<Card> p1 = players[0];
			List<Card> p2 = players[1];
			List<Card> p3 = players[2];


			SuecaGame game = new SuecaGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed(), false);
			List<Card> possibleMoves = SuecaGame.PossibleMoves(p0, infoSet.GetLeadSuit());

			// for (int j = 0; j < possibleMoves.Count; j++)
			// {
				Card card = possibleMoves[0];
				int cardValueInTrick = game.SampleGame(card);
				// int cardValueInTrick = game.SampleTrick(card);
				Console.WriteLine("cardValueInTrick - " + card + " " + cardValueInTrick);
				infoSet.AddCardValue(0, cardValueInTrick);
			// }

			// Do I really have to do the average?
			// infoSet.calculateAverages(N);
			infoSet.PrintInfoSet();
			// return infoSet.GetHighestCard();
		}
	}
}