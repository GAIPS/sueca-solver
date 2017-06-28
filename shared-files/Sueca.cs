using System;
using System.Collections.Generic;

namespace SuecaSolver
{

    public enum Rank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Queen,
        Jack,
        King,
        Seven,
        Ace,
        None
    };

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades,
        None
    };

    public static class Sueca
    {
        public const int UTILITY_FUNC = 1;
        public const int HYBRID_NUM_THREADS = 4;
        public const int WAR_NUM_THREADS = 2;
        public const int MAX_MILISEC_DELIBERATION = 4000;
        public const string PLAY_INFO_NEWTRICK = "NEW_TRICK";
        public const string PLAY_INFO_FOLLOWING = "FOLLOWING";
        public const string PLAY_INFO_NOTFOLLOWING = "NOT_FOLLOWING";
        public const string PLAY_INFO_CUT = "CUT";

        public static int CountPoints(List<int> cardsList)
        {
            int result = 0;
            for (int j = 0; j < cardsList.Count; j++)
            {
                result += Card.GetValue(cardsList[j]);
            }
            return result;
        }

        public static int CountPointsFromSuit(List<int> cardsList, int suit)
        {
            int result = 0;
            for (int j = 0; j < cardsList.Count; j++)
            {
                if (Card.GetSuit(cardsList[j]) == suit)
                {
                    result += Card.GetValue(cardsList[j]);
                }
            }
            return result;
        }

        public static int CountCardsFromSuit(List<int> cardsList, int suit)
        {
            int result = 0;
            for (int j = 0; j < cardsList.Count; j++)
            {
                if (Card.GetSuit(cardsList[j]) == suit)
                {
                    result++;
                }
            }
            return result;
        }


        public static int CountCardsFromRank(List<int> cardsList, int rank)
        {
            int result = 0;
            for (int j = 0; j < cardsList.Count; j++)
            {
                if (Card.GetRank(cardsList[j]) == rank)
                {
                    result++;
                }
            }
            return result;
        }

        public static bool HasCard(List<int> cardsList, int rank, int suit)
        {
            for (int j = 0; j < cardsList.Count; j++)
            {
                if (Card.GetRank(cardsList[j]) == rank && Card.GetSuit(cardsList[j]) == suit)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasTrumpAce(List<int> cardsList, int trump)
        {
            for (int j = 0; j < cardsList.Count; j++)
            {
                if (Card.GetRank(cardsList[j]) == (int)Rank.Ace && Card.GetSuit(cardsList[j]) == trump)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CountSuits(List<int> cardsList)
        {
            cardsList.Sort();
            int result = 0;
            int lastSuit = (int)Suit.None;
            for (int j = 0; j < cardsList.Count; j++)
            {
                int cardSuit = Card.GetSuit(cardsList[j]);
                if (cardSuit != lastSuit)
                {
                    lastSuit = cardSuit;
                    result++;
                }
            }
            return result;
        }

        public static void PrintHand(List<int> hand)
        {
            string str = "";
            foreach (int c in hand)
            {
                str += c.ToString() + ", ";
            }
            Console.WriteLine(str);
        }

        public static void PrintCurrentHand(List<int> hand)
        {
            Console.WriteLine("Your hand:");
            AscendingSuitComparer acc = new AscendingSuitComparer();
            hand.Sort(acc);

            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write("-- ");
            }
            Console.WriteLine("");
            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write(Card.ToString(hand[i]) + " ");
            }
            Console.WriteLine("");
            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write("-- ");
            }
            Console.WriteLine("");
            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write(" " + i + " ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        public static void PrintHandsReport(List<List<int>> playersHands, int trumpSuit)
        {
            for (int i = 0; i < playersHands.Count; i++)
            {
                int numOfTrumps = 0;
                int points = 0;
                int trumpPoints = 0;
                List<int> playerHand = playersHands[i];

                for (int j = 0; j < playerHand.Count; j++)
                {
                    int cardValue = Card.GetValue(playerHand[j]);
                    points += cardValue;
                    if (Card.GetSuit(playerHand[j]) == trumpSuit)
                    {
                        numOfTrumps++;
                        trumpPoints += cardValue;
                    }
                }

                Console.WriteLine("Player " + i + " - " + points + "P, " + trumpPoints + "TP, " + numOfTrumps + "#T");
            }
        }


        public static List<int> PossibleMoves(List<int> hand, int leadSuit)
        {
            List<int> result = new List<int>();
            if (leadSuit == (int)Suit.None)
            {
                return removeEquivalentMoves(new List<int>(hand));
            }

            foreach (int card in hand)
            {
                if (Card.GetSuit(card) == leadSuit)
                {
                    result.Add(card);
                }
            }

            if (result.Count > 0)
            {
                return removeEquivalentMoves(result);
            }

            return removeEquivalentMoves(new List<int>(hand));
        }

        private static List<int> removeEquivalentMoves(List<int> cards)
        {
            List<int> result = cards;
            int lastSuit = (int)Suit.None;
            int lastRank = (int)Rank.None;
            int lastValue = -1;

            result.Sort();
            for (int i = 0; i < result.Count; )
            {
                int card = result[i];
                if (lastSuit == Card.GetSuit(card))
                {
                    if (lastValue == Card.GetValue(card) && lastRank == Card.GetRank(card) - 1)
                    {
                        lastRank = Card.GetRank(card);
                        result.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        lastValue = Card.GetValue(card);
                        lastRank = Card.GetRank(card);
                    }
                }
                else
                {
                    lastSuit = Card.GetSuit(card);
                    lastValue = Card.GetValue(card);
                    lastRank = Card.GetRank(card);
                }
                i++;
            }
            return result;
        }



        public static void PrintCards(string name, List<int> cards)
        {
            string str = name + " - ";
            for (int i = 0; i < cards.Count; i++)
            {
                str += Card.ToString(cards[i]) + ", ";
            }
            Console.WriteLine(str);
        }

        public static string GetPlayLabel(Move move, int i, List<Move> game, int trump)
        {
            int leadSuit = Card.GetSuit(game[i - (i % 4)].Card);

            if ((i % 4) == 0) // Lead Play (aka new trick)
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "1"; // LeadAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "2"; // LeadSeven
                }
                //else if (Card.GetValue(move.Card) >= 2)
                //{
                //    return "3"; // LeadFig
                //}
                else
                {
                    return "4"; // LeadZero
                }
            }
            else if (Card.GetSuit(move.Card) == leadSuit) // Folow
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "5"; // FollowAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "6"; // FollowSeven
                }
                //else if (Card.GetValue(move.Card) >= 2)
                //{
                //    return "7"; // FollowFig
                //}
                else
                {
                    return "8"; // FollowZero
                }
            }
            else if (Card.GetSuit(move.Card) == trump) // Cut
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "9"; // CutAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "10"; // CutSeven
                }
                //else if (Card.GetValue(move.Card) >= 2)
                //{
                //    return "11"; // CutFig
                //}
                else
                {
                    return "12"; // CutZero
                }
            }
            else // No Follow
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "13"; // NoFollowAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "14"; // NoFollowSeven
                }
                //else if (Card.GetValue(move.Card) >= 2)
                //{
                //    return "15"; // NoFollowFig
                //}
                else
                {
                    return "16"; // NoFollowZero
                }
            }
        }

        public static int ChooseCardFromLabel(int label, List<int> hand, int leadSuit, int trump)
        {
            int randomIndex;
            if (label == 1 || label == 13) // LeadAce or NoFollowAce
            {
                if (Sueca.CountCardsFromRank(hand, (int)Rank.Ace) > 0)
                {
                    //TODO SHUFFLE IT
                    return hand.Find(x => Card.GetRank(x) == (int)Rank.Ace);
                }
                else
                {
                    Console.WriteLine("Classification gave 1 and there is no Ace (" + label + ")");
                    randomIndex = new Random().Next(0, hand.Count);
                    return hand[randomIndex];
                }
            }
            else if (label == 2 || label == 14) // LeadSeven or NoFollowSeven
            {
                if (Sueca.CountCardsFromRank(hand, (int)Rank.Seven) > 0)
                {
                    return hand.Find(x => Card.GetRank(x) == (int)Rank.Seven);
                }
                else
                {
                    Console.WriteLine("Classification gave 2 and there is no Seven (" + label + ")");
                    randomIndex = new Random().Next(0, hand.Count);
                    return hand[randomIndex];
                }
            }
            else if (label == 4 || label == 16) // LeadZero or NoFolowZero
            {
                int chosen = hand.Find(x => Card.GetValue(x) == 0);
                if (!hand.Contains(chosen))
                {
                    chosen = hand.Find(x => Card.GetValue(x) == 2);
                    if (!hand.Contains(chosen))
                    {
                        chosen = hand.Find(x => Card.GetValue(x) == 3);
                        if (!hand.Contains(chosen))
                        {
                            chosen = hand.Find(x => Card.GetValue(x) == 4);
                            if (!hand.Contains(chosen))
                            {
                                Console.WriteLine("Classification gave 4 and there is no Zero (" + label + ")");
                                randomIndex = new Random().Next(0, hand.Count);
                                return hand[randomIndex];
                            }
                        }
                    }
                }
                return chosen;
            }
            else if (label > 4 && label < 13) // Follow or Cut
            {
                List<int> possibleMoves;
                int suitToPlay;
                if (label < 9)
                {
                    possibleMoves = Sueca.PossibleMoves(hand, leadSuit);
                    suitToPlay = leadSuit;
                }
                else
                {
                    possibleMoves = Sueca.PossibleMoves(hand, trump);
                    suitToPlay = trump;
                }
                
                if (possibleMoves.Count == 0)
                {
                    Console.WriteLine("Classification gave FollowPlay and there is no lead suit in possibleMoves (" + label + ")");
                    randomIndex = new Random().Next(0, hand.Count);
                    return hand[randomIndex];
                }

                if (label == 5 || label == 9) // FollowAce or CutAce
                {
                    if (Sueca.CountCardsFromRank(possibleMoves, (int)Rank.Ace) == 1)
                    {
                        return Card.Create(Rank.Ace, (Suit) suitToPlay);
                    }
                    else
                    {
                        Console.WriteLine("Classification gave 5 and there is no Ace (" + label + ")");
                        randomIndex = new Random().Next(0, hand.Count);
                        return hand[randomIndex];
                    }
                }
                else if (label == 6 || label == 10) // FollowSeven or CutSeven
                {
                    if (Sueca.CountCardsFromRank(possibleMoves, (int)Rank.Seven) == 1)
                    {
                        return Card.Create(Rank.Seven, (Suit)suitToPlay);
                    }
                    else
                    {
                        Console.WriteLine("Classification gave 6 and there is no Seven (" + label + ")");
                        randomIndex = new Random().Next(0, hand.Count);
                        return hand[randomIndex];
                    }
                }
                else if (label == 8 || label == 12) // Follow Zero
                {
                    int chosen = possibleMoves.Find(x => Card.GetValue(x) == 0);
                    if (!possibleMoves.Contains(chosen))
                    {
                        chosen = possibleMoves.Find(x => Card.GetValue(x) == 2);
                        if (!possibleMoves.Contains(chosen))
                        {
                            chosen = possibleMoves.Find(x => Card.GetValue(x) == 3);
                            if (!possibleMoves.Contains(chosen))
                            {
                                chosen = possibleMoves.Find(x => Card.GetValue(x) == 4);
                                if (!possibleMoves.Contains(chosen))
                                {
                                    Console.WriteLine("Classification gave 8 and there is no Zero (" + label + ")");
                                    randomIndex = new Random().Next(0, hand.Count);
                                    return hand[randomIndex];
                                }
                            }
                        }
                    }
                    return chosen;
                }
            }
           
            Console.WriteLine("UNKNOWN CLASSIFICATION (" + label + ")");
            randomIndex = new Random().Next(0, hand.Count);
            return hand[randomIndex];
        }

        public static bool IsCurrentTrickWinnerTeam(List<Move> game, int i, int trump, int playerID)
        {
            Trick currentTrick = new Trick(trump);
            for (int j = (i - (i % 4)); j < i; j++)
            {
                currentTrick.ApplyMove(game[j]);
            }
            return currentTrick.GetCurrentTrickWinner() == playerID;
        }

        public static int[] GetFeaturesFromIthPlay(int playerID, List<int> playersHand, List<Move> game, int i, int trump)
        {
            List<int> playedCards = new List<int>();
            for (int j = 0; j < i; j++)
            {
                playedCards.Add(game[j].Card);
            }
            int leadCard = game[i - (i % 4)].Card;
            int leadSuit = Card.GetSuit(leadCard);

            int[] features = new int[16];
            features[0] = ((i % 4) + 1);
            features[1] = Sueca.CountCardsFromSuit(playersHand, trump);
            features[2] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Ace);
            features[3] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Seven);
            int countFigs = Sueca.CountCardsFromRank(playersHand, (int)Rank.King) + Sueca.CountCardsFromRank(playersHand, (int)Rank.Jack) + Sueca.CountCardsFromRank(playersHand, (int)Rank.Queen);
            features[4] = countFigs;
            features[5] = playersHand.Count;
            int leadSuitCardsHand = Sueca.CountCardsFromSuit(playersHand, leadSuit);
            features[6] = leadSuitCardsHand;
            int playedLeadSuitCards = Sueca.CountCardsFromSuit(playedCards, leadSuit);
            features[7] = playedLeadSuitCards;
            int unplayedLeadSuitCards = 10 - playedLeadSuitCards - leadSuitCardsHand;
            features[8] = unplayedLeadSuitCards;
            if (Sueca.HasCard(playedCards, (int)Rank.Ace, leadSuit))
            {
                features[9] = 1;
            }
            else
            {
                features[9] = 0;
            }
            if (Sueca.HasCard(playedCards, (int)Rank.Seven, leadSuit))
            {
                features[10] = 1;
            }
            else
            {
                features[10] = 0;
            }
            if (Sueca.HasCard(playedCards, (int)Rank.King, leadSuit))
            {
                features[11] = 1;
            }
            else
            {
                features[11] = 0;
            }
            if (Sueca.HasCard(playedCards, (int)Rank.Ace, trump))
            {
                features[12] = 1;
            }
            else
            {
                features[12] = 0;
            }
            if (Sueca.HasCard(playedCards, (int)Rank.Seven, trump))
            {
                features[13] = 1;
            }
            else
            {
                features[13] = 0;
            }
            if (Sueca.HasCard(playedCards, (int)Rank.King, trump))
            {
                features[14] = 1;
            }
            else
            {
                features[14] = 0;
            }
            if (i == 0 || Sueca.IsCurrentTrickWinnerTeam(game, i, trump, playerID))
            {
                features[15] = 1;
            }
            else
            {
                features[15] = 0;
            }

            return features;
        }
    }
    
}
