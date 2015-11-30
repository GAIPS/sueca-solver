using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinNode : PlayerNode
    {

        public MinNode(int id, List<int> hand, bool USE_CACHE)
            : base(id, hand, USE_CACHE)
        {
        }

        override public int PlayGame(GameState gameState, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int32.MaxValue;
            List<int> moves;

            if (gameState.reachedDepthLimit(depthLimit) || gameState.IsOtherTeamWinning() || gameState.IsEndGame())
            {
                return gameState.EvalGame();
            }

            if (card == -1)
            {
                moves = Sueca.PossibleMoves(Hand, gameState.GetLeadSuit());
                gameState.orderPossibleMoves(moves, Id);
            }
            else
            {
                moves = new List<int>();
                moves.Add(card);
            }

            //if (USE_CACHE && Hand.Count <= gameState.NUM_TRICKS - 2 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            if (USE_CACHE && Hand.Count == 5 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            {
                string[] equivalentStates = gameState.GetEquivalentStates(gameState.GetState2(Id));
                lock (GameState.MinLock) { GameState.ACCESSES++; }
                foreach (var state in equivalentStates)
                {
                    if (GameState.ComputedSubtreesMinPlayer.ContainsKey(state))
                    {
                        lock (GameState.MinLock) { GameState.SAVED_ACCESSES++; }
                        return gameState.EvalGame() + GameState.ComputedSubtreesMinPlayer[state];
                    }
                }
            }


            foreach (int move in moves)
            {
                gameState.ApplyMove(new Move(Id, move));
                Hand.Remove(move);
                HasSuit[Card.GetSuit(move)]--;
                int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, depthLimit);

                if (moveValue < v)
                {
                    v = moveValue;
                }

                gameState.UndoMove();
                Hand.Add(move);
                HasSuit[Card.GetSuit(move)]++;

                if (v <= alpha)
                {
                    NumCuts++;
                    //if (USE_CACHE && Hand.Count <= gameState.NUM_TRICKS - 2 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
                    if (USE_CACHE && Hand.Count == 5 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
                    {
                        string state = gameState.GetState2(Id);
                        int pointsUntilCurrentState = gameState.EvalGame();
                        int pointsOfSubtree = v - pointsUntilCurrentState;
                        lock (GameState.MinLock)
                        {
                            GameState.ComputedSubtreesMinPlayer.TryAdd(state, pointsOfSubtree);
                        }
                    }
                    return v;
                }

                if (moveValue < beta)
                {
                    beta = moveValue;
                }
            }

            //if (USE_CACHE && Hand.Count <= gameState.NUM_TRICKS - 2 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            if (USE_CACHE && Hand.Count == 5 && (gameState.GetCurrentTrick() == null || gameState.GetCurrentTrick().IsFull()))
            {
                string state = gameState.GetState2(Id);
                int pointsUntilCurrentState = gameState.EvalGame();
                int pointsOfSubtree = v - pointsUntilCurrentState;
                lock (GameState.MinLock)
                {
                    GameState.ComputedSubtreesMinPlayer.TryAdd(state, pointsOfSubtree);
                }
            }
            return v;
        }
    }
}