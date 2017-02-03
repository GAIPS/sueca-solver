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

namespace EmotionalPlayer
{
    class SocialAgentController
    {
        private RolePlayCharacterAsset m_rpc;
        private IntegratedAuthoringToolAsset m_iat;
        private EmotionalSuecaPlayer.ScenarioData m_scenarioData;
        private List<Name> _events = new List<Name>();
        private EmotionalSuecaPlayer _esp;

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
                        Console.WriteLine(dialog);
                        _esp.SuecaPub.PerformUtteranceWithTags("", dialog, new string[] { }, new string[] { });
                        m_rpc.Perceive(new[] { EventHelper.ActionEnd(m_rpc.CharacterName.ToString(),actionRpc.Name.ToString(),IATConsts.PLAYER) });
                        break;
                    default:
                        Console.WriteLine("Default Case");
                        break;
                }
            }
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
