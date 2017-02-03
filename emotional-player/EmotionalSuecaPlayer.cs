using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;
using SuecaSolver;
using SuecaMessages;
using SuecaTypes;
using EmoteCommonMessages;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using System.Collections;
using AssetManagerPackage;

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions, IFMLSpeech { }


    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private ScenarioData[] m_scenarios;
        private IntegratedAuthoringToolAsset _iat;
        private SocialAgentController _agentController;

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
                //Console.WriteLine(Directory.GetCurrentDirectory());
                _iat = IntegratedAuthoringToolAsset.LoadFromFile("../../../Scenarios/group.iat");
            }
        }

        private void LoadScenario(ScenarioData data)
        {
            _iat = data.IAT;

            var characterSources = _iat.GetAllCharacterSources().ToList();
            foreach (var source in characterSources)
            {
                var rpc = RolePlayCharacterAsset.LoadFromFile(_iat.GetAllCharacterSources().FirstOrDefault().Source);
                rpc.LoadAssociatedAssets();
                _iat.BindToRegistry(rpc.DynamicPropertiesRegistry);
                _agentController = new SocialAgentController(this, data, rpc, _iat);
                //_agentController.Start(this, VersionMenu);
                Task.Run(() =>
                {
                    _agentController.UpdateCoroutine();
                });
            }
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


        public ISuecaPublisher SuecaPub;
        private RBOPlayer ai;
        private int id;
        private int nameId;
        private Random randomNumberGenerator;

        public EmotionalSuecaPlayer(string clientName, string charactersNames = "")
            : base(clientName, charactersNames)
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

            //string[] entries = System.IO.File.ReadAllLines(@"C:\Users\Filipa Correia\Devel\FAtiMA-Toolkit-UnityDemo\Assets\StreamingAssets\scenarioList.txt");
            string[] entries = System.IO.File.ReadAllLines(@"../../../Scenarios/ScenarioList.txt");

            //var entries = www.text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

            List<ScenarioData> data = new List<ScenarioData>();

            for (int i = 0; i < entries.Length; i += 2)
            {
                var path = entries[i].Trim();
                var tts = entries[i + 1].Trim();

                data.Add(new ScenarioData(path, tts));
            }

            m_scenarios = data.ToArray();
            LoadScenario(data[0]);
        }

        public void Cut(int playerId)
        {
            if (playerId == 3)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Cut-SELF", "World").ToString());
            }
            else
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Cut-OTHER", "World").ToString());
            }
        }

        public void Deal(int playerId)
        {
            if (playerId == 3)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Deal-SELF", "World").ToString());
            }
            else
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Deal-OTHER", "World").ToString());
            }
        }

        public void GameEnd(int team0Score, int team1Score)
        {
            if (team0Score == 120)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-QUAD_LOST", "World").ToString());
            }
            else if (team0Score > 90)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-DOUBLE_LOST", "World").ToString());
            }
            else if(team0Score > 60)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-SINGLE_LOST", "World").ToString());
            }
            if (team1Score == 120)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-QUAD_WIN", "World").ToString());
            }
            else if (team1Score > 90)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-DOUBLE_WIN", "World").ToString());
            }
            else if (team1Score > 60)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-SINGLE_WIN", "World").ToString());
            }
            else if(team0Score == team1Score)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-DRAW", "World").ToString());
            }
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
        {
            //Console.WriteLine("The game has started.");
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
                SuecaPub.Play(this.id, cardSerialized);

                string playInfo = ai.GetLastPlayInfo();
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Playing-" + playInfo, "World").ToString());
                //Console.WriteLine("My play has been sent.");
            }

            if (id == 1)
            {
                Thread.Sleep(randomNumberGenerator.Next(2000,4000));
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "NextPlayer-TEAM_PLAYER", "World").ToString());
            }
            else if (id == 0 || id == 2)
            {
                Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "NextPlayer-OPPONENT", "World").ToString());
            }
        }

        public void Play(int id, string card)
        {

            if (ai != null && id != this.id)
            {
                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                ai.AddPlay(id, myCard);
                Console.WriteLine("Player {0} has played {1}.", id, SuecaSolver.Card.ToString(myCard));

                int[] newWinnerPoints = ai.GetWinnerAndPointsAndTrickNumber();
                float desirabilityForOther = 0.0f, desirability = (Math.Min(newWinnerPoints[1], 15) / 15.0f) * 10.0f;
                if (newWinnerPoints[0] == this.id || newWinnerPoints[0] == (this.id + 2) % 4)
                {
                    desirabilityForOther -= desirability;
                }
                else
                {
                    newWinnerPoints[1] *= -1;
                    desirabilityForOther += desirability;
                    desirability *= -1;
                }

                //A PARTIR DAQUI ERA A GERACAO DA EMOCAO PELO FATIMA ANTIGO
                string emotion = "NEUTRAL";
                if (desirability > 0 && desirabilityForOther > 0)
                {
                    emotion = "HAPPY_FOR";
                }
                else if (desirability > 0 && desirabilityForOther < 0)
                {
                    emotion = "GLOATING";
                }
                else if (desirability < 0 && desirabilityForOther > 0)
                {
                    emotion = "RESENTMENT";
                }
                else if (desirability < 0 && desirabilityForOther < 0)
                {
                    emotion = "PITTY";
                }
                Console.WriteLine("Emotion: " + emotion);
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Play-" + emotion, "World").ToString());
            }
        }

        public void ReceiveRobotCards(int playerId)
        {
            //_agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "ReceiveCards-SELF", "World").ToString());
        }

        public void Renounce(int playerId)
        {
            if(playerId == 1 || playerId == 3)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-TEAM_CHEAT", "World").ToString());
            }
            else
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "GameEnd-OTHER_CHEAT", "World").ToString());
        }

        public void ResetTrick()
        {
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
            if(team0Score > team1Score)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "SessionEnd-LOST", "World").ToString());
            }
            if(team0Score < team1Score)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "SessionEnd-WIN", "World").ToString());
            }
            if(team0Score == team1Score)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "SessionEnd-DRAW", "World").ToString());
            }
        }

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            id = agentsIds[nameId - 1];
            Console.WriteLine("My id is " + id);
            _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)","SessionStart-GREETING","World").ToString());
        }

        public void Shuffle(int playerId)
        {
            if(playerId == 3)
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Shuffle-SELF", "World").ToString());
            }
            else
            {
                _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "Shuffle-OTHER", "World").ToString());
            }
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            switch (winnerId)
            {
                case 0:
                    if(trickPoints == 0) {
                        _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 1:
                    _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-TEAM_PLAYER", "World").ToString());
                    break;
                case 2:
                    if (trickPoints == 0)
                    {
                        _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 3:
                    _agentController.AddEvent(EventHelper.PropertyChanged("DialogueState(Board)", "TrickEnd-SELF", "World").ToString());
                    break;
                default:
                    Console.WriteLine("Unknown Player");
                    break;
            }
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
        }
    }
}
