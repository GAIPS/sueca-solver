using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class GameState
    {
        private List<Trick> tricks;
        private Trick currentTrick;
        private PlayerNode[] players;
        private int trump;

        private AscendingComparer ac;
        private DescendingComparer dc;
        private AscendingCutComparer acc;

        private int predictableTrickWinner;
        private bool predictableTrickCut;

        private int botTeamPoints;
        private int otherTeamPoints;
        private List<int> pointsPerTrick;


        public GameState(int numTricks, int trumpSuit, PlayerNode[] playersList, int botTeamInitialPoints, int otherTeamInitialPoints)
        {
            ac = new AscendingComparer();
            dc = new DescendingComparer();
            acc = new AscendingCutComparer(trumpSuit);
            players = new PlayerNode[4];
            tricks = new List<Trick>(numTricks);
            currentTrick = null;
            trump = trumpSuit;
            predictableTrickWinner = -1;
            predictableTrickCut = false;
            botTeamPoints = botTeamInitialPoints;
            otherTeamPoints = otherTeamInitialPoints;
            pointsPerTrick = new List<int>(numTricks);

            for (int i = 0; i < 4; i++)
            {
                players[i] = playersList[i];
            }
        }



        private int getCurrentTrickSize()
        {
            int playInCurrentTrick = currentTrick.GetTrickSize();
            if (playInCurrentTrick == 4)
            {
                return 0;
            }
            return playInCurrentTrick;
        }

        // This function is always called after applying a move
        public PlayerNode GetNextPlayer()
        {
            int nextPlayerId;
            if (currentTrick.IsFull())
            {
                int[] winnerAndPoints = currentTrick.GetTrickWinnerAndPoints();
                int trickPoints = winnerAndPoints[1];
                if (trickPoints > 0)
                {
                    botTeamPoints += trickPoints;
                }
                if (trickPoints < 0)
                {
                    otherTeamPoints += (-1 * trickPoints);
                }
                pointsPerTrick.Add(trickPoints);
                nextPlayerId = winnerAndPoints[0];
            }
            else
            {
                int lastPlayerId = currentTrick.GetLastPlayerId();
                nextPlayerId = (lastPlayerId + 1) % 4;
            }
            return players[nextPlayerId];
        }

        private void predictTrickWinnerOfTrick(int playerID, int leadSuit, int currentPlayInTrick)
        {
            List<Move> moves = currentTrick.GetMoves();
            int bestRank = 0;
            int firstPlayerId = (4 + (playerID - currentPlayInTrick)) % 4;
            for (int i = 0; i < 4; i++)
            {
                int highestRankForPlayer;
                int pID = (firstPlayerId + i) % 4;
                if (i < currentPlayInTrick)
                {
                    highestRankForPlayer = Card.GetRank(moves[i].Card);
                }
                else
                {
                    highestRankForPlayer = players[pID].HighestRankForSuit(leadSuit, trump);
                }
                if (!predictableTrickCut)
                {
                    if (highestRankForPlayer > bestRank)
                    {
                        bestRank = highestRankForPlayer;
                        predictableTrickWinner = pID;
                    }
                    if (highestRankForPlayer < 0)
                    {
                        bestRank = highestRankForPlayer;
                        predictableTrickWinner = pID;
                        predictableTrickCut = true;
                    }
                }
                else if (highestRankForPlayer < bestRank)
                {
                    bestRank = highestRankForPlayer;
                    predictableTrickWinner = pID;
                }
            }
        }


        public List<int> orderPossibleMoves(List<int> moves, int playerID)
        {
            int leadSuit = GetLeadSuit();
            int currentPlayInTrick = getCurrentTrickSize();

            if (moves.Count == 1)
            {
                return moves;
            }

            if (currentPlayInTrick == 0)
            {
                moves.Sort(ac);
                return moves;
            }

            if (predictableTrickWinner == -1)
            {
                predictTrickWinnerOfTrick(playerID, leadSuit, currentPlayInTrick);
            }


            if (!predictableTrickCut && (predictableTrickWinner == playerID || predictableTrickWinner == (playerID + 2) % 4))
            {
                moves.Sort(ac);
            }
            else if (predictableTrickCut && (predictableTrickWinner == playerID || predictableTrickWinner == (playerID + 2) % 4))
            {
                moves.Sort(acc);
            }
            else
            {
                moves.Sort(dc);
            }

            return moves;
        }

        public void ApplyMove(Move move)
        {
            // if (tricks.Count == 0 || currentTrick.IsFull())
            if (currentTrick == null || currentTrick.IsFull())
            {
                Trick newTrick = new Trick(trump); 
                tricks.Add(newTrick);
                currentTrick = newTrick;
            }
            
            currentTrick.ApplyMove(move);
            foreach (PlayerNode player in players)
            {
                player.ApplyMove(move);
            }
            
            if (currentTrick.IsFull())
            {
                predictableTrickWinner = -1;
            }
        }

        public void UndoMove()
        {
            predictableTrickWinner = -1;
            predictableTrickCut = false;

            if (currentTrick.IsFull())
            {
                int currentTrickIndex = pointsPerTrick.Count - 1;
                int trickPoints = pointsPerTrick[currentTrickIndex];
                if (trickPoints > 0)
                {
                    botTeamPoints -= trickPoints;
                }
                if (trickPoints < 0)
                {
                    otherTeamPoints += trickPoints;
                }
                pointsPerTrick.RemoveAt(currentTrickIndex);
            }

            Move move = currentTrick.GetLastMove();
            currentTrick.UndoMove();
            foreach (PlayerNode player in players)
            {
                player.UndoMove(move);
            }
            
            if (currentTrick.IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }
        }

        public int GetLeadSuit()
        {
            if (currentTrick.IsFull())
            {
                return (int)Suit.None;
            }
            return currentTrick.LeadSuit;
        }

        public bool IsEndGame()
        {
            if (tricks.Count == tricks.Capacity && currentTrick.IsFull())
            {
                return true;
            }
            return false;
        }

        public bool IsOtherTeamWinning()
        {
            if (otherTeamPoints > 60)
            {
                return true;
            }
            return false;
        }

        public bool reachedDepthLimit(int limit)
        {
            if (tricks.Count > 0 && tricks.Count == limit && tricks[tricks.Count - 1].IsFull())
            {
                return true;
            }
            return false;
        }


        public int EvalGame()
        {
            if (botTeamPoints > otherTeamPoints)
            {
                return botTeamPoints;
            }
            else
            {
                return -1 * otherTeamPoints;
            }

            //if (botTeamPoints > otherTeamPoints)
            //{
            //    if (botTeamPoints > 90)
            //    {
            //        return 10;
            //    }
            //    if (botTeamPoints > 60)
            //    {
            //        return 5;
            //    }
            //    if (botTeamPoints > 30)
            //    {
            //        return 1;
            //    }
            //    return 0;
            //}
            //else
            //{
            //    if (otherTeamPoints > 90)
            //    {
            //        return -10;
            //    }
            //    if (otherTeamPoints > 60)
            //    {
            //        return -5;
            //    }
            //    if (otherTeamPoints > 30)
            //    {
            //        return -1;
            //    }
            //    return 0;
            //}
        }

        public int[] CalculePointsOfFinishedGame()
        {
            int[] points = new int[2] { 0, 0 };
            for (int i = 0; i < tricks.Count; i++)
            {
                int trickPoints = tricks[i].GetTrickWinnerAndPoints()[1];
                if (trickPoints > 0)
                {
                    points[0] += trickPoints;
                }
                else
                {
                    points[1] += (-1 * trickPoints);
                }
            }
            return points;
        }


        public void PrintLastTrick()
        {
            if (tricks.Count > 0 && tricks[0].IsFull())
            {
                Console.WriteLine("Last trick:");
                if (currentTrick.IsFull())
                {
                    currentTrick.PrintTrick();
                }
                else
                {
                    tricks[tricks.Count - 2].PrintTrick();
                }
                Console.WriteLine("");
            }
        }

        public void PrintCurrentTrick()
        {
            Console.WriteLine("Current trick:");
            if (tricks.Count > 0 && !currentTrick.IsFull())
            {
                currentTrick.PrintTrick();
            }
            Console.WriteLine("");
        }


        public void PrintTricks()
        {
            Console.WriteLine("printTricks - tricks.Count " + tricks.Count);
            for (int i = 0; i < tricks.Count; i++)
            {
                Console.WriteLine("--- Trick " + i + "---");
                tricks[i].PrintTrick();
            }
        }
    }
}