using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class RandomPlayer : ArtificialPlayer
	{
		private List<Card> hand;
		private Suit leadSuit;
		private int currentPlay;
		private Random randomNumber;

		public RandomPlayer(List<Card> initialHand)
		{
			hand = new List<Card>(initialHand);
			currentPlay = 0;
			randomNumber = new Random(Guid.NewGuid().GetHashCode());
		}

		override public void AddPlay(int playerID, Card card)
		{
			if (currentPlay == 0)
			{
				leadSuit = card.Suit;
			}
			currentPlay = (currentPlay + 1) % 4;
		}


		override public Card Play(){
			if (currentPlay == 0)
			{
				leadSuit = Suit.None;
			}

			List<Card> possibleMoves = SuecaGame.PossibleMoves(hand, leadSuit);
			int randomIndex = randomNumber.Next(0, possibleMoves.Count);
			Card chosenCard = possibleMoves[randomIndex];
			hand.Remove(chosenCard);
			currentPlay = (currentPlay + 1) % 4;

			return chosenCard;
		}
	}
}