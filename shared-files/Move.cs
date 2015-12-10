
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

        //The namespace disambiguates the static class from non-static attribute
        public override string ToString()
        {
            return "Player " + PlayerId + " has played " + SuecaSolver.Card.ToString(Card);
        }
    }
}