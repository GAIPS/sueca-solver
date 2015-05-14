using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class MainTest
	{

		private static bool checkSuitsHaveAllPlayers (Dictionary<int,List<int>> suitHasPlayer)
		{
			if (suitHasPlayer [0].Count == 3 &&
			    suitHasPlayer [1].Count == 3 &&
			    suitHasPlayer [2].Count == 3 &&
			    suitHasPlayer [3].Count == 3) {
				return true;
			}
			return false;
		}

		public static void Main ()
		{
			int NUM_TRICKS = 6;
			Deck deck = new Deck ();
			List<Card> hand = deck.GetHand (NUM_TRICKS);
			InformationSet infoSet = new InformationSet (hand, Suit.Clubs);
			// PIMC pimc = new PIMC(1);
			// pimc.ExecuteTestVersion(infoSet, hand, NUM_TRICKS);



			List<Card> p0 = new List<Card> () {new Card (Rank.Jack, Suit.Clubs),
				new Card (Rank.Five, Suit.Diamonds),
				new Card (Rank.Jack, Suit.Hearts),
				new Card (Rank.King, Suit.Hearts),
				new Card (Rank.Two, Suit.Spades),
				new Card (Rank.Jack, Suit.Spades),
				new Card (Rank.Seven, Suit.Spades)
			};
			List<Card> p1 = new List<Card> () {new Card (Rank.Six, Suit.Clubs),
				new Card (Rank.Two, Suit.Diamonds),
				new Card (Rank.Queen, Suit.Hearts),
				new Card (Rank.Four, Suit.Spades),
				new Card (Rank.Six, Suit.Spades),
				new Card (Rank.Queen, Suit.Spades),
				new Card (Rank.Ace, Suit.Spades)
			};
			List<Card> p2 = new List<Card> () {new Card (Rank.Two, Suit.Clubs),
				new Card (Rank.Three, Suit.Clubs),
				new Card (Rank.Queen, Suit.Clubs),
				new Card (Rank.King, Suit.Clubs),
				new Card (Rank.Ace, Suit.Clubs),
				new Card (Rank.Four, Suit.Diamonds),
				new Card (Rank.King, Suit.Spades)
			};
			List<Card> p3 = new List<Card> () {new Card (Rank.Five, Suit.Clubs),
				new Card (Rank.Seven, Suit.Clubs),
				new Card (Rank.Three, Suit.Diamonds),
				new Card (Rank.Six, Suit.Diamonds),
				new Card (Rank.King, Suit.Diamonds),
				new Card (Rank.Seven, Suit.Diamonds),
				new Card (Rank.Four, Suit.Hearts)
			};


			// List<Card> p0 = new List<Card>() {new Card(Rank.Four, Suit.Diamonds),
			// 									new Card(Rank.Six, Suit.Diamonds),
			// 									new Card(Rank.Jack, Suit.Diamonds),
			// 									new Card(Rank.Seven, Suit.Diamonds),
			// 									new Card(Rank.Ace, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Spades),
			// 									new Card(Rank.Seven, Suit.Spades)};
			// List<Card> p1 = new List<Card>() {new Card(Rank.Six, Suit.Clubs),
			// 									new Card(Rank.Five, Suit.Diamonds),
			// 									new Card(Rank.King, Suit.Diamonds),
			// 									new Card(Rank.Two, Suit.Hearts),
			// 									new Card(Rank.Six, Suit.Hearts),
			// 									new Card(Rank.Two, Suit.Spades),
			// 									new Card(Rank.Jack, Suit.Spades)};
			// List<Card> p2 = new List<Card>() {new Card(Rank.Two, Suit.Clubs),
			// 									new Card(Rank.Three, Suit.Clubs),
			// 									new Card(Rank.King, Suit.Clubs),
			// 									new Card(Rank.Two, Suit.Diamonds),
			// 									new Card(Rank.Five, Suit.Hearts),
			// 									new Card(Rank.King, Suit.Hearts),
			// 									new Card(Rank.Queen, Suit.Spades)};
			// List<Card> p3 = new List<Card>() {new Card(Rank.Queen, Suit.Clubs),
			// 									new Card(Rank.Ace, Suit.Diamonds),
			// 									new Card(Rank.Three, Suit.Hearts),
			// 									new Card(Rank.Four, Suit.Hearts),
			// 									new Card(Rank.Four, Suit.Spades),
			// 									new Card(Rank.Six, Suit.Spades),
			// 									new Card(Rank.King, Suit.Spades)};


			// List<Card> p0 = new List<Card>() {new Card(Rank.Three, Suit.Clubs),
			// 									new Card(Rank.Six, Suit.Clubs),
			// 									new Card(Rank.Seven, Suit.Clubs),
			// 									new Card(Rank.Two, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Hearts),
			// 									new Card(Rank.Seven, Suit.Hearts),
			// 									new Card(Rank.Two, Suit.Spades)};
			// List<Card> p1 = new List<Card>() {new Card(Rank.Queen, Suit.Clubs),
			// 									new Card(Rank.King, Suit.Clubs),
			// 									new Card(Rank.Three, Suit.Diamonds),
			// 									new Card(Rank.Three, Suit.Hearts),
			// 									new Card(Rank.Six, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Spades),
			// 									new Card(Rank.Seven, Suit.Spades)};
			// List<Card> p2 = new List<Card>() {new Card(Rank.Jack, Suit.Clubs),
			// 									new Card(Rank.Four, Suit.Diamonds),
			// 									new Card(Rank.Six, Suit.Diamonds),
			// 									new Card(Rank.Queen, Suit.Diamonds),
			// 									new Card(Rank.King, Suit.Diamonds),
			// 									new Card(Rank.Jack, Suit.Hearts),
			// 									new Card(Rank.Queen, Suit.Spades)};
			// List<Card> p3 = new List<Card>() {new Card(Rank.Five, Suit.Diamonds),
			// 									new Card(Rank.Ace, Suit.Hearts),
			// 									new Card(Rank.Three, Suit.Spades),
			// 									new Card(Rank.Four, Suit.Spades),
			// 									new Card(Rank.Jack, Suit.Spades),
			// 									new Card(Rank.King, Suit.Spades),
			// 									new Card(Rank.Ace, Suit.Spades)};


			// List<Card> p0 = new List<Card>() {new Card(Rank.Four, Suit.Clubs),
			// 									new Card(Rank.Jack, Suit.Clubs),
			// 									new Card(Rank.Two, Suit.Diamonds),
			// 									new Card(Rank.Ace, Suit.Diamonds),
			// 									new Card(Rank.Queen, Suit.Hearts),
			// 									new Card(Rank.Ace, Suit.Hearts),
			// 									new Card(Rank.King, Suit.Spades)};
			// List<Card> p1 = new List<Card>() {new Card(Rank.Ace, Suit.Clubs),
			// 									new Card(Rank.Jack, Suit.Diamonds),
			// 									new Card(Rank.King, Suit.Diamonds),
			// 									new Card(Rank.Seven, Suit.Diamonds),
			// 									new Card(Rank.Three, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Hearts),
			// 									new Card(Rank.Four, Suit.Spades)};
			// List<Card> p2 = new List<Card>() {new Card(Rank.Five, Suit.Clubs),
			// 									new Card(Rank.King, Suit.Clubs),
			// 									new Card(Rank.Five, Suit.Diamonds),
			// 									new Card(Rank.Three, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Hearts),
			// 									new Card(Rank.Four, Suit.Spades),
			// 									new Card(Rank.Jack, Suit.Spades)};
			// List<Card> p3 = new List<Card>() {new Card(Rank.Three, Suit.Clubs),
			// 									new Card(Rank.Six, Suit.Clubs),
			// 									new Card(Rank.Seven, Suit.Clubs),
			// 									new Card(Rank.Jack, Suit.Hearts),
			// 									new Card(Rank.Seven, Suit.Hearts),
			// 									new Card(Rank.Two, Suit.Spades),
			// 									new Card(Rank.Three, Suit.Spades)};


			// List<Card> p0 = new List<Card>() {new Card(Rank.Five, Suit.Clubs),
			// 									new Card(Rank.Seven, Suit.Clubs),
			// 									new Card(Rank.Four, Suit.Diamonds),
			// 									new Card(Rank.Five, Suit.Diamonds),
			// 									new Card(Rank.Queen, Suit.Diamonds),
			// 									new Card(Rank.Two, Suit.Spades),
			// 									new Card(Rank.Six, Suit.Spades)};
			// List<Card> p1 = new List<Card>() {new Card(Rank.Three, Suit.Clubs),
			// 									new Card(Rank.Six, Suit.Clubs),
			// 									new Card(Rank.Queen, Suit.Clubs),
			// 									new Card(Rank.Two, Suit.Diamonds),
			// 									new Card(Rank.Seven, Suit.Diamonds),
			// 									new Card(Rank.Four, Suit.Hearts),
			// 									new Card(Rank.Five, Suit.Spades)};
			// List<Card> p2 = new List<Card>() {new Card(Rank.Jack, Suit.Clubs),
			// 									new Card(Rank.Ace, Suit.Clubs),
			// 									new Card(Rank.Jack, Suit.Diamonds),
			// 									new Card(Rank.Ace, Suit.Diamonds),
			// 									new Card(Rank.Five, Suit.Hearts),
			// 									new Card(Rank.King, Suit.Spades),
			// 									new Card(Rank.Ace, Suit.Spades)};
			// List<Card> p3 = new List<Card>() {new Card(Rank.Two, Suit.Hearts),
			// 									new Card(Rank.Three, Suit.Hearts),
			// 									new Card(Rank.Six, Suit.Hearts),
			// 									new Card(Rank.King, Suit.Hearts),
			// 									new Card(Rank.Seven, Suit.Hearts),
			// 									new Card(Rank.Three, Suit.Spades),
			// 									new Card(Rank.Queen, Suit.Spades)};




			SuecaGame game = new SuecaGame (p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed ());
			Card card = p0 [0];
			int cardValueInTrick = game.SampleGame (card);
			infoSet.AddCardValue (card, cardValueInTrick);
			infoSet.PrintInfoSet ();
		}
	}
}