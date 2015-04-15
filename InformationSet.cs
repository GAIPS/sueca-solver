using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class InformationSet
	{
		public List<Card> Hand;
		private List<Card> alreadyPlayed;
		private Suit trump;
		private int firstPlayerId;
		public List<InformationSet> Children;


		public InformationSet()
		{
			trump = Suit.Clubs;
			firstPlayerId = 0;
			Children = new List<InformationSet>();
			alreadyPlayed = new List<Card>();

			Deck deck = new Deck();
			Hand = deck.GetHand(10);
		}

		public InformationSet(List<Card> hand, List<Card> played, Card card)
		{
			trump = Suit.Clubs;
			firstPlayerId = 0;
			Hand = new List<Card>();
			Children = new List<InformationSet>();
			alreadyPlayed = new List<Card>();

			setHand(hand);
			Hand.Remove(card);
			setAlreadyPlayed(played);
			alreadyPlayed.Add(card);
		}

		private void setHand(List<Card> cards)
		{
			for (int i = 0; i < cards.Count; i++)
			{
				Hand.Add(cards[i]);
			}
		}

		// public List<Card> GetAvailableMoves()
		// {

		// }

		private void setAlreadyPlayed(List<Card> cards)
		{
			for (int i = 0; i < cards.Count; i++)
			{
				alreadyPlayed.Add(cards[i]);
			}
		}

		public InformationSet createInformationSet(Card card)
		{
			InformationSet newChild = new InformationSet(Hand, alreadyPlayed, card);
			Children.Add(newChild);

			return newChild;
		}

		public void PrintInfoSet()
		{
			Console.WriteLine("------------INFOSET------------");
			SuecaGame.PrintCards("Hand", Hand);
			SuecaGame.PrintCards("Already Played", alreadyPlayed);
			Console.WriteLine("Trump - " + trump);
			Console.WriteLine("First player ID - " + firstPlayerId);
			Console.WriteLine("# Children - " + Children.Count);
		}
	}
}