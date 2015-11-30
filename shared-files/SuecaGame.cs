using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class SuecaGame
    {

        protected PlayerNode[] players;
        protected int trump;
        protected GameState gameState;
        protected int possiblePoints;
        protected int points;

        public SuecaGame(int numTricks, List<List<int>> playersHands, int trumpSuit, List<Move> alreadyPlayed, int botTeamInitialPoints, int otherTeamInitialPoints, bool USE_CACHE = false)
        {
            trump = trumpSuit;
            points = 0;

            if (playersHands[0].Count == 10
                && playersHands[1].Count == 10
                && playersHands[2].Count == 10
                && playersHands[3].Count == 10)
            {
                possiblePoints = 120;
            }
            else
            {
                possiblePoints = Sueca.CountPoints(playersHands[0])
                    + Sueca.CountPoints(playersHands[1])
                    + Sueca.CountPoints(playersHands[2])
                    + Sueca.CountPoints(playersHands[3]);
            }

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

        public void PlayCard(int playerID, int card)
        {
            gameState.ApplyMove(new Move(playerID, card));
        }

        public int SampleGame(int depthLimit, int card = -1)
        {
            PlayerNode myPlayer = players[0];

            points = myPlayer.PlayGame(gameState, Int16.MinValue, Int16.MaxValue, depthLimit, card);
            return points;
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

        public void PrintTricks()
        {
            gameState.PrintTricks();
        }

        

        public void PrintNumCuts()
        {
            Console.WriteLine("--- PrintNumCuts ---");
            int average = 0;
            foreach (PlayerNode p in players)
            {
                average += p.NumCuts;
                Console.WriteLine(p.NumCuts);
            }
            average /= 4;
            Console.WriteLine("Average " + average);
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
















