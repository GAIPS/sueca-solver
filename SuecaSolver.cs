using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class SuecaSolver
	{

		public static void Main (string[] args)
		{
			PIMC pimc = new PIMC();

			if (args.Length > 0)
			{
				// Deck d = new Deck();
				// Card[] p0 = d.GetHand(10);
				// Card[] p1 = d.GetHand(10);
				// Card[] p2 = d.GetHand(10);
				// Card[] p3 = d.GetHand(10);

				// Move[] alreadyPlayed = new Move[1];
				// alreadyPlayed[0] = new Move(3, p3[0]);
				// SuecaGame game = new SuecaGame(p0, p1, p2, p3, Suit.Clubs, alreadyPlayed);
				// game.PrintPlayersHands();
				//game.SampleTrick(SuecaGame.PossibleMoves(p0, p3[0].Suit)[0]);

				InformationSet[] infoSet = new InformationSet[2];
				for (int i = 0; i < 2; i++)
				{
					infoSet[i] = new InformationSet();
					// infoSet[i].PrintInfoSet();
					pimc.Execute(infoSet[i], Convert.ToInt32(args[0]));

				}

				foreach (InformationSet child in infoSet)
				{
					//top moves ?!
					for (int i = 0; i < 2; i++)
					{
						Card c = child.Hand[i];
						InformationSet childOfChild = child.createInformationSet(c);
						// childOfChild.PrintInfoSet();
					}
				}

			} else {
				Console.WriteLine("Choose the number of sample for PIMC.");
			}


		}
	}
}