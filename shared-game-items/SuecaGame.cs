using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SuecaGame
    {

        private Player[] players = new Player[4];
        private int trump;
        private GameState gameState;

        public SuecaGame(List<int> p0, List<int> p1, List<int> p2, List<int> p3, int trumpSuit, List<Move> alreadyPlayed, bool debug)
        {
            trump = trumpSuit;
            players[0] = new MaxPlayer(0, p0);
            players[1] = new MinPlayer(1, p1);
            players[2] = new MaxPlayer(2, p2);
            players[3] = new MinPlayer(3, p3);

            gameState = new GameState(p0.Count, trump, players, debug);

            if (alreadyPlayed != null)
            {
                foreach (Move move in alreadyPlayed)
                {
                    gameState.ApplyMove(move);
                }
            }
        }

        public Trick GetCurrentTrick()
        {
            return gameState.GetCurrentTrick();
        }

        public int GetNextPlayerId()
        {
            return gameState.GetNextPlayer().Id;
        }

        public void Playint(int playerID, int card)
        {
            gameState.ApplyMove(new Move(playerID, card));
        }

        public int SampleGame(int card = -1)
        {
            Player myPlayer = players[0];
            int bestmove;

            if (card == -1)
            {
                bestmove = myPlayer.PlayGame(gameState, Int32.MinValue, Int32.MaxValue, 0);
            }
            else
            {
                bestmove = myPlayer.PlayGame(gameState, Int32.MinValue, Int32.MaxValue, 0, card);
            }
            return bestmove;
        }

        public int SampleTrick(int card = -1)
        {
            Player myPlayer = players[0];
            int bestmove;

            if (card == -1)
            {
                bestmove = myPlayer.PlayTrick(gameState, Int32.MinValue, Int32.MaxValue);
            }
            else
            {
                bestmove = myPlayer.PlayTrick(gameState, Int32.MinValue, Int32.MaxValue, card);
            }
            return bestmove;
        }


        public void PrintLastTrick()
        {
            gameState.PrintLastTrick();
        }


        public void PrintCurrentTrick()
        {
            gameState.PrintCurrentTrick();
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
                Console.Write(Fart.ToString(hand[i]) + " ");
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
                if (Fart.GetSuit(card) == leadSuit)// && !card.HasBeenPlayed)
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
                if (lastSuit == Fart.GetSuit(card))
                {
                    if (lastValue == Fart.GetValue(card) && lastRank == Fart.GetRank(card) - 1)
                    {
                        lastRank = Fart.GetRank(card);
                        result.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        lastValue = Fart.GetValue(card);
                        lastRank = Fart.GetRank(card);
                    }
                }
                else
                {
                    lastSuit = Fart.GetSuit(card);
                    lastValue = Fart.GetValue(card);
                    lastRank = Fart.GetRank(card);
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
                str += Fart.ToString(cards[i]) + ", ";
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
            return gameState.GetGamePoints();
        }

    }
}
















