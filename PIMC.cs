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
			int n = N;
			infoSet.CleanCardValues();
			List<Card> possibleMoves = infoSet.GetPossibleMoves();

			if (possibleMoves.Count == 1)
			{
				return possibleMoves[0];
			}

			for (int i = 0; i < n; i++)
			{
				List<List<Card>> players = infoSet.Sample();
				List<Card> p0 = players[0];
				List<Card> p1 = players[1];
				List<Card> p2 = players[2];
				List<Card> p3 = players[3];

				// SuecaGame.PrintCards("My hand", p0);
				// SuecaGame.PrintCards("sample p1", p1);
				// SuecaGame.PrintCards("sample p2", p2);
				// SuecaGame.PrintCards("sample p3", p3);

				SuecaGame game = new SuecaGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed(), false);

				for (int cardValueInTrick, j = 0; j < possibleMoves.Count; j++)
				{
					Card card = possibleMoves[j];

					if (p0.Count > 5)
					{
						n = 3;
						cardValueInTrick = game.SampleTrick(card);
					}
					else
					{
						N = 3;
						cardValueInTrick = game.SampleGame(card);
					}

					infoSet.AddCardValue(card, cardValueInTrick);
				}
			}

			infoSet.PrintInfoSet();
			Card highestCard = infoSet.GetHighestCardIndex();
			return highestCard;
		}


		public void ExecuteTestVersion(InformationSet infoSet, List<Card> hand)
		{
			List<Card> possibleMoves = SuecaGame.PossibleMoves(hand, infoSet.GetLeadSuit());

			if (possibleMoves.Count == 1)
			{
				Console.WriteLine("Only one move available: " + possibleMoves[0]);
				return;
			}

			List<List<Card>> players = infoSet.SampleThree(8);
			List<Card> p0 = hand;
			List<Card> p1 = players[0];
			List<Card> p2 = players[1];
			List<Card> p3 = players[2];


			SuecaGame game = new SuecaGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed(), false);

			// for (int j = 0; j < possibleMoves.Count; j++)
			// {
				Card card = possibleMoves[0];
				int cardValueInTrick = game.SampleGame(card);
				// int cardValueInTrick = game.SampleTrick(card);
				Console.WriteLine("cardValueInTrick - " + card + " " + cardValueInTrick);
				infoSet.AddCardValue(card, cardValueInTrick);
			// }

			infoSet.PrintInfoSet();
		}
	}
}