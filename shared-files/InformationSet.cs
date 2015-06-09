using System;
using System.Collections;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class InformationSet
    {
        private List<int> hand;
        private Trick currentTrick;
        public int Trump;
        private Dictionary<int,int> pointsPerCard;
        private Deck deck;
        private Dictionary<int, List<int>> suitHasPlayer;
        private Dictionary<int, List<int>> othersPointCards;
        public int BotTeamPoints;
        public int OtherTeamPoints;
        private int trickPoints;


        public InformationSet(List<int> currentHand, int trumpSuit, Random randomLOL, int seed)
        {
            Trump = trumpSuit;
            hand = new List<int>(currentHand);
            pointsPerCard = new Dictionary<int,int>();
            suitHasPlayer = new Dictionary<int,List<int>>
            {
                { (int)Suit.Clubs, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Diamonds, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Hearts, new List<int>(3){ 1, 2, 3 } },
                { (int)Suit.Spades, new List<int>(3){ 1, 2, 3 } }
            };
            othersPointCards = new Dictionary<int, List<int>>
            {
                { (int)Suit.Clubs, new List<int>(3){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Diamonds, new List<int>(3){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Hearts, new List<int>(3){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Spades, new List<int>(3){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } }
            };

            //remove my point cards from the dictionary othersPointCards
            for (int i = 0; i < hand.Count; i++)
            {
                int card = hand[i];
                int suit = Card.GetSuit(card);
                int rank = Card.GetRank(card);
                othersPointCards[suit].Remove(rank);
            }

            currentTrick = new Trick(Trump);
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
            return SuecaGame.PossibleMoves(hand, currentTrick.LeadSuit);
        }

        public List<Move> GetCardsOnTable()
        {
            return currentTrick.GetMoves();
        }

        public int GetHighestCardIndex()
        {
            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in pointsPerCard)
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
            int cardSuit = Card.GetSuit(card);
            int cardValue = Card.GetValue(card);
            
            //count points
            if (currentTrick.IsFull())
            {
                OtherTeamPoints += trickPoints;
                trickPoints = 0;
                currentTrick = new Trick(Trump);
            }
            trickPoints += cardValue;
            currentTrick.ApplyMove(new Move(playerID, card));

            //check if player has the leadSuit
            int leadSuit = currentTrick.LeadSuit;
            if (cardSuit != leadSuit && leadSuit != (int)Suit.None)
            {
                suitHasPlayer[leadSuit].Remove(playerID);
            }

            //Remove pointcards from dicitonary othersPointCards
            if (cardValue > 0)
            {
                othersPointCards[cardSuit].Remove(Card.GetRank(card));
            }

            deck.RemoveCard(card);
        }

        public void AddMyPlay(int card)
        {
            if (currentTrick.IsFull())
            {
                BotTeamPoints += trickPoints;
                trickPoints = 0;
                currentTrick = new Trick(Trump);
            }
            trickPoints += Card.GetValue(card);
            currentTrick.ApplyMove(new Move(0, card));
            hand.Remove(card);
        }

        public void CleanCardValues()
        {
            pointsPerCard.Clear();
        }

        public void AddCardValue(int card, int val)
        {
            if (pointsPerCard.ContainsKey(card))
            {
                pointsPerCard[card] += val;
            }
            else
            {
                pointsPerCard[card] = val;
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
            int currentTrickSize = currentTrick.GetPlayInTrick();
            if (currentTrickSize > 3)
            {
                currentTrickSize = 0;
            }
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

        private List<int> getSuits(List<int> cards)
        {
            cards.Sort();
            int lastSuit = (int) Suit.None;
            List<int> list = new List<int>();
            foreach (var card in cards)
            {
                int cardSuit = Card.GetSuit(card);
                if (cardSuit != lastSuit)
                {
                    lastSuit = cardSuit;
                    list.Add(cardSuit);
                }
            }
            return list;
        }

        public int RuleBasedDecision()
        {
            List<int> possibleMoves = GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            List<int> suitsFromMoves = getSuits(possibleMoves);

            if (suitsFromMoves.Count == 1)
            {
                possibleMoves.Sort();
                int highestCard = possibleMoves[possibleMoves.Count - 1];
                int highestCardSuit = Card.GetSuit(highestCard);

                int othersHighestRankFromSuit;
                if (othersPointCards[highestCardSuit].Count > 0)
                {
                    othersHighestRankFromSuit = othersPointCards[highestCardSuit][0];
                }
                else
                {
                    othersHighestRankFromSuit = -1;
                }

                int highestOnTable = currentTrick.HighestCardOnCurrentTrick();

                if (Card.GetRank(highestCard) > othersHighestRankFromSuit)
                {
                    return highestCard;
                }
                else
                {
                    return possibleMoves[0];
                }
            }
            else
            {
                return possibleMoves[0];
            }
        }


        private void printDictionary(string name)
        {
            string str = name + " -";
            foreach (KeyValuePair<int, int> cardValue in pointsPerCard)
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