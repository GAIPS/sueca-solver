using System;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using System.Collections.Generic;
using EmoteCommonMessages;
using System.Threading;

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

            public void MoveExpectations(int playerId, string desirability, string desirabilityForOther, string successProbability, string failureProbability, string additionalInfo)
            {
                publisher.MoveExpectations(playerId, desirability, desirabilityForOther, successProbability, failureProbability, additionalInfo);
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

            public void ForwardTrumpCard(string trumpCard, int playerId)
            {
                publisher.ForwardTrumpCard(trumpCard, playerId);
            }

            //public void ForwardTrumpcard(string trumpCard, int playerId)
            //{
            //    publisher.ForwardTrumpcard(trumpCard, playerId);
            //}

            public void ForwardGameEnd(int team0Score, int team1Score)
            {
                publisher.ForwardGameEnd(team0Score, team1Score);
            }

            public void ForwardGameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
            {
                publisher.ForwardGameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards);
            }

            public void ForwardNextPlayer(int id)
            {
                publisher.ForwardNextPlayer(id);
            }

            public void ForwardTrickEnd(int winnerId, int trickPoints)
            {
                publisher.ForwardTrickEnd(winnerId, trickPoints);
            }

            public void ForwardReceiveRobotCards(int playerId)
            {
                publisher.ForwardReceiveRobotCards(playerId);
            }

            public void ForwardSessionEnd(int team0Score, int team1Score)
            {
                publisher.ForwardSessionEnd(team0Score, team1Score);
            }

            public void ForwardSessionStart(int numGame, int numRobots, int playerId)
            {
                publisher.ForwardSessionStart(numGame, numRobots, playerId);
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
        private int id;
        private int teamId;
        private bool allSet;
        private bool sessionStart;
        private bool processingPlay;
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
            processingPlay = false;
            processingRepeat = false;

            ai = null;
            SetPublisher<IIAPublisher>();
            iaPublisher = new IAPublisher(Publisher);
        }


        public void SessionStart(int numGames, int[] agentsIds)
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
            processingPlay = false;
            allSet = false;
            processingRepeat = false;

            iaPublisher.ForwardSessionStart(numGames, agentsIds.Length, id);
            sessionStart = true;

        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
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

                iaPublisher.ForwardGameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards);
            }
            
        }

        public void GameEnd(int team0Score, int team1Score)
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

            iaPublisher.ForwardGameEnd(team0Score, team1Score);
        }

        public void SessionEnd(int team0Score, int team1Score)
        {
            iaPublisher.ForwardSessionEnd(team0Score, team1Score);
        }

        public void Shuffle(int playerId)
        {
            while (!sessionStart) { }
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

        public void TrumpCard(string trumpCard, int playerId)
        {
            iaPublisher.ForwardTrumpCard(trumpCard, playerId);
        }

        public void ReceiveRobotCards(int playerId)
        {
            iaPublisher.ForwardReceiveRobotCards(playerId);
        }

        public void NextPlayer(int id)
        {
            while (!allSet) { }
            while (processingRepeat) { }
            while (processingPlay) { }

            iaPublisher.ForwardNextPlayer(id);

            if (this.id == id && ai != null)
            {
                int[] pastWinnerPoints = ai.GetWinnerAndPointsAndTrickNumber();

                Console.WriteLine("I am thinking about what to play...");
                int chosenCard = ai.Play();
                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();

                ai.AddPlay(id, chosenCard);
                Console.WriteLine("UnityPlayerID " + id + " played " + SuecaSolver.Card.ToString(chosenCard));

                if (moveCounter % 4 == 0)
                {
                    leadSuit = SuecaSolver.Card.GetSuit(chosenCard).ToString();
                }
                moveCounter++;

                int[] newWinnerPoints = ai.GetWinnerAndPointsAndTrickNumber();
                string additionalInfo = "";
                float desirabilityForOther = 0.0f, desirability = (Math.Min(newWinnerPoints[1], 15) / 15.0f) * 10.0f;


                if (pastWinnerPoints[0] == this.id || pastWinnerPoints[0] == (this.id + 2) % 4)
                {
                    additionalInfo += "OURS_";
                }
                else
                {
                    pastWinnerPoints[1] *= -1;
                    additionalInfo += "THEIRS_";
                }

                if (newWinnerPoints[0] == this.id || newWinnerPoints[0] == (this.id + 2) % 4)
                {
                    desirabilityForOther -= desirability;
                    additionalInfo += "OURS_";
                    if (pastWinnerPoints[0] == -1 || pastWinnerPoints[2] != newWinnerPoints[2])
                    {
                        additionalInfo = "NEW_TRICK";
                    }
                }
                else
                {
                    newWinnerPoints[1] *= -1;
                    desirabilityForOther += desirability;
                    desirability *= -1;
                    additionalInfo += "THEIRS_";
                    if (pastWinnerPoints[0] == -1 || pastWinnerPoints[2] != newWinnerPoints[2])
                    {
                        Console.WriteLine("This should never happen!");
                        additionalInfo = "NEW_TRICK";
                    }
                }

                Console.WriteLine("Past trick " + pastWinnerPoints[2] + " current trick " + newWinnerPoints[2]);
                if ((Math.Abs(newWinnerPoints[1] - pastWinnerPoints[1]) >= 10) || (Math.Abs(newWinnerPoints[1]) >= 10 && pastWinnerPoints[2] != newWinnerPoints[2]))
                {
                    additionalInfo += "HIGH";
                }
                else if ((Math.Abs(newWinnerPoints[1] - pastWinnerPoints[1]) >= 3) || (Math.Abs(newWinnerPoints[1]) >= 3 && pastWinnerPoints[2] != newWinnerPoints[2]))
                {
                    additionalInfo += "LOW";
                }

                Console.WriteLine("AdditionalInfo of my play: " + additionalInfo);
                iaPublisher.Decision(cardSerialized, chosenCardRank.ToString(), chosenCardSuit.ToString(), additionalInfo);

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

                iaPublisher.MoveExpectations(id, desirability.ToString(), desirabilityForOther.ToString(), successProbability.ToString(), failureProbability.ToString(), additionalInfo);

            }

        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            if (ai.GetHandSize() > 1)
            {
                iaPublisher.ForwardTrickEnd(winnerId, trickPoints);
            }
        }

        public void Play(int id, string card)
        {
            processingPlay = true;
            while (processingRepeat) { }

            if (ai != null && id != this.id)
            {
                int[] pastWinnerPoints = ai.GetWinnerAndPointsAndTrickNumber();
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



                int[] newWinnerPoints = ai.GetWinnerAndPointsAndTrickNumber();
                string additionalInfo = "";
                float desirabilityForOther = 0.0f, desirability = (Math.Min(newWinnerPoints[1], 15) / 15.0f) * 10.0f;

                if (pastWinnerPoints[0] == this.id || pastWinnerPoints[0] == (this.id + 2) % 4)
                {
                    additionalInfo += "OURS_";
                }
                else
                {
                    pastWinnerPoints[1] *= -1;
                    additionalInfo += "THEIRS_";
                }


                if (newWinnerPoints[0] == this.id || newWinnerPoints[0] == (this.id + 2) % 4)
                {
                    desirabilityForOther -= desirability;
                    additionalInfo += "OURS_";
                    if (pastWinnerPoints[0] == -1 || pastWinnerPoints[2] != newWinnerPoints[2])
                    {
                        additionalInfo = "OURS_OURS_";
                    }
                }
                else
                {
                    newWinnerPoints[1] *= -1;
                    desirabilityForOther += desirability;
                    desirability *= -1;
                    additionalInfo += "THEIRS_";
                    if (pastWinnerPoints[0] == -1 || pastWinnerPoints[2] != newWinnerPoints[2])
                    {
                        additionalInfo = "THEIRS_THEIRS_";
                    }
                }

                Console.WriteLine("Past trick " + pastWinnerPoints[2] + " current trick " + newWinnerPoints[2]);

                if ((Math.Abs(newWinnerPoints[1] - pastWinnerPoints[1]) >= 10 && pastWinnerPoints[2] == newWinnerPoints[2]) || (Math.Abs(newWinnerPoints[1]) >= 10 && pastWinnerPoints[2] != newWinnerPoints[2]))
                {
                    additionalInfo += "HIGH";
                }
                else if ((Math.Abs(newWinnerPoints[1] - pastWinnerPoints[1]) >= 3) || (Math.Abs(newWinnerPoints[1]) >= 3 && pastWinnerPoints[2] != newWinnerPoints[2]))
                {
                    additionalInfo += "LOW";
                }

                Console.WriteLine("AdditionalInfo of play: " + additionalInfo);

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

                iaPublisher.MoveExpectations(id, desirability.ToString(), desirabilityForOther.ToString(), successProbability.ToString(), failureProbability.ToString(), additionalInfo);
            }
            processingPlay = false;
        }


        public void Renounce(int playerId)
        {
            iaPublisher.ForwardRenounce(playerId);
        }


        public void ResetTrick()
        {
            processingRepeat = true;
            //ai.ResetTrick();
            iaPublisher.ForwardResetTrick();
            processingRepeat = false;
        }
    }
}
