using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using System.Collections.Generic;
using EmoteCommonMessages;

namespace SuecaPlayer
{
    public interface IIAPublisher : IThalamusPublisher, IIAActions, IGazeStateActions { }

    class SuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private class IAPublisher : IIAPublisher
        {
            dynamic publisher;
            public IAPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Decision(string card, string rank, string suit, string followingInfo)
            {
                publisher.Decision(card, rank, suit, followingInfo);
            }

            public void MoveExpectations(int playerId, string desirability, string desirabilityForOther, string successProbability, string failureProbability)
            {
                publisher.MoveExpectations(playerId, desirability, desirabilityForOther, successProbability, failureProbability);
            }

            public void ForwardShuffle(int playerId, int talkingRobot)
            {
                publisher.ForwardShuffle(playerId, talkingRobot);
            }

            public void ForwardCut(int playerId, int talkingRobot)
            {
                publisher.ForwardCut(playerId, talkingRobot);
            }

            public void ForwardDeal(int playerId, int talkingRobot)
            {
                publisher.ForwardDeal(playerId, talkingRobot);
            }

            public void ForwardTrumpCard(string trumpCard, int playerId, int talkingRobot)
            {
                publisher.ForwardTrumpCard(trumpCard, playerId, talkingRobot);
            }

            //public void ForwardTrumpcard(string trumpCard, int playerId)
            //{
            //    publisher.ForwardTrumpcard(trumpCard, playerId);
            //}

            public void ForwardGameEnd(int team0Score, int team1Score, int talkingRobot)
            {
                publisher.ForwardGameEnd(team0Score, team1Score, talkingRobot);
            }

            public void ForwardGameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards, int talkingRobot)
            {
                publisher.ForwardGameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards, talkingRobot);
            }

            public void ForwardNextPlayer(int id, int talkingRobot)
            {
                publisher.ForwardNextPlayer(id, talkingRobot);
            }

            public void ForwardTrickEnd(int winnerId, int trickPoints, int talkingRobot)
            {
                publisher.ForwardTrickEnd(winnerId, trickPoints, talkingRobot);
            }

            public void ForwardReceiveRobotCards(int playerId, int talkingRobot)
            {
                publisher.ForwardReceiveRobotCards(playerId, talkingRobot);
            }

            public void ForwardSessionEnd(int team0Score, int team1Score, int talkingRobot)
            {
                publisher.ForwardSessionEnd(team0Score, team1Score, talkingRobot);
            }

            public void ForwardSessionStart(int numGame, int talkingRobot)
            {
                publisher.ForwardSessionStart(numGame, talkingRobot);
            }

            public void GazeAtScreen(double x, double y)
            {
                publisher.GazeAtScreen(x, y);
            }

            public void GazeAtTarget(string targetName)
            {
                publisher.GazeAtTarget(targetName);
            }

            public void GlanceAtScreen(double x, double y)
            {
                publisher.GlanceAtScreen(x, y);
            }

            public void GlanceAtTarget(string targetName)
            {
                publisher.GlanceAtTarget(targetName);
            }

            public void ForwardRenounce(int playerId, int talkingRobot)
            {
                publisher.ForwardRenounce(playerId, talkingRobot);
            }


            public void ForwardResetTrick(int talkingRobot)
            {
                publisher.ForwardResetTrick(talkingRobot);
            }
        }


        private int moveCounter;
        private string trumpSuit;
        private string leadSuit;
        private int sessionGames;
        private int ourWins;
        private int theirWins;
        private int id;
        private int teamId;
        private bool allSet;
        private bool sessionStart;
        private bool processPlay;
        private bool processingRepeat;

        private IAPublisher iaPublisher;
        private RBOPlayer ai;
        private int nameId;


        public SuecaPlayer(string clientName, string charactersNames = "")
            : base(clientName, charactersNames)
        {
            nameId = Int16.Parse("" + clientName[clientName.Length - 1]);
            moveCounter = 0;
            trumpSuit = "None";
            leadSuit = "None";
            sessionGames = 0;
            ourWins = 0;
            theirWins = 0;
            id = 3; //default
            teamId = 1; //default
            allSet = false;
            sessionStart = false;
            processPlay = true;
            processingRepeat = false;

            ai = null;
            SetPublisher<IIAPublisher>();
            iaPublisher = new IAPublisher(Publisher);
        }


        public void SessionStart(int numGames, int[] agentsIds, int talkingRobot)
        {
            sessionStart = false;
            id = agentsIds[nameId - 1];
            Console.WriteLine("My id is " + id);
            teamId = 1; //default
            moveCounter = 0;
            trumpSuit = "None";
            leadSuit = "None";
            sessionGames = numGames;
            ourWins = 0;
            theirWins = 0;
            processPlay = true;
            allSet = false;
            processingRepeat = false;

            iaPublisher.ForwardSessionStart(numGames, talkingRobot);
            sessionStart = true;

        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards, int talkingRobot)
        {
            if (id == playerId)
            {
                moveCounter = 0;
                SuecaTypes.Card sharedTrumpCard = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(trumpCard);
                SuecaSolver.Rank trumpRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), sharedTrumpCard.Rank.ToString());
                SuecaSolver.Suit trumpSuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), sharedTrumpCard.Suit.ToString());
                int myTrumpCard = SuecaSolver.Card.Create(trumpRank, trumpSuit);
                this.teamId = teamId;
                List<int> initialCards = new List<int>();
                Console.Write("GameStart cards: ");
                foreach (string cardSerialized in cards)
                {
                    SuecaTypes.Card card = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(cardSerialized);
                    SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                    SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                    int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                    Console.Write(SuecaSolver.Card.ToString(myCard) + " ");
                    initialCards.Add(myCard);
                }
                Console.WriteLine("");

                ai = new RBOPlayer(playerId, initialCards, myTrumpCard, trumpCardPlayer);
                allSet = true;
                processingRepeat = false;

                iaPublisher.ForwardGameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards, talkingRobot);
            }
            
        }

        public void GameEnd(int team0Score, int team1Score, int talkingRobot)
        {
            allSet = false;
            if (teamId == 0)
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

            iaPublisher.ForwardGameEnd(team0Score, team1Score, talkingRobot);
        }

        public void SessionEnd(int team0Score, int team1Score, int talkingRobot)
        {
            iaPublisher.ForwardSessionEnd(team0Score, team1Score, talkingRobot);
        }

        public void Shuffle(int playerId, int talkingRobot)
        {
            while (!sessionStart) { }
            iaPublisher.ForwardShuffle(playerId, talkingRobot);
        }

        public void Cut(int playerId, int talkingRobot)
        {
            iaPublisher.ForwardCut(playerId, talkingRobot);
        }

        public void Deal(int playerId, int talkingRobot)
        {
            iaPublisher.ForwardDeal(playerId, talkingRobot);
        }

        public void TrumpCard(string trumpCard, int playerId, int talkingRobot)
        {
            iaPublisher.ForwardTrumpCard(trumpCard, playerId, talkingRobot);
        }

        public void ReceiveRobotCards(int playerId, int talkingRobot)
        {
            iaPublisher.ForwardReceiveRobotCards(playerId, talkingRobot);
        }

        public void NextPlayer(int id, int talkingRobot)
        {
            while (!allSet) { }
            while (processingRepeat) { }

            if (this.id == id && ai != null)
            {
                Console.WriteLine("I am thinking about what to play...");
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

                iaPublisher.Decision(cardSerialized, chosenCardRank.ToString(), chosenCardSuit.ToString(), additionalInfo);
            }

            while (!processPlay) { }
            iaPublisher.ForwardNextPlayer(id, talkingRobot);
        }

        public void TrickEnd(int winnerId, int trickPoints, int talkingRobot)
        {
            if (ai.GetHandSize() > 1)
            {
                iaPublisher.ForwardTrickEnd(winnerId, trickPoints, talkingRobot);
            }
        }

        public void Play(int id, string card, int talkingRobot)
        {
            processPlay = false;
            while (processingRepeat) { }

            if (ai != null)
            {
                iaPublisher.GazeAtTarget("cardsZone");

                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                //int localPlayerId = (id + 4 - myIdOnUnityGame) % 4;
                ai.AddPlay(id, myCard);
                Console.WriteLine("UnityPlayerID " + id + " played " + SuecaSolver.Card.ToString(myCard));

                if (moveCounter % 4 == 0)
                {
                    leadSuit = c.Suit.ToString();
                }
                moveCounter++;
            }
            
            // explain 15.0
            float desirabilityForOther, desirability = (ai.TrickExpectedReward / 15.0f) * 10.0f;
            if (desirability > 10.0f)
            {
                desirability = 10.0f;
            }
            else if (desirability < -10.0f)
            {
                desirability = -10.0f;
            }

            if (this.id == id || id == (id + 2) % 4)
            {
                desirabilityForOther = desirability;
            }
            else
            {
                desirabilityForOther = -desirability;
            }

            float totalWins = ourWins + theirWins;
            float ourWinsOfSessionRacio = ourWins / totalWins;
            float theirWinsOfSessionRacio = theirWins / totalWins;
            float ourPointsOfGameRacio = ai.PointsPercentage();
            float theirPointsOfGameRacio = 1.0f - ourPointsOfGameRacio;
            float successProbability, failureProbability;

            if (totalWins != 0)
            {
                successProbability = 0.5f * ourWinsOfSessionRacio + 0.5f * ourPointsOfGameRacio;
                failureProbability = 0.5f * theirWinsOfSessionRacio + 0.5f * theirPointsOfGameRacio;
            }
            else
            {
                successProbability = ourPointsOfGameRacio;
                failureProbability = theirPointsOfGameRacio;
            }

            iaPublisher.MoveExpectations(id, desirability.ToString(), desirabilityForOther.ToString(), successProbability.ToString(), failureProbability.ToString());
            processPlay = true;
        }


        public void Renounce(int playerId, int talkingRobot)
        {
            iaPublisher.ForwardRenounce(playerId, talkingRobot);
        }


        public void ResetTrick(int talkingRobot)
        {
            processingRepeat = true;
            //ai.ResetTrick();
            iaPublisher.ForwardResetTrick(talkingRobot);
            processingRepeat = false;
        }
    }
}
