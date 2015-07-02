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

        override public int PlayGame(GameState gameState, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int32.MinValue;
            List<int> moves;

            if (gameState.reachedDepthLimit(depthLimit))
            {
                return gameState.EvalGame();// + gameState.Heuristic(depthLimit);
            }

            if (gameState.IsOtherTeamWinning() || gameState.IsEndGame())
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

            if (Hand.Count <= 8 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            {
                string state = gameState.GetState();
                lock (GameState.MaxPlayerLock)
                {
                    GameState.ACCESSES_MAX++;
                    if (GameState.ComputedSubtreesMaxPlayer.ContainsKey(state))
                    {
                        GameState.SAVED_ACCESSES_MAX++;
                        return gameState.EvalGame() + GameState.ComputedSubtreesMaxPlayer[state];
                    }
                }
            }


            foreach (int move in moves)
            {
                gameState.ApplyMove(new Move(Id, move));
                Hand.Remove(move);
                HasSuit[Card.GetSuit(move)]--;
                int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, depthLimit);

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
                    if (Hand.Count <= 8 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
                    {
                        string state = gameState.GetState();
                        int pointsUntilCurrentState = gameState.EvalGame();
                        lock (GameState.MaxPlayerLock)
                        {
                            if (!GameState.ComputedSubtreesMaxPlayer.ContainsKey(state))
                            {
                                GameState.ComputedSubtreesMaxPlayer.Add(state, v - pointsUntilCurrentState);
                            }
                        }
                    }
                    return v;
                }

                if (v > alpha)
                {
                    alpha = v;
                }
            }

            if (Hand.Count <= 8 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            {
                string state = gameState.GetState();
                int pointsUntilCurrentState = gameState.EvalGame();
                lock (GameState.MaxPlayerLock)
                {
                    if (!GameState.ComputedSubtreesMaxPlayer.ContainsKey(state))
                    {
                        GameState.ComputedSubtreesMaxPlayer.Add(state, v - pointsUntilCurrentState);
                    }
                }
            }

            return v;
        }
    }
}