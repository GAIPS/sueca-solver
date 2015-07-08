using System;
using Thalamus;
using SuecaSolver;
using System.Collections.Generic;

namespace SuecaPlayer
{

    class SuecaPlayer : ThalamusClient, SuecaInterface.IStartEvents, SuecaInterface.ICardsEvents
    {

        private SmartPlayer ai;


        public SuecaPlayer()
            : base("SuecaPlayer", "Tiago")
        {
            ai = null;
        }

        void SuecaInterface.IStartEvents.Init(int trump, int firstPlayer, int[] initialCards)
        {
            ai = new SmartPlayer(0, new List<int>(initialCards), trump, new Random(), 03129840);
            Debug(">>>>>SuecaPlayer has inited the game");
        }

        void SuecaInterface.ICardsEvents.PlayedCard(int playerId, int card)
        {
            if (ai != null)
            {
                ai.AddPlay(playerId, card);
            }
        }

        void SuecaInterface.ICardsEvents.Play()
        {
            if (ai != null)
            {
                int chosenCard = ai.Play();
                //publish somewhere the chosenCard
            }
        }
    }
}
