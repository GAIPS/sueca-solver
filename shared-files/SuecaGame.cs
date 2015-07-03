using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SuecaGame
    {

        private Player[] players = new Player[4];
        private int trump;
        private GameState gameState;
        private int possiblePoints;
        private int points;

        public SuecaGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
        {
            trump = trumpSuit;
            players[0] = new MaxPlayer(0, playersHands[0], USE_CACHE);
            players[1] = new MinPlayer(1, playersHands[1], USE_CACHE);
            players[2] = new MaxPlayer(2, playersHands[2], USE_CACHE);
            players[3] = new MinPlayer(3, playersHands[3], USE_CACHE);
            points = 0;

            if (playersHands[0].Count == 10
                && playersHands[1].Count == 10
                && playersHands[2].Count == 10
                && playersHands[3].Count == 10)
            {
                possiblePoints = 120;
            }
            else
            {
                possiblePoints = CountPoints(playersHands[0])
                    + CountPoints(playersHands[1])
                    + CountPoints(playersHands[2])
                    + CountPoints(playersHands[3]);
            }
            gameState = new GameState(numTricks, trump, players, possiblePoints, botTeamInitialPoints, otherTeamInitialPoints);

            if (alreadyPlayed != null)
            {
                foreach (Move move in alreadyPlayed)
                {
                    gameState.ApplyMove(move);
                }
            }
        }

        public static int CountPoints(List<int> playerHand)
        {
            int result = 0;
            for (int j = 0; j < playerHand.Count; j++)
            {
                result += Card.GetValue(playerHand[j]);
            }
            return result;
        }

        public static int CountPointsFromSuit(List<int> playerHand, int suit)
        {
            int result = 0;
            for (int j = 0; j < playerHand.Count; j++)
            {
                if (Card.GetSuit(playerHand[j]) == suit)
                {
                    result += Card.GetValue(playerHand[j]);
                }
            }
            return result;
        }

        public static int CountCardsFromSuit(List<int> playerHand, int suit)
        {
            int result = 0;
            for (int j = 0; j < playerHand.Count; j++)
            {
                if (Card.GetSuit(playerHand[j]) == suit)
                {
                    result ++;
                }
            }
            return result;
        }


        public static int CountCardsFromRank(List<int> playerHand, int rank)
        {
            int result = 0;
            for (int j = 0; j < playerHand.Count; j++)
            {
                if (Card.GetRank(playerHand[j]) == rank)
                {
                    result++;
                }
            }
            return result;
        }

        public static bool HasTrumpAce(List<int> playerHand, int trump)
        {
            for (int j = 0; j < playerHand.Count; j++)
            {
                if (Card.GetRank(playerHand[j]) == (int) Rank.Ace && Card.GetSuit(playerHand[j]) == trump)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CountSuits(List<int> playerHand)
        {
            playerHand.Sort();
            int result = 0;
            int lastSuit = (int) Suit.None;
            for (int j = 0; j < playerHand.Count; j++)
            {
                int cardSuit = Card.GetSuit(playerHand[j]);
                if (cardSuit != lastSuit)
                {
                    lastSuit = cardSuit;
                    result++;
                }
            }
            return result;
        }

        public Trick GetCurrentTrick()
        {
            return gameState.GetCurrentTrick();
        }

        public int GetNextPlayerId()
        {
            return gameState.GetNextPlayer().Id;
        }

        public void PlayCard(int playerID, int card)
        {
            gameState.ApplyMove(new Move(playerID, card));
        }

        public int SampleGame(int depthLimit, int card = -1)
        {
            Player myPlayer = players[0];

            points = myPlayer.PlayGame(gameState, Int16.MinValue, Int16.MaxValue, depthLimit, card);
            return points;
        }

        public void PrintPlayersHands()
        {
            Console.WriteLine("---------ints---------");
            players[0].PrintHand();
            players[1].PrintHand();
            players[2].PrintHand();
            players[3].PrintHand();
            Console.WriteLine("-----------------------");
        }

        public void PrintTricks()
        {
            gameState.PrintTricks();
        }

        public static void PrintHand(List<int> hand)
        {
            Console.WriteLine("Your hand:");
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
            for (int i = 0; i < result.Count;)
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

        public void PrintNumCuts()
        {
            Console.WriteLine("--- PrintNumCuts ---");
            int average = 0;
            foreach (Player p in players)
            {
                average += p.NumCuts;
                Console.WriteLine(p.NumCuts);
            }
            average /= 4;
            Console.WriteLine("Average " + average);
        }

        public int[] GetGamePoints()
        {
//            int otherteamPoints = possiblePoints - points;
//            return new int[] { points, otherteamPoints };
            return gameState.CalculePointsOfFinishedGame();
        }


        public void PrintLastTrick()
        {
            gameState.PrintLastTrick();
        }


        public void PrintCurrentTrick()
        {
            gameState.PrintCurrentTrick();
        }

    }
}
















