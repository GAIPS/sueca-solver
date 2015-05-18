using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MaxPlayer : Player
    {

        public MaxPlayer(int id, List<int> hand)
            : base(id, hand)
        {
        }

        override public int PlayGame(GameState gameState, int alpha, int beta, int lol, int card = -1)
        {
            int v = Int32.MinValue;
            List<int> moves;

            if (gameState.IsEndGame())
            {
                return gameState.EvalGame();
            }

            if (card == -1)
            {
                moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
                gameState.orderPossibleMoves(moves, Id);
            }
            else
            {
                moves = new List<int>();
                moves.Add(card);
            }


            // lol++;
            // if (lol == 10)
            // {
            //   System.Environment.Exit(1);
            // }


            foreach (int move in moves)
            {
                gameState.ApplyMove(new Move(Id, move));
                Hand.Remove(move);
                HasSuit[Card.GetSuit(move)]--;
                // Console.WriteLine("Max player played " + move);
                int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, lol);

                if (moveValue > v)
                {
                    v = moveValue;
                }

                gameState.UndoMove();
                Hand.Add(move);
                HasSuit[Card.GetSuit(move)]++;

                if (v >= beta)
                {
                    NumCuts++;
                    return v;
                }

                if (v > alpha)
                {
                    alpha = v;
                }
            }

            return v;
        }
    }
}