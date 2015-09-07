using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using System.Collections.Generic;

namespace SuecaPlayer
{
    public interface IIAPublisher : IThalamusPublisher, IIAActions {}

    class SuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private class IAPublisher : IIAPublisher
        {
            dynamic publisher;
            public IAPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Decision(string card, string followingInfo)
            {
                publisher.Decision(card, followingInfo);
            }

            public void ExpectedScores(int team0Score, int team1Score)
            {
                publisher.ExpectedScores(team0Score, team1Score);
            }
        }


        private int moveCounter;
        private string trumpSuit;
        private string leadSuit;

        private IAPublisher iaPublisher;
        private SmartPlayer ai;
        private int myIdOnUnityGame;


        public SuecaPlayer() : base("IA", "")
        {
            ai = null;
            SetPublisher<IIAPublisher>();
            iaPublisher = new IAPublisher(Publisher);
        }


        public void SessionStart(int numGames)
        {
            
        }

        public void GameStart(int gameId, int playerId, int teamId, string trump, string[] cards)
        {
            moveCounter = 0;
            trumpSuit = trump;
            myIdOnUnityGame = playerId;
            List<int> initialCards = new List<int>();
            foreach (string cardSerialized in cards)
            {
                SuecaTypes.Card card = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(cardSerialized);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank) Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                initialCards.Add(myCard);
            }
            SuecaSolver.Suit myTrump = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), trump);

            ai = new SmartPlayer(0, initialCards, (int) myTrump, new Random(), 03129840);
            Debug(">>>>>SuecaPlayer has inited the game");
        }

        public void GameEnd(int team0Score, int team1Score)
        {

        }

        public void SessionEnd(int team0Score, int team1Score)
        {
            
        }

        public void Shuffle(int playerId)
        {
            
        }

        public void Cut(int playerId)
        {
            
        }

        public void Deal(int playerId)
        {
            
        }

        public void NextPlayer(int id)
        {
            if (myIdOnUnityGame == id && ai != null)
            {
                int chosenCard = ai.Play();
                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();

                string additionalInfo;
                if (moveCounter % 4 == 0)
                {
                    additionalInfo = "NEW_TRICK";
                    leadSuit = chosenCardSuit.ToString();
                }
                else if (leadSuit == chosenCardSuit.ToString())
                {
                    additionalInfo = "FOLLOWING";
                }
                else if (leadSuit != trumpSuit && chosenCardSuit.ToString() == trumpSuit)
                {
                    additionalInfo = "CUT";
                }
                else
                {
                    additionalInfo = "NOT_FOLLOWING";
                }
                moveCounter++;

                iaPublisher.Decision(cardSerialized, additionalInfo);
            }

        }

        public void Play(int id, string card)
        {
            if (myIdOnUnityGame != id && ai != null)
            {
                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                int localPlayerId = (id + 4 - myIdOnUnityGame) % 4;
                ai.AddPlay(localPlayerId, myCard);

                if (moveCounter % 4 == 0)
                {
                    leadSuit = c.Suit.ToString();
                }
                moveCounter++;
            }

            iaPublisher.ExpectedScores(ai.GetExpectedScore(), -ai.GetExpectedScore());
        }
    }
}
