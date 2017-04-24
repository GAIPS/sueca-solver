using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using EmoteCommonMessages;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using AssetManagerPackage;
using WellFormedNames;
using Utilities;
using IntegratedAuthoringTool.DTOs;
using EmotionalAppraisal.OCCModel;

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions, IFMLSpeech { }

    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        public static ISuecaPublisher SuecaPub;
        private RuleBasedPlayer _ai;
        private int _id;
        private int _nameId;
        private bool _robotHasPlayed;
        private bool _initialyzing;
        private SuecaRolePlayCharacter _suecaRPC;
        private string _agentType;

        public EmotionalSuecaPlayer(string clientName, string scenarioPath, string agentType, string charactersNames = "") : base(clientName, charactersNames)
        {
            try
            {
                _nameId = Int16.Parse("" + clientName[clientName.Length - 1]);
            }
            catch (Exception)
            {
                _nameId = 1;
            }

            SetPublisher<ISuecaPublisher>();
            SuecaPub = new SuecaPublisher(Publisher);
            _ai = null;
            _suecaRPC = new SuecaRolePlayCharacter(agentType, scenarioPath);
            _initialyzing = false;
            _robotHasPlayed = false;
            _agentType = agentType;

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
        }
        #endregion

        #region Game Actions

        #region Game Actions - Before or after the game

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            _id = agentsIds[_nameId - 1];
            //Console.WriteLine("My id is " + _id);

            SuecaEvent ev = new SuecaEvent(Consts.STATE_SESSION_START);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SESSION_START, Consts.DEFAULT_SUBJECT);
            ev.Finished = true;
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
        {
            _initialyzing = true;
            SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_START);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_START, Consts.DEFAULT_SUBJECT);
            ev.Finished = true;

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

            _ai = new RuleBasedPlayer(playerId, initialCards, myTrumpCard, trumpCardPlayer);
            _initialyzing = false;
        }

        public void Shuffle(int playerId)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_SHUFFLE);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SHUFFLE, Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerId1|" }, new string[] { playerId.ToString() });
            ev.Finished = true;
        }

        public void Cut(int playerId)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_CUT);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_CUT, Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerId1|" }, new string[] { playerId.ToString() });
            ev.Finished = true;
        }

        public void Deal(int playerId)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_DEAL);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_DEAL, Consts.DEFAULT_SUBJECT);
            ev.ChangeTagsAndMeanings(new string[] { "|playerId1|" }, new string[] { playerId.ToString() });
            ev.Finished = true;
        }

        public void ReceiveRobotCards(int playerId)
        {
            //MAYBE JUST GAZE
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
            //MAYBE JUST GAZE RANDOM
        }

        public void Renounce(int playerId)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_END, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.END_GAME, "Renounce", Consts.DEFAULT_SUBJECT);
            ev.Finished = true;
        }

        public void GameEnd(int team0Score, int team1Score)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_GAME_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_GAME_END, Consts.DEFAULT_SUBJECT);

            if (team0Score == 120)
            {
                ev.AddPropertyChange(Consts.END_GAME, "LostQuad", Consts.DEFAULT_SUBJECT);
            }
            else if (team0Score > 90)
            {
                ev.AddPropertyChange(Consts.END_GAME, "LostDouble", Consts.DEFAULT_SUBJECT);
            }
            else if (team0Score > 60)
            {
                ev.AddPropertyChange(Consts.END_GAME, "LostSingle", Consts.DEFAULT_SUBJECT);
            }

            if (team1Score == 120)
            {
                ev.AddPropertyChange(Consts.END_GAME, "WinQuad", Consts.DEFAULT_SUBJECT);
            }
            else if (team1Score > 90)
            {
                ev.AddPropertyChange(Consts.END_GAME, "WinDouble", Consts.DEFAULT_SUBJECT);
            }
            else if (team1Score > 60)
            {
                ev.AddPropertyChange(Consts.END_GAME, "WinSingle", Consts.DEFAULT_SUBJECT);
            }

            if (team0Score == team1Score)
            {
                ev.AddPropertyChange(Consts.END_GAME, "Draw", Consts.DEFAULT_SUBJECT);
            }
            ev.Finished = true;
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
            SuecaEvent ev = new SuecaEvent(Consts.STATE_SESSION_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_SESSION_END, Consts.DEFAULT_SUBJECT);
            if (team0Score > team1Score)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Lost", Consts.DEFAULT_SUBJECT);
            }
            if (team0Score < team1Score)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Win", Consts.DEFAULT_SUBJECT);
            }
            if (team0Score == team1Score)
            {
                ev.AddPropertyChange(Consts.END_SESSION, "Draw", Consts.DEFAULT_SUBJECT);
            }
            ev.Finished = true;
        }

        #endregion

        #region Game Actions - During the game

        public void NextPlayer(int id)
        {
            //NextPlayer events arrive to Thalamus Client around 10miliseconds later than Play events, however this method is called first than Play
            //This sleep allows Play event to be fully processed
            Thread.Sleep(100);
            SuecaEvent ev = new SuecaEvent(Consts.STATE_NEXT_PLAYER);

            //Console.WriteLine("The next player is {0}.", id);

            //If a GameStart event has been received but not fully proccessed wait
            while (_initialyzing) { }

            if (this._id == id && _ai != null)
            {
                int chosenCard = _ai.Play();
                _ai.AddPlay(id, chosenCard);

                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();
                string playInfo = _ai.GetLastPlayInfo();
                
                SuecaPub.Play(this._id, cardSerialized, playInfo);
                ev.Name = Consts.STATE_PLAYING;
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_PLAYING, Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.PLAY_INFO, playInfo, Consts.DEFAULT_SUBJECT);
                ev.ChangeTagsAndMeanings(new string[] { "|rank|", "|suit|" }, new string[] { convertRankToPortuguese(msgRank.ToString()), convertSuitToPortuguese(msgSuit.ToString()) });


                int currentPlayPoints = _ai.GetCurrentTrickPoints();
                bool hasNewTrickWinner = _ai.HasNewTrickTeamWinner();
                bool lastPlayOfTrick = _ai.IsLastPlayOfTrick();

                ev.AddPropertyChange(Consts.TRICK_SCORE, currentPlayPoints.ToString(), subjectName(id));

                //if (hasNewTrickWinner && !lastPlayOfTrick && !robotHasPlayed)
                if (hasNewTrickWinner)
                {
                    int currentWinnerID = _ai.GetCurrentTrickWinner();
                    string lastPlayInfo = _ai.GetLastPlayInfo();
                    if (lastPlayInfo == Sueca.PLAY_INFO_NEWTRICK)
                    {
                        ev.AddPropertyChange(Consts.TRICK_WINNER, subjectName(currentWinnerID), Sueca.PLAY_INFO_NEWTRICK);
                    }
                    else
                    {
                        ev.AddPropertyChange(Consts.TRICK_WINNER, subjectName(currentWinnerID), subjectName(id));
                    }
                }
                if (!lastPlayOfTrick) { 
                    int trickIncrease = _ai.GetTrickIncrease();

                    if (trickIncrease > 0)
                    {
                        ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), subjectName(id));
                    }
                }
                _robotHasPlayed = true;
            }
            else
            {
                ev.AddPropertyChange(Consts.NEXT_PLAYER, subjectName(id), Consts.DEFAULT_SUBJECT);
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_NEXT_PLAYER, Consts.DEFAULT_SUBJECT);
                ev.ChangeTagsAndMeanings(new string[] {"|nextPlayerId|" }, new string[] { id.ToString() });
            }
            ev.Finished = true;
            _suecaRPC.AddSuecaEvent(ev);
        }

        public void Play(int id, string card, string playInfo)
        {

            SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
            SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
            SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
            int myCard = SuecaSolver.Card.Create(myRank, mySuit);

            if (_ai != null && id != this._id)
            {
                SuecaEvent ev = new SuecaEvent(Consts.STATE_PLAY);
                _suecaRPC.AddSuecaEvent(ev);
                ev.AddPropertyChange(Consts.CURRENT_PLAYER, subjectName(id), Consts.DEFAULT_SUBJECT);
                _ai.AddPlay(id, myCard);
                ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_PLAY, Consts.DEFAULT_SUBJECT);
                string[] tags = new string[] { "|rank|", "|suit|", "|playerId|" };
                string[] meanings = new string[] { convertRankToPortuguese(myRank.ToString()), convertSuitToPortuguese(mySuit.ToString()), id.ToString() };
                ev.ChangeTagsAndMeanings(tags, meanings);

                int currentPlayPoints = _ai.GetCurrentTrickPoints();
                bool hasNewTrickWinner = _ai.HasNewTrickTeamWinner();
                bool lastPlayOfTrick = _ai.IsLastPlayOfTrick();

                ev.AddPropertyChange(Consts.TRICK_SCORE, currentPlayPoints.ToString(), subjectName(id));

                //if (hasNewTrickWinner && !lastPlayOfTrick && !robotHasPlayed)
                if (hasNewTrickWinner)
                {
                    int currentWinnerID = _ai.GetCurrentTrickWinner();
                    string lastPlayInfo = _ai.GetLastPlayInfo();
                    if (lastPlayInfo == Sueca.PLAY_INFO_NEWTRICK)
                    {
                        ev.AddPropertyChange(Consts.TRICK_WINNER, subjectName(currentWinnerID), Sueca.PLAY_INFO_NEWTRICK);
                    }
                    else
                    {
                        ev.AddPropertyChange(Consts.TRICK_WINNER, subjectName(currentWinnerID), subjectName(id));
                    }

                    int trickIncrease = _ai.GetTrickIncrease();
                    if (trickIncrease > 0)
                    {
                        ev.AddPropertyChange(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), subjectName(id));
                    }
                }
                ev.Finished = true;
            }
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            _robotHasPlayed = false;

            SuecaEvent ev = new SuecaEvent(Consts.STATE_TRICK_END);
            _suecaRPC.AddSuecaEvent(ev);
            ev.AddPropertyChange(Consts.DIALOGUE_STATE_PROPERTY, Consts.STATE_TRICK_END, Consts.DEFAULT_SUBJECT);
            ev.AddPropertyChange(Consts.TRICK_WINNER, subjectName(winnerId), subjectName(winnerId));

            if (_agentType == Consts.AGENT_TYPE_GROUP)
            {
                //attribute the event always to himself
                Thread.Sleep(100);
                ev.AddPropertyChange(Consts.TRICK_END, trickPoints.ToString(), subjectName(_id));
            }
            else
            {
                //attribute the event to the winner when he is from my team and blame himself or the partner when winner is an opponent
                int resposibleForTrick = _ai.GetResposibleForLastTrick();
                Console.WriteLine("RESPOSIBLE: " + resposibleForTrick);
                ev.AddPropertyChange(Consts.TRICK_END, trickPoints.ToString(), subjectName(resposibleForTrick));
            }
            ev.ChangeTagsAndMeanings(new string[] {"|playerId|","|trickpoints|"}, new string[] {winnerId.ToString(),trickPoints.ToString()});
            ev.Finished = true;
        }

        #endregion

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

        private string subjectName(int id)
        {
            string subject = "";
            switch (_agentType)
            {
                case Consts.AGENT_TYPE_GROUP:
                    if (id == 1 || id == 3)
                        //Agent Team
                        subject = "T1";
                    if (id == 0 || id == 2)
                        //Opponent Team
                        subject = "T0";
                    break;
                case Consts.AGENT_TYPE_INDIVIDUAL:
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
                            //robot
                            subject = "P3";
                            break;
                        default:
                            Console.WriteLine("Unknown Player ID");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Agent Type is playing");
                    break;
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
