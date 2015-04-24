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
		private Dictionary<int,int> dictionary;
		private Deck deck;


		public InformationSet(List<Card> currentHand, List<Card> alreadyPlayed, Suit trumpSuit)
		{
			Trump = trumpSuit;
			hand = new List<Card>(currentHand);
			dictionary = new Dictionary<int,int>();

			currentTrick = new List<Move>();
			processAlreadyPlayed(alreadyPlayed);
			List<Card> temp = new List<Card>(currentHand);
			temp.AddRange(alreadyPlayed);
			deck = new Deck(temp);
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

		public int GetHighestCardIndex()
		{
			int bestIndex = 0;
			int bestValue = Int32.MinValue;

			foreach (KeyValuePair<int, int> cardValue in dictionary)
			{
				if (cardValue.Value > bestValue)
				{
					bestValue = cardValue.Value;
					bestIndex = cardValue.Key;
				}
			}

			return bestIndex;
		}

		public void CleanCardValues()
		{
			dictionary.Clear();
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

		// public void calculateAverages(int n)
		// {
		// 	foreach (KeyValuePair<int, int> cardValue in dictionary)
		// 	{
		// 		dictionary[cardValue.Key] = cardValue.Value / n;
		// 	}
		// }


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
			List<List<Card>> sampledHands = deck.SampleHands(handSizes);

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
			Console.WriteLine("Trump - " + Trump);
			printDictionary("Dictionary");
			Console.WriteLine("-------------------------------------------");
		}
	}
}