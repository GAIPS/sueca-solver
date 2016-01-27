using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class PlayerNode
    {

        public int Id;
        public List<int> Hand;
        public Dictionary<int, int> HasSuit;
        public int NumCuts;
        protected bool USE_CACHE;


        public PlayerNode(int id, List<int> hand, bool cacheFlag)
        {
            Id = id;
            NumCuts = 0;
            Hand = hand;
            USE_CACHE = cacheFlag;
            HasSuit = new Dictionary<int, int>() { { (int)Suit.Clubs, 0 }, { (int)Suit.Diamonds, 0 }, { (int)Suit.Hearts, 0 }, { (int)Suit.Spades, 0 } };

            foreach (int c in hand)
            {
                HasSuit[Card.GetSuit(c)]++;
            }
        }

        abstract public int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1);

        abstract public void ApplyMove(Move move);
        
        public virtual void UndoMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                Hand.Add(move.Card);
                HasSuit[Card.GetSuit(move.Card)]++;  
            }
            else
            {
                //keep track of other players state
            }
        }

        public int GetHighestRankFromSuit(int suit, int trump)
        {
            int highestCard = -1;
            int highestRank = -1;
            int highestTrump = -1;
            int highestTrumpRank = -1;

            foreach (int c in Hand)
            {
                if (Card.GetSuit(c) == suit && Card.GetRank(c) > highestRank)
                {
                    highestCard = c;
                    highestRank = Card.GetRank(c);
                }

                if (Card.GetSuit(c) == trump && Card.GetRank(c) > highestTrumpRank)
                {
                    highestTrump = c;
                    highestTrumpRank = Card.GetRank(c);
                }
            }

            if (highestCard == -1)
            {
                return highestTrump;
            }
            return highestCard;
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