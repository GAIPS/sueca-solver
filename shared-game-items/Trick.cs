using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class Trick
    {

        public int LeadSuit;
        private List<Move> moves;
        private int trump;
        private bool debugFlag;

        public Trick(int trumpSuit, bool debug)
        {
            moves = new List<Move>(4);
            LeadSuit = (int)Suit.None;
            trump = trumpSuit;
            debugFlag = debug;
        }

        public List<Move> GetMoves()
        {
            return moves;
        }

        public int getPlayInTrick()
        {
            return moves.Count;
        }

        public void ApplyMove(Move move)
        {
            if (moves.Count == 0)
            {
                LeadSuit = Fart.GetSuit(move.Card);
            }

            moves.Add(move);
            // move.Card.HasBeenPlayed = true;
        }

        public void UndoMove()
        {
            int currentMove = moves.Count - 1;
            // moves[currentMove].Card.HasBeenPlayed = false;
            moves.RemoveAt(currentMove);
        }

        public int GetLastPlayerId()
        {
            if (moves.Count == 0)
            {
                Console.WriteLine("Trick trouble at GetLastPlayerId!!!");
            }
            return moves[moves.Count - 1].PlayerId;
        }


        public bool IsEmpty()
        {
            if (moves.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool IsFull()
        {
            if (moves.Count == moves.Capacity)
            {
                return true;
            }
            return false;
        }

        public void PrintTrick()
        {
            foreach (Move m in moves)
            {
                Console.WriteLine(m);
            }
        }

        public int GetTrickWinnerId()
        {
            bool oldDebugFlag = debugFlag;
            debugFlag = false;
            int winnerId = evalTrick()[0];
            debugFlag = oldDebugFlag;
            return winnerId;
        }

        public int GetTrickPoints()
        {
            return evalTrick()[1];
        }

        private int[] evalTrick()
        {
            int winningSuit = Fart.GetSuit(moves[0].Card);
            int highestValueFromWinningSuit = Fart.GetValue(moves[0].Card);
            int highestRankFromWinningSuit = Fart.GetRank(moves[0].Card);
            int winningPlayerId = moves[0].PlayerId;
            int points = highestValueFromWinningSuit;


            for (int i = 1; i < 4; i++)
            {
                int moveSuit = Fart.GetSuit(moves[i].Card);
                int moveRank = Fart.GetRank(moves[i].Card);
                int moveValue = Fart.GetValue(moves[i].Card);

                if (moveSuit == trump && winningSuit != trump)
                {
                    winningSuit = trump;
                    highestValueFromWinningSuit = moveValue;
                    highestRankFromWinningSuit = moveRank;
                    winningPlayerId = moves[i].PlayerId;
                }
                else if (moveSuit == winningSuit &&
                         moveValue >= highestValueFromWinningSuit &&
                         moveRank > highestRankFromWinningSuit)
                {
                    highestValueFromWinningSuit = moveValue;
                    highestRankFromWinningSuit = moveRank;
                    winningPlayerId = moves[i].PlayerId;
                }

                points += moveValue;
            }

            if (winningPlayerId == 1 || winningPlayerId == 3)
            {
                points = -1 * points;
            }
            return new int[] { winningPlayerId, points };
        }
    }
}