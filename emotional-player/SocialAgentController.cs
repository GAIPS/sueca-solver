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

        public void UpdateCoroutine()
        {
            _events.Clear();

            while (m_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Thread.Sleep(500);
                if (_events.Count == 0)
                {
                    //Console.WriteLine("No events");
                    m_rpc.Update();
                    continue;
                }

                m_rpc.Perceive(_events);
                var actionRpc = m_rpc.Decide().FirstOrDefault();
                _events.Clear();
                m_rpc.Update();

                m_rpc.SaveToFile("../../../Scenarios/log.rpc");

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
                        m_rpc.Perceive(new[] { EventHelper.ActionEnd(m_rpc.CharacterName.ToString(),actionRpc.Name.ToString(),IATConsts.PLAYER) });
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
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("AgentCardRank(Board)"));
                        break;
                    case "|suit|":
                        //Console.WriteLine("Found Tag: Suit");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("AgentCardSuit(Board)"));
                        break;
                    case "|nextPlayerId|":
                        //Console.WriteLine("Found Tag: NextPlayerID");
                        _tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId|":
                        //Console.WriteLine("Found Tag: PlayerID");
                        //_tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId1|":
                        //Console.WriteLine("Found Tag: PlayerID1");
                        //_tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|playerId2|":
                        //Console.WriteLine("Found Tag: PlayerID2");
                        //_tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|intensity|":
                        //Console.WriteLine("Found Tag: Intensity");
                        //_tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    case "|trickPoints|":
                        //Console.WriteLine("Found Tag: Trick Points");
                        //_tagMeaningsList.Add(m_rpc.GetBeliefValue("NextPlayerId(Board)"));
                        break;
                    default:
                        Console.WriteLine("Unknown Tag");
                        break;
                }
            }
            _tagMeanings = _tagMeaningsList.ToArray();
        }

        static void WriteAction(IAction a)
        {
            if (a == null)
            {
                Console.WriteLine("Null action");
                return;
            }

            Console.WriteLine("Selected Action: " + a.Key);
            Console.WriteLine("Parameters: ");
            foreach (var p in a.Parameters)
            {
                Console.Write( p + ", ");
            }
            Console.WriteLine();
        }
    }
}
