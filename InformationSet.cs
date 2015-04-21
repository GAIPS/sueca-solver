using System;
using System.Collections;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class InformationSet
	{
		public List<Card> Hand;
		private List<Card> alreadyPlayed;
		private Suit trump;
		private int firstPlayerId;
		private Dictionary<int,int> dictionary;
		private Deck deck;


		public InformationSet()
		{
			trump = Suit.Clubs;
			firstPlayerId = 0;
			Hand = new List<Card>();
			alreadyPlayed = new List<Card>();
			dictionary = new Dictionary<int,int>();

			deck = new Deck();
			copyToHand(deck.GetHand(10));
		}


		public InformationSet(List<Card> hand, List<Card> played)
		{
			trump = Suit.Clubs;
			firstPlayerId = 0;
			Hand = new List<Card>(hand);
			alreadyPlayed = new List<Card>();
			dictionary = new Dictionary<int,int>();

			deck = new Deck(hand.ToArray());
			copyToAlreadyPlayed(played);
		}

		public void AddCardValue(Card card, int val)
		{
			int cardID = card.ID;
			if (dictionary.ContainsKey(cardID))
			{
				dictionary[cardID] += val;
			} else {
				dictionary[cardID] = val;
			}
		}

		public void calculateAverages(int n)
		{
			foreach (Card card in Hand)
			{
				dictionary[card.ID] = dictionary[card.ID] / n;
			}
		}


		private void copyToHand(List<Card> cards)
		{
			for (int i = 0; i < cards.Count; i++)
			{
				Hand.Add(cards[i]);
			}
		}


		private void copyToAlreadyPlayed(List<Card> cards)
		{
			for (int i = 0; i < cards.Count; i++)
			{
				alreadyPlayed.Add(cards[i]);
			}
		}


		public List<List<Card>> Sample()
		{
			return deck.Sample(Hand.Count);
		}


		public List<List<Card>> SampleAll(int n)
		{
			return deck.SampleAll(n);
		}


		private void printDictionary(string name)
		{
			string str = name + " -";
			foreach (KeyValuePair<int, int> cardValue in dictionary)
			{
				str += " <" + cardValue.Key + "," + cardValue.Value + ">";
			}
			Console.WriteLine(str);
		}


		public void PrintInfoSet()
		{
			Console.WriteLine("------------------INFOSET------------------");
			SuecaGame.PrintCards("Hand", Hand);
			SuecaGame.PrintCards("Already Played", alreadyPlayed);
			Console.WriteLine("Trump - " + trump);
			Console.WriteLine("First player ID - " + firstPlayerId);
			printDictionary("Dictionary");
			Console.WriteLine("-------------------------------------------");
		}
	}
}