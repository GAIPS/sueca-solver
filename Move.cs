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
			return "MOVE by player: " + PlayerId + " with card: " + Card;
		}
	}
}