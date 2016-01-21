using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuecaSolver
{
    public class FutureSuecaGame
    {
        private int trump;
        private List<Trick> tricks;
        private int firstPlayer;

        public FutureSuecaGame(int trumpSuit, int firstPlayerId)
        {
            trump = trumpSuit;
            tricks = new List<Trick>(10);
            tricks.Add(new Trick(trumpSuit));
            firstPlayer = firstPlayerId;
        }

        public void PlayCard(int playerId, int card)
        {
            if (tricks.Count - 1 < 0)
            {
                Console.WriteLine("SuecaGame.PlayerCard >> Negative index");
                System.Environment.Exit(1);
            }

            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                tricks.Add(new Trick(trump));
                currentTrick = tricks[tricks.Count - 1];
            }
            currentTrick.ApplyMove(new Move(playerId, card));
        }


        public int GetNextPlayerId()
        {
            if (tricks.Count - 1 < 0)
            {
                return firstPlayer;
            }
            
            return tricks[tricks.Count - 1].GetNextPlayerId();
        }

        public int[] GetGamePoints()
        {
            int firstTeamPoints = 0, secondTeamPoints = 0;
            foreach (Trick t in tricks)
            {
                int[] winnerAndPoints = t.GetTrickWinnerAndPoints();
                if (winnerAndPoints[0] == 0 || winnerAndPoints[0] == 2)
                {
                    firstTeamPoints += winnerAndPoints[1];
                }
                else
                {
                    secondTeamPoints += winnerAndPoints[1];
                }
            }

            return new int[] { firstTeamPoints, secondTeamPoints};
        }
    }
}
