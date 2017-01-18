using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RolePlayCharacter;
using IntegratedAuthoringTool;
using System.Collections;
using WellFormedNames;
using System.Threading;
using System.Diagnostics;

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
            _events.Add((Name)eventName);
        }

        public void UpdateCoroutine()
        {
            _events.Clear();
            var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)", m_rpc.CharacterName);
            AddEvent(enterEventRpcOne);
            AddEvent(string.Format("Event(Property-change,Self,DialogueState(Player),{0})", IntegratedAuthoringToolAsset.INITIAL_DIALOGUE_STATE));

            while (m_rpc.GetBeliefValue("DialogueState(Player)") != "Disconnected")
            {
                Thread.Sleep(1000);
                //yield return new WaitForSeconds(1);
                if (_events.Count == 0)
                {
                    m_rpc.Update();
                    continue;
                }


                var actionRpc = m_rpc.PerceptionActionLoop(_events);
                _events.Clear();
                m_rpc.Update();

                if (actionRpc == null)
                    continue;

                string actionKey = actionRpc.ActionName.ToString();
                //Debug.Log("Action Key: " + actionKey);

                switch (actionKey)
                {
                    case "Play":
                        Console.Write("Hello");
                        break;
                    case "Speak":
                        //m_activeController.StartCoroutine(HandleSpeak(actionRpc));
                        break;
                    case "Fix":
                        //m_activeController.StartCoroutine(HandleFix(actionRpc));
                        break;
                    case "Disconnect":
                        //m_activeController.StartCoroutine(HandleDisconnectAction(actionRpc));
                        break;
                    default:
                        //Debug.LogWarning("Unknown action: " + actionKey);
                        break;
                }
                
            }
            //m_dialogController.AddDialogLine(string.Format("- {0} disconnects -", m_rpc.CharacterName));
            //_currentCoroutine = null;
            //Object.Destroy(_body.Body);
        }

    }
}
