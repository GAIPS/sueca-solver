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

            public void ForwardShuffle(int playerId)
            {
                publisher.ForwardShuffle(playerId);
            }

            public void ForwardCut(int playerId)
            {
                publisher.ForwardCut(playerId);
            }

            public void ForwardDeal(int playerId)
            {
                publisher.ForwardDeal(playerId);
            }

            public void ForwardGameEnd(int team0Score, int team1Score)
            {
                publisher.ForwardGameEnd(team0Score, team1Score);
            }

            public void ForwardGameStart(int gameId, int playerId, int teamId, string trump, string[] cards)
            {
                publisher.ForwardGameStart(gameId, playerId, teamId, trump, cards);
            }

            public void ForwardNextPlayer(int id)
            {
                publisher.ForwardNextPlayer(id);
            }

            public void ForwardTrickEnd(int winnerId, int trickPoints)
            {
                publisher.ForwardTrickEnd(winnerId, trickPoints);
            }

            public void ForwardReceiveRobotCards()
            {
                publisher.ForwardReceiveRobotCards();
            }

            public void ForwardSessionEnd(int team0Score, int team1Score)
            {
                publisher.ForwardSessionEnd(team0Score, team1Score);
            }

            public void ForwardSessionStart(int numGames)
            {
                publisher.ForwardSessionStart(numGames);
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

            public void ForwardRenounce(int playerId)
            {
                publisher.ForwardRenounce(playerId);
            }


            public void ForwardResetTrick()
            {
                publisher.ForwardResetTrick();
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
        private bool allSet;
        private bool processPlay;

        private IAPublisher iaPublisher;
        private SmartPlayer ai;


        public SuecaPlayer()
            : base("IA", "SuecaDemo")
        {
            moveCounter = 0;
            trumpSuit = "None";
            leadSuit = "None";
            sessionGames = 0;
            ourWins = 0;
            theirWins = 0;
            myIdOnUnityGame = 3; //default
            myTeamIdOnUnityGame = 1; //default
            allSet = false;
            processPlay = true;

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
            myIdOnUnityGame = 3; //default
            myTeamIdOnUnityGame = 1; //default
            allSet = false;

            iaPublisher.ForwardSessionStart(numGames);
        }

        public void GameStart(int gameId, int playerId, int teamId, string trump, string[] cards)
        {
            moveCounter = 0;
            trumpSuit = trump;
            myIdOnUnityGame = playerId;
            myTeamIdOnUnityGame = teamId;
            List<int> initialCards = new List<int>();
            Console.Write("GameStart cards: ");
            foreach (string cardSerialized in cards)
            {
                SuecaTypes.Card card = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(cardSerialized);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank) Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                Console.Write(SuecaSolver.Card.ToString(myCard) + " ");
                initialCards.Add(myCard);
            }
            Console.WriteLine("");
            SuecaSolver.Suit myTrump = (SuecaSolver.Suit) Enum.Parse(typeof(SuecaSolver.Suit), trump);

            ai = new SmartPlayer(0, initialCards, (int)myTrump, new Random(), 03129840);
            //ai = new RandomPlayer(0, initialCards, new Random());//, 03129840);
            allSet = true;

            iaPublisher.ForwardGameStart(gameId, playerId, teamId, trump, cards);
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

            iaPublisher.ForwardGameEnd(team0Score, team1Score);
        }

        public void SessionEnd(int team0Score, int team1Score)
        {
            iaPublisher.ForwardSessionEnd(team0Score, team1Score);
        }

        public void Shuffle(int playerId)
        {
            iaPublisher.ForwardShuffle(playerId);
        }

        public void Cut(int playerId)
        {
            iaPublisher.ForwardCut(playerId);
        }

        public void Deal(int playerId)
        {
            iaPublisher.ForwardDeal(playerId);
        }

        public void ReceiveRobotCards()
        {
            iaPublisher.ForwardReceiveRobotCards();
        }

        public void NextPlayer(int id)
        {
            while (!allSet) {}

            if (myIdOnUnityGame == id && ai != null)
            {
                Console.WriteLine("I am thinking about what to play...");
                int chosenCard = ai.Play();
                Console.WriteLine("UnityPlayerID " + id + " played " + SuecaSolver.Card.ToString(chosenCard));
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

                iaPublisher.Decision(cardSerialized, chosenCardRank.ToString(), chosenCardSuit.ToString(), additionalInfo);
            }

            while (!processPlay) { }
            iaPublisher.ForwardNextPlayer(id);
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            if (ai.HandSize > 1)
            {
                iaPublisher.ForwardTrickEnd(winnerId, trickPoints);
            }
        }

        public void Play(int id, string card)
        {
            processPlay = false;

            if (myIdOnUnityGame != id && ai != null)
            {
                iaPublisher.GazeAtTarget("cardsZone");

                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                int localPlayerId = (id + 4 - myIdOnUnityGame) % 4;
                ai.AddPlay(localPlayerId, myCard);
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

            if (id == myIdOnUnityGame || id == (myIdOnUnityGame + 2) % 4)
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


        public void Renounce(int playerId)
        {
            iaPublisher.ForwardRenounce(playerId);
        }


        public void ResetTrick()
        {
            ai.ResetTrick();
            iaPublisher.ForwardResetTrick();
        }
    }
}
