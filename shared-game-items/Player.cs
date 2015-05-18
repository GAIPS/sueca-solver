using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class Player
    {

        public int Id;
        public List<int> Hand;
        public Dictionary<int, int> HasSuit;
        public int NumCuts;


        public Player(int id, List<int> hand)
        {
            Id = id;
            NumCuts = 0;
            Hand = hand;
            HasSuit = new Dictionary<int, int>() { { (int)Suit.Clubs, 0 }, { (int)Suit.Diamonds, 0 }, { (int)Suit.Hearts, 0 }, { (int)Suit.Spades, 0 } };

            foreach (int c in hand)
            {
                HasSuit[Card.GetSuit(c)]++;
            }
        }


        abstract public int PlayGame(GameState gameState, int alpha, int beta, int lol, int move = -1);


        public int HighestRankForSuit(int leadSuit, int trump)
        {
            if (HasSuit[leadSuit] > 0)
            {
                int highestFromLeadSuit = 0;

                foreach (int card in Hand)
                {
                    if (Card.GetSuit(card) == leadSuit && Card.GetRank(card) + 1 > highestFromLeadSuit)
                    {
                        highestFromLeadSuit = Card.GetRank(card) + 1;
                    }
                }

                return highestFromLeadSuit;
            }
            else if (HasSuit[trump] > 0)
            {
                int highestTrump = 0;

                foreach (int card in Hand)
                {
                    if (Card.GetSuit(card) == trump && Card.GetRank(card) + 1 > highestTrump)
                    {
                        highestTrump = Card.GetRank(card) + 1;
                    }
                }

                return highestTrump * -1;
            }
            else
            {
                return 0;
            }
        }



        private void printCards(List<int> cards)
        {
            string str = "PlayerId: " + Id + " - ";
            foreach (int c in cards)
            {
                str += c.ToString() + ", ";
            }
            Console.WriteLine(str);
        }


        public void PrintHand()
        {
            printCards(Hand);
        }


        public override string ToString()
        {
            return "PID: " + Id;
        }
    }
}