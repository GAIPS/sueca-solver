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
            : base("SuecaPlayer", "tiago")
        {
            ai = null;
        }

        void SuecaMessages.ISuecaPerceptions.GameStart(int id, int teamId, string trump, string[] cards)
        {
            List<int> initialCards = new List<int>();
            foreach (string cardSerialized in cards)
            {
                SuecaMessages.Card card = JsonSerializable.DeserializeFromJson<SuecaMessages.Card>(cardSerialized);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                initialCards.Add(myCard);
            }
            SuecaSolver.Suit myTrump = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), trump);

            ai = new SmartPlayer(0, initialCards, (int) myTrump, new Random(), 03129840);
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
                SuecaMessages.Card c = JsonSerializable.DeserializeFromJson<SuecaMessages.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                ai.AddPlay(id, myCard);
            }
        }

    }
}
