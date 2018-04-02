using System.Collections.Generic;

namespace SuecaSolver
{
    public class TimeLimitedPlayer : ArtificialPlayer
    {
        //private int _idDiff;
        private int _handSize;
        //private PIMC pimc;
        //private InformationSet InfoSet;


        public TimeLimitedPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            //_idDiff = 0 - id;
            _handSize = initialHand.Count;
            //pimc = new PIMC();
            InfoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
        }

        override public void AddPlay(int playerID, int card)
        {
            //int playerIdForMe = playerID + _idDiff;
            //if (playerIdForMe < 0)
            //{
            //    playerIdForMe += 4;
            //}
            InfoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (_handSize > 10)
            {
                chosenCard = InfoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.ExecuteWithTimeLimit(_id, InfoSet, new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            }

            //infoSet.AddMyPlay(chosenCard);
            _handSize--;
            return chosenCard;
        }
    }
}