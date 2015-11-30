using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinMaxGame : SuecaGame
    {

        public MinMaxGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
            : base(numTricks, playersHands, trumpSuit, alreadyPlayed, botTeamInitialPoints, otherTeamInitialPoints, USE_CACHE)
        {
            players = new PlayerNode[4];
            players[0] = new MaxNode(0, playersHands[0], USE_CACHE);
            players[1] = new MinNode(1, playersHands[1], USE_CACHE);
            players[2] = new MaxNode(2, playersHands[2], USE_CACHE);
            players[3] = new MinNode(3, playersHands[3], USE_CACHE);

            gameState = new GameState(numTricks, trump, players, possiblePoints, botTeamInitialPoints, otherTeamInitialPoints);
        }
    }
}
















