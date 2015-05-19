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

        public SuecaGame(int numTricks, List<int> p0, List<int> p1, List<int> p2, List<int> p3, int trumpSuit, List<Move> alreadyPlayed)
        {
            trump = trumpSuit;
            players[0] = new MaxPlayer(0, p0);
            players[1] = new MinPlayer(1, p1);
            players[2] = new MaxPlayer(2, p2);
            players[3] = new MinPlayer(3, p3);
            points = 0;

            if (p0.Count == 10 && p1.Count == 10 && p2.Count == 10 && p3.Count == 10)
            {
                possiblePoints = 120;
            }
            else
            {
                possiblePoints = countPoints(p0, p1, p2, p3);
            }
            gameState = new GameState(numTricks, trump, players, possiblePoints);

            if (alreadyPlayed != null)
            {
                foreach (Move move in alreadyPlayed)
                {
                    gameState.ApplyMove(move);
                }
            }
        }

        private int countPoints(List<int> p0, List<int> p1, List<int> p2, List<int> p3)
        {
            int result = 0;
            for (int i = 0; i < p0.Count; i++)
            {
                result += Card.GetValue(p0[i]);
            }
            for (int i = 0; i < p1.Count; i++)
            {
                result += Card.GetValue(p1[i]);
            }
            for (int i = 0; i < p2.Count; i++)
            {
                result += Card.GetValue(p2[i]);
            }
            for (int i = 0; i < p3.Count; i++)
            {
                result += Card.GetValue(p3[i]);
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

            points = myPlayer.PlayGame(gameState, Int32.MinValue, Int32.MaxValue, depthLimit, card);

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
            int otherteamPoints = possiblePoints - points;
            return new int[] { points, otherteamPoints };
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
















