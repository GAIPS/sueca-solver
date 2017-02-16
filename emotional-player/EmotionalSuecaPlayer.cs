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

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions, IFMLSpeech { }

    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private ScenarioData[] _scenarios;
        private IntegratedAuthoringToolAsset _iat;
        private RolePlayCharacterAsset _rpc;
        private List<Name> _events = new List<Name>();
        private int i;
        private List<string> _tagList = new List<string>();
        private List<string> _tagMeaningsList = new List<string>();
        private string[] _tags = new string[] { };
        private string[] _tagMeanings = new string[] { };

        public ISuecaPublisher SuecaPub;
        private RBOPlayer ai;
        private int id;
        private int nameId;
        private Random randomNumberGenerator;

        public EmotionalSuecaPlayer(string clientName, string charactersNames = "") : base(clientName, charactersNames)
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
            string[] entries = System.IO.File.ReadAllLines(@"../../../Scenarios/ScenarioList.txt");

            List<ScenarioData> data = new List<ScenarioData>();

            for (int i = 0; i < entries.Length; i += 2)
            {
                var path = entries[i].Trim();
                var tts = entries[i + 1].Trim();

                data.Add(new ScenarioData(path, tts));
            }

            _scenarios = data.ToArray();
            LoadScenario(data[0]);
        }

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

        public struct ScenarioData
        {
            public readonly string ScenarioPath;
            public readonly string TTSFolder;
            private IntegratedAuthoringToolAsset _iat;

            public IntegratedAuthoringToolAsset IAT { get { return _iat; } }

            public ScenarioData(string path, string tts)
            {
                ScenarioPath = path;
                TTSFolder = tts;
                _iat = IntegratedAuthoringToolAsset.LoadFromFile("../../../Scenarios/group.iat");
            }
        }

        private void LoadScenario(ScenarioData data)
        {
            _iat = data.IAT;

            var characterSources = _iat.GetAllCharacterSources().ToList();
            foreach (var source in characterSources)
            {
                _rpc = RolePlayCharacterAsset.LoadFromFile(_iat.GetAllCharacterSources().FirstOrDefault().Source);
                _rpc.LoadAssociatedAssets();
                _iat.BindToRegistry(_rpc.DynamicPropertiesRegistry);
                // = new SocialAgentController(this, data, _rpc, _iat);
                Task.Run(() =>
                {
                    UpdateCoroutine();
                });
            }
        }

        private void AddEvent(string eventName)
        {
            //Console.WriteLine("Adding Event: " + eventName);
            _events.Add((Name)eventName);
        }

        private void UpdateCoroutine()
        {
            _events.Clear();

            while (_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Thread.Sleep(1000);
                if (_events.Count == 0)
                {
                    //Console.WriteLine("No events");
                    _rpc.Update();
                    continue;
                }
            }
        }

        private void PerceiveAndDecide()
        {
            _rpc.Perceive(_events);

            Console.WriteLine("Mood: " + _rpc.Mood);
            Console.WriteLine("Current Strongest Emotion: " + getEmotion());

            var actionRpc = _rpc.Decide().FirstOrDefault();
            _events.Clear();
            _rpc.Update();

            _rpc.SaveToFile("../../../Scenarios/Logs/log" + i + ".rpc");
            i++;

            if (actionRpc == null)
            {
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
                    ParseTags(dialog);
                    Console.WriteLine(dialog);
                    SuecaPub.PerformUtteranceWithTags("", dialog, _tags, _tagMeanings);
                    ClearLists();

                    break;
                default:
                    Console.WriteLine("Default Case");
                    break;
            }
        }

        public string getEmotion()
        {
            if (_rpc.GetStrongestActiveEmotion() != null)
            {
                return _rpc.GetStrongestActiveEmotion().EmotionType;
            }
            return "none";
        }

        private void ClearLists()
        {
            _tags.Initialize();
            _tagList.Clear();
            _tagMeanings.Initialize();
            _tagMeaningsList.Clear();
        }

        private void ParseTags(string dialog)
        {
            string pattern = @"\|\w+\|";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(dialog);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    _tagList.Add(match.Value);
                }
            }
            _tags = _tagList.ToArray();
            foreach (string tag in _tags)
            {
                switch (tag)
                {
                    case "|rank|":
                        //Console.WriteLine("Found Tag: Rank");
                        _tagMeaningsList.Add(convertRankToPortuguese(_rpc.GetBeliefValue("AgentCard(Rank)")));
                        break;
                    case "|suit|":
                        //Console.WriteLine("Found Tag: Suit");
                        _tagMeaningsList.Add(convertSuitToPortuguese(_rpc.GetBeliefValue("AgentCard(Suit)")));
                        break;
                    case "|nextPlayerId|":
                        //Console.WriteLine("Found Tag: NextPlayerID");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId|":
                        //Console.WriteLine("Found Tag: PlayerID");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId1|":
                        //Console.WriteLine("Found Tag: PlayerID1");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId2|":
                        //Console.WriteLine("Found Tag: PlayerID2");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|intensity|":
                        //Console.WriteLine("Found Tag: Intensity");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|trickPoints|":
                        //Console.WriteLine("Found Tag: Trick Points");
                        _tagMeaningsList.Add(_rpc.GetBeliefValue("TrickPoints(Board)"));
                        break;
                    default:
                        Console.WriteLine("Unknown Tag");
                        break;
                }
            }
            _tagMeanings = _tagMeaningsList.ToArray();
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

        //GAME ACTIONS
        public void Cut(int playerId)
        {
            if (playerId == 3)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Cut-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Cut-OTHER", "World").ToString());
            }
            AddEvent(EventHelper.PropertyChanged("WhoCutLast(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide();
        }

        public void Deal(int playerId)
        {
            if (playerId == 3)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Deal-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Deal-OTHER", "World").ToString());
            }
            AddEvent(EventHelper.PropertyChanged("WhoDealtLast(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide();
        }

        public void GameEnd(int team0Score, int team1Score)
        {
            if (team0Score == 120)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-QUAD_LOST", "World").ToString());
            }
            else if (team0Score > 90)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DOUBLE_LOST", "World").ToString());
            }
            else if(team0Score > 60)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-SINGLE_LOST", "World").ToString());
            }
            if (team1Score == 120)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-QUAD_WIN", "World").ToString());
            }
            else if (team1Score > 90)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DOUBLE_WIN", "World").ToString());
            }
            else if (team1Score > 60)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-SINGLE_WIN", "World").ToString());
            }
            else if(team0Score == team1Score)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-DRAW", "World").ToString());
            }
            AddEvent(EventHelper.PropertyChanged("OurTeamFinalScore(Board)", team1Score.ToString(), "World").ToString());
            AddEvent(EventHelper.PropertyChanged("TheirTeamFinalScore(Board)", team0Score.ToString(), "World").ToString());
            PerceiveAndDecide();
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
            PerceiveAndDecide();
        }

        public void NextPlayer(int id)
        {
            Console.WriteLine("The next player is {0}.", id);
            
            if (this.id == id && ai != null)
            {
                //Console.WriteLine("I am going to play...");

                int chosenCard = ai.Play();
                ai.AddPlay(id, chosenCard);

                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();

                AddEvent(EventHelper.PropertyChanged("AgentCard(Rank)", msgRank.ToString(), "World").ToString());
                AddEvent(EventHelper.PropertyChanged("AgentCard(Suit)", msgSuit.ToString(), "World").ToString());

                SuecaPub.Play(this.id, cardSerialized);
          
                string playInfo = ai.GetLastPlayInfo();
                Console.WriteLine("Robot has played {0}.", SuecaSolver.Card.ToString(chosenCard));
                Console.WriteLine("PlayInfo: " + playInfo);
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Playing-" + playInfo, "World").ToString());
                //Console.WriteLine("My play has been sent.");
            }

            if (id == 1)
            {
                //Thread.Sleep(randomNumberGenerator.Next(2000,4000));
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "NextPlayer-TEAM_PLAYER", "World").ToString());
            }
            else if (id == 0 || id == 2)
            {
                //Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "NextPlayer-OPPONENT", "World").ToString());
            }
            AddEvent(EventHelper.PropertyChanged("NextPlayerId(Board)", id.ToString(), "World").ToString());
            PerceiveAndDecide();
        }

        public void Play(int id, string card)
        {
            AddEvent(EventHelper.PropertyChanged("Current(PlayerID)", id.ToString(), "World").ToString());
            if (ai != null && id != this.id)
            {
                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);

                AddEvent(EventHelper.PropertyChanged("Current(PlayerCardRank)", myRank.ToString(), "World").ToString());
                AddEvent(EventHelper.PropertyChanged("Current(PlayerCardSuit)", mySuit.ToString(), "World").ToString());

                ai.AddPlay(id, myCard);
                Console.WriteLine("Player {0} has played {1}.", id, SuecaSolver.Card.ToString(myCard));

                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Play", "World").ToString());
                
                int currentPlayPoints = ai.GetCurrentTrickPoints();
                bool hasNewTrickWinner = ai.HasNewTrickWinner();

                AddEvent(EventHelper.PropertyChanged(Consts.TRICK_SCORE, currentPlayPoints.ToString(), id.ToString()).ToString());

                if (hasNewTrickWinner)
                {
                    int currentWinnerID = ai.GetCurrentTrickWinner();
                    AddEvent(EventHelper.PropertyChanged(Consts.TRICK_WINNER, currentWinnerID.ToString(), currentWinnerID.ToString()).ToString());
                }

                int trickIncrease = ai.GetTrickIncrease();
                AddEvent(EventHelper.PropertyChanged(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), id.ToString()).ToString());

                PerceiveAndDecide();
            }
        }

        public void ReceiveRobotCards(int playerId)
        {
            //AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "ReceiveCards-SELF", "World").ToString());
        }

        public void Renounce(int playerId)
        {
            if(playerId == 1 || playerId == 3)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-TEAM_CHEAT", "World").ToString());
            }
            else
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "GameEnd-OTHER_CHEAT", "World").ToString());
            AddEvent(EventHelper.PropertyChanged("WhoRenounced(Board)", playerId.ToString(), "World").ToString());
            PerceiveAndDecide();
        }

        public void ResetTrick()
        {
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
            if(team0Score > team1Score)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-LOST", "World").ToString());
            }
            if(team0Score < team1Score)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-WIN", "World").ToString());
            }
            if(team0Score == team1Score)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "SessionEnd-DRAW", "World").ToString());
            }
            PerceiveAndDecide();
        }

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            id = agentsIds[nameId - 1];
            Console.WriteLine("My id is " + id);
            AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY,"SessionStart-GREETING","World").ToString());
            PerceiveAndDecide();
        }

        public void Shuffle(int playerId)
        {
            if(playerId == 3)
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Shuffle-SELF", "World").ToString());
            }
            else
            {
                AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "Shuffle-OTHER", "World").ToString());
            }
            PerceiveAndDecide();
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            switch (winnerId)
            {
                case 0:
                    if(trickPoints == 0) {
                        AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 1:
                    AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-TEAM_PLAYER", "World").ToString());
                    break;
                case 2:
                    if (trickPoints == 0)
                    {
                        AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 3:
                    AddEvent(EventHelper.PropertyChanged(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-SELF", "World").ToString());
                    break;
                default:
                    Console.WriteLine("Unknown Player");
                    break;
            }

            if (trickPoints> 7.0f)
            {
                //an above average score play
                AddEvent(EventHelper.ActionEnd(winnerId.ToString(), "PositiveTrick", "Board").ToString());
            }
            if (trickPoints <= 7.0f)
            {
                //below average score play
                AddEvent(EventHelper.ActionEnd(winnerId.ToString(), "NegativeTrick", "Board").ToString());
            }

            AddEvent(EventHelper.PropertyChanged("TrickWinner(Board)", winnerId.ToString(), "World").ToString());
            PerceiveAndDecide();
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
        }
    }
}
