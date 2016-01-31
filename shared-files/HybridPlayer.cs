using System.Collections.Generic;

namespace SuecaSolver
{
    public class HybridPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;


        public HybridPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            infoSet = new InformationSet(id, initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (infoSet.GetHandSize() > 10)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.ExecuteWithHybridSearch(_id, infoSet, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 });
            }

            return chosenCard;
        }
    }
}