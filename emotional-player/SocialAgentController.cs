using System;
using System.Collections.Generic;
using RolePlayCharacter;
using IntegratedAuthoringTool;
using WellFormedNames;
using System.Threading;
using Utilities;
using System.Linq;

namespace EmotionalPlayer
{
    class SocialAgentController
    {
        private RolePlayCharacterAsset m_rpc;
        private IntegratedAuthoringToolAsset m_iat;
        private EmotionalSuecaPlayer.ScenarioData m_scenarioData;
        private List<Name> _events = new List<Name>();

        public SocialAgentController(EmotionalSuecaPlayer.ScenarioData scenarioData, RolePlayCharacterAsset rpc, IntegratedAuthoringToolAsset iat)
        {
            m_scenarioData = scenarioData;
            m_iat = iat;
            m_rpc = rpc;
        }

        public void AddEvent(string eventName)
        {
            Console.WriteLine("Adding Event: " + eventName);
            _events.Add((Name)eventName);
        }

        public void UpdateCoroutine()
        {
            _events.Clear();
            AddEvent(string.Format("Event(Property-Change,{0},Playing(Self),Game)", m_rpc.CharacterName));
            AddEvent(string.Format("Event(Property-Change,Self,DialogueState(Player),{0})", IntegratedAuthoringToolAsset.INITIAL_DIALOGUE_STATE));
            while (m_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Console.WriteLine("updating");
                Thread.Sleep(1000);
                if (_events.Count == 0)
                {
                    Console.WriteLine("Event count = 0");
                    m_rpc.Update();
                    continue;
                }

                var actionRpc = m_rpc.PerceptionActionLoop(_events);
                _events.Clear();
                m_rpc.Update();

                if (actionRpc == null)
                {
                    Console.WriteLine("Action RPC is null");
                    continue;
                }

                string actionKey = actionRpc.ActionName.ToString();
                Console.WriteLine("Action Key: " + actionKey);

                switch (actionKey)
                {
                    case "Speak":
                        Console.WriteLine("Entered Compliment");
                        Name nextState = actionRpc.Parameters[1];
                        var dialog =
                            m_iat.GetDialogueActions(IntegratedAuthoringToolAsset.AGENT, actionRpc.Parameters[0], nextState,
                                actionRpc.Parameters[2], actionRpc.Parameters[3]).Shuffle().FirstOrDefault();
                        Console.WriteLine("Said the following utterance: " + dialog.Utterance);
                        break;
                    case "Talk":
                        Console.WriteLine("Insert Utterance Here");
                        break;
                    case "Fix":
                        //m_activeController.StartCoroutine(HandleFix(actionRpc));
                        break;
                    case "Disconnect":
                        //m_activeController.StartCoroutine(HandleDisconnectAction(actionRpc));
                        break;
                    default:
                        Console.WriteLine("Default Switch");
                        //Debug.LogWarning("Unknown action: " + actionKey);
                        break;
                }
                //m_rpc.SaveStateToFile("../../../Scenarios/log.rpc");
            }
            //m_dialogController.AddDialogLine(string.Format("- {0} disconnects -", m_rpc.CharacterName));
            //_currentCoroutine = null;
            //Object.Destroy(_body.Body);
        }
    }
}
