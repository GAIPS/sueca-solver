using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuecaSolver
{
    public class PerfectInformationGame
    {
        private int trump;
        private List<Trick> tricks;
        private PlayerNode[] players;
        private int firstTeamPoints; // p0 and p2
        private int secondTeamPoints; // p1 and p3

        public PerfectInformationGame(PlayerNode p0, PlayerNode p1, PlayerNode p2, PlayerNode p3, int numberTricks, int trumpSuit, int myTeamPoints, int otherTeamPoints)
        {
            trump = trumpSuit;
            tricks = new List<Trick>(numberTricks);
            tricks.Add(new Trick(trumpSuit));
            players = new PlayerNode[4] { p0, p1, p2, p3, };
            firstTeamPoints = myTeamPoints;
            secondTeamPoints = otherTeamPoints;
        }

        public int SampleGame(int depthLimit, int card)
        {
            PlayerNode myPlayer = players[0];
            int gameUtility = myPlayer.PlayGame(this, Int16.MinValue, Int16.MaxValue, depthLimit, card);
            return gameUtility;
        }

        internal bool reachedDepthLimit(int depthLimit)
        {
            //TODO check trick length > 0
            if (tricks.Count == depthLimit && tricks[tricks.Count - 1].IsFull())
            {
                return true;
            }
            return false;
        }

        internal bool IsEndGame()
        {
            if (tricks.Count == tricks.Capacity && tricks[tricks.Count - 1].IsFull())
            {
                return true;
            }
            return false;
        }

        internal bool IsOtherTeamWinning()
        {
            if (secondTeamPoints > 60)
            {
                return true;
            }
            return false;
        }

        internal int EvalGame()
        {
            if (firstTeamPoints > secondTeamPoints)
            {
                return firstTeamPoints;
            }
            else
            {
                return -1 * secondTeamPoints;
            }
        }

        internal int GetLeadSuit()
        {
            return tricks[tricks.Count - 1].LeadSuit;
        }

        internal PlayerNode GetNextPlayer()
        {
            int nextPlayerId = tricks[tricks.Count - 1].GetNextPlayerId();
            return players[nextPlayerId];
        }

        internal void ApplyMove(Move move)
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                tricks.Add(new Trick(trump));
                currentTrick = tricks[tricks.Count - 1];
            }
            currentTrick.ApplyMove(move);

            if (currentTrick.IsFull())
            {
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == 0 || winnerPoints[0] == 2)
                {
                    firstTeamPoints += winnerPoints[1];
                }
                else
                {
                    secondTeamPoints += winnerPoints[1];
                }
            }

            foreach (var p in players)
            {
                //TODO review if is this the correct method
                p.ApplyMove(move);
            }
        }

        internal void UndoMove(Move move)
        {
            if (tricks.Count - 1 < 0)
            {
                Console.WriteLine("PerfectInformationGame.UndoMove >> Negative index");
                System.Environment.Exit(1);
            }
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == 0 || winnerPoints[0] == 2)
                {
                    firstTeamPoints -= winnerPoints[1];
                }
                else
                {
                    secondTeamPoints -= winnerPoints[1];
                }
            }
            currentTrick.UndoMove();
            if (currentTrick.IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }
            foreach (PlayerNode p in players)
            {
                //TODO review if is this the correct method
                p.UndoMove(move);
            }
        }
    }
}
