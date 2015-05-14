using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class Player
    {

        public int Id;
        public List<Card> Hand;
        public List<List<Card>> HandsBySuit;
        public Dictionary<Suit, int> HasSuit;
        public int NumCuts;


        public Player(int id, List<Card> hand)
        {
            Id = id;
            NumCuts = 0;
            Hand = new List<Card>(hand);
            HandsBySuit = new List<List<Card>>(4);
            populateHandsBySuit(hand);
            HasSuit = new Dictionary<Suit, int>()
            {
                { Suit.Clubs, 0 },
                { Suit.Diamonds, 0 },
                { Suit.Hearts, 0 },
                { Suit.Spades, 0 }
            };

            foreach (Card c in hand)
            {
                HasSuit[c.Suit]++;
            }
        }

        abstract public int PlayGame(GameState gameState, int alpha, int beta, int lol, Card move = null);

        abstract public int PlayTrick(GameState gameState, int alpha, int beta, Card move = null);

        protected void applyMove(Card card)
        {
            Hand.Remove(card);
            HasSuit[card.Suit]--;
//            HandsBySuit[(int)card.Suit].Remove(card);
        }

        protected void undoMove(Card card)
        {
            Hand.Add(card);
            HasSuit[card.Suit]++;
//            HandsBySuit[(int)card.Suit].Add(card);
        }

        private void populateHandsBySuit(List<Card> hand)
        {
            for (int i = 0; i < 4; i++)
            {
                HandsBySuit.Add(new List<Card>());
            }
            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];
                int suit = (int)card.Suit;
                HandsBySuit[suit].Add(card);
            }
        }


        public List<Card> PossibleMoves(Suit leadSuit)
        {
            if (leadSuit == Suit.None || HasSuit[leadSuit] == 0)
            {
                List<Card> result = new List<Card>(HandsBySuit[0]);
                for (int i = 1; i < 4; i++)
                {
                    result.AddRange(HandsBySuit[i]);
                }
                return result;
            }

            return new List<Card>(HandsBySuit[(int)leadSuit]);
        }

        public int HighestRankForSuit(Suit leadSuit, Suit trump)
        {
            if (HasSuit[leadSuit] > 0)
            {
                int highestFromLeadSuit = 0;
        
                foreach (Card card in Hand)
                {
                    if (card.Suit == leadSuit && ((int)card.Rank) + 1 > highestFromLeadSuit)
                    {
                        highestFromLeadSuit = ((int)card.Rank) + 1;
                    }
                }
        
                return highestFromLeadSuit;
            }
            else if (HasSuit[trump] > 0)
            {
                int highestTrump = 0;
        
                foreach (Card card in Hand)
                {
                    if (card.Suit == trump && ((int)card.Rank) + 1 > highestTrump)
                    {
                        highestTrump = ((int)card.Rank) + 1;
                    }
                }
        
                return highestTrump * -1;
            }
            else
            {
                return 0;
            }
        }

        public void printHandsBySuit()
        {
            Console.WriteLine("----- HandsBySuit of player " + Id + " -----");
            for (int i = 0; i < HandsBySuit.Count; i++)
            {
                SuecaGame.PrintCards((Suit)i + " suit", HandsBySuit[i]);
            }
            Console.WriteLine("-----------------------------------");
        }


        private void printCards(List<Card> cards)
        {
            string str = "PlayerId: " + Id + " - ";
            foreach (Card c in cards)
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