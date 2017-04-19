using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class InformationSet
    {
        private int id;
        private List<int> hand;
        public int Trump;
        private Deck unknownOwnerCards;
        public int TrumpCard;
        public int TrumpPlayerId;
        private bool trumpCardWasPlayed;
        private Dictionary<int, List<int>> suitHasPlayer;
        private Dictionary<int, List<int>> othersPointCards;
        public int MyTeamPoints;
        public int OtherTeamPoints;
        private int remainingTrumps;
        private List<Trick> tricks;


        public InformationSet(int id, List<int> currentHand, int trumpCard, int trumpPlayerId)
        {
            this.id = id;
            Trump = Card.GetSuit(trumpCard);
            this.TrumpCard = trumpCard;
            this.TrumpPlayerId = trumpPlayerId;
            trumpCardWasPlayed = false;
            hand = new List<int>(currentHand);
            suitHasPlayer = new Dictionary<int,List<int>>
            {
                { (int)Suit.Clubs, new List<int>(4){ 0, 1, 2, 3 } },
                { (int)Suit.Diamonds, new List<int>(4){ 0, 1, 2, 3 } },
                { (int)Suit.Hearts, new List<int>(4){ 0, 1, 2, 3 } },
                { (int)Suit.Spades, new List<int>(4){ 0, 1, 2, 3 } }
            };
            othersPointCards = new Dictionary<int, List<int>>
            {
                { (int)Suit.Clubs, new List<int>(5){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Diamonds, new List<int>(5){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Hearts, new List<int>(5){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } },
                { (int)Suit.Spades, new List<int>(5){ (int) Rank.Ace, (int) Rank.Seven, (int) Rank.King, (int) Rank.Jack, (int) Rank.Queen } }
            };

            //remove my point cards from the dictionary othersPointCards
            for (int i = 0; i < hand.Count; i++)
            {
                int card = hand[i];
                int suit = Card.GetSuit(card);
                int rank = Card.GetRank(card);
                othersPointCards[suit].Remove(rank);
            }
            suitHasPlayer[(int)Suit.Clubs].Remove(id);
            suitHasPlayer[(int)Suit.Diamonds].Remove(id);
            suitHasPlayer[(int)Suit.Hearts].Remove(id);
            suitHasPlayer[(int)Suit.Spades].Remove(id);
            unknownOwnerCards = new Deck(currentHand);
            unknownOwnerCards.RemoveCard(trumpCard);
            MyTeamPoints = 0;
            OtherTeamPoints = 0;
            remainingTrumps = 10;
            
            tricks = new List<Trick>();
            tricks.Add(new Trick(Trump));
        }

        internal int GetCurrentTrickResponsible()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                int partnerID = ((id + 2) % 4);
                int winnerId = currentTrick.GetCurrentTrickWinner();
                if (winnerId == id || winnerId == partnerID)
                {
                    return winnerId;
                }
                else
                {
                    int myPlayPoints = Card.GetValue(currentTrick.GetPlayOf(id));
                    int partnerPlayPoints = Card.GetValue(currentTrick.GetPlayOf(partnerID));
                    
                    return myPlayPoints >= partnerPlayPoints ? id : partnerID;
                }
            }
            return -1;
        }

        public int GetHandSize()
        {
            return hand.Count;
        }

        public List<Move> GetCurrentTrickMoves()
        {
            return tricks[tricks.Count - 1].GetMoves();
        }

        public List<Trick> GetPastMoves()
        {
            return tricks;
        }


        public List<int> GetPossibleMoves()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return Sueca.PossibleMoves(hand, currentTrick.LeadSuit);
        }

        public void AddPlay(int playerID, int card)
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == id || winnerPoints[0] == (id + 2) % 4)
                {
                    MyTeamPoints += winnerPoints[1];
                }
                else
                {
                    //TODO checks valence of points!!!
                    OtherTeamPoints += winnerPoints[1];
                }
                tricks.Add(new Trick(Trump));
                currentTrick = tricks[tricks.Count - 1];
            }

            //check if player has the leadSuit
            int leadSuit = currentTrick.LeadSuit;
            int cardSuit = Card.GetSuit(card);
            int cardValue = Card.GetValue(card);

            if (playerID != id && cardSuit == leadSuit && !suitHasPlayer[leadSuit].Contains(playerID))
            {
                Console.WriteLine("AddPlay: The player has renounced!");
            }

            currentTrick.ApplyMove(new Move(playerID, card));

            if (playerID != id && cardSuit != leadSuit && leadSuit != (int)Suit.None)
            {
                if (suitHasPlayer[leadSuit].Contains(playerID))
                {
                    suitHasPlayer[leadSuit].Remove(playerID);
                }
            }

            if (cardSuit == Trump)
            {
                remainingTrumps--;
            }
            if (playerID == id)
            {
                if (hand.Remove(card) == false)
                {
                    //Console.WriteLine("INFOSET Trying to remove an nonexisting card!!!");
                }
            }
            else
            {
                if (TrumpCard == card)
                {
                    trumpCardWasPlayed = true;
                }
                else
                {
                    if (unknownOwnerCards.RemoveCard(card) == false)
                    {
                        Console.WriteLine("FILIPA");
                    }
                }
                if (cardValue > 0)
                {
                    othersPointCards[cardSuit].Remove(Card.GetRank(card));
                }
            }
        }

        public void RemovePlay(int playerId, int card)
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == id || winnerPoints[0] == (id + 2) % 4)
                {
                    MyTeamPoints -= winnerPoints[1];
                }
                else
                {
                    //TODO checks valence of points!!!
                    OtherTeamPoints -= winnerPoints[1];
                }
            }

            currentTrick.UndoMove();

            if (currentTrick.IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }

            if (playerId == id)
            {
                hand.Add(card);
            }
            else
            {
                int cardSuit = Card.GetSuit(card);
                if (!suitHasPlayer[cardSuit].Contains(playerId))
                {
                    suitHasPlayer[cardSuit].Add(playerId);
                }
                if (TrumpCard == card)
                {
                    trumpCardWasPlayed = false;
                }
                else
                {
                    unknownOwnerCards.Add(card);
                }
                if (Card.GetValue(card) > 0)
                {
                    othersPointCards[Card.GetSuit(card)].Add(Card.GetRank(card));
                }
            }

            if (Card.GetSuit(card) == Trump)
            {
                remainingTrumps++;
            }
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            int[] trickAndWinner = currentTrick.GetTrickWinnerAndPoints();
            return new int[] { trickAndWinner[0], trickAndWinner[1], tricks.Count - 1 };
        }

        public int GetCurrentTrickWinner()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.GetCurrentTrickWinner();
        }

        public int GetCurrentTrickPoints()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.GetCurrentTrickPoints();
        }

        public bool HasNewTrickWinner()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.HasNewTrickWinner();
        }

        public bool HasNewTrickTeamWinner()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.HasNewTrickTeamWinner();
        }

        internal bool IsLastPlayOfTrick()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.IsLastPlayOfTrick();
        }

        public int GetTrickIncrease()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            return currentTrick.GetTrickIncrease();
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
            int[] playerIDs = new int[] { (id + 1) % 4, (id + 2) % 4, (id + 3) % 4 };
            int[] handSizes = new int[] { myHandSize, myHandSize, myHandSize };
            
            int currentTrickSize = tricks[tricks.Count - 1].GetCurrentTrickSize();
            if (currentTrickSize > 3)
            {
                currentTrickSize = 0;
            }
            for (int i = 0; i < currentTrickSize; i++)
            {
                //int pid = (4 + id - i - 1) % 4;
                //int pIndex = ((4 + pid - id) % 4) - 1;
                //if (playerIDs[pIndex] != pid)
                //{
                //    Console.WriteLine("InfoSet::Sample >> Wrong calculation");
                //}
                int pIndex = 3 - currentTrickSize + i;
                handSizes[pIndex]--;
            }

            hands.Add(new List<int>(hand));
            List<List<int>> sampledHands = new List<List<int>>(
                new List<int>[] {
                    new List<int>(handSizes[0]),
                    new List<int>(handSizes[1]),
                    new List<int>(handSizes[2]) });
            if (!trumpCardWasPlayed && TrumpPlayerId != id)
            {
                int trumpPlayerIDindex = ((TrumpPlayerId - id + 4) % 4) - 1;
                sampledHands[trumpPlayerIDindex].Add(TrumpCard);
            }
            if (checkPlayersHaveAllSuits(suitHasPlayer))
            {
                unknownOwnerCards.SampleHands(ref sampledHands);
            }
            else
            {

                sampledHands = unknownOwnerCards.SampleHands(suitHasPlayer, playerIDs, ref sampledHands);
                if (sampledHands == null)
                {
                    Console.WriteLine("Warning >> CSP returned null");
                    suitHasPlayer = new Dictionary<int, List<int>>
                    {
                        { (int)Suit.Clubs, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Diamonds, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Hearts, new List<int>(3){ 1, 2, 3 } },
                        { (int)Suit.Spades, new List<int>(3){ 1, 2, 3 } }
                    };
                    Console.WriteLine("resetting sampled hands to h0 " + handSizes[0] + " h1 " + handSizes[1] + " h2 " + handSizes[2]);
                    sampledHands = new List<List<int>>(
                        new List<int>[] {
                            new List<int>(handSizes[0]),
                            new List<int>(handSizes[1]),
                            new List<int>(handSizes[2]) });
                    if (!trumpCardWasPlayed && TrumpPlayerId != id)
                    {
                        int trumpPlayerIDindex = ((TrumpPlayerId - id + 4) % 4) - 1;
                        sampledHands[trumpPlayerIDindex].Add(TrumpCard);
                    }
                    unknownOwnerCards.SampleHands(ref sampledHands);
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

        private int getRandomStrawCard(List<int> possibleCards)
        {
            List<int> strawCards = new List<int>();
            int randomIndex;
            foreach (int c in possibleCards)
            {
                if (Card.GetValue(c) == 0)
	            {
                    strawCards.Add(c);
	            }
            }

            if (strawCards.Count == 0)
            {
                randomIndex = new Random().Next(0, possibleCards.Count);
                return possibleCards[randomIndex];
            }
            randomIndex = new Random().Next(0, strawCards.Count);
            return strawCards[randomIndex];
        }

        public int RuleBasedDecision()
        {
            List<int> possibleMoves = GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            List<int> highestPerSuit = getHighestPerSuit(hand);

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
                    //return possibleMoves[0];
                    return getRandomStrawCard(possibleMoves);
                }
            }
            else
            {
                int trickSize = tricks[tricks.Count - 1].GetCurrentTrickSize();
                if (trickSize == 4 || trickSize == 0)
                {
                    List<int> counterList = counterPerSuit(hand);
                    Random r = new Random();

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
                            if (counterList[i] > r.Next(3, 6) && Card.GetSuit(highestCard) == Trump)
                            {
                                return highestCard;
                            }
                            else if (counterList[i] > r.Next(1, 5) && Card.GetSuit(highestCard) != Trump)
                            {
                                break;
                            }
                            else
                            {
                                return highestCard;
                            }
                        }
                    }
                    //return possibleMoves[0];
                    return getRandomStrawCard(possibleMoves);
                }
                else
                {
                    //we may check if our mate has cut the trick and chose an highest card
                    possibleMoves.Sort(new DescendingComparer());
                    //return possibleMoves[0];
                    return getRandomStrawCard(possibleMoves);
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

            int winninCard = tricks[tricks.Count - 1].GetCurrentWinningCard();
            int highestRankOnTable = Card.GetRank(winninCard);

            if (highestRankOnTable >= 0 && highestCardRank > highestRankOnTable && highestCardRank > othersHighestRankFromSuit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal int GetDeckSize()
        {
            return unknownOwnerCards.GetDeckSize();
        }

        public string GetLastPlayInfo()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.LastPlayIsNewTrick())
            {
                return Sueca.PLAY_INFO_NEWTRICK;
            }
            else if (currentTrick.LastPlayIsFollowing())
            {
                return Sueca.PLAY_INFO_FOLLOWING;
            }
            else if (currentTrick.LastPlayIsCut())
            {
                return Sueca.PLAY_INFO_CUT;
            }
            else
            {
                return Sueca.PLAY_INFO_NOTFOLLOWING;
            }
        }
    }
}