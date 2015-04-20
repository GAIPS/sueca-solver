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
				int N = Convert.ToInt32(args[0]);
				InformationSet infoSet = new InformationSet();
				pimc.Execute(infoSet, N);
				// infoSet.PrintInfoSet();
			} else {
				Console.WriteLine("Choose the number of sample for PIMC.");
			}


		}
	}
}