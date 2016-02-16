using System.Collections.Generic;

namespace SuecaSolver
{
    public class TimeLimitedPlayer : ArtificialPlayer
    {
        //private int _idDiff;
        private int _handSize;
        //private PIMC pimc;
        private InformationSet infoSet;


        public TimeLimitedPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            //_idDiff = 0 - id;
            _handSize = initialHand.Count;
            //pimc = new PIMC();
            infoSet = new InformationSet(id, initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            //int playerIdForMe = playerID + _idDiff;
            //if (playerIdForMe < 0)
            //{
            //    playerIdForMe += 4;
            //}
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
                chosenCard = PIMC.ExecuteWithTimeLimit(_id, infoSet, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 });
            }

            //infoSet.AddMyPlay(chosenCard);
            _handSize--;
            return chosenCard;
        }
    }
}