using System;

namespace SuecaSolver
{
    public class Move
    {

        public int PlayerId;
        public int Card;

        public Move(int playerId, int card)
        {
            PlayerId = playerId;
            Card = card;
        }

        public override string ToString()
        {
            return "Player " + PlayerId + " has played " + Fart.ToString(Card);
        }
    }
}