using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class Trick
    {

        public int LeadSuit;
        private List<Move> moves;
        public int Trump;
        private List<int> points;
        private int currentPoints;
        //keep record of winning player and card for every move in order to undo more easily
        private List<int> winningPlayer;
        private int currentWinner;
        private List<int> winningCard;
        private int currentWinningCard;

        public Trick(int trumpSuit)
        {
            moves = new List<Move>(4);
            LeadSuit = (int)Suit.None;
            Trump = trumpSuit;
            points = new List<int>(4);
            currentPoints = 0;
            winningPlayer = new List<int>(4);
            currentWinner = -1;
            winningCard = new List<int>(4);
            currentWinningCard = -1;
        }

        internal List<Move> GetMoves()
        {
            return moves;
        }

        public bool IsCut()
        {
            return LeadSuit != Card.GetSuit(currentWinningCard);
        }

        public bool LastPlayIsCut()
        {
            int lastPlaySuit = Card.GetSuit(moves[moves.Count - 1].Card);
            return LeadSuit != lastPlaySuit && lastPlaySuit == Trump;
        }

        public bool LastPlayIsFollowing()
        {
            int lastPlaySuit = Card.GetSuit(moves[moves.Count - 1].Card);
            return LeadSuit == lastPlaySuit;
        }

        public bool LastPlayIsNewTrick()
        {
            if (moves.Count == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            if (moves.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool IsFull()
        {
            if (moves.Count == moves.Capacity)
            {
                return true;
            }
            return false;
        }
        
        public int GetCurrentTrickSize()
        {
            return moves.Count;
        }

        public void ApplyMove(Move move)
        {
            //the first move sets the leadsuit
            if (moves.Count == 0)
            {
                LeadSuit = Card.GetSuit(move.Card);
                winningPlayer.Add(move.PlayerId);
                currentWinner = move.PlayerId;
                winningCard.Add(move.Card);
                currentWinningCard = move.Card;
            }
            else
            {
                //the next moves might follow the leadsuit, cut with the trump suit, or play other suits
                if (Card.GetSuit(move.Card) == LeadSuit)
                {
                    //by playing the leadsuit, one just wins if no one had cut yet and his rank is higher than the previous highest 
                    if (Card.GetSuit(currentWinningCard) == LeadSuit && Card.GetRank(move.Card) > Card.GetRank(currentWinningCard))
                    {
                        winningPlayer.Add(move.PlayerId);
                        currentWinner = move.PlayerId;
                        winningCard.Add(move.Card);
                        currentWinningCard = move.Card;
                    }
                    else
                    {
                        //last winner remains the winner after the current move
                        winningPlayer.Add(currentWinner);
                        winningCard.Add(currentWinningCard);
                    }
                }
                else if(Card.GetSuit(move.Card) == Trump)
                {
                    //by playing trump suit, one just wins if is the first player cutting the trick OR is cutting higher than the previous cut
                    if (Card.GetSuit(currentWinningCard) != Trump ||
                    (Card.GetSuit(currentWinningCard) == Trump && Card.GetRank(move.Card) > Card.GetRank(currentWinningCard)))
                    {
                        winningPlayer.Add(move.PlayerId);
                        currentWinner = move.PlayerId;
                        winningCard.Add(move.Card);
                        currentWinningCard = move.Card;
                    }
                    else
                    {
                        //last winner remains the winner after the current move
                        winningPlayer.Add(currentWinner);
                        winningCard.Add(currentWinningCard);
                    }
                }
                else
                {
                    //by playing other suits, one never wins and the last winner remains the winner after the current move
                    winningPlayer.Add(currentWinner);
                    winningCard.Add(currentWinningCard);
                }
            }
            currentPoints += Card.GetValue(move.Card);
            points.Add(currentPoints);

            foreach (var item in moves)
            {
                if (item.PlayerId == move.PlayerId)
                {
                    Console.WriteLine("Please help!");
                }
            }
            moves.Add(move);
        }

        public void UndoMove()
        {
            int currentMoveIndex = moves.Count - 1;
            currentPoints -= Card.GetValue(moves[currentMoveIndex].Card);
            moves.RemoveAt(currentMoveIndex);
            winningCard.RemoveAt(currentMoveIndex);
            winningPlayer.RemoveAt(currentMoveIndex);
            points.RemoveAt(currentMoveIndex);

            //update index
            currentMoveIndex = moves.Count - 1;
            if (currentMoveIndex == -1)
            {
                currentWinner = -1;
                currentWinningCard = -1;
            }
            else
            {
                currentWinner = winningPlayer[currentMoveIndex];
                currentWinningCard = winningCard[currentMoveIndex];
            }
        }

        public int GetNextPlayerId()
        {
            if (IsFull())
            {
                return currentWinner;
            }

            if (moves.Count == 0)
            {
                return -1;
            }
            
            return (moves[moves.Count - 1].PlayerId + 1) % 4;
        }


        public int GetCurrentWinningCard()
        {
            return currentWinningCard;
        }

        public int GetCurrentWinningPlayer()
        {
            return currentWinner;
        }

        public void PrintTrick()
        {
            foreach (Move m in moves)
            {
                Console.WriteLine(m);
            }
        }

        public int GetCurrentTrickWinner()
        {
            return currentWinner;
        }

        public int GetCurrentTrickPoints()
        {
            return currentPoints;
        }

        public bool HasNewTrickWinner()
        {
            if (winningPlayer.Count <= 1 || winningPlayer[winningPlayer.Count - 1] != winningPlayer[winningPlayer.Count - 2])
	        {
                return true;
	        }
            return false;
        }

        public bool HasNewTrickTeamWinner()
        {
            //if the absolute difference between the ids of the two last players is 2, then they are on the same team
            if (winningPlayer.Count <= 1 || ((winningPlayer[winningPlayer.Count - 1] - winningPlayer[winningPlayer.Count - 2] + 4) % 4 != 2))
            {
                return true;
            }
            return false;
        }

        public int GetTrickIncrease()
        {
            if (points.Count == 1)
            {
                return points[points.Count - 1];
            }
            
            return points[points.Count - 1] - points[points.Count - 2];
        }

        public int[] GetTrickWinnerAndPoints()
        {
            return new int[] { currentWinner, currentPoints};
        }

        internal bool IsLastPlayOfTrick()
        {
            return moves.Count == 4;
        }
    }
}