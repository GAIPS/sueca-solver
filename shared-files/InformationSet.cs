using System;
using System.Collections;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class InformationSet
    {
        private List<int> hand;
        private List<Move> currentTrick;
        public int Trump;
        private Dictionary<int,int> dictionary;
        private Deck deck;
        private Dictionary<int,List<int>> suitHasPlayer;
        public int BotTeamPoints;
        public int OtherTeamPoints;
        private int trickPoints;


        public InformationSet(List<int> currentHand, int trumpSuit, Random randomLOL, int seed)
        {
            Trump = trumpSuit;
            hand = new List<int>(currentHand);
            dictionary = new Dictionary<int,int>();
            suitHasPlayer = new Dictionary<int,List<int>>
            {
                { (int)Suit.Clubs, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Diamonds, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Hearts, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Spades, new List<int>(3){ 1, 2, 3 } }
            };
            currentTrick = new List<Move>();
            deck = new Deck(currentHand, randomLOL, seed);
            BotTeamPoints = 0;
            OtherTeamPoints = 0;
            trickPoints = 0;
        }

        public int GetHandSize()
        {
            return hand.Count;
        }


        public List<int> GetPossibleMoves()
        {
            return SuecaGame.PossibleMoves(hand, GetLeadSuit());
        }

        public int GetLeadSuit()
        {
            if (currentTrick.Count == 0)
            {
                return (int)Suit.None;
            }

            return Card.GetSuit(currentTrick[0].Card);
        }

        public List<Move> GetCardsOnTable()
        {
            return currentTrick;
        }

        public int GetHighestCardIndex()
        {
            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in dictionary)
            {
                if (cardValue.Value > bestValue)
                {
                    bestValue = cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetHighestCardIndex()");
            }

            return bestCard;
        }

        public void AddPlay(int playerID, int card)
        {
            int leadSuit = GetLeadSuit();
            if (Card.GetSuit(card) != leadSuit && leadSuit != (int)Suit.None)
            {
                suitHasPlayer[leadSuit].Remove(playerID);
            }

            if (currentTrick.Count == 3)
            {
                currentTrick.Clear();
                trickPoints += Card.GetValue(card);
            }
            else
            {
                if (currentTrick.Count == 0)
                {
                    if (playerID == 0 || playerID == 2)
                    {
                        BotTeamPoints += trickPoints;
                    }
                    else
                    {
                        OtherTeamPoints += trickPoints;
                    }
                    trickPoints = 0;
                }
                trickPoints += Card.GetValue(card);
                currentTrick.Add(new Move(playerID, card));
            }

            deck.RemoveCard(card);
        }

        public void AddMyPlay(int card)
        {
            if (currentTrick.Count == 3)
            {
                currentTrick.Clear();
            }
            else
            {
                currentTrick.Add(new Move(0, card));
            }
            hand.Remove(card);
        }

        public void CleanCardValues()
        {
            dictionary.Clear();
        }

        public void AddCardValue(int card, int val)
        {
            if (dictionary.ContainsKey(card))
            {
                dictionary[card] += val;
            }
            else
            {
                dictionary[card] = val;
            }
        }

        private bool checkPlayersHaveAllSuits(Dictionary<int,List<int>> suitHasPlayer)
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

        public List<List<int>> Sample()
        {
            List<List<int>> hands = new List<List<int>>();
            int myHandSize = hand.Count;
            int[] handSizes = new int[3] { myHandSize, myHandSize, myHandSize };
            int currentTrickSize = currentTrick.Count;

            for (int i = 0; i < currentTrickSize; i++)
            {
                handSizes[2 - i]--;
            }

            hands.Add(new List<int>(hand));
            List<List<int>> sampledHands;

            if (checkPlayersHaveAllSuits(suitHasPlayer))
            {
                sampledHands = deck.SampleHands(handSizes);
            }
            else
            {
                sampledHands = deck.SampleHands(suitHasPlayer, handSizes);
            }

            for (int i = 0; i < 3; i++)
            {
                hands.Add(sampledHands[i]);
            }

            return hands;
        }


        private void printDictionary(string name)
        {
            string str = name + " -";
            foreach (KeyValuePair<int, int> cardValue in dictionary)
            {
                str += " <" + Card.ToString(cardValue.Key) + "," + cardValue.Value + ">";
            }
            Console.WriteLine(str);
        }


        public void PrintInfoSet()
        {
            Console.WriteLine("------------------INFOSET------------------");
            SuecaGame.PrintCards("Hand", hand);
            Console.WriteLine("Trump - " + Trump);
            printDictionary("Dictionary");
            Console.WriteLine("-------------------------------------------");
        }
    }
}