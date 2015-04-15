using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class PIMC
	{
		private int perfectInfoGame(InformationSet i, int n)
		{
			return 1;
		}

		public void Execute(InformationSet i, int N)
		{
			// Dictionary<int, int> movesValues = new Dictionary<int, int>();
			// foreach (int move in i.Hand)
			// {
			// 	movesValues.Add(move, 0);
			// }

			// for (int j = 0; j < N; j++)
			// {
			// 	i.sample();
			// 	foreach (int move in i.Hand)
			// 	{
			// 		movesValues[move] = movesValues[move] + perfectInfoGame(i, move);
			// 	}
			// }

			Console.WriteLine("Executing PIMC");
		}
	}
}