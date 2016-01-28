using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class PlayerNode
    {

        public int Id;
        public List<int> Hand;
        public int NumCuts;


        public PlayerNode(int id, List<int> hand)
        {
            Id = id;
            NumCuts = 0;
            Hand = hand;
        }

        public abstract int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1);

        public abstract void ApplyMove(Move move);
        
        public abstract void UndoMove(Move move);

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