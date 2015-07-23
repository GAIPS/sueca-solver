using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using System.Collections.Generic;

namespace SuecaPlayer
{

    class SuecaPlayer : ThalamusClient, SuecaMessages.ISuecaPerceptions
    {

        private SmartPlayer ai;


        public SuecaPlayer()
            : base("SuecaPlayer", "Tiago")
        {
            ai = null;
        }

        void SuecaMessages.ISuecaPerceptions.GameStart(int id, int teamId, int trump, string[] cards)
        {
            //ai = new SmartPlayer(0, new List<int>(initialCards), trump, new Random(), 03129840);
            Debug(">>>>>SuecaPlayer has inited the game");
        }

        void SuecaMessages.ISuecaPerceptions.GameEnd(int team0Score, int team1Score)
        {

        }

        void SuecaMessages.ISuecaPerceptions.NextPlayer(int id)
        {

        }

        void SuecaMessages.ISuecaPerceptions.Play(int id, string card)
        {
            if (ai != null)
            {
                //ai.AddPlay(id, card);
            }
        }

    }
}
