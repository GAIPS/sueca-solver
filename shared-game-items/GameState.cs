using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class GameState
    {

        private List<Trick> tricks;
        private Player[] players;
        private Suit trump;
        private AscendingComparer ac;
        private DescendingComparer dc;
        private AscendingCutComparer acc;
        //		private DescendingCutComparer dcc;
        private int predictableTrickWinner;
        private bool predictableTrickCut;

        public GameState(int numTricks, Suit trumpSuit, Player[] playersList)
        {
            ac = new AscendingComparer();
            dc = new DescendingComparer();
            acc = new AscendingCutComparer(trumpSuit);
//			dcc = new DescendingCutComparer(trumpSuit);
            players = new Player[4];
            tricks = new List<Trick>(numTricks);
            trump = trumpSuit;
            predictableTrickWinner = -1;
            predictableTrickCut = false;

            for (int i = 0; i < 4; i++)
            {
                players[i] = playersList[i];
            }
        }

        public int GetPlayInTrick()
        {
            int playInCurrentTrick = GetCurrentTrick().getPlayInTrick();
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
                Console.WriteLine("Trouble at GameState in GetCurrentTrick");
            }
            return tricks[tricks.Count - 1];
        }

        // This function is always called after applying a move
        public Player GetNextPlayer()
        {
            int nextPlayerId;
            if (GetCurrentTrick().IsFull())
            {
                nextPlayerId = GetCurrentTrick().GetTrickWinnerId();
            }
            else
            {
                int lastPlayerId = GetCurrentTrick().GetLastPlayerId();
                nextPlayerId = (lastPlayerId + 1) % 4;
            }
            return players[nextPlayerId];
        }


        public List<Card> orderPossibleMoves(List<Card> moves, int playerID)
        {
            Suit leadSuit = GetLeadSuit();
            int currentPlayInTrick = GetPlayInTrick();
        
            if (currentPlayInTrick == 0 || moves.Count == 1)
            {
                return moves;
            }
        
            if (predictableTrickWinner == -1)
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
                        highestRankForPlayer = (int)currentTrick[i].Card.Rank;
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

        private bool cardsHaveSuit(List<Card> cards, Suit leadSuit)
        {
            foreach (Card card in cards)
            {
                if (card.Suit == leadSuit)
                {
                    return true;
                }
            }
            return false;
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
            predictableTrickWinner = -1;
            predictableTrickCut = false;
            GetCurrentTrick().UndoMove();
            if (GetCurrentTrick().IsEmpty())
            {
                tricks.RemoveAt(tricks.Count - 1);
            }
        }

        public Suit GetLeadSuit()
        {
            Trick currentTrick = GetCurrentTrick();
            if (currentTrick.IsFull())
            {
                return Suit.None;
            }
            return currentTrick.LeadSuit;
        }

        public bool IsNewTrick()
        {
            if (tricks.Count == 0 || GetCurrentTrick().IsEmpty())
            {
                return true;
            }
            return false;
        }

        public bool IsEndGame()
        {
            if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
            {
                // Console.WriteLine("ENG GAME!!!!");
                return true;
            }
            return false;
        }


        public bool IsEndFirstTrick()
        {
            if (tricks.Count > 0 && tricks[0].IsFull())
            {
                return true;
            }
            return false;
        }

        public bool IsEndTrick()
        {
            if (GetCurrentTrick().IsFull())
            {
                return true;
            }
            return false;
        }



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
            int result = 0;
            for (int i = 0; i < tricks.Count; i++)
            {
                int trickResult = tricks[i].GetTrickPoints();
                result += trickResult;
            }
            return result;
        }

        public int GetTrickWinnerId()
        {
            return GetCurrentTrick().GetTrickWinnerId();
        }

        public int GetFirstTrickPoints()
        {
            int trickResult = tricks[0].GetTrickPoints();
            return trickResult;
        }

        public int GetTrickPoints()
        {
            int trickResult = GetCurrentTrick().GetTrickPoints();
            return trickResult;
        }


        private void printTricks()
        {
            Console.WriteLine("printTricks - tricks.Count " + tricks.Count);
            for (int i = 0; i < tricks.Count; i++)
            {
                Console.WriteLine("--- Trick " + i + "---");
                tricks[i].PrintTrick();
            }
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

    }
}