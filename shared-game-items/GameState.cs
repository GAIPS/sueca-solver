using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class GameState
    {

        private List<Trick> tricks;
        private Player[] players;
        private int trump;

        private AscendingComparer ac;
        private DescendingComparer dc;
        private AscendingCutComparer acc;
        //        private DescendingCutComparer dcc;

        private int predictableTrickWinner;
        private bool predictableTrickCut;

        private int points;
        private List<int> pointsPerTrick;


        public GameState(int numTricks, int trumpSuit, Player[] playersList)
        {
            ac = new AscendingComparer();
            dc = new DescendingComparer();
            acc = new AscendingCutComparer(trumpSuit);
            //            dcc = new DescendingCutComparer(trumpSuit);
            players = new Player[4];
            tricks = new List<Trick>(numTricks);
            trump = trumpSuit;
            predictableTrickWinner = -1;
            predictableTrickCut = false;
            points = 0;
            pointsPerTrick = new List<int>(numTricks);

            for (int i = 0; i < 4; i++)
            {
                players[i] = playersList[i];
            }
        }

        private int getPlayInTrick()
        {
            int playInCurrentTrick = GetCurrentTrick().GetPlayInTrick();
            if (playInCurrentTrick == 4)
            {
                return 0;
            }
            return playInCurrentTrick;
        }

        public Trick GetCurrentTrick()
        {
            if (tricks.Count == 0)
            {
                Console.WriteLine("GameState.GetCurrentTrick - No tricks available");
            }
            return tricks[tricks.Count - 1];
        }

        // This function is always called after applying a move
        public Player GetNextPlayer()
        {
            int nextPlayerId;
            if (GetCurrentTrick().IsFull())
            {
                int[] winnerAndPoints = GetCurrentTrick().GetTrickWinnerAndPoints();
                int trickPoints = winnerAndPoints[1];
                points += trickPoints;
                pointsPerTrick.Add(trickPoints);
                nextPlayerId = winnerAndPoints[0];
            }
            else
            {
                int lastPlayerId = GetCurrentTrick().GetLastPlayerId();
                nextPlayerId = (lastPlayerId + 1) % 4;
            }
            return players[nextPlayerId];
        }

        private void predictTrickWinner(int playerID, int leadSuit, int currentPlayInTrick)
        {
            List<Move> currentTrick = GetCurrentTrick().GetMoves();
            int bestRank = 0;
            int firstPlayerId = (4 + (playerID - currentPlayInTrick)) % 4;
            for (int i = 0; i < 4; i++)
            {
                int highestRankForPlayer;
                int pID = (firstPlayerId + i) % 4;
                if (i < currentPlayInTrick)
                {
                    highestRankForPlayer = Card.GetRank(currentTrick[i].Card);
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
            int currentPlayInTrick = getPlayInTrick();

            if (currentPlayInTrick == 0 || moves.Count == 1)
            {
                return moves;
            }

            if (predictableTrickWinner == -1)
            {
                predictTrickWinner(playerID, leadSuit, currentPlayInTrick);
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
            if (tricks.Count == 0 || GetCurrentTrick().IsFull())
            {
                tricks.Add(new Trick(trump));
            }
            GetCurrentTrick().ApplyMove(move);
            if (GetCurrentTrick().IsFull())
            {
                predictableTrickWinner = -1;
            }
        }

        public void UndoMove()
        {
            Trick currentTrick = GetCurrentTrick();
            predictableTrickWinner = -1;
            predictableTrickCut = false;

            if (currentTrick.IsFull())
            {
                int currentTrickIndex = pointsPerTrick.Count - 1;
                points -= pointsPerTrick[currentTrickIndex];
                pointsPerTrick.RemoveAt(currentTrickIndex);
            }

            currentTrick.UndoMove();
            if (currentTrick.IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }
        }

        public int GetLeadSuit()
        {
            Trick currentTrick = GetCurrentTrick();
            if (currentTrick.IsFull())
            {
                return (int)Suit.None;
            }
            return currentTrick.LeadSuit;
        }

        //        public bool IsNewTrick()
        //        {
        //            if (tricks.Count == 0 || GetCurrentTrick().IsEmpty())
        //            {
        //                return true;
        //            }
        //            return false;
        //        }

        public bool IsEndGame()
        {
            if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
            {
                return true;
            }
            return false;
        }


        //        public bool IsEndFirstTrick()
        //        {
        //            if (tricks.Count > 0 && tricks[0].IsFull())
        //            {
        //                return true;
        //            }
        //            return false;
        //        }


        public int[] GetGamePoints()
        {
            int[] result = new int[2] { 0, 0 };
            for (int i = 0; i < tricks.Count; i++)
            {
                int trickResult = tricks[i].GetTrickPoints();
                if (trickResult > 0)
                {
                    result[0] += trickResult;
                }
                else
                {
                    result[1] += (-1 * trickResult);
                }
            }
            return result;
        }

        public int EvalGame()
        {
            return points;
        }

        //        public int GetTrickWinnerId()
        //        {
        //            return GetCurrentTrick().GetTrickWinner();
        //        }

        //        public int GetFirstTrickPoints()
        //        {
        //            return tricks[0].GetTrickPoints();
        //        }

        //        public int GetTrickPoints()
        //        {
        //            return GetCurrentTrick().GetTrickPoints();
        //        }

        public void PrintLastTrick()
        {
            if (tricks.Count > 0 && tricks[0].IsFull())
            {
                Console.WriteLine("Last trick:");
                if (GetCurrentTrick().IsFull())
                {
                    GetCurrentTrick().PrintTrick();
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
            if (tricks.Count > 0 && !GetCurrentTrick().IsFull())
            {
                GetCurrentTrick().PrintTrick();
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