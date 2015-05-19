using System;
using System.Collections.Generic;
using SuecaSolver;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Security.Cryptography.X509Certificates;

namespace SuecaSolver
{
    public class Test
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
            int NUM_TRICKS = 7;
            Deck deck = new Deck();
            List<int> hand = deck.GetHand(NUM_TRICKS);
            InformationSet infoSet = new InformationSet(hand, (int)Suit.Clubs);
//            PIMC pimc = new PIMC(1);
//            pimc.ExecuteTestVersion(infoSet, hand, NUM_TRICKS);



            List<int> p0 = new List<int>()
            {Card.Create(Rank.Jack, Suit.Clubs),
                Card.Create(Rank.Five, Suit.Diamonds),
                Card.Create(Rank.Jack, Suit.Hearts),
                Card.Create(Rank.King, Suit.Hearts),
                Card.Create(Rank.Two, Suit.Spades),
                Card.Create(Rank.Jack, Suit.Spades),
                Card.Create(Rank.Seven, Suit.Spades)
            };
            List<int> p1 = new List<int>()
            {Card.Create(Rank.Six, Suit.Clubs),
                Card.Create(Rank.Two, Suit.Diamonds),
                Card.Create(Rank.Queen, Suit.Hearts),
                Card.Create(Rank.Four, Suit.Spades),
                Card.Create(Rank.Six, Suit.Spades),
                Card.Create(Rank.Queen, Suit.Spades),
                Card.Create(Rank.Ace, Suit.Spades)
            };
            List<int> p2 = new List<int>()
            {Card.Create(Rank.Two, Suit.Clubs),
                Card.Create(Rank.Three, Suit.Clubs),
                Card.Create(Rank.Queen, Suit.Clubs),
                Card.Create(Rank.King, Suit.Clubs),
                Card.Create(Rank.Ace, Suit.Clubs),
                Card.Create(Rank.Four, Suit.Diamonds),
                Card.Create(Rank.King, Suit.Spades)
            };
            List<int> p3 = new List<int>()
            {Card.Create(Rank.Five, Suit.Clubs),
                Card.Create(Rank.Seven, Suit.Clubs),
                Card.Create(Rank.Three, Suit.Diamonds),
                Card.Create(Rank.Six, Suit.Diamonds),
                Card.Create(Rank.King, Suit.Diamonds),
                Card.Create(Rank.Seven, Suit.Diamonds),
                Card.Create(Rank.Four, Suit.Hearts)
            };


//            List<int> p0 = new List<int>()
//            {Card.Create(Rank.Four, Suit.Diamonds),
//                Card.Create(Rank.Six, Suit.Diamonds),
//                Card.Create(Rank.Jack, Suit.Diamonds),
//                Card.Create(Rank.Seven, Suit.Diamonds),
//                Card.Create(Rank.Ace, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Spades),
//                Card.Create(Rank.Seven, Suit.Spades)
//            };
//            List<int> p1 = new List<int>()
//            {Card.Create(Rank.Six, Suit.Clubs),
//                Card.Create(Rank.Five, Suit.Diamonds),
//                Card.Create(Rank.King, Suit.Diamonds),
//                Card.Create(Rank.Two, Suit.Hearts),
//                Card.Create(Rank.Six, Suit.Hearts),
//                Card.Create(Rank.Two, Suit.Spades),
//                Card.Create(Rank.Jack, Suit.Spades)
//            };
//            List<int> p2 = new List<int>()
//            {Card.Create(Rank.Two, Suit.Clubs),
//                Card.Create(Rank.Three, Suit.Clubs),
//                Card.Create(Rank.King, Suit.Clubs),
//                Card.Create(Rank.Two, Suit.Diamonds),
//                Card.Create(Rank.Five, Suit.Hearts),
//                Card.Create(Rank.King, Suit.Hearts),
//                Card.Create(Rank.Queen, Suit.Spades)
//            };
//            List<int> p3 = new List<int>()
//            {Card.Create(Rank.Queen, Suit.Clubs),
//                Card.Create(Rank.Ace, Suit.Diamonds),
//                Card.Create(Rank.Three, Suit.Hearts),
//                Card.Create(Rank.Four, Suit.Hearts),
//                Card.Create(Rank.Four, Suit.Spades),
//                Card.Create(Rank.Six, Suit.Spades),
//                Card.Create(Rank.King, Suit.Spades)
//            };


//            List<int> p0 = new List<int>()
//            {Card.Create(Rank.Three, Suit.Clubs),
//                Card.Create(Rank.Six, Suit.Clubs),
//                Card.Create(Rank.Seven, Suit.Clubs),
//                Card.Create(Rank.Two, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Hearts),
//                Card.Create(Rank.Seven, Suit.Hearts),
//                Card.Create(Rank.Two, Suit.Spades)
//            };
//            List<int> p1 = new List<int>()
//            {Card.Create(Rank.Queen, Suit.Clubs),
//                Card.Create(Rank.King, Suit.Clubs),
//                Card.Create(Rank.Three, Suit.Diamonds),
//                Card.Create(Rank.Three, Suit.Hearts),
//                Card.Create(Rank.Six, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Spades),
//                Card.Create(Rank.Seven, Suit.Spades)
//            };
//            List<int> p2 = new List<int>()
//            {Card.Create(Rank.Jack, Suit.Clubs),
//                Card.Create(Rank.Four, Suit.Diamonds),
//                Card.Create(Rank.Six, Suit.Diamonds),
//                Card.Create(Rank.Queen, Suit.Diamonds),
//                Card.Create(Rank.King, Suit.Diamonds),
//                Card.Create(Rank.Jack, Suit.Hearts),
//                Card.Create(Rank.Queen, Suit.Spades)
//            };
//            List<int> p3 = new List<int>()
//            {Card.Create(Rank.Five, Suit.Diamonds),
//                Card.Create(Rank.Ace, Suit.Hearts),
//                Card.Create(Rank.Three, Suit.Spades),
//                Card.Create(Rank.Four, Suit.Spades),
//                Card.Create(Rank.Jack, Suit.Spades),
//                Card.Create(Rank.King, Suit.Spades),
//                Card.Create(Rank.Ace, Suit.Spades)
//            };


//            List<int> p0 = new List<int>()
//            {Card.Create(Rank.Four, Suit.Clubs),
//                Card.Create(Rank.Jack, Suit.Clubs),
//                Card.Create(Rank.Two, Suit.Diamonds),
//                Card.Create(Rank.Ace, Suit.Diamonds),
//                Card.Create(Rank.Queen, Suit.Hearts),
//                Card.Create(Rank.Ace, Suit.Hearts),
//                Card.Create(Rank.King, Suit.Spades)
//            };
//            List<int> p1 = new List<int>()
//            {Card.Create(Rank.Ace, Suit.Clubs),
//                Card.Create(Rank.Jack, Suit.Diamonds),
//                Card.Create(Rank.King, Suit.Diamonds),
//                Card.Create(Rank.Seven, Suit.Diamonds),
//                Card.Create(Rank.Three, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Hearts),
//                Card.Create(Rank.Four, Suit.Spades)
//            };
//            List<int> p2 = new List<int>()
//            {Card.Create(Rank.Five, Suit.Clubs),
//                Card.Create(Rank.King, Suit.Clubs),
//                Card.Create(Rank.Five, Suit.Diamonds),
//                Card.Create(Rank.Three, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Hearts),
//                Card.Create(Rank.Four, Suit.Spades),
//                Card.Create(Rank.Jack, Suit.Spades)
//            };
//            List<int> p3 = new List<int>()
//            {Card.Create(Rank.Three, Suit.Clubs),
//                Card.Create(Rank.Six, Suit.Clubs),
//                Card.Create(Rank.Seven, Suit.Clubs),
//                Card.Create(Rank.Jack, Suit.Hearts),
//                Card.Create(Rank.Seven, Suit.Hearts),
//                Card.Create(Rank.Two, Suit.Spades),
//                Card.Create(Rank.Three, Suit.Spades)
//            };


//            List<int> p0 = new List<int>()
//            {Card.Create(Rank.Five, Suit.Clubs),
//                Card.Create(Rank.Seven, Suit.Clubs),
//                Card.Create(Rank.Four, Suit.Diamonds),
//                Card.Create(Rank.Five, Suit.Diamonds),
//                Card.Create(Rank.Queen, Suit.Diamonds),
//                Card.Create(Rank.Two, Suit.Spades),
//                Card.Create(Rank.Six, Suit.Spades)
//            };
//            List<int> p1 = new List<int>()
//            {Card.Create(Rank.Three, Suit.Clubs),
//                Card.Create(Rank.Six, Suit.Clubs),
//                Card.Create(Rank.Queen, Suit.Clubs),
//                Card.Create(Rank.Two, Suit.Diamonds),
//                Card.Create(Rank.Seven, Suit.Diamonds),
//                Card.Create(Rank.Four, Suit.Hearts),
//                Card.Create(Rank.Five, Suit.Spades)
//            };
//            List<int> p2 = new List<int>()
//            {Card.Create(Rank.Jack, Suit.Clubs),
//                Card.Create(Rank.Ace, Suit.Clubs),
//                Card.Create(Rank.Jack, Suit.Diamonds),
//                Card.Create(Rank.Ace, Suit.Diamonds),
//                Card.Create(Rank.Five, Suit.Hearts),
//                Card.Create(Rank.King, Suit.Spades),
//                Card.Create(Rank.Ace, Suit.Spades)
//            };
//            List<int> p3 = new List<int>()
//            {Card.Create(Rank.Two, Suit.Hearts),
//                Card.Create(Rank.Three, Suit.Hearts),
//                Card.Create(Rank.Six, Suit.Hearts),
//                Card.Create(Rank.King, Suit.Hearts),
//                Card.Create(Rank.Seven, Suit.Hearts),
//                Card.Create(Rank.Three, Suit.Spades),
//                Card.Create(Rank.Queen, Suit.Spades)
//            };




            SuecaGame game = new SuecaGame(7, p0, p1, p2, p3, infoSet.Trump, infoSet.GetCardsOnTable());
            int card = p0[0];
            int cardValueInTrick = game.SampleGame(card);
            game.PrintTricks();
            infoSet.AddCardValue(card, cardValueInTrick);
            infoSet.PrintInfoSet();
        }
    }
}