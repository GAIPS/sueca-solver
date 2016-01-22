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

        public PerfectInformationGame(PlayerNode p0, PlayerNode p1, PlayerNode p2, PlayerNode p3, int numberTricks, int trumpSuit)
        {
            trump = trumpSuit;
            tricks = new List<Trick>(numberTricks);
            tricks.Add(new Trick(trumpSuit));
            players = new PlayerNode[4] { p0, p1, p2, p3, };
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
            if (tricks[tricks.Count - 1].IsFull())
            {
                tricks.Add(new Trick(trump));
            }
            tricks[tricks.Count - 1].ApplyMove(move);

            foreach (PlayerNode p in players)
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
            if (tricks[tricks.Count - 1].IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }
            tricks[tricks.Count - 1].UndoMove();
            foreach (PlayerNode p in players)
            {
                //TODO review if is this the correct method
                p.UndoMove(move);
            }
        }
    }
}
