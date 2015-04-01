using System;

namespace SuecaSolver
{
	public class PIMC
	{

		public static void Main ()
		{
			Deck d = new Deck();
			SuecaHelper sh = new SuecaHelper();
			InformationSet i = new InformationSet();
			sh.PIMC(i, 1);
		}
	}
}