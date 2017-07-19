using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RandomNode : PlayerNode
    {
        private Random randomGen;

        public RandomNode(int id, List<int> hand, int trumpCard, int trumpPlayerId)
            : base(id, hand, trumpCard, trumpPlayerId)
        {
            randomGen = new Random(Guid.NewGuid().GetHashCode());
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            if (pig.IsEndGame())
            {
                return pig.EvalGame1();
            }

            List<int> possibleMoves = InfoSet.GetPossibleMoves();
            int randomIndex = randomGen.Next(0, possibleMoves.Count);
            int chosenCard = possibleMoves[randomIndex];
            Move move = new Move(Id, chosenCard);
            pig.ApplyMove(move);

            int value = pig.GetNextPlayer().PlayGame(pig, alpha, beta, depthLimit);
            pig.UndoMove(move);

            return value;
        }
    }
}