using System;
using System.Collections;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class InformationSet
	{
		private List<Card> hand;
		private List<Move> currentTrick;
		private Suit trump;
		private Dictionary<int,int> dictionary;
		private Deck deck;


		public InformationSet(List<Card> currentHand, List<Card> alreadyPlayed, Suit trumpSuit)
		{
			trump = trumpSuit;
			hand = new List<Card>(currentHand);
			dictionary = new Dictionary<int,int>();

			currentTrick = new List<Move>();
			processAlreadyPlayed(alreadyPlayed);
			List<Card> temp = new List<Card>(currentHand);
			temp.AddRange(alreadyPlayed);
			deck = new Deck(temp);
		}

		public List<Move> GetJustPlayed()
		{
			return currentTrick;
		}

		public Card GetHighestCard()
		{
			int bestIndex = 0;
			int bestValue = Int32.MinValue;

			for (int i = 0; i < hand.Count; i++)
			{
				if (dictionary[i] > bestValue)
				{
					bestValue = dictionary[i];
					bestIndex = i;
				}
			}
			return hand[bestIndex];
		}

		private void processAlreadyPlayed(List<Card> alreadyPlayed)
		{
			int size = alreadyPlayed.Count;
			int trickSize = size % 4;
			for (int index, i = trickSize; i > 0; i--)
			{
				index = size - i;
				Card card = alreadyPlayed[index];
				int playerID = 3 - i + 1;
				currentTrick.Add(new Move(playerID, card));
			}
		}

		public void AddCardValue(int index, int val)
		{
			if (dictionary.ContainsKey(index))
			{
				dictionary[index] += val;
			} else {
				dictionary[index] = val;
			}
		}

		public void calculateAverages(int n)
		{
			for (int i = 0; i < hand.Count; i++)
			{
				dictionary[i] = dictionary[i] / n;
			}
		}


		public List<List<Card>> Sample()
		{
			List<List<Card>> hands = new List<List<Card>>();
			int myHandSize = hand.Count;
			int[] handSizePerPlayer = new int[4] {myHandSize, myHandSize, myHandSize, myHandSize};
			int currentTrickSize = currentTrick.Count;

			for (int i = 0; i < currentTrickSize; i++)
			{
				handSizePerPlayer[3 - i]--;
			}

			for (int i = 0; i < 4; i++)
			{
				hands.Add(deck.SampleHand(handSizePerPlayer[i]));
			}

			return hands;
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
				str += " <" + hand[cardValue.Key] + "," + cardValue.Value + ">";
			}
			Console.WriteLine(str);
		}


		public void PrintInfoSet()
		{
			Console.WriteLine("------------------INFOSET------------------");
			SuecaGame.PrintCards("Hand", hand);
			Console.WriteLine("Trump - " + trump);
			printDictionary("Dictionary");
			Console.WriteLine("-------------------------------------------");
		}
	}
}