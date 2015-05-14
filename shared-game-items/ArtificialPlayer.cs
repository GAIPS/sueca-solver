using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class ArtificialPlayer
	{

		public ArtificialPlayer(){}

		public abstract void AddPlay(int playerID, Card card);

		public abstract Card Play();
	}
}