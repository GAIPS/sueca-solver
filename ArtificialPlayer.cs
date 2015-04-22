using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class ArtificialPlayer
	{

		private Suit trump;
		private List<Card> hand;
		private List<Card> alreadyPlayed;
		private PIMC pimc;


		public ArtificialPlayer(List<Card> initialHand, Suit trumpSuit, int N)
		{
			trump = trumpSuit;
			hand = new List<Card>(initialHand);
			alreadyPlayed = new List<Card>();
			pimc = new PIMC(N);
		}


		public void AddPlay(Card card)
		{
			alreadyPlayed.Add(card);
		}


		public Card Play(){

			InformationSet infoSet = new InformationSet(hand, alreadyPlayed, trump);
			Card chosenCard = pimc.Execute(infoSet);
			alreadyPlayed.Add(chosenCard);
			hand.Remove(chosenCard);

			return chosenCard;
		}
	}
}