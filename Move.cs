using System;

namespace SuecaSolver
{
	public class Move
	{

		public int PlayerId;
		public Card Card;

		public Move(int playerId, Card card)
		{
			PlayerId = playerId;
			Card = card;
		}

		public override string ToString()
		{
			return "Player " + PlayerId + " has played " + Card;
		}
	}
}