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

            public void Expectation(string successProbability, string failureProbability)
            {
                publisher.Expectation(successProbability, failureProbability);
            }


            public void MoveDesirabilities(string desirability, string desirabilityForOther)
            {
                publisher.MoveDesirabilities(desirability, desirabilityForOther);
            }
        }


        private int moveCounter;
        private string trumpSuit;
        private string leadSuit;
        private int sessionGames;
        private int ourWins;
        private int theirWins;
        private int myIdOnUnityGame;
        private int myTeamIdOnUnityGame;

        private IAPublisher iaPublisher;
        private SmartPlayer ai;


        public SuecaPlayer() : base("IA", "")
        {
            moveCounter = 0;
            trumpSuit = "None";
            leadSuit = "None";
            sessionGames = 0;
            ourWins = 0;
            theirWins = 0;
            myIdOnUnityGame = 1; //default
            myTeamIdOnUnityGame = 1; //default

            ai = null;
            SetPublisher<IIAPublisher>();
            iaPublisher = new IAPublisher(Publisher);
        }


        public void SessionStart(int numGames)
        {
            moveCounter = 0;
            trumpSuit = "None";
            leadSuit = "None";
            sessionGames = numGames;
            ourWins = 0;
            theirWins = 0;
            myIdOnUnityGame = 1; //default
            myTeamIdOnUnityGame = 1; //default
        }

        public void GameStart(int gameId, int playerId, int teamId, string trump, string[] cards)
        {
            moveCounter = 0;
            trumpSuit = trump;
            myIdOnUnityGame = playerId;
            myTeamIdOnUnityGame = teamId;
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
            if (myTeamIdOnUnityGame == 0)
            {
                if (team0Score == 120)
                {
                    ourWins += 4;
                }
                else if (team0Score > 90)
                {
                    ourWins += 2;
                }
                else if (team0Score > 60)
                {
                    ourWins += 1;
                }
                else if (team1Score == 120)
                {
                    theirWins += 4;
                }
                else if (team1Score > 90)
                {
                    theirWins += 2;
                }
                else if (team1Score > 60)
                {
                    theirWins += 1;
                }
            }
            else
            {
                if (team0Score == 120)
                {
                    theirWins += 4;
                }
                else if (team0Score > 90)
                {
                    theirWins += 2;
                }
                else if (team0Score > 60)
                {
                    theirWins += 1;
                }
                else if (team1Score == 120)
                {
                    ourWins += 4;
                }
                else if (team1Score > 90)
                {
                    ourWins += 2;
                }
                else if (team1Score > 60)
                {
                    ourWins += 1;
                }
            }
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

            float desirabilityForOther, desirability = ai.TrickExpectedReward / 30.0f;
            if (desirability > 1.0f)
            {
                desirability = 1.0f;
            }
            else if (desirability < -1.0f)
            {
                desirability = -1.0f;
            }

            if (id == myIdOnUnityGame || id == (myIdOnUnityGame + 2) % 4)
            {
                desirabilityForOther = desirability;
            }
            else
            {
                desirabilityForOther = -desirability;
            }

            iaPublisher.MoveDesirabilities(desirability.ToString(), desirabilityForOther.ToString());

            float ourWinsOfSessionRacio = ourWins / sessionGames;
            float theirWinsOfSessionRacio = theirWins / sessionGames;
            float ourPointsOfGameRacio = ai.PointsPercentage();
            float theirPointsOfGameRacio = 1.0f - ourPointsOfGameRacio;
            float ourHandHope = ai.GetHandHope();
            float theirHandHope = 1.0f - ourHandHope;

            float successProbability = (0.5f * ourWinsOfSessionRacio) + (0.25f * ourPointsOfGameRacio) + (0.25f * ourHandHope);
            float failureProbability = (0.5f * theirWinsOfSessionRacio) + (0.25f * theirPointsOfGameRacio) + (0.25f * theirHandHope);

            iaPublisher.Expectation(successProbability.ToString(), failureProbability.ToString());
        }
    }
}
