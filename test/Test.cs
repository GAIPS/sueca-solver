using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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



            //List<int> p0 = new List<int>()
            //{Card.Create(Rank.Four, Suit.Diamonds),
            //    Card.Create(Rank.Six, Suit.Diamonds),
            //    Card.Create(Rank.Jack, Suit.Diamonds),
            //    Card.Create(Rank.Seven, Suit.Diamonds),
            //    Card.Create(Rank.Ace, Suit.Hearts),
            //    Card.Create(Rank.Five, Suit.Spades),
            //    Card.Create(Rank.Four, Suit.Clubs),
            //    Card.Create(Rank.Two, Suit.Hearts),
            //    Card.Create(Rank.Ace, Suit.Clubs),
            //    Card.Create(Rank.Seven, Suit.Spades)
            //};
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
            //List<int> p0Copy = new List<int>(p0);

            //foreach (int card in p0Copy)
            //{
            //    SuecaGame game = new SuecaGame(7, playersHands, (int)Suit.Clubs, new List<Move>(), 0, 0, true);
            //    int cardValueInTrick = game.SampleGame(10, card);
            //    Console.WriteLine("Card " + Card.ToString(card) + " gave " + cardValueInTrick);
            //}


            int seed = Guid.NewGuid().GetHashCode();
            Random randomNumber = new Random(seed);
            int NUM_TRICKS = 10;
            Deck deck = new Deck(randomNumber, seed);
            List<int> hand = deck.GetHand(NUM_TRICKS);
            //ElephantPlayer ep = new ElephantPlayer(0, hand, (int)Suit.Clubs, randomNumber, seed);
            SmartPlayer ep = new SmartPlayer(0, hand, (int)Suit.Clubs, randomNumber, seed);
            //TrickPlayer ep = new TrickPlayer(0, hand, (int)Suit.Clubs, randomNumber, seed);
            int chosenCard = ep.Play();

            SuecaGame.PrintCards("Initial hand", hand);
            Console.WriteLine("Chosen card: " + Card.ToString(chosenCard));

            sw.Stop();
            Console.WriteLine("Total Time taken by functions is {0} seconds", sw.ElapsedMilliseconds / 1000); //seconds
        }
    }
}