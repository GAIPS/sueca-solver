using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class PlayerNode
    {

        public int Id;
        public List<int> Hand;
        public InformationSet InfoSet;


        public PlayerNode(int id, List<int> hand, int trumpCard, int trumpPlayerId)
        {
            Id = id;
            Hand = hand;
            InfoSet = new InformationSet(id, hand, trumpCard, trumpPlayerId);
        }

        public PlayerNode(int id, List<int> hand, InformationSet infoSet)
        {
            Id = id;
            Hand = hand;
            this.InfoSet = infoSet;
        }

        public abstract int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1);

        public void ApplyMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                if (Hand.Remove(move.Card) == false)
                {
                    //Console.WriteLine("PLAYERNODE Trying to remove an nonexisting card!!!");
                }
            }
            else
            {
            }
            InfoSet.AddPlay(move.PlayerId, move.Card);
        }

        public void UndoMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                Hand.Add(move.Card);
            }
            else
            {
            }
            InfoSet.RemovePlay(move.PlayerId, move.Card);
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