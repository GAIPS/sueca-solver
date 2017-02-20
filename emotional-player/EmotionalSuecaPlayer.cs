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
        public ISuecaPublisher SuecaPub;
        private RBOPlayer ai;
        private int id;
        private int nameId;
        private Random randomNumberGenerator;
        private string[] tags;
        private string[] meanings;

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
                Task.Run(() => { UpdateCoroutine(); });
            }
        }

        private void AddEvent(string eventName)
        {
            //Console.WriteLine("Adding Event: " + eventName);
            _events.Add((Name)eventName);
        }

        private void UpdateCoroutine()
        {
            while (_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Thread.Sleep(1000);
                _rpc.Update();
            }
        }

        private void PerceiveAndDecide(string[] tags, string[] tagMeanings)
        {         
            _rpc.Perceive(_events);

            //Console.WriteLine("Mood: " + _rpc.Mood);
            //Console.WriteLine("Current Strongest Emotion: " + _rpc.GetStrongestActiveEmotion().ToString());

            var actionRpc = _rpc.Decide().FirstOrDefault();

            _rpc.SaveToFile("../../../Scenarios/Logs/log" + i + ".rpc");
            i++;

            _events.Clear();
            _rpc.Update();

            if (actionRpc == null)
            {
                Console.WriteLine("No action");
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

                    Console.WriteLine(dialog);
                    SuecaPub.PerformUtteranceWithTags("", dialog, tags, tagMeanings);
                    tags = new string[] { };
                    tagMeanings = new string[] { };

                    break;
                default:
                    Console.WriteLine("Default Case");
                    break;
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

        //GAME ACTIONS
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
            Console.WriteLine("The next player is {0}.", id);
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

                //AddEvent(EventHelper.PropertyChange("Current(AgentCardRank)", msgRank.ToString(), "World").ToString());
                //AddEvent(EventHelper.PropertyChange("Current(AgentCardSuit)", msgSuit.ToString(), "World").ToString());

                SuecaPub.Play(this.id, cardSerialized);
          
                string playInfo = ai.GetLastPlayInfo();
                //Console.WriteLine("Robot has played {0}.", SuecaSolver.Card.ToString(chosenCard));
                //Console.WriteLine("PlayInfo: " + playInfo);
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Playing-" + playInfo, "World").ToString());
                //Console.WriteLine("My play has been sent.");
            }
            //AddEvent(EventHelper.PropertyChange("Next(PlayerId)", id.ToString(), "World").ToString());

            if (id == 1)
            {
                Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "NextPlayer-TEAM_PLAYER", "World").ToString());
            }
            else if (id == 0 || id == 2)
            {
                Thread.Sleep(randomNumberGenerator.Next(2000, 4000));
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "NextPlayer-OPPONENT", "World").ToString());
            }
            Thread.Sleep(1);
            PerceiveAndDecide(new string[] { "|rank|", "|suit|", "|nextPlayerId|", "|playerId1|", "|playerId1|" }, new string[] { convertRankToPortuguese(msgRank.ToString()), convertSuitToPortuguese(msgSuit.ToString()), id.ToString(), "0", "2" });
        }

        public void Play(int id, string card)
        {
            //AddEvent(EventHelper.PropertyChange("Current(PlayerID)", id.ToString(), "World").ToString());

            if (ai != null && id != this.id)
            {
                SuecaTypes.Card c = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
                SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), c.Rank.ToString());
                SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), c.Suit.ToString());
                int myCard = SuecaSolver.Card.Create(myRank, mySuit);

                //AddEvent(EventHelper.PropertyChange("Current(PlayerCardRank)", myRank.ToString(), "World").ToString());
                //AddEvent(EventHelper.PropertyChange("Current(PlayerCardSuit)", mySuit.ToString(), "World").ToString());

                ai.AddPlay(id, myCard);
                Console.WriteLine("Jogador {0} jogou {1}.", id, SuecaSolver.Card.ToString(myCard));

                Thread.Sleep(1);
                AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "Play", "World").ToString());
                
                int currentPlayPoints = ai.GetCurrentTrickPoints();
                bool hasNewTrickWinner = ai.HasNewTrickWinner();

                AddEvent(EventHelper.PropertyChange(Consts.TRICK_SCORE, currentPlayPoints.ToString(), id.ToString()).ToString());

                if (hasNewTrickWinner)
                {
                    int currentWinnerID = ai.GetCurrentTrickWinner();
                    AddEvent(EventHelper.PropertyChange(Consts.TRICK_WINNER, currentWinnerID.ToString(), currentWinnerID.ToString()).ToString());
                }

                int trickIncrease = ai.GetTrickIncrease();

                AddEvent(EventHelper.PropertyChange(Consts.TRICK_INCREASE_PROPERTY, trickIncrease.ToString(), id.ToString()).ToString());

                tags = new string[] { "|rank|", "|suit|", "|playerId|", "|playerId1|", "|playerId1|" };
                meanings = new string[] { convertRankToPortuguese(myRank.ToString()), convertSuitToPortuguese(mySuit.ToString()), id.ToString(), "0", "2" };

                PerceiveAndDecide(tags, meanings);
            }
        }

        public void ReceiveRobotCards(int playerId)
        {
            //AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "ReceiveCards-SELF", "World").ToString());
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
            Console.WriteLine("My id is " + id);
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
            switch (winnerId)
            {
                case 0:
                    if(trickPoints == 0) {
                        AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 1:
                    AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-TEAM_PLAYER", "World").ToString());
                    break;
                case 2:
                    if (trickPoints == 0)
                    {
                        AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT_ZERO", "World").ToString());
                    }
                    else
                        AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-OPPONENT", "World").ToString());
                    break;
                case 3:
                    AddEvent(EventHelper.PropertyChange(Consts.DIALOGUE_STATE_PROPERTY, "TrickEnd-SELF", "World").ToString());
                    break;
                default:
                    Console.WriteLine("Unknown Player");
                    break;
            }

            Thread.Sleep(randomNumberGenerator.Next(2000, 4000));

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

            //AddEvent(EventHelper.PropertyChange("TrickWinner(Board)", winnerId.ToString(), "World").ToString());

            PerceiveAndDecide(new string[] { }, new string[] { });
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
        }
    }
}
