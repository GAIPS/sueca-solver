using System.Collections.Generic;

namespace SuecaSolver
{
    public class RBOPlayer : ArtificialPlayer
    {
        private int _handSize;
        private InformationSet infoSet;


        public RBOPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            _handSize = initialHand.Count;
            infoSet = new InformationSet(id, initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (_handSize > 10)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.Execute(_id, infoSet, 1, new List<int> { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 }, new List<int> { 1000, 1000, 1000, 1000, 1000, 4, 4, 3, 3, 3 });
            }

            _handSize--;
            return chosenCard;
        }
    }
}