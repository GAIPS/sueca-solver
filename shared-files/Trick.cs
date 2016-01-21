using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class Trick
    {

        public int LeadSuit;
        private List<Move> moves;
        public int Trump;
        private int points;
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
            points = 0;
            winningPlayer = new List<int>(4);
            currentWinner = -1;
            winningCard = new List<int>(4);
            currentWinningCard = -1;
        }

        public List<Move> GetMoves()
        {
            return moves;
        }
        
        public Move GetLastMove()
        {
            return moves[moves.Count - 1];
        }
        
        //nome infeliz para o metodo!
        public int GetTrickSize()
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
            points += Card.GetValue(move.Card);
            moves.Add(move);
        }

        public void UndoMove()
        {
            int currentMoveIndex = moves.Count - 1;
            moves.RemoveAt(currentMoveIndex);
            winningCard.RemoveAt(currentMoveIndex);
            winningPlayer.RemoveAt(currentMoveIndex);
            
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
            
            return (moves[moves.Count - 1].PlayerId + 1) % 4;
        }

        public int GetLastPlayerId()
        {
            if (moves.Count == 0)
            {
                Console.WriteLine("Trick trouble at GetLastPlayerId!!!");
                return -1;
            }
            return moves[moves.Count - 1].PlayerId;
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


        public int HighestCardOnCurrentTrick()
        {
            int result = 0;
            bool cut = false;
            foreach (var move in moves)
            {
                int card = move.Card;
                // int cardSuit = Card.GetSuit(card);
                int cardRank = Card.GetRank(card);

                if (!cut)
                {
                    if (cardRank > result)
                    {
                        result = cardRank;
                    }
                    else if (cardRank < 0)
                    {
                        result = cardRank;
                        cut = true;
                    }
                }
                else if (cut && cardRank < result)
                {
                    result = cardRank;
                }
            }
            return result;
        }

        public void PrintTrick()
        {
            foreach (Move m in moves)
            {
                Console.WriteLine(m);
            }
        }

        public int[] GetTrickWinnerAndPoints()
        {
            return new int[] { currentWinner, points};
        }
    }
}