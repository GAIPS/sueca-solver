using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using System.Collections.Generic;

namespace SuecaPlayer
{
    public interface IIAPublisher : IThalamusPublisher, SuecaMessages.ISuecaActions {}

    class SuecaPlayer : ThalamusClient, SuecaMessages.ISuecaPerceptions
    {
        private class IAPublisher : IIAPublisher
        {
            dynamic publisher;
            public IAPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Play(int id, string card)
            {
                publisher.Play(id, card);
            }
        }


        private IAPublisher iaPublisher;
        private SmartPlayer ai;
        private int myIdOnUnityGame;


        public SuecaPlayer() : base("SuecaIA", "tiago")
        {
            ai = null;
            SetPublisher<IIAPublisher>();
            iaPublisher = new IAPublisher(Publisher);
        }

        void SuecaMessages.ISuecaPerceptions.GameStart(int id, int teamId, string trump, string[] cards)
        {
            myIdOnUnityGame = id;
            List<int> initialCards = new List<int>();
            foreach (string cardSerialized in cards)
            {
                SuecaMessages.Card card = JsonSerializable.DeserializeFromJson<SuecaMessages.Card>(cardSerialized);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank) Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                initialCards.Add(myCard);
            }
            SuecaSolver.Suit myTrump = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), trump);

            ai = new SmartPlayer(0, initialCards, (int) myTrump, new Random(), 03129840);
            Debug(">>>>>SuecaPlayer has inited the game");
        }

        void SuecaMessages.ISuecaPerceptions.GameEnd(int team0Score, int team1Score)
        {

        }

        void SuecaMessages.ISuecaPerceptions.NextPlayer(int id)
        {
            if (myIdOnUnityGame == id && ai != null)
            {
                int chosenCard = ai.Play();
                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaMessages.Rank msgRank = (SuecaMessages.Rank)Enum.Parse(typeof(SuecaMessages.Rank), chosenCardRank.ToString());
                SuecaMessages.Suit msgSuit = (SuecaMessages.Suit)Enum.Parse(typeof(SuecaMessages.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaMessages.Card(msgRank, msgSuit).SerializeToJson();

                iaPublisher.Play(myIdOnUnityGame, cardSerialized);
            }

        }

        void SuecaMessages.ISuecaPerceptions.Play(int id, string card)
        {
            if (myIdOnUnityGame != id && ai != null)
            {
                SuecaMessages.Card c = JsonSerializable.DeserializeFromJson<SuecaMessages.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                int myIdOfOtherPlayer = (id + 4 - myIdOnUnityGame) % 4;
                ai.AddPlay(myIdOfOtherPlayer, myCard);
            }
        }

    }
}
