using System;
using System.Threading;
using System.Collections.Generic;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using EmoteCommonMessages;
using System.Diagnostics;
using Thalamus.BML;

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions, IFMLSpeech, IAnimationActions, Thalamus.BML.IPostureActions, EmoteCommonMessages.IGazeStateActions, IRobotPerceptions { }

    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions, IRobotPerceptions, IFMLSpeechEvents
    {
        public static ISuecaPublisher SuecaPub;
        private TIPlayer _ai;
        public int _id;
        private int _teamId;
        private int _nameId;
        private string _partnerName;
        private bool _initialyzing;
        private SuecaRolePlayCharacter _suecaRPC;
        private string _socialAgentType;
        private string _playAgentType;
        private int _numGames;
        private int _currentGameId;
        private int _currentTrickId;
        private int _currentPlayInTrickId;

        private Random _randomNumberGenerator;
        private bool PendingRequest;
        public bool Talking;
        public bool SomeoneIsTalking;
        private string pendingCategory;
        private string pendingSubcategory;
        private int requestCounter;
        private bool Retrying;
        private int numRobots;

        public EmotionalSuecaPlayer(string charactersNames, string clientName, string scenarioPath, string socialAgent, string playAgent) : base(clientName, charactersNames)
        {
            try
            {
                _nameId = Int16.Parse("" + clientName[clientName.Length - 1]);
            }
            catch (Exception)
            {
                _nameId = 1;
            }


            _randomNumberGenerator = new Random(System.Guid.NewGuid().GetHashCode());
            SetPublisher<ISuecaPublisher>();
            SuecaPub = new SuecaPublisher(Publisher);
            _ai = null;
            _suecaRPC = new SuecaRolePlayCharacter(_nameId, socialAgent, scenarioPath, this);
            _initialyzing = false;
            _socialAgentType = socialAgent;
            _playAgentType = playAgent;
            numRobots = 1; //default
        }


        #region Sueca Publisher
        private class SuecaPublisher : ISuecaPublisher
        {
            dynamic publisher;

            public SuecaPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Play(int id, string card, string playInfo)
            {
                this.publisher.Play(id, card, playInfo);
            }

            public void CancelUtterance(string id)
            {
                this.publisher.CancelUtterance(id);
            }

            public void PerformUtterance(string id, string utterance, string category)
            {
                this.publisher.PerformUtterance(id, utterance, category);
            }

            public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
            {
                this.publisher.PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
            }

            public void PerformUtteranceWithTags(string id, string utterance, string[] tagNames, string[] tagValues)
            {
                this.publisher.PerformUtteranceWithTags(id, utterance, tagNames, tagValues);
            }

            public void RequestUtterance(int playerId, string category, string subcategory)
            {
                this.publisher.RequestUtterance(playerId, category, subcategory);
            }

            public void OKUtterance(int playerId)
            {
                this.publisher.OKUtterance(playerId);
            }

            public void NOUtterance(int playerId)
            {
                this.publisher.NOUtterance(playerId);
            }

            public void StartedUtterance(int playerId, string category, string subcategory)
            {
                this.publisher.StartedUtterance(playerId, category, subcategory);
            }

            public void FinishedUtterance(int playerId)
            {
                this.publisher.FinishedUtterance(playerId);
            }

            public void PlayAnimation(string id, string animation)
            {
                this.publisher.PlayAnimation(id, animation);
            }

            public void PlayAnimationQueued(string id, string animation)
            {
                this.publisher.PlayAnimationQueued(id, animation);
            }

            public void StopAnimation(string id)
            {
                this.publisher.StopAnimation(id);
            }

            public void SetPosture(string id, string posture, double percent = 1, double decay = 1)
            {
                this.publisher.SetPosture(id, posture, percent, decay);
            }

            public void ResetPose()
            {
                this.publisher.ResetPose();
            }

            public void GazeAtScreen(double x, double y)
            {
                this.publisher.GazeAtScreen(x, y);
            }

            public void GlanceAtScreen(double x, double y)
            {
                this.publisher.GlanceAtScreen(x, y);
            }

            public void GazeAtTarget(string targetName)
            {
                this.publisher.GazeAtTarget(targetName);
            }

            public void GlanceAtTarget(string targetName)
            {
                this.publisher.GlanceAtTarget(targetName);
            }
        }
        #endregion

        #region Game Actions

        #region Game Actions - Before or after the game

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int floorId)
        {
            _id = agentsIds[_nameId - 1];
            Console.WriteLine("EMYS-" + _nameId + "--- " + "My id is " + _id);
            if (_id == 0 || _id == 2)
            {
                _teamId = 0;
            }
            else
            {
                _teamId = 1;
            }
            _numGames = numGames;
            _currentGameId = 0;
            numRobots = agentsIds.Length;

            SuecaEvent ev1 = new SuecaEvent(Consts.INIT);
            _suecaRPC.AddSuecaEvent(ev1);
            ev1.OtherStringInfos = new string[] { SubjectName(_id) };
            
            //if (_socialAgentType.StartsWith(Consts.AGENT_TYPE_GROUP))
            {
                ev1.AddPropertyChange("Player(" + SubjectName(_id) + ")", Consts.PARTNER, Consts.DEFAULT_SUBJECT);
                ev1.AddPropertyChange("Player(" + SubjectName((_id + 1) % 4) + ")", Consts.OPPONENT, Consts.DEFAULT_SUBJECT);
            }
            /*else if (_socialAgentType.StartsWith(Consts.AGENT_TYPE_INDIVIDUAL))
            {
                ev1.AddPropertyChange("Player(" + SubjectName(_id) + ")", Consts.PARTNER, Consts.DEFAULT_SUBJECT);
                ev1.AddPropertyChange("Player(" + SubjectName((_id + 1) % 4) + ")", Consts.OPPONENT, Consts.DEFAULT_SUBJECT);
                ev1.AddPropertyChange("Player(" + SubjectName((_id + 2) % 4) + ")", Consts.PARTNER, Consts.DEFAULT_SUBJECT);
                ev1.AddPropertyChange("Player(" + SubjectName((_id + 3) % 4) + ")", Consts.OPPONENT, Consts.DEFAULT_SUBJECT);
            }*/

            if (_nameId == 1)
            {
                ev1.AddPropertyChange("Dialogue(Style)", "A", Consts.DEFAULT_SUBJECT);
            }
            else
            {
                ev1.AddPropertyChange("Dialogue(Style)", "B", Consts.DEFAULT_SUBJECT);
            }
            ev1.AddPropertyChange(Consts.ID_PROPERTY, _id.ToString(), Consts.DEFAULT_SUBJECT);
            ev1.Finished = true;

            int playerID1 = _randomNumberGenerator.Next(1, 2);
            int playerID2 = ((playerID1 + 1) % 2);
            SuecaPub.GazeAtTarget("player" + playerID1);


            SuecaEvent ev2 = new SuecaEvent(Consts.STATE_SESSION_START);
            _suecaRPC.AddSuecaEvent(ev2);
            ev2.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SESSION_START, Consts.DEFAULT_SUBJECT);
            ev2.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            ev2.ChangeTagsAndMeanings(new string[] { "|playerID1|", "|playerID2|", "|partnerName|" }, new string[] { playerID1.ToString(), playerID2.ToString(), _partnerName });
            ev2.Finished = true;
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards, string partnerName, int floorId)
        {
            int playerID1 = _randomNumberGenerator.Next(1, 2);
            int playerID2 = ((playerID1 + 1) % 2);
            SuecaPub.GazeAtTarget("player" + playerID1);
            if (playerId == _id)
            {
                _initialyzing = true;

                _teamId = teamId;
                _currentGameId = gameId;
                _currentTrickId = 0;
                _currentPlayInTrickId = 0;
                _partnerName = partnerName;

                if (gameId != 0)
                {
                    //Console.WriteLine("next games");
                    SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_START);
                    _suecaRPC.AddSuecaEvent(ev);
                    ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_START, Consts.DEFAULT_SUBJECT);
                    ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
                    ev.ChangeTagsAndMeanings(new string[] { "|playerID1|", "|playerID2|", "|partnerName|" }, new string[] { playerID1.ToString(), playerID2.ToString(), _partnerName });
                    ev.Finished = true;
                }

                List<int> initialCards = new List<int>();
                foreach (string cardSerialized in cards)
                {
                    SuecaTypes.Card card = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(cardSerialized);
                    SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), card.Rank.ToString());
                    SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), card.Suit.ToString());
                    int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                    initialCards.Add(myCard);
                }
                SuecaTypes.Card sharedTrumpCard = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(trumpCard);
                SuecaSolver.Rank trumpRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), sharedTrumpCard.Rank.ToString());
                SuecaSolver.Suit trumpSuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), sharedTrumpCard.Suit.ToString());
                int myTrumpCard = SuecaSolver.Card.Create(trumpRank, trumpSuit);


                if (_playAgentType == "Worst")
                {
                    _ai = new WorstPlayer(playerId, initialCards, myTrumpCard, trumpCardPlayer);
                }
                else if (_playAgentType == "Best")
                {
                    _ai = new RBOPlayer(playerId, initialCards, myTrumpCard, trumpCardPlayer);
                }
                _initialyzing = false;
            }
        }

        public void Shuffle(int playerId, int floorId)
        {
            Thread.Sleep(500);
            SuecaPub.GazeAtTarget("player" + playerId);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_SHUFFLE);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SHUFFLE, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|partnerName|" }, new string[] { playerId.ToString(), _partnerName });
            ev.Finished = true;
            //if robot has floor speaks, otherwise just gazes random
            SuecaPub.GazeAtTarget("random");
        }

        public void Cut(int playerId, int floorId)
        {
            SuecaPub.GazeAtTarget("player" + playerId);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_CUT);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_CUT, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|partnerName|" }, new string[] { playerId.ToString(), _partnerName });
            ev.Finished = true;
        }

        public void Deal(int playerId, int floorId)
        {
            SuecaPub.GazeAtTarget("player" + playerId);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_DEAL);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_DEAL, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|partnerName|" }, new string[] { playerId.ToString(), _partnerName });
            ev.Finished = true;
            //if robot has floor speaks, otherwise just gazes random
            SuecaPub.GazeAtTarget("random");
        }

        public void ReceiveRobotCards(int playerId, int floorId)
        {
            SuecaPub.GazeAtTarget("random");
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer, int floorId)
        {
            SuecaPub.GazeAtTarget("cardsZone");
        }

        public void Renounce(int playerId, int floorId)
        {
            SuecaPub.GazeAtTarget("player" + playerId);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_END, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.END_GAME, "Renounce", Consts.DEFAULT_SUBJECT);
            ev.Finished = true;
        }

        public void GameEnd(int team0Score, int team1Score, int floorId)
        {
            int playerId = ((_id + 2) % 4); //partner
            SuecaPub.GazeAtTarget("player" + playerId);
            //do not talk for the last game end event
            if (_currentGameId != _numGames - 1)
            {
                SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_END);
                _suecaRPC.AddSuecaEvent(ev);
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_END, Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);

                int myTeamScore, otherTeamScore;
                if (_teamId == 0)
                {
                    myTeamScore = team0Score;
                    otherTeamScore = team1Score;
                }
                else
                {
                    myTeamScore = team1Score;
                    otherTeamScore = team0Score;
                }

                if (otherTeamScore == 120)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "LostQuad", SubjectName(_id));
                }
                else if (otherTeamScore > 90)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "LostDouble", SubjectName(_id));
                }
                else if (otherTeamScore > 60)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "LostSingle", SubjectName(_id));
                }

                if (myTeamScore == 120)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "WinQuad", SubjectName(_id));
                }
                else if (myTeamScore > 90)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "WinDouble", SubjectName(_id));
                }
                else if (myTeamScore > 60)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "WinSingle", SubjectName(_id));
                }

                if (myTeamScore == otherTeamScore)
                {
                    ev.AddPropertyChange(Consts.END_GAME, "Tie", SubjectName(_id));
                }
                ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|partnerName|" }, new string[] { playerId.ToString(), _partnerName });
                ev.Finished = true;
            }
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score, int floorId)
        {
            int playerId = ((_id + 2) % 4); //partner
            SuecaPub.GazeAtTarget("player" + playerId);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_SESSION_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SESSION_END, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);

            int myTeamScore, otherTeamScore;
            if (_teamId == 0)
            {
                myTeamScore = team0Score;
                otherTeamScore = team1Score;
            }
            else
            {
                myTeamScore = team1Score;
                otherTeamScore = team0Score;
            }

            if (otherTeamScore > myTeamScore)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Lost", SubjectName(_id));
            }
            if (otherTeamScore < myTeamScore)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Win", SubjectName(_id));
            }
            if (otherTeamScore == myTeamScore)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Tie", SubjectName(_id));
            }
            ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|partnerName|" }, new string[] { playerId.ToString(), _partnerName });
            ev.Finished = true;
        }

        #endregion

        #region Game Actions - During the game

        public void NextPlayer(int id, int floorId)
        {
            //NextPlayer events arrive to Thalamus Client around 10miliseconds later than Play events, however this method is called first than Play
            //This sleep allows Play event to be fully processed before the next player
            SuecaEvent ev = new SuecaEvent(Consts.STATE_NEXT_PLAYER);
            _suecaRPC.AddSuecaEvent(ev);
            Thread.Sleep(500);

            //Console.WriteLine("The next player is {0}.", id);

            //If a GameStart event has been received but not fully proccessed wait
            while (_initialyzing) { }

            if (this._id == id && _ai != null)
            {
                SuecaPub.GazeAtTarget("cards" + _id);
                int chosenCard = _ai.Play();
                _ai.AddPlay(id, chosenCard);

                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();
                string playInfo = _ai.GetLastPlayInfo();

                //ev.AddPropertyChange(Consts.PLAY_INFO, playInfo, Consts.DEFAULT_SUBJECT);


                ev.ChangeTagsAndMeanings(new string[] { "|rank|", "|suit|", "|playerID|", "|nextPlayerID|", "|partnerName|" }, new string[] { convertRankToPortuguese(msgRank.ToString()), convertSuitToPortuguese(msgSuit.ToString()), ((id+2)%4).ToString(), _ai.GetNextPlayerId().ToString(), _partnerName });
                ev.OtherIntInfos = new int[] { this._id };
                ev.OtherStringInfos = new string[] { cardSerialized, playInfo };

                if (_ai.GetTrickIncrease() > 0 || _ai.HasNewTrickTeamWinner())
                {
                    ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, "True", SubjectName(id));
                }
                else
                {
                    ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, "False", SubjectName(id));
                }

                int trickScore = _ai.GetZeroSumTrickScore();
                Console.WriteLine("EMYS" + _nameId + " the current trickScore is: " + trickScore);
                ev.AddPropertyChange(Consts.TRICK_SCORE, trickScore.ToString(), SubjectName(id));

                ev.Name = Consts.STATE_PLAYSELF;
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_PLAYSELF, Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, _id.ToString(), Consts.DEFAULT_SUBJECT);

            }
            else
            {
                ev.AddPropertyChange(Consts.NEXT_PLAYER, SubjectName(id), Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_NEXT_PLAYER, Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
                ev.ChangeTagsAndMeanings(new string[] { "|nextPlayerID|", "|partnerName|" }, new string[] { id.ToString(), _partnerName });
                ev.OtherIntInfos = new int[] { id };
            }
            ev.Finished = true;
            Console.WriteLine("FINISHED");
        }

        public void Play(int id, string card, string playInfo, int floorId)
        {
            SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
            SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
            SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
            int myCard = SuecaSolver.Card.Create(myRank, mySuit);

            if (_ai != null && id != this._id)
            {
                SuecaPub.GazeAtTarget("player" + id);

                SuecaEvent ev = new SuecaEvent(Consts.STATE_PLAYPARTNER);
                _suecaRPC.AddSuecaEvent(ev);
                ev.AddPropertyChange(Consts.CURRENT_PLAYER, SubjectName(id), Consts.DEFAULT_SUBJECT);
                _ai.AddPlay(id, myCard);
                string[] tags = new string[] { "|rank|", "|suit|", "|playerID|", "|nextPlayerID|", "|partnerName|" };
                string[] meanings = new string[] { convertRankToPortuguese(myRank.ToString()), convertSuitToPortuguese(mySuit.ToString()), id.ToString(), _ai.GetNextPlayerId().ToString(), _partnerName };
                ev.ChangeTagsAndMeanings(tags, meanings);

                SuecaPub.GazeAtTarget("cardsZone");

                if (_ai.GetTrickIncrease() > 0 || _ai.HasNewTrickTeamWinner())
                {
                    ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, "True", SubjectName(id));
                }
                else
                {
                    ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, "False", SubjectName(id));
                }

                int trickScore = _ai.GetZeroSumTrickScore();
                Console.WriteLine("EMYS" + _nameId + " the current trickScore is: " + trickScore);
                ev.AddPropertyChange(Consts.TRICK_SCORE, trickScore.ToString(), SubjectName(id));


                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_PLAYPARTNER, Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
                ev.OtherIntInfos = new int[] { id };
                ev.Finished = true;
                _currentPlayInTrickId++;
            }
        }

        public void TrickEnd(int winnerId, int trickPoints, int floorId)
        {
            int partnerID = ((_id + 2) % 4);
            _currentTrickId++;
            _currentPlayInTrickId = 0;

            
            SuecaEvent ev = new SuecaEvent(Consts.STATE_TRICK_END);
            //int currentPlayPoints = _ai.GetCurrentTrickPoints();
            //int currentWinnerID = _ai.GetCurrentTrickWinner();
            //bool isOurs = false;

            //if (this._id == currentWinnerID || ((this._id + 2) % 4 == currentWinnerID))
            //    isOurs = true;
            //else
            //    isOurs = false;
            //var trickScore = currentPlayPoints * (isOurs ? 1 : -1);
            //if (trickScore >= 9 || trickScore <= -9)
            //{
            //    ev.AddPropertyChange(Consts.TRICK_SCORE, trickScore.ToString(), SubjectName(currentWinnerID));
            //}
            _suecaRPC.AddSuecaEvent(ev);
            //ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_TRICK_END, Consts.DEFAULT_SUBJECT);
            //ev.AddPropertyChange(Consts.DIALOGUE_FLOOR_PROPERTY, floorId.ToString(), Consts.DEFAULT_SUBJECT);
            //ev.ChangeTagsAndMeanings(new string[] { "|playerID|", "|trickpoints|" }, new string[] { partnerID.ToString(), trickPoints.ToString() });
            ev.Finished = true;
        }

        #endregion

        #endregion

        #region SynchronizeBothAgents

        public void RequestUtterance(int playerId, string category, string subcategory)
        {
            if (playerId != _id)
            {
                if (PendingRequest || Talking)
                {
                    SuecaPub.NOUtterance(_id);
                }
                else
                {
                    SuecaPub.OKUtterance(_id);
                    SomeoneIsTalking = true;
                }
            }
        }


        public void OKUtterance(int playerId)
        {
            if (playerId != _id)
            {
                Talking = true;
                PendingRequest = false;
                Retrying = false;
                requestCounter = 0;
            }
        }


        public void NOUtterance(int playerId)
        {
            if (playerId != _id)
            {
                if (PendingRequest && requestCounter < 3)
                {
                    Retrying = true;
                    PendingRequest = false;
                    Thread.Sleep(_randomNumberGenerator.Next(2000));
                    retryRequest();
                }
                else
                {
                    requestCounter = 0;
                    Retrying = false;
                    PendingRequest = false;
                }
            }
        }


        public void StartedUtterance(int playerId, string category, string subcategory)
        {
            if (playerId != _id)
            {
                SomeoneIsTalking = true;
            }
            else
            {
                Talking = true;
            }
        }


        public void FinishedUtterance(int playerId)
        {
            if (playerId != _id)
            {
                SomeoneIsTalking = false;
            }
        }

        public void RequestUtterance(string category, string subcategory)
        {
            if (numRobots > 1)
            {
                PendingRequest = true;
                pendingCategory = category;
                pendingSubcategory = subcategory;
                requestCounter++;
                SuecaPub.RequestUtterance(_id, category, subcategory);
            }
            else
            {
                Talking = true;
            }

        }

        private void retryRequest()
        {
            requestCounter++;
            RequestUtterance(pendingCategory, pendingSubcategory);
        }

        public void WaitForResponse()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            while (PendingRequest || Retrying || SomeoneIsTalking)
            {
                if (s.ElapsedMilliseconds > 3000)
                {
                    PendingRequest = false;
                    Retrying = false;
                    pendingCategory = "";
                    pendingSubcategory = "";
                    requestCounter = 0;
                    s.Stop();
                }
            }
        }


        void IFMLSpeechEvents.UtteranceFinished(string id)
        {
            SuecaPub.FinishedUtterance(_id);
            Talking = false;
        }

        void IFMLSpeechEvents.UtteranceStarted(string id)
        {
            Talking = true;
        }

        #endregion

        #region Auxiliary Methods

        private string convertRankToPortuguese(string englishRank)
        {
            string portugueseRank = "";
            switch (englishRank)
            {
                case "Two":
                    portugueseRank = "um dois";
                    break;
                case "Three":
                    portugueseRank = "um três";
                    break;
                case "Four":
                    portugueseRank = "um quatro";
                    break;
                case "Five":
                    portugueseRank = "um cinco";
                    break;
                case "Six":
                    portugueseRank = "um seis";
                    break;
                case "Queen":
                    portugueseRank = "uma dama";
                    break;
                case "Jack":
                    portugueseRank = "um váléte";
                    break;
                case "King":
                    portugueseRank = "um rei";
                    break;
                case "Seven":
                    portugueseRank = "uma manilha";
                    break;
                case "Ace":
                    portugueseRank = "um ás";
                    break;
                default:
                    break;
            }
            return portugueseRank;
        }

        private string convertSuitToPortuguese(string englishSuit)
        {
            string portugueseSuit = "";
            switch (englishSuit)
            {
                case "Clubs":
                    portugueseSuit = "paus";
                    break;
                case "Diamonds":
                    portugueseSuit = "oúros";
                    break;
                case "Hearts":
                    portugueseSuit = "copas";
                    break;
                case "Spades":
                    portugueseSuit = "espadas";
                    break;
                default:
                    break;
            }
            return portugueseSuit;
        }

        public string SubjectName(int id)
        {
            string subject = "";

            if (_socialAgentType.StartsWith(Consts.AGENT_TYPE_INDIVIDUAL))
            {
                switch (id)
                {
                    case 0:
                        subject = "P0";
                        break;
                    case 1:
                        subject = "P1";
                        break;
                    case 2:
                        subject = "P2";
                        break;
                    case 3:
                        subject = "P3";
                        break;
                    default:
                        Console.WriteLine("Unknown Player ID");
                        break;
                }
            }
            else
            {
                string[] teams = new string[] { "T0", "T1" };
                if (id == _id || id == ((_id + 2) % 4))
                {
                    subject = teams[_teamId];
                }
                else
                {
                    subject = teams[(_teamId + 1) % 2];
                }
            }
            return subject;
        }

        private bool isPartner(int id)
        {
            return id == 1;
        }
        #endregion
    }
}
