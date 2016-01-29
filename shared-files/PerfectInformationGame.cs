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
        private int firstTeamPoints; 
        private int secondTeamPoints; 
        private int predictableTrickWinner;
        private bool predictableTrickCut;
        private bool hybridFlag;

        public PerfectInformationGame(PlayerNode p0, PlayerNode p1, PlayerNode p2, PlayerNode p3, int numberTricks, int trumpSuit, List<Move> currentTrickMoves, int myTeamPoints, int otherTeamPoints, bool HYBRID_FLAG = false)
        {
            players = new PlayerNode[4] { p0, p1, p2, p3, };
            trump = trumpSuit;
            tricks = new List<Trick>(numberTricks);
            Trick currentTrick = new Trick(trumpSuit);
            tricks.Add(currentTrick);
            foreach (Move m in currentTrickMoves)
            {
                currentTrick.ApplyMove(m);
            }
            firstTeamPoints = myTeamPoints;
            secondTeamPoints = otherTeamPoints;
            predictableTrickWinner = -1;
            predictableTrickCut = false;
            hybridFlag = HYBRID_FLAG;
        }

        public int SampleGame(int depthLimit, int card)
        {
            PlayerNode myPlayer = players[0];
            int nextPlayerId = tricks[tricks.Count - 1].GetNextPlayerId();
            if (nextPlayerId != -1 && myPlayer.Id != nextPlayerId)
            {
                Console.WriteLine("PIG::SampleGame >> Problem with player ids");
            }
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
            if (hybridFlag && tricks.Count == 5 && tricks[tricks.Count - 1].IsFull())
            {
                players[0] = new MaxNode(players[0].Id, players[0].Hand);
                players[1] = new MinNode(players[1].Id, players[1].Hand);
                players[2] = new MaxNode(players[2].Id, players[2].Hand);
                players[3] = new MinNode(players[3].Id, players[3].Hand);
            }
            int nextPlayerId = tricks[tricks.Count - 1].GetNextPlayerId();
            foreach (PlayerNode p in players)
            {
                if (p.Id == nextPlayerId)
                {
                    return p;
                }
            }

            Console.WriteLine("PIG::GetNextPlayer >> Unpected next player id");
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
                predictableTrickWinner = -1;
                predictableTrickCut = false;
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == players[0].Id || winnerPoints[0] == (players[0].Id + 2) % 4)
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

            predictableTrickWinner = -1;
            predictableTrickCut = false;
            Trick currentTrick = tricks[tricks.Count - 1];
            if (currentTrick.IsFull())
            {
                int[] winnerPoints = currentTrick.GetTrickWinnerAndPoints();
                if (winnerPoints[0] == players[0].Id || winnerPoints[0] == (players[0].Id + 2) % 4)
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


        public void predictTrickWinner()
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            int winningCard = currentTrick.GetCurrentWinningCard();
            int winningPlayerId = currentTrick.GetCurrentWinningPlayer();
            int remainingPlays = 4 - currentTrick.GetCurrentTrickSize();
            int leadSuit = currentTrick.LeadSuit;

            if (winningCard == -1)
            {
                Console.WriteLine("PIG::predictTrickWinner >> Impossible winningCard");
            }

            //if (Card.GetSuit(winningCard) != leadSuit)
            //{
            //    predictableTrickCut = true;
            //}
            predictableTrickCut = false;

            for (int i = 0, pid = currentTrick.GetNextPlayerId(); i < remainingPlays; i++, pid = (pid + 1) % 4)
            {
                int bestCard = players[pid].GetHighestRankFromSuit(leadSuit, trump);
                if (bestCard == -1)
                {
                    //the player does not have the leadsuit neigher the trump
                    continue;
                }

                if (Card.GetSuit(bestCard) != leadSuit)
                {
                    if (Card.GetSuit(winningCard) == leadSuit || Card.GetRank(bestCard) > Card.GetRank(winningCard))
                    {
                        predictableTrickCut = true;
                        winningCard = bestCard;
                        predictableTrickWinner = pid;
                    }
                }
                else if (Card.GetRank(bestCard) > Card.GetRank(winningCard) && !predictableTrickCut)
                {
                    winningCard = bestCard;
                    predictableTrickWinner = pid;
                }
            }
        }

        public List<int> orderPossibleMoves(List<int> moves, int playerID)
        {
            Trick currentTrick = tricks[tricks.Count - 1];
            int leadSuit = currentTrick.LeadSuit;
            int currentTrickSize = currentTrick.GetCurrentTrickSize();

            if (moves.Count == 1)
            {
                return moves;
            }

            if (currentTrickSize == 0)
            {
                AscendingComparer ac = new AscendingComparer();
                moves.Sort(ac);
                return moves;
            }

            if (predictableTrickWinner == -1)
            {
                predictTrickWinner();
            }

            if (!predictableTrickCut && (predictableTrickWinner == playerID || predictableTrickWinner == (playerID + 2) % 4))
            {
                AscendingComparer ac = new AscendingComparer();
                moves.Sort(ac);
            }
            else if (predictableTrickCut && (predictableTrickWinner == playerID || predictableTrickWinner == (playerID + 2) % 4))
            {
                AscendingCutComparer acc = new AscendingCutComparer(trump);
                moves.Sort(acc);
            }
            else
            {
                DescendingComparer dc = new DescendingComparer();
                moves.Sort(dc);
            }

            return moves;
        }

    }
}
