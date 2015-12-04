using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinMaxGame : SuecaGame
    {

        private PlayerNode[] players;

        public MinMaxGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
            : base(numTricks, playersHands, trumpSuit)
        {
            players = new PlayerNode[4];
            players[0] = new MaxNode(0, playersHands[0], USE_CACHE);
            players[1] = new MinNode(1, playersHands[1], USE_CACHE);
            players[2] = new MaxNode(2, playersHands[2], USE_CACHE);
            players[3] = new MinNode(3, playersHands[3], USE_CACHE);

            gameState = new GameState(numTricks, trumpSuit, players, botTeamInitialPoints, otherTeamInitialPoints);

            if (alreadyPlayed != null)
            {
                foreach (Move move in alreadyPlayed)
                {
                    gameState.ApplyMove(move);
                }
            }
        }

        public int SampleGame(int depthLimit, int card)
        {
            PlayerNode myPlayer = players[0];

            int gameUtility = myPlayer.PlayGame(gameState, Int16.MinValue, Int16.MaxValue, depthLimit, card);
            return gameUtility;
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
    }
}
















