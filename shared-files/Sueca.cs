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
        public const int WAR_NUM_THREADS = 4;
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

            if (Card.GetValue(move.Card) == 11 && Card.GetSuit(move.Card) == trump)
            {
                return "1"; // LeadAceTrump
            }
            else if (Card.GetValue(move.Card) == 10 && Card.GetSuit(move.Card) == trump)
            {
                return "2"; // LeadSevenTrump
            }
            else if (Card.GetValue(move.Card) == 4 && Card.GetSuit(move.Card) == trump)
            {
                return "3"; // LeadKingTrump
            }
            else if (Card.GetValue(move.Card) == 3 && Card.GetSuit(move.Card) == trump)
            {
                return "4"; // LeadJackTrump
            }
            else if (Card.GetValue(move.Card) == 2 && Card.GetSuit(move.Card) == trump)
            {
                return "5"; // LeadQueenTrump
            }
            else if(Card.GetSuit(move.Card) == trump)
            {
                return "6"; // LeadOtherTrump
            }
            else if (Card.GetValue(move.Card) == 11)
            {
                return "7"; // LeadAce
            }
            else if (Card.GetValue(move.Card) == 10)
            {
                return "8"; // LeadSeven
            }
            else if (Card.GetValue(move.Card) == 4)
            {
                return "9"; // LeadKing
            }
            else if (Card.GetValue(move.Card) == 3)
            {
                return "10"; // LeadJack
            }
            else if (Card.GetValue(move.Card) == 2)
            {
                return "11"; // LeadQueen
            }
            else 
            {
                return "12"; // LeadOther
            }
        }

        public static int ChooseCardFromLabel(int label, List<int> hand, int leadSuit, int trump)
        {
            int randomIndex;

            if (label == 1)
            {
                List<int> aceTrump = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Ace && Card.GetSuit(x) == trump);
                if (aceTrump.Count == 1)
                {
                    return aceTrump[0];
                }
                return -1;
            }
            else if (label == 2)
            {
                List<int> sevenTrump = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Seven && Card.GetSuit(x) == trump);
                if (sevenTrump.Count == 1)
                {
                    return sevenTrump[0];
                }
                return -1;
            }
            else if (label == 3)
            {
                List<int> kingTrump = hand.FindAll(x => Card.GetRank(x) == (int)Rank.King && Card.GetSuit(x) == trump);
                if (kingTrump.Count == 1)
                {
                    return kingTrump[0];
                }
                return -1;
            }
            else if (label == 4)
            {
                List<int> jackTrump = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Jack && Card.GetSuit(x) == trump);
                if (jackTrump.Count == 1)
                {
                    return jackTrump[0];
                }
                return -1;
            }
            else if (label == 5)
            {
                List<int> queenTrump = hand.FindAll(x => Card.GetRank(x) == (int)Rank.Queen && Card.GetSuit(x) == trump);
                if (queenTrump.Count == 1)
                {
                    return queenTrump[0];
                }
                return -1;
            }
            else if (label == 6)
            {
                List<int> otherTrump = hand.FindAll(x => Card.GetSuit(x) == trump);
                if (otherTrump.Count > 0)
                {
                    randomIndex = new Random().Next(0, otherTrump.Count);
                    return otherTrump[randomIndex];
                }
                return -1;
            }
            else if (label == 7)
            {
                List<int> aces = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Ace);
                if (aces.Count > 0)
                {
                    randomIndex = new Random().Next(0, aces.Count);
                    return aces[randomIndex];
                }
                return -1;
            }
            else if (label == 8)
            {
                List<int> sevens = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Seven);
                if (sevens.Count > 0)
                {
                    randomIndex = new Random().Next(0, sevens.Count);
                    return sevens[randomIndex];
                }
                return -1;
            }
            else if (label == 9)
            {
                List<int> kings = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.King);
                if (kings.Count > 0)
                {
                    randomIndex = new Random().Next(0, kings.Count);
                    return kings[randomIndex];
                }
                return -1;
            }
            else if (label == 10)
            {
                List<int> jacks = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Jack);
                if (jacks.Count > 0)
                {
                    randomIndex = new Random().Next(0, jacks.Count);
                    return jacks[randomIndex];
                }
                return -1;
            }
            else if (label == 11)
            {
                List<int> queens = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetRank(x) == (int)Rank.Queen);
                if (queens.Count > 0)
                {
                    randomIndex = new Random().Next(0, queens.Count);
                    return queens[randomIndex];
                }
                return -1;
            }
            else if (label == 12)
            {
                List<int> others = hand.FindAll(x => Card.GetSuit(x) != trump && Card.GetValue(x) == 0);
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

        public static List<KeyValuePair<int, float>> GetClassesProbabilities(List<int> possibleMoves, int[] features, int[] classes, int[] filteredClasses, float[][] weightsPerClass)
        {
            List<int> possibleClasses = new List<int>(filteredClasses);
            List<KeyValuePair<int, float>> classProbs = new List<KeyValuePair<int, float>>();

            for (int i = 0; i < weightsPerClass.Length; i++)
            {
                if (classes.Length == filteredClasses.Length || possibleClasses.Contains(classes[i]))
                {
                    float total = 0;
                    for (int j = 0; j < weightsPerClass[i].Length; j++)
                    {
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

        public static int[] GetFeaturesFromState(int playerID, List<int> playersHand, List<Move> game, int i, int trump)
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
            features[9] = Sueca.HasCard(playedCards, (int)Rank.Ace, leadSuit) ? 1 : 0;
            features[10] = Sueca.HasCard(playedCards, (int)Rank.Seven, leadSuit) ? 1 : 0;
            features[11] = Sueca.HasCard(playedCards, (int)Rank.King, leadSuit) ? 1 : 0;
            features[12] = Sueca.HasCard(playedCards, (int)Rank.Ace, trump) ? 1 : 0;
            features[13] = Sueca.HasCard(playedCards, (int)Rank.Seven, trump) ? 1 : 0;
            features[14] = Sueca.HasCard(playedCards, (int)Rank.King, trump) ? 1 : 0;
            features[15] = (i > 0 && Sueca.IsCurrentTrickWinnerTeam(game, i, trump, playerID)) ? 1 : 0;
            //features[16] = Sueca.IsPartnerCuttingLeadsuit(game, i, playerID) ? 1 : 0;

            return features;
        }
    }
    
}
