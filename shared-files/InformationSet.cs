using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class InformationSet
    {
        private List<int> hand;
        private Trick currentTrick;
        private Trick lastTrick;
        public int Trump;
        private Dictionary<int, float> pointsPerCard;
        private Deck deck;
        private Dictionary<int, List<int>> suitHasPlayer;
        private Dictionary<int, List<int>> othersPointCards;
        public int BotTeamPoints;
        public int OtherTeamPoints;
        public int remainingTrumps;
        //public int ExpectedGameValue;
        private int trickPoints;


        public InformationSet(List<int> currentHand, int trumpSuit)
        {
            Trump = trumpSuit;
            hand = new List<int>(currentHand);
            pointsPerCard = new Dictionary<int,float>();
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
            lastTrick = null;
            deck = new Deck(currentHand);
            BotTeamPoints = 0;
            OtherTeamPoints = 0;
            remainingTrumps = 10;
            //ExpectedGameValue = 0;
            trickPoints = 0;
        }


        public int GetHandSize()
        {
            return hand.Count;
        }


        public List<int> GetPossibleMoves()
        {
            return Sueca.PossibleMoves(hand, currentTrick.LeadSuit);
        }

        public List<Move> GetCardsOnTable()
        {
            return currentTrick.GetMoves();
        }

        public int GetBestCard()
        {
            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, float> cardValue in pointsPerCard)
            {
                if (cardValue.Value > bestValue)
                {
                    bestValue = (int) cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }

        public void AddPlay(int playerID, int card)
        {
            int cardSuit = Card.GetSuit(card);
            int cardValue = Card.GetValue(card);
            
            //count points
            //to avoid trick winner computation, we assume the first player of the next trick is the winner of the last one
            if (currentTrick.IsFull())
            {
                if (playerID == 2)
                {
                    BotTeamPoints += trickPoints;
                }
                else
                {
                    OtherTeamPoints += trickPoints;
                }
                trickPoints = 0;
                lastTrick = currentTrick;
                currentTrick = new Trick(Trump);
            }
            trickPoints += cardValue;
            currentTrick.ApplyMove(new Move(playerID, card));

            //check if player has the leadSuit
            int leadSuit = currentTrick.LeadSuit;
            if (cardSuit != leadSuit && leadSuit != (int)Suit.None)
            {
                if (suitHasPlayer[leadSuit].Contains(playerID))
                {
                    suitHasPlayer[leadSuit].Remove(playerID);
                }
                else
                {
                    suitHasPlayer = new Dictionary<int, List<int>>
                    {
                        { (int)Suit.Clubs, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Diamonds, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Hearts, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Spades, new List<int>(3){ 1, 2, 3 } }
                    };
                }
            }

            //Remove pointcards from dicitonary othersPointCards
            if (cardValue > 0)
            {
                othersPointCards[cardSuit].Remove(Card.GetRank(card));
            }

            if (cardSuit == Trump)
            {
                remainingTrumps--;
            }

            deck.RemoveCard(card);
        }

        public void AddMyPlay(int card)
        {
            //count points
            //to avoid trick winner computation, we assume the first player of the next trick is the winner of the last one
            if (currentTrick.IsFull())
            {
                BotTeamPoints += trickPoints;
                trickPoints = 0;
                lastTrick = currentTrick;
                currentTrick = new Trick(Trump);
            }
            trickPoints += Card.GetValue(card);
            currentTrick.ApplyMove(new Move(0, card));
            hand.Remove(card);

            if (Card.GetSuit(card) == Trump)
            {
                remainingTrumps--;
            }
        }

        public void RemoveMyPlay(int card)
        {
            //count points
            //to avoid trick winner computation, we assume the first player of the next trick is the winner of the last one
            if (currentTrick.IsEmpty())
            {
                BotTeamPoints += trickPoints;
                trickPoints = 0;
                currentTrick = lastTrick;
            }
            trickPoints += Card.GetValue(card);
            currentTrick.ApplyMove(new Move(0, card));
            hand.Remove(card);

            if (Card.GetSuit(card) == Trump)
            {
                remainingTrumps--;
            }
        }

        public int predictTrickPoints()
        {
            return currentTrick.GetTrickWinnerAndPoints()[1];
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

        public void calculateAverageCardValues(int n)
        {
            List<int> keysCopy = new List<int>(pointsPerCard.Keys);
            foreach (int card in keysCopy)
            {
                pointsPerCard[card] /= n;
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
            int currentTrickSize = currentTrick.GetTrickSize();
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
                if (sampledHands == null)
                {
                    suitHasPlayer = new Dictionary<int, List<int>>
                    {
                        { (int)Suit.Clubs, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Diamonds, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Hearts, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Spades, new List<int>(3){ 1, 2, 3 } }
                    };
                    sampledHands = deck.SampleHands(handSizes);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                hands.Add(sampledHands[i]);
            }

            //SuecaGame.PrintCards("p0", hands[0]);
            //SuecaGame.PrintCards("p1", hands[1]);
            //SuecaGame.PrintCards("p2", hands[2]);
            //SuecaGame.PrintCards("p3", hands[3]);
            return hands;
        }

        private List<int> getHighestPerSuit(List<int> cards)
        {
            cards.Sort();
            List<int> list = new List<int>();

            int firstCard = cards[0];
            int lastCard = cards[cards.Count - 1];
            if (Card.GetSuit(firstCard) == Card.GetSuit(lastCard))
            {
                list.Add(lastCard);
                return list;
            }

            int lastSuit = Card.GetSuit(firstCard);
            for (int i = 0; i < cards.Count; i++)
			{
                int card = cards[i];
                int cardSuit = Card.GetSuit(card);

                if (cardSuit != lastSuit)
                {
                    lastSuit = cardSuit;
                    list.Add(cards[i - 1]);
                }
                if (i == cards.Count - 1)
                {
                    list.Add(card);
                }
            }
            return list;
        }

        private List<int> counterPerSuit(List<int> cards)
        {
            List<int> list = new List<int>();
            int lastSuit = (int) Suit.None;
            for (int i = 0; i < cards.Count; i++)
            {
                int cardSuit = Card.GetSuit(cards[i]);
                if (cardSuit != lastSuit)
                {
                    lastSuit = cardSuit;
                    list.Add(1);
                }
                else
                {
                    list[list.Count - 1]++;
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

            List<int> highestPerSuit = getHighestPerSuit(possibleMoves);

            if (highestPerSuit.Count == 1)
            {
                possibleMoves.Sort();
                int highestCard = possibleMoves[possibleMoves.Count - 1];
                if (shouldPlay(highestCard))
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
                int trickSize = currentTrick.GetTrickSize();
                if (trickSize == 4 || trickSize == 0)
                {
                    List<int> counterList = counterPerSuit(possibleMoves);

                    //debug
                    if(counterList.Count != highestPerSuit.Count)  
                    {
                        Console.WriteLine("PROBLEM!!! RuleBasedDecision");
                    }

                    for (int i = 0; i < highestPerSuit.Count; i++)
                    {
                        int highestCard = highestPerSuit[i];
                        if (shouldPlay(highestCard))
                        {
                            if (counterList[i] > 5 && Card.GetSuit(highestCard) == Trump)
                            {
                                return highestCard;
                            }
                            else if (counterList[i] > 5 && Card.GetSuit(highestCard) != Trump)
                            {
                                break;
                            }
                            else
                            {
                                return highestCard;
                            }
                        }
                    }
                    return possibleMoves[0];
                }
                else
                {
                    //we may check if our mate has cut the trick and chose an highest card
                    possibleMoves.Sort(new DescendingComparer());
                    return possibleMoves[0];
                }
            }
        }

        private bool shouldPlay(int highestCard)
        {
            int highestCardSuit = Card.GetSuit(highestCard);
            int highestCardRank = Card.GetRank(highestCard);

            int othersHighestRankFromSuit;
            if (othersPointCards[highestCardSuit].Count > 0)
            {
                othersHighestRankFromSuit = othersPointCards[highestCardSuit][0];
            }
            else
            {
                othersHighestRankFromSuit = 0;
            }

            int highestRankOnTable = currentTrick.HighestCardOnCurrentTrick();

            if (highestRankOnTable >= 0 && highestCardRank > highestRankOnTable && highestCardRank > othersHighestRankFromSuit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void printDictionary(string name)
        {
            string str = name + " -";
            foreach (KeyValuePair<int, float> cardValue in pointsPerCard)
            {
                str += " <" + Card.ToString(cardValue.Key) + "," + cardValue.Value + ">";
            }
            Console.WriteLine(str);
        }


        public void PrintInfoSet()
        {
            Console.WriteLine("------------------INFOSET------------------");
            Sueca.PrintCards("Hand", hand);
            Console.WriteLine("Trump - " + Trump);
            printDictionary("Dictionary");
            Console.WriteLine("-------------------------------------------");
        }

        public float GetHandHope()
        {
            int trumpCounter = 0;
            int handPoints = 0;
            int remainingPoints = 120 - BotTeamPoints - OtherTeamPoints;

            foreach (int card in hand)
            {
                if (Card.GetSuit(card) == Trump)
                {
                    trumpCounter++;
                }
                handPoints += Card.GetValue(card);
            }

            float hope = 1.0f;

            if (remainingPoints > 0 && remainingTrumps > 0)
            {
                hope = 0.7f * ((handPoints * 1.0f) / (remainingPoints * 1.0f));
                hope += 0.3f * ((trumpCounter * 1.0f) / (remainingTrumps * 1.0f));
            }
            if (remainingTrumps > 0)
            {
                hope = (trumpCounter * 1.0f) / (remainingTrumps * 1.0f);
            }
            if (remainingPoints > 0)
            {
                hope = (handPoints * 1.0f) / (remainingPoints * 1.0f);
            }
            return hope;
        }

        //internal bool ResetTrick()
        //{
        //    bool botHasplayedInCurrentTrick = false;
        //    int leadSuit = currentTrick.LeadSuit;
        //    foreach (Move move in currentTrick.GetMoves())
        //    {
        //        int cardSuit = Card.GetSuit(move.Card);
        //        int cardValue = Card.GetValue(move.Card);
        //        int playerId = move.PlayerId;

        //        if (cardSuit != leadSuit && !suitHasPlayer[leadSuit].Contains(playerId))
        //        {
        //            suitHasPlayer[leadSuit].Add(playerId);
        //        }

        //        if (playerId == 0)
        //        {
        //            botHasplayedInCurrentTrick = true;
        //            hand.Add(move.Card);
        //        }

        //        if (cardValue > 0)
        //        {
        //            othersPointCards[cardSuit].Add(Card.GetRank(move.Card));
        //        }

        //        if (cardSuit == Trump)
        //        {
        //            remainingTrumps++;
        //        }

        //        deck.Add(move.Card);
        //    }
        //    trickPoints = 0;
        //    currentTrick = new Trick(Trump);
        //    return botHasplayedInCurrentTrick;
        //}
    }
}