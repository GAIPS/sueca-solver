using System;
using System.Collections;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class InformationSet
	{
		private List<Card> hand;
		private List<Move> currentTrick;
		public Suit Trump;
		private Dictionary<Card,int> dictionary;
		private Deck deck;
		private Dictionary<int,bool> playerHasSuit;


		public InformationSet(List<Card> currentHand, Suit trumpSuit)
		{
			Trump = trumpSuit;
			hand = new List<Card>(currentHand);
			dictionary = new Dictionary<Card,int>();
			playerHasSuit = new Dictionary<int,bool> {
				{10, true},
				{11, true},
				{12, true},
				{13, true},
				{20, true},
				{21, true},
				{22, true},
				{23, true},
				{30, true},
				{31, true},
				{32, true},
				{33, true}};
			currentTrick = new List<Move>();
			deck = new Deck(currentHand);
		}


		public List<Card> GetPossibleMoves()
		{
			return SuecaGame.PossibleMoves(hand, GetLeadSuit());
		}

		public Suit GetLeadSuit()
		{
			if (currentTrick.Count == 0)
			{
				return Suit.None;
			}

			return currentTrick[0].Card.Suit;
		}

		public List<Move> GetJustPlayed()
		{
			return currentTrick;
		}

		public Card GetHighestCardIndex()
		{
			Card bestCard = null;
			int bestValue = Int32.MinValue;

			foreach (KeyValuePair<Card, int> cardValue in dictionary)
			{
				if (cardValue.Value > bestValue)
				{
					bestValue = cardValue.Value;
					bestCard = cardValue.Key;
				}
			}

			if (bestCard == null) 
			{
				Console.WriteLine("Trouble at InformationSet.GetHighestCardIndex()");
			}

			return bestCard;
		}

		public void AddPlay(int playerID, Card card)
		{

			if (currentTrick.Count == 3) 
			{
				currentTrick.Clear();
			}
			else
			{
				currentTrick.Add(new Move(playerID, card));
			}
			deck.RemoveCard(card);
		}

		public void AddMyPlay(Card card)
		{
			if (currentTrick.Count == 3) 
			{
				currentTrick.Clear();
			}
			else
			{
				currentTrick.Add(new Move(0, card));
			}
			hand.Remove(card);
		}

		public void CleanCardValues()
		{
			dictionary.Clear();
		}

		public void AddCardValue(Card card, int val)
		{
			if (dictionary.ContainsKey(card))
			{
				dictionary[card] += val;
			} else {
				dictionary[card] = val;
			}
		}

		public List<List<Card>> Sample()
		{
			List<List<Card>> hands = new List<List<Card>>();
			int myHandSize = hand.Count;
			int[] handSizes = new int[3] {myHandSize, myHandSize, myHandSize};
			int currentTrickSize = currentTrick.Count;

			for (int i = 0; i < currentTrickSize; i++)
			{
				handSizes[2 - i]--;
			}

			hands.Add(new List<Card>(hand));
			List<List<Card>> sampledHands = deck.SampleHands(playerHasSuit, handSizes);

			for (int i = 0; i < 3; i++)
			{
				hands.Add(sampledHands[i]);
			}

			return hands;
		}


		public List<List<Card>> SampleThree(int n)
		{
			List<List<Card>> hands = new List<List<Card>>();
			hands.Add(deck.GetHand(n));
			hands.Add(deck.GetHand(n));
			hands.Add(deck.GetHand(n));
			return hands;
		}


		public List<List<Card>> SampleAll(int n)
		{
			return deck.SampleAll(n);
		}


		private void printDictionary(string name)
		{
			string str = name + " -";
			foreach (KeyValuePair<Card, int> cardValue in dictionary)
			{
				str += " <" + cardValue.Key + "," + cardValue.Value + ">";
			}
			Console.WriteLine(str);
		}


		public void PrintInfoSet()
		{
			Console.WriteLine("------------------INFOSET------------------");
			SuecaGame.PrintCards("Hand", hand);
			Console.WriteLine("Trump - " + Trump);
			printDictionary("Dictionary");
			Console.WriteLine("-------------------------------------------");
		}
	}
}