using System;
using System.Collections.Generic;
using SuecaSolver;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Security.Cryptography.X509Certificates;

namespace SuecaSolver
{
    public class MainTest
    {

        private static bool checkSuitsHaveAllPlayers(Dictionary<int,List<int>> suitHasPlayer)
        {
            if (suitHasPlayer[0].Count == 3 &&
                suitHasPlayer[1].Count == 3 &&
                suitHasPlayer[2].Count == 3 &&
                suitHasPlayer[3].Count == 3)
            {
                return true;
            }
            return false;
        }

        public static void Main()
        {
            int NUM_TRICKS = 5;
            Deck deck = new Deck();
            List<int> hand = deck.GetHand(NUM_TRICKS);
            InformationSet infoSet = new InformationSet(hand, (int)Suit.Clubs);
            //            PIMC pimc = new PIMC(1);
            //            pimc.ExecuteTestVersion(infoSet, hand, NUM_TRICKS);



            List<int> p0 = new List<int>()
            {Fart.Create(Rank.Jack, Suit.Clubs),
                Fart.Create(Rank.Five, Suit.Diamonds),
                Fart.Create(Rank.Jack, Suit.Hearts),
                Fart.Create(Rank.King, Suit.Hearts),
                Fart.Create(Rank.Two, Suit.Spades),
                Fart.Create(Rank.Jack, Suit.Spades),
                Fart.Create(Rank.Seven, Suit.Spades)
            };
            List<int> p1 = new List<int>()
            {Fart.Create(Rank.Six, Suit.Clubs),
                Fart.Create(Rank.Two, Suit.Diamonds),
                Fart.Create(Rank.Queen, Suit.Hearts),
                Fart.Create(Rank.Four, Suit.Spades),
                Fart.Create(Rank.Six, Suit.Spades),
                Fart.Create(Rank.Queen, Suit.Spades),
                Fart.Create(Rank.Ace, Suit.Spades)
            };
            List<int> p2 = new List<int>()
            {Fart.Create(Rank.Two, Suit.Clubs),
                Fart.Create(Rank.Three, Suit.Clubs),
                Fart.Create(Rank.Queen, Suit.Clubs),
                Fart.Create(Rank.King, Suit.Clubs),
                Fart.Create(Rank.Ace, Suit.Clubs),
                Fart.Create(Rank.Four, Suit.Diamonds),
                Fart.Create(Rank.King, Suit.Spades)
            };
            List<int> p3 = new List<int>()
            {Fart.Create(Rank.Five, Suit.Clubs),
                Fart.Create(Rank.Seven, Suit.Clubs),
                Fart.Create(Rank.Three, Suit.Diamonds),
                Fart.Create(Rank.Six, Suit.Diamonds),
                Fart.Create(Rank.King, Suit.Diamonds),
                Fart.Create(Rank.Seven, Suit.Diamonds),
                Fart.Create(Rank.Four, Suit.Hearts)
            };


            // List<int> p0 = new List<int>() {Fart.Create(Rank.Four, Suit.Diamonds),
            //                                  Fart.Create(Rank.Six, Suit.Diamonds),
            //                                  Fart.Create(Rank.Jack, Suit.Diamonds),
            //                                  Fart.Create(Rank.Seven, Suit.Diamonds),
            //                                  Fart.Create(Rank.Ace, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Spades),
            //                                  Fart.Create(Rank.Seven, Suit.Spades)};
            // List<int> p1 = new List<int>() {Fart.Create(Rank.Six, Suit.Clubs),
            //                                  Fart.Create(Rank.Five, Suit.Diamonds),
            //                                  Fart.Create(Rank.King, Suit.Diamonds),
            //                                  Fart.Create(Rank.Two, Suit.Hearts),
            //                                  Fart.Create(Rank.Six, Suit.Hearts),
            //                                  Fart.Create(Rank.Two, Suit.Spades),
            //                                  Fart.Create(Rank.Jack, Suit.Spades)};
            // List<int> p2 = new List<int>() {Fart.Create(Rank.Two, Suit.Clubs),
            //                                  Fart.Create(Rank.Three, Suit.Clubs),
            //                                  Fart.Create(Rank.King, Suit.Clubs),
            //                                  Fart.Create(Rank.Two, Suit.Diamonds),
            //                                  Fart.Create(Rank.Five, Suit.Hearts),
            //                                  Fart.Create(Rank.King, Suit.Hearts),
            //                                  Fart.Create(Rank.Queen, Suit.Spades)};
            // List<int> p3 = new List<int>() {Fart.Create(Rank.Queen, Suit.Clubs),
            //                                  Fart.Create(Rank.Ace, Suit.Diamonds),
            //                                  Fart.Create(Rank.Three, Suit.Hearts),
            //                                  Fart.Create(Rank.Four, Suit.Hearts),
            //                                  Fart.Create(Rank.Four, Suit.Spades),
            //                                  Fart.Create(Rank.Six, Suit.Spades),
            //                                  Fart.Create(Rank.King, Suit.Spades)};


            // List<int> p0 = new List<int>() {Fart.Create(Rank.Three, Suit.Clubs),
            //                                  Fart.Create(Rank.Six, Suit.Clubs),
            //                                  Fart.Create(Rank.Seven, Suit.Clubs),
            //                                  Fart.Create(Rank.Two, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Hearts),
            //                                  Fart.Create(Rank.Seven, Suit.Hearts),
            //                                  Fart.Create(Rank.Two, Suit.Spades)};
            // List<int> p1 = new List<int>() {Fart.Create(Rank.Queen, Suit.Clubs),
            //                                  Fart.Create(Rank.King, Suit.Clubs),
            //                                  Fart.Create(Rank.Three, Suit.Diamonds),
            //                                  Fart.Create(Rank.Three, Suit.Hearts),
            //                                  Fart.Create(Rank.Six, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Spades),
            //                                  Fart.Create(Rank.Seven, Suit.Spades)};
            // List<int> p2 = new List<int>() {Fart.Create(Rank.Jack, Suit.Clubs),
            //                                  Fart.Create(Rank.Four, Suit.Diamonds),
            //                                  Fart.Create(Rank.Six, Suit.Diamonds),
            //                                  Fart.Create(Rank.Queen, Suit.Diamonds),
            //                                  Fart.Create(Rank.King, Suit.Diamonds),
            //                                  Fart.Create(Rank.Jack, Suit.Hearts),
            //                                  Fart.Create(Rank.Queen, Suit.Spades)};
            // List<int> p3 = new List<int>() {Fart.Create(Rank.Five, Suit.Diamonds),
            //                                  Fart.Create(Rank.Ace, Suit.Hearts),
            //                                  Fart.Create(Rank.Three, Suit.Spades),
            //                                  Fart.Create(Rank.Four, Suit.Spades),
            //                                  Fart.Create(Rank.Jack, Suit.Spades),
            //                                  Fart.Create(Rank.King, Suit.Spades),
            //                                  Fart.Create(Rank.Ace, Suit.Spades)};


            // List<int> p0 = new List<int>() {Fart.Create(Rank.Four, Suit.Clubs),
            //                                  Fart.Create(Rank.Jack, Suit.Clubs),
            //                                  Fart.Create(Rank.Two, Suit.Diamonds),
            //                                  Fart.Create(Rank.Ace, Suit.Diamonds),
            //                                  Fart.Create(Rank.Queen, Suit.Hearts),
            //                                  Fart.Create(Rank.Ace, Suit.Hearts),
            //                                  Fart.Create(Rank.King, Suit.Spades)};
            // List<int> p1 = new List<int>() {Fart.Create(Rank.Ace, Suit.Clubs),
            //                                  Fart.Create(Rank.Jack, Suit.Diamonds),
            //                                  Fart.Create(Rank.King, Suit.Diamonds),
            //                                  Fart.Create(Rank.Seven, Suit.Diamonds),
            //                                  Fart.Create(Rank.Three, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Hearts),
            //                                  Fart.Create(Rank.Four, Suit.Spades)};
            // List<int> p2 = new List<int>() {Fart.Create(Rank.Five, Suit.Clubs),
            //                                  Fart.Create(Rank.King, Suit.Clubs),
            //                                  Fart.Create(Rank.Five, Suit.Diamonds),
            //                                  Fart.Create(Rank.Three, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Hearts),
            //                                  Fart.Create(Rank.Four, Suit.Spades),
            //                                  Fart.Create(Rank.Jack, Suit.Spades)};
            // List<int> p3 = new List<int>() {Fart.Create(Rank.Three, Suit.Clubs),
            //                                  Fart.Create(Rank.Six, Suit.Clubs),
            //                                  Fart.Create(Rank.Seven, Suit.Clubs),
            //                                  Fart.Create(Rank.Jack, Suit.Hearts),
            //                                  Fart.Create(Rank.Seven, Suit.Hearts),
            //                                  Fart.Create(Rank.Two, Suit.Spades),
            //                                  Fart.Create(Rank.Three, Suit.Spades)};


            // List<int> p0 = new List<int>() {Fart.Create(Rank.Five, Suit.Clubs),
            //                                  Fart.Create(Rank.Seven, Suit.Clubs),
            //                                  Fart.Create(Rank.Four, Suit.Diamonds),
            //                                  Fart.Create(Rank.Five, Suit.Diamonds),
            //                                  Fart.Create(Rank.Queen, Suit.Diamonds),
            //                                  Fart.Create(Rank.Two, Suit.Spades),
            //                                  Fart.Create(Rank.Six, Suit.Spades)};
            // List<int> p1 = new List<int>() {Fart.Create(Rank.Three, Suit.Clubs),
            //                                  Fart.Create(Rank.Six, Suit.Clubs),
            //                                  Fart.Create(Rank.Queen, Suit.Clubs),
            //                                  Fart.Create(Rank.Two, Suit.Diamonds),
            //                                  Fart.Create(Rank.Seven, Suit.Diamonds),
            //                                  Fart.Create(Rank.Four, Suit.Hearts),
            //                                  Fart.Create(Rank.Five, Suit.Spades)};
            // List<int> p2 = new List<int>() {Fart.Create(Rank.Jack, Suit.Clubs),
            //                                  Fart.Create(Rank.Ace, Suit.Clubs),
            //                                  Fart.Create(Rank.Jack, Suit.Diamonds),
            //                                  Fart.Create(Rank.Ace, Suit.Diamonds),
            //                                  Fart.Create(Rank.Five, Suit.Hearts),
            //                                  Fart.Create(Rank.King, Suit.Spades),
            //                                  Fart.Create(Rank.Ace, Suit.Spades)};
            // List<int> p3 = new List<int>() {Fart.Create(Rank.Two, Suit.Hearts),
            //                                  Fart.Create(Rank.Three, Suit.Hearts),
            //                                  Fart.Create(Rank.Six, Suit.Hearts),
            //                                  Fart.Create(Rank.King, Suit.Hearts),
            //                                  Fart.Create(Rank.Seven, Suit.Hearts),
            //                                  Fart.Create(Rank.Three, Suit.Spades),
            //                                  Fart.Create(Rank.Queen, Suit.Spades)};




            SuecaGame game = new SuecaGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetJustPlayed(), false);

            //            int[] a = new int[6] { 1, 2, 3, 4, 5, 0 };
            //            int z = a[2];
            //            z++;
            //
            //            foreach (var i in a)
            //            {
            //                Console.WriteLine(i);
            //            }

            int card = p0[0];
            int cardValueInTrick = game.SampleGame(card);
            infoSet.AddCardValue(card, cardValueInTrick);
            infoSet.PrintInfoSet();
        }
    }
}