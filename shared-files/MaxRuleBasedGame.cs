using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MaxRuleBasedGame : SuecaGame
    {

        public MaxRuleBasedGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
            : base(numTricks, playersHands, trumpSuit, alreadyPlayed, botTeamInitialPoints, otherTeamInitialPoints, USE_CACHE)
        {
            players = new PlayerNode[4];
            players[0] = new MaxNode(0, playersHands[0], USE_CACHE);
            players[1] = new RuleBasedNode(1, playersHands[1], trumpSuit);
            players[2] = new RuleBasedNode(2, playersHands[2], trumpSuit);
            players[3] = new RuleBasedNode(3, playersHands[3], trumpSuit);

            gameState = new GameState(numTricks, trump, players, possiblePoints, botTeamInitialPoints, otherTeamInitialPoints);
        }
    }
}
















