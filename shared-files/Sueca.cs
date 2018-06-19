﻿using System;
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
        public const int WAR_NUM_THREADS = 4;
        public const int MAX_MILISEC_DELIBERATION = 4000;
        public const string PLAY_INFO_NEWTRICK = "NEW_TRICK";
        public const string PLAY_INFO_FOLLOWING = "FOLLOWING";
        public const string PLAY_INFO_NOTFOLLOWING = "NOT_FOLLOWING";
        public const string PLAY_INFO_CUT = "CUT";

        public const int HASH_CODE = 0;

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
                return hand;
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
                return result;
            }
            return hand;
        }


        public static List<int> PossibleMovesReduced(List<int> hand, int leadSuit)
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
            if (i % 4 == 0)
            {
                if (Card.GetValue(move.Card) == 11 && Card.GetSuit(move.Card) == trump)
                {
                    return "1"; // LeadAceTrump
                }
                else if (Card.GetValue(move.Card) == 10 && Card.GetSuit(move.Card) == trump)
                {
                    return "2"; // LeadSevenTrump
                }
                else if (Card.GetSuit(move.Card) == trump)
                {
                    return "3"; // LeadOtherTrump
                }
                else if (Card.GetValue(move.Card) == 11)
                {
                    return "4"; // LeadAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "5"; // LeadSeven
                }
                else
                {
                    return "6"; // LeadOther
                }
            }
            else if (Card.GetSuit(move.Card) == leadSuit)
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "7"; // FollowAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "8"; // FollowSeven
                }
                else
                {
                    return "9"; // FollowOther
                }
            }
            else if (Card.GetSuit(move.Card) == trump)
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "10"; // CutAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "11"; // CutSeven
                }
                else
                {
                    return "12"; // CutOther
                }
            }
            else
            {
                if (Card.GetValue(move.Card) == 11)
                {
                    return "13"; // NoFollowAce
                }
                else if (Card.GetValue(move.Card) == 10)
                {
                    return "14"; // NoFollowSeven
                }
                else 
                {
                    return "15"; // NoFollowOther
                }
            }
            
        }

        public static List<int> GetPossibleLabels(List<int> possibleMoves, int leadSuit, int trump)
        {
            List<int> filteredClasses = new List<int>();

            if (leadSuit == (int) Suit.None)
            {
                foreach (int card in possibleMoves)
                {
                    if (Card.GetSuit(card) == trump && Card.GetRank(card) == (int) Rank.Ace)
                    {
                        filteredClasses.Add(1);
                    }
                    else if (Card.GetSuit(card) == trump && Card.GetRank(card) == (int) Rank.Seven)
                    {
                        filteredClasses.Add(2);
                    }
                    else if (Card.GetSuit(card) == trump && !filteredClasses.Contains(3))
                    {
                        filteredClasses.Add(3);
                    }
                    else if (Card.GetRank(card) == (int) Rank.Ace && !filteredClasses.Contains(4))
                    {
                        filteredClasses.Add(4);
                    }
                    else if (Card.GetRank(card) == (int) Rank.Seven && !filteredClasses.Contains(5))
                    {
                        filteredClasses.Add(5);
                    }
                    else if (!filteredClasses.Contains(6))
                    {
                        filteredClasses.Add(6);
                    }
                }
            }
            else if (leadSuit == Card.GetSuit(possibleMoves[0]))
            {
                foreach (int card in possibleMoves)
                {
                    if (Card.GetRank(card) == (int) Rank.Ace)
                    {
                        filteredClasses.Add(7);
                    }
                    else if (Card.GetRank(card) == (int) Rank.Seven)
                    {
                        filteredClasses.Add(8);
                    }
                    else if (!filteredClasses.Contains(9))
                    {
                        filteredClasses.Add(9);
                    }
                }
            }
            else
            {
                foreach (int card in possibleMoves)
                {
                    if (Card.GetSuit(card) == trump && Card.GetRank(card) == (int) Rank.Ace)
                    {
                        filteredClasses.Add(10);
                    }
                    else if (Card.GetSuit(card) == trump && Card.GetRank(card) == (int) Rank.Seven)
                    {
                        filteredClasses.Add(11);
                    }
                    else if (Card.GetSuit(card) == trump && !filteredClasses.Contains(12))
                    {
                        filteredClasses.Add(12);
                    }
                    else if (Card.GetRank(card) == (int) Rank.Ace && !filteredClasses.Contains(13))
                    {
                        filteredClasses.Add(13);
                    }
                    else if (Card.GetRank(card) == (int) Rank.Seven && !filteredClasses.Contains(14))
                    {
                        filteredClasses.Add(14);
                    }
                    else if (!filteredClasses.Contains(15))
                    {
                        filteredClasses.Add(15);
                    }
                }
            }
            return filteredClasses;
        }

        public static int ChooseCardFromLabel(int label, List<int> hand, int leadSuit, int trump)
        {
            int randomIndex;
            int suit = (int) Suit.None;
            if (label <= 3 || (label >= 10 && label <= 12))
            {
                suit = trump;
            }
            else if (label >= 7 && label <= 9)
            {
                suit = leadSuit;
            }



            if (label == 1 || label == 7 || label == 10)
            {
                List<int> aceOfSuit = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Ace && Card.GetSuit(x) == suit);
                if (aceOfSuit.Count == 1)
                {
                    return aceOfSuit[0];
                }
                return -1;
            }
            else if (label == 2 || label == 8 || label == 11)
            {
                List<int> sevenOfSuit = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Seven && Card.GetSuit(x) == suit);
                if (sevenOfSuit.Count == 1)
                {
                    return sevenOfSuit[0];
                }
                return -1;
            }
            else if (label == 3 || label == 9 || label == 12)
            {
                List<int> otherOfSuit = hand.FindAll(x => (Card.GetValue(x) == 0 || Card.GetValue(x) == 2 || Card.GetValue(x) == 3 || Card.GetValue(x) == 4) && Card.GetSuit(x) == suit);
                if (otherOfSuit.Count > 0)
                {
                    randomIndex = new Random().Next(0, otherOfSuit.Count);
                    return otherOfSuit[randomIndex];
                }
                return -1;
            }
            else if (label == 4 || label == 13)
            {
                List<int> aces = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Ace);
                if (aces.Count > 0)
                {
                    randomIndex = new Random().Next(0, aces.Count);
                    return aces[randomIndex];
                }
                return -1;
            }
            else if (label == 5 || label == 14)
            {
                List<int> sevens = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Seven);
                if (sevens.Count > 0)
                {
                    randomIndex = new Random().Next(0, sevens.Count);
                    return sevens[randomIndex];
                }
                return -1;
            }
            else if (label == 6 || label == 15)
            {
                List<int> others = hand.FindAll(x => Card.GetSuit(x) != trump && (Card.GetValue(x) == 0 || Card.GetValue(x) == 2 || Card.GetValue(x) == 3 || Card.GetValue(x) == 4));
                if (others.Count > 0)
                {
                    randomIndex = new Random().Next(0, others.Count);
                    return others[randomIndex];
                }
                return -1;
            }

            Console.WriteLine("UNKNOWN CLASSIFICATION (" + label + ")");
            randomIndex = new Random().Next(0, hand.Count);
            return hand[randomIndex];
        }



        public class ClassProbComparer : IComparer<KeyValuePair<int, float>>
        {
            public int Compare(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
            {
                if (b.Value > a.Value)
                {
                    return 1;
                }
                if (b.Value < a.Value)
                {
                    return -1;
                }
                return 0;
            }
        }

        public static List<KeyValuePair<int, float>> GetClassesProbabilities(List<int> possibleMoves, float[] features, int[] classes, float[][] weightsPerClass)
        {
            List<KeyValuePair<int, float>> classProbs = new List<KeyValuePair<int, float>>();

            for (int i = 0; i < weightsPerClass.Length; i++)
            {
                float total = 0;
                for (int j = 0; j < weightsPerClass[i].Length; j++)
                {
                    // coeficient of w0 is 1 and it is not in the features vec
                    if (j == weightsPerClass[i].Length - 1)
                    {
                        total += weightsPerClass[i][j];
                    }
                    else
                    {
                        total += features[j] * weightsPerClass[i][j];
                    }
                }
                classProbs.Add(new KeyValuePair<int, float>(classes[i], total));
            }

            classProbs.Sort(new ClassProbComparer());
            return classProbs;
        }


        public static bool IsCurrentTrickWinnerTeam(List<Move> game, int i, int trump, int playerID)
        {
            Trick currentTrick = new Trick(trump);
            for (int j = (i - (i % 4)); j < i; j++)
            {
                currentTrick.ApplyMove(game[j]);
            }
            int currentTrickWinner = currentTrick.GetCurrentTrickWinner();
            return currentTrickWinner == playerID || currentTrickWinner == ((playerID + 2) % 4);
        }

        public static int CountPointsInTrick(List<Move> game, int i)
        {
            int points = 0;
            if ((i % 4) > 0)
            {
                for (int j = i - (i % 4); j < i; j++)
                {
                    points += Card.GetValue(game[j].Card);
                }
            }
            return points;
        }

        public static float[] GetFeaturesFromState(int playerID, List<int> playersHand, List<Move> game, int i, int trump, ref Dictionary<int, List<int>> suitHasPlayer)
        {
            List<int> playedCards = new List<int>();

            for (int j = 0; i > 0 && j < i; j++)
            {
                playedCards.Add(game[j].Card);
            }
            int leadSuit;
            if ((i % 4) == 0)
            {
                leadSuit = (int)Suit.None;
            }
            else
            {
                int leadCard = game[i - (i % 4)].Card;
                leadSuit = Card.GetSuit(leadCard);
            }


            float[] features = new float[40];

            // hand-related features
            float leadSuitCardsHand = Sueca.CountCardsFromSuit(playersHand, leadSuit);
            features[0] = leadSuitCardsHand > 0 ? 1 : 0;
            features[1] = Sueca.HasCard(playersHand, (int)Rank.Ace, leadSuit) ? 1 : 0;
            features[2] = Sueca.HasCard(playersHand, (int)Rank.Seven, leadSuit) ? 1 : 0;
            features[3] = Sueca.HasCard(playersHand, (int)Rank.King, leadSuit) ? 1 : 0;
            features[4] = Sueca.HasCard(playersHand, (int)Rank.Jack, leadSuit) ? 1 : 0;
            features[5] = Sueca.HasCard(playersHand, (int)Rank.Queen, leadSuit) ? 1 : 0;
            float remainingCardsLeadSuit = Sueca.CountCardsFromSuit(playersHand, leadSuit) - features[1] - features[2] - features[3] - features[4] - features[5];
            features[6] = remainingCardsLeadSuit > 0 ? 1 : 0;
            features[7] = Sueca.CountCardsFromSuit(playersHand, trump);
            features[8] = Sueca.HasCard(playersHand, (int)Rank.Ace, trump) ? 1 : 0;
            features[9] = Sueca.HasCard(playersHand, (int)Rank.Seven, trump) ? 1 : 0;
            features[10] = Sueca.HasCard(playersHand, (int)Rank.King, trump) ? 1 : 0;
            features[11] = Sueca.HasCard(playersHand, (int)Rank.Jack, trump) ? 1 : 0;
            features[12] = Sueca.HasCard(playersHand, (int)Rank.Queen, trump) ? 1 : 0;
            features[13] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Ace);
            features[14] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Seven);
            features[15] = Sueca.CountCardsFromRank(playersHand, (int)Rank.King);
            features[16] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Jack);
            features[17] = Sueca.CountCardsFromRank(playersHand, (int)Rank.Queen);
            features[18] = (Sueca.CountCardsFromSuit(playersHand, trump) - features[8] - features[9] - features[10] - features[11] - features[12]);
            features[19] = playersHand.Count;
            
            //trick-related features
            features[20] = ((i % 4) + 1) / 4;
            features[21] = (i > 0 && Sueca.IsCurrentTrickWinnerTeam(game, i, trump, playerID)) ? 1 : 0;
            features[22] = (suitHasPlayer[leadSuit].Contains((playerID + 1) % 4) && suitHasPlayer[leadSuit].Contains((playerID + 3) % 4)) ? 1 : 0;
            features[23] = suitHasPlayer[leadSuit].Contains((playerID + 2) % 4) ? 1 : 0;
            features[24] = CountPointsInTrick(game, i);
            features[25] = leadSuit == trump ? 1 : 0;
            
            //game-related features
            float playedLeadSuitCards = Sueca.CountCardsFromSuit(playedCards, leadSuit);
            features[26] = playedLeadSuitCards;
            float unplayedLeadSuitCards = 10 - playedLeadSuitCards - leadSuitCardsHand;
            features[27] = unplayedLeadSuitCards;
            features[28] = Sueca.HasCard(playedCards, (int)Rank.Ace, leadSuit) ? 1 : 0;
            features[29] = Sueca.HasCard(playedCards, (int)Rank.Seven, leadSuit) ? 1 : 0;
            features[30] = Sueca.HasCard(playedCards, (int)Rank.King, leadSuit) ? 1 : 0;
            features[31] = Sueca.HasCard(playedCards, (int)Rank.Jack, leadSuit) ? 1 : 0;
            features[32] = Sueca.HasCard(playedCards, (int)Rank.Queen, leadSuit) ? 1 : 0;
            features[33] = Sueca.HasCard(playedCards, (int)Rank.Ace, trump) ? 1 : 0;
            features[34] = Sueca.HasCard(playedCards, (int)Rank.Seven, trump) ? 1 : 0;
            features[35] = Sueca.HasCard(playedCards, (int)Rank.King, trump) ? 1 : 0;
            features[36] = Sueca.HasCard(playedCards, (int)Rank.Jack, trump) ? 1 : 0;
            features[37] = Sueca.HasCard(playedCards, (int)Rank.Queen, trump) ? 1 : 0;
            features[38] = CountCardsFromSuit(playedCards, trump);
            //unplayedTrumps
            features[39] = 10 - features[38] - features[7];
            return features;
        }
    }
    
}
