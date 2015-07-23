using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using System.Collections.Generic;

namespace SuecaPlayer
{

    class SuecaPlayer : ThalamusClient, SuecaMessages.IStartEvents, SuecaMessages.ICardsEvents
    {

        private SmartPlayer ai;


        public SuecaPlayer()
            : base("SuecaPlayer", "Tiago")
        {
            ai = null;
        }

        void SuecaMessages.IStartEvents.Init(int id, int trump, int firstPlayer, int[] initialCards)
        {
            ai = new SmartPlayer(0, new List<int>(initialCards), trump, new Random(), 03129840);
            Debug(">>>>>SuecaPlayer has inited the game");
        }

        void SuecaMessages.ICardsEvents.PlayedCard(int playerId, int card)
        {
            if (ai != null)
            {
                ai.AddPlay(playerId, card);
            }
        }

        void SuecaMessages.ICardsEvents.Play()
        {
            if (ai != null)
            {
                int chosenCard = ai.Play();
                //publish somewhere the chosenCard
            }
        }
    }
}
