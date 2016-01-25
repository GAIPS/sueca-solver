using System.Collections.Generic;

namespace SuecaSolver
{
    public class ElephantPlayer : ArtificialPlayer
    {
        private int _idDiff;
        private int _handSize;
        //private PIMC pimc;
        private InformationSet infoSet;


        public ElephantPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            _idDiff = 0 - id;
            _handSize = initialHand.Count;
            //pimc = new PIMC();
            infoSet = new InformationSet(id, initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            int playerIdForMe = playerID + _idDiff;
            if (playerIdForMe < 0)
            {
                playerIdForMe += 4;
            }
            infoSet.AddPlay(playerIdForMe, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (_handSize > 7)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.Execute(infoSet, new List<int> { 1000, 1000, 1000, 500, 50, 10, 3, 3, 1, 1 }, null, true);
            }

            //infoSet.AddMyPlay(chosenCard);
            _handSize--;
            return chosenCard;
        }
    }
}