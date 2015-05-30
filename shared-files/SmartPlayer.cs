using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SmartPlayer : ArtificialPlayer
    {

        private PIMC pimc;
        private InformationSet infoSet;


        public SmartPlayer(List<int> initialHand, int trumpSuit)
        {
            pimc = new PIMC();
            infoSet = new InformationSet(initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {

            int chosenCard = pimc.Execute(infoSet);
            infoSet.AddMyPlay(chosenCard);

            return chosenCard;
        }
    }
}