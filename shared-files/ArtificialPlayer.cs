
namespace SuecaSolver
{
    public abstract class ArtificialPlayer
    {

        protected int _id;

        public ArtificialPlayer(int id)
        {
            _id = id;
        }

        public abstract void AddPlay(int playerID, int card);

        public abstract int Play();
    }
}