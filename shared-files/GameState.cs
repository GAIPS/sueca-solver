using System;
using System.Collections.Generic;
using System.Configuration;

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

        private int botTeamPoints;
        private int otherTeamPoints;
        private int maxPointsInGame;
        private List<int> pointsPerTrick;


        public GameState(int numTricks, int trumpSuit, Player[] playersList, int possiblePoints, int botTeamInitialPoints, int otherTeamInitialPoints)
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
            botTeamPoints = botTeamInitialPoints;
            otherTeamPoints = otherTeamInitialPoints;
            maxPointsInGame = possiblePoints;
            pointsPerTrick = new List<int>(numTricks);

            for (int i = 0; i < 4; i++)
            {
                players[i] = playersList[i];
            }
        }

        private int getCurrentTrickSize()
        {
            int playInCurrentTrick = GetCurrentTrick().GetTrickSize();
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
                int lastPlayerId = GetCurrentTrick().GetLastPlayerId();
                nextPlayerId = (lastPlayerId + 1) % 4;
            }
            return players[nextPlayerId];
        }

        private void predictTrickWinnerOfTrick(int playerID, int leadSuit, int currentPlayInTrick)
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


        private int predictTrickWinnerOfSuit(int leadSuit)
        {
            int highestRankForPlayer = 0;
            int highestRank = 0;
            int winnerId = 0;
            bool cut = false;

            for (int j = 0; j < 4; j++)
            {
                highestRankForPlayer = players[j].HighestRankForSuit(leadSuit, trump);
                if (!cut && highestRankForPlayer < 0)
                {
                    cut = true;
                    highestRank = highestRankForPlayer;
                    winnerId = j;
                }
                else if (!cut && highestRankForPlayer > highestRank)
                {
                    highestRank = highestRankForPlayer;
                    winnerId = j;
                }
                else if (cut && highestRankForPlayer < highestRank)
                {
                    highestRank = highestRankForPlayer;
                    winnerId = j;
                }
            }
            return winnerId;
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

        public bool IsEndGame()
        {
            if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
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
            return pointsPrediction();

        }

        private int pointsPrediction()
        {
            int teamHandPoints = 0, teamHandTrumps = 0;
            int botPredicitonPoints = botTeamPoints;
            int otherPredicitonPoints = otherTeamPoints;
            for (int i = 0; i < players.Length; i = i + 2)
            {
                List<int> pHand = players[i].Hand;
                teamHandTrumps += players[i].HasSuit[trump];

                for (int j = 0; j < pHand.Count; j++)
                {
                    teamHandPoints += Card.GetValue(pHand[j]);
                }
            }

            int remainingTrumps = teamHandTrumps + players[1].HasSuit[trump] + players[3].HasSuit[trump];
            int remainingPoints = 120 - botTeamPoints - otherTeamPoints;

            if (teamHandPoints > (remainingPoints * 0.5) && teamHandTrumps > (remainingTrumps * 0.5))
            {
                botPredicitonPoints += (int)(remainingPoints * 0.75);
                otherPredicitonPoints += (int)(remainingPoints * 0.25);
            }
            else
            {

                botPredicitonPoints += (int)(remainingPoints * 0.25);
                otherPredicitonPoints += (int)(remainingPoints * 0.75);
            }

            if (botPredicitonPoints > otherTeamPoints)
            {
                return botPredicitonPoints;
            }
            else
            {
                return -1 * otherTeamPoints;
            }
        }

        private bool botTeamWinningPrediction()
        {
            int teamHandPoints = 0, teamHandTrumps = 0;
            for (int i = 0; i < players.Length; i = i + 2)
            {
                List<int> pHand = players[i].Hand;
                teamHandTrumps += players[i].HasSuit[trump];

                for (int j = 0; j < pHand.Count; j++)
                {
                    teamHandPoints += Card.GetValue(pHand[j]);
                }
            }

            int remainingTrumps = teamHandTrumps + players[1].HasSuit[trump] + players[3].HasSuit[trump];
            int remainingPoints = 120 - botTeamPoints - otherTeamPoints;

            if (teamHandPoints > (remainingPoints * 0.5) && teamHandTrumps > (remainingTrumps * 0.5) && botTeamPoints + (int) (remainingPoints * 0.4) > 60)
            {
                return true;
            }
            return false;
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

        public int Heuristic(int depth)
        {
            int remainPoints = maxPointsInGame - botTeamPoints - otherTeamPoints;
            return remainPoints / 3;
        }


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