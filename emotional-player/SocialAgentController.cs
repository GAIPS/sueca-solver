using System;
using System.Collections.Generic;
using RolePlayCharacter;
using IntegratedAuthoringTool;
using EmotionalDecisionMaking;
using WellFormedNames;
using System.Threading;
using Utilities;
using System.Linq;
using ActionLibrary;
using System.IO;
using System.Text.RegularExpressions;
using EmotionalAppraisal;

namespace EmotionalPlayer
{
    class SocialAgentController
    {
        private RolePlayCharacterAsset m_rpc;
        private IntegratedAuthoringToolAsset m_iat;
        private EmotionalSuecaPlayer.ScenarioData m_scenarioData;
        private List<Name> _events = new List<Name>();
        private EmotionalSuecaPlayer _esp;
        private List<string> _tagList = new List<string>();
        private List<string> _tagMeaningsList = new List<string>();
        private string[] _tags = new string[] { };
        private string[] _tagMeanings = new string[] { };
        private String emotion = "none";
        private static int i;

        public SocialAgentController(EmotionalSuecaPlayer esp, EmotionalSuecaPlayer.ScenarioData scenarioData, RolePlayCharacterAsset rpc, IntegratedAuthoringToolAsset iat)
        {
            _esp = esp;
            m_scenarioData = scenarioData;
            m_iat = iat;
            m_rpc = rpc;
        }

        public void AddEvent(string eventName)
        {
            //Console.WriteLine("Adding Event: " + eventName);
            _events.Add((Name)eventName);
        }

        public string getEmotion()
        {
            if (m_rpc.GetStrongestActiveEmotion() != null)
            {
                return m_rpc.GetStrongestActiveEmotion().EmotionType;
            }
            return "none";
        }

        public void UpdateCoroutine()
        {
            _events.Clear();

            while (m_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Thread.Sleep(1000);
                if (_events.Count == 0)
                {
                    //Console.WriteLine("No events");
                    m_rpc.Update();
                    continue;
                }

                m_rpc.Perceive(_events);

                Console.WriteLine("Mood: " + m_rpc.Mood);
                Console.WriteLine("Current Strongest Emotion: " + getEmotion());

                var actionRpc = m_rpc.Decide().FirstOrDefault();
                _events.Clear();
                m_rpc.Update();

                m_rpc.SaveToFile("../../../Scenarios/Logs/log"+i+".rpc");
                i++;

                if (actionRpc == null)
                {
                    continue;
                }
                switch (actionRpc.Key.ToString())
                {
                    case "Speak":
                        Name currentState = actionRpc.Parameters[0];
                        Name nextState = actionRpc.Parameters[1];
                        Name meaning = actionRpc.Parameters[2];
                        Name style = actionRpc.Parameters[3];
                       
                        var dialog = m_iat.GetDialogueAction(IATConsts.AGENT, currentState, nextState, meaning, style).Utterance;
                        ParseTags(dialog);
                        Console.WriteLine(dialog);
                        _esp.SuecaPub.PerformUtteranceWithTags("", dialog, _tags, _tagMeanings);
                        ClearLists();

                        //m_rpc.Perceive(new[] { EventHelper.ActionEnd(m_rpc.CharacterName.ToString(),actionRpc.Name.ToString(),IATConsts.PLAYER) });
                        break;
                    default:
                        Console.WriteLine("Default Case");
                        break;
                }
            }
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
            foreach(string tag in _tags)
            {
                switch (tag)
                {
                    case "|rank|":
                        //Console.WriteLine("Found Tag: Rank");
                        _tagMeaningsList.Add(convertRankToPortuguese(m_rpc.GetBeliefValue("AgentCard(Rank)")));
                        break;
                    case "|suit|":
                        //Console.WriteLine("Found Tag: Suit");
                        _tagMeaningsList.Add(convertSuitToPortuguese(m_rpc.GetBeliefValue("AgentCard(Suit)")));
                        break;
                    case "|nextPlayerId|":
                        //Console.WriteLine("Found Tag: NextPlayerID");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId|":
                        //Console.WriteLine("Found Tag: PlayerID");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId1|":
                        //Console.WriteLine("Found Tag: PlayerID1");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId2|":
                        //Console.WriteLine("Found Tag: PlayerID2");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|intensity|":
                        //Console.WriteLine("Found Tag: Intensity");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|trickPoints|":
                        //Console.WriteLine("Found Tag: Trick Points");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("TrickPoints(Board)"));
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
    }
}
