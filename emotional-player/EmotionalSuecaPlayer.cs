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
using System.Text.RegularExpressions;
using System.IO;
using Utilities;

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions, IFMLSpeech { }

    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private IntegratedAuthoringToolAsset _iat;
        private Dictionary<string, RolePlayCharacterAsset> _rpc = new Dictionary<string, RolePlayCharacterAsset>();
        private List<Name> _events = new List<Name>();
        private int i;
        public ISuecaPublisher SuecaPub;
        private RBOPlayer ai;
        private int id;
        private int nameId;
        private Random randomNumberGenerator;
        private string[] tags;
        private string[] meanings;
        private string _agentType;
        private Object threadLock = new Object();
        private Object anotherLock = new object();
        private List<Utterance> usedUtterances = new List<Utterance>();

        public EmotionalSuecaPlayer(string clientName, string path, string type, string charactersNames = "") : base(clientName, charactersNames)
        {
            try
            {
                nameId = Int16.Parse("" + clientName[clientName.Length - 1]);
            }
            catch (Exception)
            {
                nameId = 1;
            }

            SetPublisher<ISuecaPublisher>();
            SuecaPub = new SuecaPublisher(Publisher);
            ai = null;
            randomNumberGenerator = new Random(System.Guid.NewGuid().GetHashCode());
            AssetManager.Instance.Bridge = new AssetManagerBridge();
            LoadScenario(new ScenarioData(path));
            _agentType = type;

        }

        public struct ScenarioData
        {
            public readonly string ScenarioPath;
            private IntegratedAuthoringToolAsset _iat;

            public IntegratedAuthoringToolAsset IAT { get { return _iat; } }

            public ScenarioData(string path)
            {
                ScenarioPath = path;
                _iat = IntegratedAuthoringToolAsset.LoadFromFile(ScenarioPath);
            }
        }

        private struct Utterance
        {
            public string _text { get; set; }
            public int _uses { get; set; }

            public Utterance(string text, int uses)
            {
                _text = text;
                _uses = uses;
            }

        }

        private void LoadScenario(ScenarioData data)
        {
            _iat = data.IAT;

            var characterSources = _iat.GetAllCharacterSources().ToList();
            foreach (var source in characterSources)
            {
                RolePlayCharacterAsset character = RolePlayCharacterAsset.LoadFromFile(source.Source);
                character.LoadAssociatedAssets();
                _iat.BindToRegistry(character.DynamicPropertiesRegistry);
                _rpc.Add(character.BodyName.ToString(), character);
            }
            Task.Run(() => { UpdateCoroutine(); });
        }

        #region Sueca Publisher
        private class SuecaPublisher : ISuecaPublisher
        {
            dynamic publisher;

            public SuecaPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Play(int id, string card)
            {
                this.publisher.Play(id, card);
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

        private void AddEvent(string eventName)
        {
            _events.Add((Name)eventName);
        }

        private void UpdateCoroutine()
        {
            while (_rpc[_agentType].GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                //Console.WriteLine("Updating");
                Thread.Sleep(1000);
                _rpc[_agentType].Update();
            }
        }

        private bool checkUsedUtterances(string text)
        {
            int index = usedUtterances.FindIndex(o => string.Equals(text, o._text, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
            {
                usedUtterances.Add(new Utterance(text, 3));
                return true;
            }
            else if(index > -1)
            {
                var previous = usedUtterances[index];

                if (previous._uses > 0)
                {
                    usedUtterances.Add(new Utterance(previous._text, previous._uses--));
                }
                else if (previous._uses <= 0)
                {
                    usedUtterances.RemoveAt(index);
                }
                return false;
            }
            return false;
        }

        private void PerceiveAndDecide(string[] tags, string[] tagMeanings)
        {
            lock (threadLock)
            {
                _rpc[_agentType].Perceive(_events);

                if (_rpc[_agentType].GetStrongestActiveEmotion() != null)
                {
                    Console.WriteLine("Mood: " + _rpc[_agentType].Mood);
                    Console.WriteLine("Current Strongest Emotion: " + _rpc[_agentType].GetStrongestActiveEmotion().EmotionType);
                }
                
                var actionRpc = _rpc[_agentType].Decide().Shuffle().FirstOrDefault();

                _rpc[_agentType].SaveToFile("../../../Scenarios/Logs/log" + i + ".rpc");
                i++;

                _events.Clear();
                _rpc[_agentType].Update();

                if (actionRpc == null)
                {
                    //Console.WriteLine("No action");
                    return;
                }

                switch (actionRpc.Key.ToString())
                {
                    case "Speak":
                        Name currentState = actionRpc.Parameters[0];
                        Name nextState = actionRpc.Parameters[1];
                        Name meaning = actionRpc.Parameters[2];
                        Name style = actionRpc.Parameters[3];

                        var dialog = _iat.GetDialogueAction(IATConsts.AGENT, currentState, nextState, meaning, style).Utterance;
                        //if (checkUsedUtterances(dialog))
                        //{
                            Console.WriteLine(dialog);
                            SuecaPub.PerformUtteranceWithTags("", dialog, tags, tagMeanings);
                            tags = new string[] { };
                            tagMeanings = new string[] { };
                        //}

                        break;
                    default:
                        Console.WriteLine("Default Case");
                        break;
                }
            }
        }

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
                    portugueseSuit = "ouros";
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

        #region Game Actions

        public void Cut(int playerId)
        {
            if (playerId == 3)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Cut-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Cut-OTHER", "World").ToString());
            }
            //AddEvent(EventHelper.PropertyChange("WhoCutLast(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void Deal(int playerId)
        {
            if (playerId == 3)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Deal-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Deal-OTHER", "World").ToString());
            }
            //AddEvent(EventHelper.PropertyChange("WhoDealtLast(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void GameEnd(int team0Score, int team1Score)
        {
            if (team0Score == 120)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-QUAD_LOST", "World").ToString());
            }
            else if (team0Score > 90)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DOUBLE_LOST", "World").ToString());
            }
            else if(team0Score > 60)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-SINGLE_LOST", "World").ToString());
            }
            if (team1Score == 120)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-QUAD_WIN", "World").ToString());
            }
            else if (team1Score > 90)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DOUBLE_WIN", "World").ToString());
            }
            else if (team1Score > 60)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-SINGLE_WIN", "World").ToString());
            }
            else if(team0Score == team1Score)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DRAW", "World").ToString());
            }
           // AddEvent(EventHelper.PropertyChange("OurTeamFinalScore(Board)", team1Score.ToString(), "World").ToString());
            //AddEvent(EventHelper.PropertyChange("TheirTeamFinalScore(Board)", team0Score.ToString(), "World").ToString());
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
        {
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

            ai = new RBOPlayer(playerId, initialCards, myTrumpCard, trumpCardPlayer);
        }

        public void NextPlayer(int id)
        {
            lock (anotherLock)
            {
                AddEvent(EventHelper.PropertyChange("Next(PlayerID)", checkTeam(id), "World").ToString());
                //Console.WriteLine("The next player is {0}.", id);
                SuecaTypes.Rank msgRank = new SuecaTypes.Rank();
                SuecaTypes.Suit msgSuit = new SuecaTypes.Suit();

                if (this.id == id && ai != null)
                {
                    //Console.WriteLine("I am going to play...");

                    int chosenCard = ai.Play();
                    ai.AddPlay(id, chosenCard);

                    SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                    SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                    msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                    msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                    string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();

                    SuecaPub.Play(this.id, cardSerialized);

                    string playInfo = ai.GetLastPlayInfo();
                    Console.WriteLine("Robot has played {0}.", SuecaSolver.Card.ToString(chosenCard));
                    //Console.WriteLine("PlayInfo: " + playInfo);
                    AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Playing-" + playInfo, "World").ToString());
                    //Console.WriteLine("My play has been sent.");

                    int currentPlayPoints = ai.GetCurrentTrickPoints();
                    bool hasNewTrickWinner = ai.HasNewTrickWinner();

                    AddEvent(EventHelper.PropertyChange(Consts.TRICK_SCORE, currentPlayPoints.ToString(), checkTeam(id)).ToString());

                    if (hasNewTrickWinner)
                    {
                        int currentWinnerID = ai.GetCurrentTrickWinner();
                        AddEvent(EventHelper.PropertyChange(Consts.TRICK_WINNER, checkTeam(currentWinnerID), checkTeam(currentWinnerID)).ToString());
                    }

                    int trickIncrease = ai.GetTrickIncrease();

                    if (trickIncrease > 0)
                    {
                        AddEvent(EventHelper.PropertyChange(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), checkTeam(id)).ToString());
                    }

                }

                Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "NextPlayer", "World").ToString());

                PerceiveAndDecide(new string[] { "|rank|", "|suit|", "|nextPlayerId|", "|playerId1|", "|playerId2|" }, new string[] { convertRankToPortuguese(msgRank.ToString()), convertSuitToPortuguese(msgSuit.ToString()), id.ToString(), "0", "2" });
            }
        }

        public void Play(int id, string card)
        {
            lock (anotherLock)
            {
                AddEvent(EventHelper.PropertyChange("Current(PlayerID)", checkTeam(id), "World").ToString());

                if (ai != null && id != this.id)
                {
                    SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                    SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                    SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                    int myCard = SuecaSolver.Card.Create(myRank, mySuit);

                    ai.AddPlay(id, myCard);
                    Console.WriteLine("Jogador {0} jogou {1}.", id, SuecaSolver.Card.ToString(myCard));

                    Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                    AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Play", "World").ToString());

                    int currentPlayPoints = ai.GetCurrentTrickPoints();
                    bool hasNewTrickWinner = ai.HasNewTrickWinner();

                    AddEvent(EventHelper.PropertyChange(Consts.TRICK_SCORE, currentPlayPoints.ToString(), checkTeam(id)).ToString());

                    if (hasNewTrickWinner)
                    {
                        int currentWinnerID = ai.GetCurrentTrickWinner();
                        AddEvent(EventHelper.PropertyChange(Consts.TRICK_WINNER, checkTeam(currentWinnerID), checkTeam(currentWinnerID)).ToString());
                    }

                    int trickIncrease = ai.GetTrickIncrease();

                    if (trickIncrease > 0)
                    {
                        AddEvent(EventHelper.PropertyChange(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), checkTeam(id)).ToString());
                    }

                    tags = new string[] { "|rank|", "|suit|", "|playerId|", "|playerId1|", "|playerId1|" };
                    meanings = new string[] { convertRankToPortuguese(myRank.ToString()), convertSuitToPortuguese(mySuit.ToString()), id.ToString(), "0", "2" };
                                       
                    PerceiveAndDecide(tags, meanings);
                }
            }
        }

        private string checkTeam(int id)
        {
            string subject = "";
            switch (_agentType)
            {
                case "Group":
                    if (id == 1 || id == 3)
                        //Agent Team
                        subject = "T1";
                    if (id == 0 || id == 2)
                        //Opponent Team
                        subject = "T0";
                    break;
                case "Individual":
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
                    break;
                default:
                    Console.WriteLine("Unknown Agent Type is playing");
                    break;
            }
            return subject;
        }

        public void ReceiveRobotCards(int playerId)
        {
            AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "ReceiveCards", "World").ToString());
        }

        public void Renounce(int playerId)
        {
            if(playerId == 1 || playerId == 3)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-TEAM_CHEAT", "World").ToString());
            }
            else
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-OTHER_CHEAT", "World").ToString());
            //AddEvent(EventHelper.PropertyChange("WhoRenounced(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void ResetTrick()
        {

        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
            if(team0Score > team1Score)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-LOST", "World").ToString());
            }
            if(team0Score < team1Score)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-WIN", "World").ToString());
            }
            if(team0Score == team1Score)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-DRAW", "World").ToString());
            }
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            id = agentsIds[nameId - 1];
            //Console.WriteLine("My id is " + id);
            AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY,"SessionStart","World").ToString());
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void Shuffle(int playerId)
        {
            if(playerId == 3)
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Shuffle-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Shuffle-OTHER", "World").ToString());
            }
            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            AddEvent(EventHelper.PropertyChange(Consts.TRICK_END, checkTeam(winnerId), trickPoints.ToString()).ToString());

            if (trickPoints> 20.0f)
            {
                //an above average score play
                AddEvent(EventHelper.ActionEnd(checkTeam(winnerId), "BigTrick", "Board").ToString());
            }
            if (trickPoints <= 7.0f)
            {
                //below average score play
                AddEvent(EventHelper.ActionEnd(checkTeam(winnerId), "SmallTrick", "Board").ToString());
            }

            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {

        }
        #endregion
    }
}
