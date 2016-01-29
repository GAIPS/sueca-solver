using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuecaSolver
{
    public class Test
    {

        public static void Main()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //int seed = Guid.NewGuid().GetHashCode();
            //Random randomNumber = new Random(seed);
            //int NUM_TRICKS = 10;
            //Deck deck = new Deck(randomNumber, seed);


            //List<int> hand = deck.GetHand(NUM_TRICKS);
            //InformationSet infoSet = new InformationSet(hand, (int)Suit.Clubs, randomNumber, seed);
            //int card = deck.GetRandomCard();
            //infoSet.AddPlay(3, card);
            //Console.WriteLine(Card.ToString(card));
            //PIMC pimc = new PIMC();
            //pimc.ExecuteWithTimeLimit(infoSet);
            //infoSet.PrintInfoSet();



            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Jack, Suit.Clubs),
            //    Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.Jack, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Hearts),
            //    Card.Create(Rank.Two, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Spades),
            //    Card.Create(Rank.Seven, Suit.Spades)
            //};
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Diamonds),
            //    Card.Create(Rank.Queen, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Spades),
            //    Card.Create(Rank.Six, Suit.Spades),
            //    Card.Create(Rank.Queen, Suit.Spades),
            //    Card.Create(Rank.Ace, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Two, Suit.Clubs),
            //    Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.Queen, Suit.Clubs),
            //    Card.Create(Rank.King, Suit.Clubs),
            //    Card.Create(Rank.Ace, Suit.Clubs),
            //    Card.Create(Rank.Four, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Five, Suit.Clubs),
            //    Card.Create(Rank.Seven, Suit.Clubs),
            //    Card.Create(Rank.Three, Suit.Diamonds),
            //    Card.Create(Rank.Six, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Diamonds),
            //    Card.Create(Rank.Four, Suit.Hearts)
            //};


            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Four, Suit.Diamonds),
            //    Card.Create(Rank.Six, Suit.Diamonds),
            //    Card.Create(Rank.Jack, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Diamonds),
            //    Card.Create(Rank.Ace, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Spades),
            //    Card.Create(Rank.Seven, Suit.Spades)
            //};
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Diamonds),
            //    Card.Create(Rank.Two, Suit.Hearts),
            //    Card.Create(Rank.Six, Suit.Hearts),
            //    Card.Create(Rank.Two, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Two, Suit.Clubs),
            //    Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.King, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Diamonds),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Hearts),
            //    Card.Create(Rank.Queen, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Queen, Suit.Clubs),
            //    Card.Create(Rank.Ace, Suit.Diamonds),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Spades),
            //    Card.Create(Rank.Six, Suit.Spades),
            //    Card.Create(Rank.King, Suit.Spades)
            //};


            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Seven, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.Seven, Suit.Hearts),
            //    Card.Create(Rank.Two, Suit.Spades)
            //};
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Queen, Suit.Clubs),
            //    Card.Create(Rank.King, Suit.Clubs),
            //    Card.Create(Rank.Three, Suit.Diamonds),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Six, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Spades),
            //    Card.Create(Rank.Seven, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Jack, Suit.Clubs),
            //    Card.Create(Rank.Four, Suit.Diamonds),
            //    Card.Create(Rank.Six, Suit.Diamonds),
            //    Card.Create(Rank.Queen, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Diamonds),
            //    Card.Create(Rank.Jack, Suit.Hearts),
            //    Card.Create(Rank.Queen, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.Ace, Suit.Hearts),
            //    Card.Create(Rank.Three, Suit.Spades),
            //    Card.Create(Rank.Four, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Spades),
            //    Card.Create(Rank.King, Suit.Spades),
            //    Card.Create(Rank.Ace, Suit.Spades)
            //};


            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Four, Suit.Clubs),
            //    Card.Create(Rank.Jack, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Diamonds),
            //    Card.Create(Rank.Ace, Suit.Diamonds),
            //    Card.Create(Rank.Queen, Suit.Hearts),
            //    Card.Create(Rank.Ace, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Spades)
            //};
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Ace, Suit.Clubs),
            //    Card.Create(Rank.Jack, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Diamonds),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Five, Suit.Clubs),
            //    Card.Create(Rank.King, Suit.Clubs),
            //    Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Seven, Suit.Clubs),
            //    Card.Create(Rank.Jack, Suit.Hearts),
            //    Card.Create(Rank.Seven, Suit.Hearts),
            //    Card.Create(Rank.Two, Suit.Spades),
            //    Card.Create(Rank.Three, Suit.Spades)
            //};


            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Five, Suit.Clubs),
            //    Card.Create(Rank.Seven, Suit.Clubs),
            //    Card.Create(Rank.Four, Suit.Diamonds),
            //    Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.Queen, Suit.Diamonds),
            //    Card.Create(Rank.Two, Suit.Spades),
            //    Card.Create(Rank.Six, Suit.Spades)
            //};
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Queen, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Diamonds),
            //    Card.Create(Rank.Four, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Jack, Suit.Clubs),
            //    Card.Create(Rank.Ace, Suit.Clubs),
            //    Card.Create(Rank.Jack, Suit.Diamonds),
            //    Card.Create(Rank.Ace, Suit.Diamonds),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Spades),
            //    Card.Create(Rank.Ace, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Two, Suit.Hearts),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Six, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Hearts),
            //    Card.Create(Rank.Seven, Suit.Hearts),
            //    Card.Create(Rank.Three, Suit.Spades),
            //    Card.Create(Rank.Queen, Suit.Spades)
            //};



            List<int> p0 = new List<int>()
            {Card.Create(Rank.Four, Suit.Diamonds),
                Card.Create(Rank.Six, Suit.Diamonds),
                Card.Create(Rank.Jack, Suit.Diamonds),
                Card.Create(Rank.Seven, Suit.Diamonds),
                Card.Create(Rank.Ace, Suit.Hearts),
                Card.Create(Rank.Five, Suit.Spades),
                Card.Create(Rank.Four, Suit.Clubs),
                Card.Create(Rank.Two, Suit.Hearts),
                Card.Create(Rank.Ace, Suit.Clubs),
                Card.Create(Rank.Seven, Suit.Spades)
            };
            //List<int> p1 = new List<int>()
            //{Card.Create(Rank.Six, Suit.Clubs),
            //    Card.Create(Rank.Five, Suit.Diamonds),
            //    Card.Create(Rank.King, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Clubs),
            //    Card.Create(Rank.Six, Suit.Hearts),
            //    Card.Create(Rank.Two, Suit.Spades),
            //    Card.Create(Rank.Three, Suit.Hearts),
            //    Card.Create(Rank.Seven, Suit.Hearts),
            //    Card.Create(Rank.Three, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Spades)
            //};
            //List<int> p2 = new List<int>()
            //{Card.Create(Rank.Two, Suit.Clubs),
            //    Card.Create(Rank.Three, Suit.Clubs),
            //    Card.Create(Rank.King, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Diamonds),
            //    Card.Create(Rank.Five, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Hearts),
            //    Card.Create(Rank.Ace, Suit.Diamonds),
            //    Card.Create(Rank.Queen, Suit.Diamonds),
            //    Card.Create(Rank.Jack, Suit.Clubs),
            //    Card.Create(Rank.Queen, Suit.Spades)
            //};
            //List<int> p3 = new List<int>()
            //{Card.Create(Rank.Queen, Suit.Clubs),
            //    Card.Create(Rank.Ace, Suit.Spades),
            //    Card.Create(Rank.Three, Suit.Diamonds),
            //    Card.Create(Rank.Four, Suit.Hearts),
            //    Card.Create(Rank.Four, Suit.Spades),
            //    Card.Create(Rank.Six, Suit.Spades),
            //    Card.Create(Rank.Jack, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Clubs),
            //    Card.Create(Rank.Queen, Suit.Hearts),
            //    Card.Create(Rank.King, Suit.Spades)
            //};


            //List<List<int>> playersHands = new List<List<int>>();
            //playersHands.Add(p0);
            //playersHands.Add(p1);
            //playersHands.Add(p2);
            //playersHands.Add(p3);
            //Sueca.PrintCards("Initial hand", p0);
            //List<int> p0Copy = new List<int>(p0);
            //MaxNode max0 = new MaxNode(0, p0Copy, false);
            //MinNode min1 = new MinNode(1, p1, false);
            //MaxNode max2 = new MaxNode(2, p2, false);
            //MinNode min3 = new MinNode(3, p3, false);
            //PerfectInformationGame lol = new PerfectInformationGame(max0, min1, max2, min3, p0Copy.Count, (int)Suit.Clubs, new List<Move>(), 0, 0);
            //int cardValue = lol.SampleGame(10, p0[0]);
            //Console.WriteLine("Card: " + Card.ToString(p0[0]) + " value: " + cardValue);



            int NUM_TRICKS = 10;
            Deck deck = new Deck();
            List<int> hand = deck.GetHand(NUM_TRICKS);
            //RBOPlayer ep = new RBOPlayer(0, hand, (int)Suit.Clubs);
            HybridPlayer ep = new HybridPlayer(0, hand, (int)Suit.Clubs);
            //RuleBasedPlayer ep = new RuleBasedPlayer(0, hand, (int)Suit.Clubs);
            //SmartPlayer ep = new SmartPlayer(0, hand, (int)Suit.Clubs);
            //TrickPlayer ep = new TrickPlayer(0, hand, (int)Suit.Clubs);
            int chosenCard = ep.Play();
            Sueca.PrintCards("Initial hand", hand);
            Console.WriteLine("Chosen card: " + Card.ToString(chosenCard));

            sw.Stop();
            Console.WriteLine("Total Time taken by functions is {0} seconds", sw.ElapsedMilliseconds / 1000); //seconds
        }
    }
}