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
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using System.Collections;
using AssetManagerPackage;

namespace EmotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions { }


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
                var rpc = RolePlayCharacterAsset.LoadFromFile(source.Source);
                Console.WriteLine("RPC: "+ rpc.CharacterName.ToString());
                rpc.Initialize();
                _iat.BindToRegistry(rpc.DynamicPropertiesRegistry);
                _agentController = new SocialAgentController(data, rpc, _iat);
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
        }


        private ISuecaPublisher suecaPublisher;
        private RBOPlayer ai;
        private int id;
        private int nameId;

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
            suecaPublisher = new SuecaPublisher(Publisher);
            ai = null;

            AssetManager.Instance.Bridge = new AssetManagerBridge();

            //string[] entries = System.IO.File.ReadAllLines(@"C:\Users\Filipa Correia\Devel\FAtiMA-Toolkit-UnityDemo\Assets\StreamingAssets\scenarioList.txt");
            string[] entries = System.IO.File.ReadAllLines(@"C:\Users\higino\Documents\FAtiMA-Toolkit-UnityDemo\Assets\StreamingAssets\scenarioList.txt");

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
        }

        public void Deal(int playerId)
        {
        }

        public void GameEnd(int team0Score, int team1Score)
        {
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
        {
            Console.WriteLine("The game has started.");

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
                Console.WriteLine("I am going to play...");

                int chosenCard = ai.Play();
                ai.AddPlay(id, chosenCard);

                SuecaSolver.Rank chosenCardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(chosenCard);
                SuecaSolver.Suit chosenCardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(chosenCard);
                SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), chosenCardRank.ToString());
                SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), chosenCardSuit.ToString());
                string cardSerialized = new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();
                suecaPublisher.Play(this.id, cardSerialized);
                Console.WriteLine("My play has been sent.");
            }
        }

        public void Play(int id, string card)
        {
            Console.WriteLine("Player {0} has played {1}.", id, card);

            if (ai != null && id != this.id)
            {
                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);
                ai.AddPlay(id, myCard);
            }
        }

        public void ReceiveRobotCards(int playerId)
        {
        }

        public void Renounce(int playerId)
        {
        }

        public void ResetTrick()
        {
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
        }

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            id = agentsIds[nameId - 1];
            Console.WriteLine("My id is " + id);
        }

        public void Shuffle(int playerId)
        {
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
        }
    }
}
