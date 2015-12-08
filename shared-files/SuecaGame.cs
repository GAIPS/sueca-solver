// using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class SuecaGame
    {
        protected GameState gameState;

        public SuecaGame(int numTricks, List<List<int>> playersHands, int trumpSuit)
        {

        }

        // public Trick GetCurrentTrick()
        // {
        //     return gameState.GetCurrentTrick();
        // }

        public int GetNextPlayerId()
        {
            return gameState.GetNextPlayer().Id;
        }

        public void PlayCard(int playerID, int card)
        {
            gameState.ApplyMove(new Move(playerID, card));
        }

        public void PrintTricks()
        {
            gameState.PrintTricks();
        }
      
        public int[] GetGamePoints()
        {
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
















