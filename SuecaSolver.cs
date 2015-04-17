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
				InformationSet[] infoSet = new InformationSet[1];
				for (int i = 0; i < 1; i++)
				{
					infoSet[i] = new InformationSet();
					pimc.Execute(infoSet[i], N);
					infoSet[i].PrintInfoSet();

				}

				foreach (InformationSet child in infoSet)
				{
					//top moves ?!
					for (int i = 0; i < 2; i++)
					{
						Card c = child.Hand[i];
						InformationSet childOfChild = child.createInformationSet(c);
						pimc.Execute(childOfChild, N);
						childOfChild.PrintInfoSet();
					}
				}

			} else {
				Console.WriteLine("Choose the number of sample for PIMC.");
			}


		}
	}
}