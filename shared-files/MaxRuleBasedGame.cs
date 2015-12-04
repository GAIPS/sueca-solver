using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MaxRuleBasedGame : SuecaGame
    {

        private PlayerNode[] players;

        public MaxRuleBasedGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
            : base(numTricks, playersHands, trumpSuit)
        {
            players = new PlayerNode[4];
            players[0] = new MaxNode(0, playersHands[0], USE_CACHE);
            players[1] = new RuleBasedNode(1, playersHands[1], trumpSuit);
            players[2] = new RuleBasedNode(2, playersHands[2], trumpSuit);
            players[3] = new RuleBasedNode(3, playersHands[3], trumpSuit);

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
    }
}
















