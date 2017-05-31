using AssetManagerPackage;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using WellFormedNames;
using EmotionalAppraisal.DTOs;

namespace EmotionalPlayer
{
    class SuecaRolePlayCharacter
    {
        private struct Utterance
        {
            public string text { get; set; }
            public int uses { get; set; }

            public Utterance(string utterance, int repetitions)
            {
                text = utterance;
                uses = repetitions;
            }
        }

        private IntegratedAuthoringToolAsset _iat;
        public RolePlayCharacterAsset _rpc;
        private Object eventsLock;
        private List<SuecaEvent> _events;
        private List<Utterance> usedUtterances;
        private const string LOGS_PATH = "../../Scenarios/Logs";
        private int i;
        private Random _randomNumberGenerator;
        private bool _sleepNotify = false;
        public string _agentName;
        private EmotionalSuecaPlayer _esp;

        public SuecaRolePlayCharacter(int nameId, string agentType, string scenarioPath, EmotionalSuecaPlayer esp)
        {
            _esp = esp;
            _agentName = "EMYS-" + nameId + "(" + agentType + ")";
            _randomNumberGenerator = new Random(System.Guid.NewGuid().GetHashCode());
            _events = new List<SuecaEvent>();
            AssetManager.Instance.Bridge = new AssetManagerBridge();
            _iat = IntegratedAuthoringToolAsset.LoadFromFile(scenarioPath);
            eventsLock = new Object();
            loadScenario(agentType);
            usedUtterances = new List<Utterance>();
            Task.Run(() => { UpdateCoroutine(); });
        }



        private void loadScenario(string agentType)
        {
            System.IO.Directory.CreateDirectory(LOGS_PATH);

            var characterSources = _iat.GetAllCharacterSources().ToList();
            foreach (var source in characterSources)
            {
                RolePlayCharacterAsset character = RolePlayCharacterAsset.LoadFromFile(source.Source);
                character.LoadAssociatedAssets();
                if (character.BodyName.ToString() == agentType)
                {
                    _iat.BindToRegistry(character.DynamicPropertiesRegistry);
                    _rpc = character;
                    break;
                }
            }
        }


        private void saveToFile()
        {
            _rpc.SaveToFile(LOGS_PATH + "/" + _agentName + "log" + i + ".rpc");
            i++;
        }

        private void showEmotions()
        {
            string emotions = "";
            foreach (var emotion in _rpc.GetAllActiveEmotions())
            {
                emotions += " " + emotion.Type.ToString();
            }
            Console.WriteLine(_agentName + "---" + "Emotions: " + emotions);
        }


        private void UpdateCoroutine()
        {
            string currentBelief = _rpc.GetBeliefValue(Consts.DIALOGUE_STATE_PROPERTY);

            while (currentBelief != Consts.STATE_SESSION_END)
            {
                //showEmotions();
                _rpc.Update();
                currentBelief = _rpc.GetBeliefValue(Consts.DIALOGUE_STATE_PROPERTY);
                if (_events.Count > 0)
                {
                    SuecaEvent ev;
                    lock (eventsLock)
                    {
                        ev = _events.First();
                        _events.RemoveAt(0);
                    }

                    //wait until event is finished
                    while (!ev.Finished) { }
                    

                    if (ev.Name == Consts.INIT)
                    {
                        _rpc.CharacterName = (Name)ev.OtherStringInfos[0];
                        _rpc.m_kb.SetPerspective((Name)ev.OtherStringInfos[0]);
                    }
                    if (ev.Name == Consts.STATE_TRICK_END)
                    {
                        Console.WriteLine("\nCLEANING EMOTIONAL POOL\n");
                        _rpc.ResetEmotionalState();
                    }

                    perceive(ev);

                    if (ev.Name == Consts.STATE_NEXT_PLAYER)
                    {
                        if (ev.OtherIntInfos.Length > 0 && !_esp.Talking)
                        {
                            Thread.Sleep(1000);
                            EmotionalSuecaPlayer.SuecaPub.GazeAtTarget("player" + ev.OtherIntInfos[0]);
                        }
                        
                        if (_randomNumberGenerator.Next(0, 10) < 6)
                        {
                            // Sleep randomly until decide
                            Thread waitForDeciding = new Thread(this.SleepForNextPlayerEvent);
                            waitForDeciding.Start();
                            while (!_sleepNotify && _events.Count == 0)
                            {

                            }
                            waitForDeciding.Interrupt();
                            _sleepNotify = false;
                            if (_events.Count == 0)
                            {
                                //decide only if after sleeping no one has played
                                decide(ev);
                            }
                        }
                    }
                    else if (ev.Name == Consts.STATE_PLAYSELF)
                    {
                        EmotionalSuecaPlayer.SuecaPub.GazeAtTarget("cardsZone");
                        if (ev.OtherStringInfos.Length > 0)
                        {
                            Thread.Sleep(1000);
                            decide(ev);
                            Thread.Sleep(1000);
                            EmotionalSuecaPlayer.SuecaPub.Play(ev.OtherIntInfos[0], ev.OtherStringInfos[0], ev.OtherStringInfos[1]);
                        }
                    }
                    else if (ev.Name == Consts.STATE_PLAYPARTNER)
                    {
                        decide(ev);

                        if (ev.OtherIntInfos[0] == ((_esp._id + 2) % 4))
                        {
                            string attributionEmotion = getStrongestAttributionEmotion(_rpc.GetAllActiveEmotions());
                            EmotionalSuecaPlayer.SuecaPub.SetPosture("", attributionEmotion);
                        }
                        else
                        {
                            string wellbeingEmotion = getStrongestWellbeingEmotion(_rpc.GetAllActiveEmotions());
                            EmotionalSuecaPlayer.SuecaPub.SetPosture("", wellbeingEmotion);
                        }
                    }
                    else if (ev.Name != Consts.INIT)
                    {
                        decide(ev);
                    }
                }
                Thread.Sleep(100);
            }
        }

        private string getStrongestAttributionEmotion(IEnumerable<EmotionDTO> activeEmotions)
        {
            float intensity = 0;
            string strongestEmotion = "";
            foreach (var emotion in activeEmotions)
            {
                if (emotion.Type == "Pride" || emotion.Type == "Admiration" || emotion.Type == "Shame" || emotion.Type == "Reproach")
                {
                    if (emotion.Intensity > intensity)
                    {
                        intensity = emotion.Intensity;
                        strongestEmotion = emotion.Type.ToLower();
                    }
                }
            }
            return strongestEmotion;
        }

        private string getStrongestWellbeingEmotion(IEnumerable<EmotionDTO> activeEmotions)
        {
            float intensity = 0;
            string strongestEmotion = "";
            foreach (var emotion in activeEmotions)
            {
                if (emotion.Type == "Joy" || emotion.Type == "Distress")
                {
                    if (emotion.Intensity > intensity)
                    {
                        intensity = emotion.Intensity;
                        strongestEmotion = emotion.Type.ToLower();
                    }
                }
            }
            return strongestEmotion;
        }

        public void SleepForNextPlayerEvent(object data)
        {
            try
            {
                Thread.Sleep(_randomNumberGenerator.Next(20000, 50000));
                _sleepNotify = true;
            }
            catch (Exception)
            {
                Console.WriteLine(_agentName + "---" + "Skipped a nextPlayer event.");
            }
        }

        public void AddSuecaEvent(SuecaEvent ev)
        {
            lock (eventsLock)
            {
                _events.Add(ev);
            }
        }

        private void perceive(SuecaEvent ev)
        {
            //DEBUG
            Console.WriteLine(_agentName + "---" + "Going to perceive event " + ev.Name);
            foreach (var el in ev.Events)
            {
                Console.WriteLine(_agentName + "---" + el.ToString());
            }
            _rpc.Perceive(ev.Events);
        }

        private void decide(SuecaEvent ev)
        {
            Console.WriteLine(_agentName + "---" + "DECING FOR EVENT: " + ev.Name);
            string[] tags = ev.Tags.ToArray();
            string[] meanings = ev.Meanings.ToArray();

            try
            {
                IEnumerable<ActionLibrary.IAction> possibleActions = _rpc.Decide();

                if (possibleActions == null || possibleActions.IsEmpty())
                {
                    Console.WriteLine(_agentName + "---" + "No action");
                    saveToFile();
                    return;
                }
                else
                {
                    ActionLibrary.IAction chosenAction = possibleActions.FirstOrDefault();
                    saveToFile();

                    switch (chosenAction.Key.ToString())
                    {
                        case "Speak":
                            
                            Name currentState = chosenAction.Parameters[0];
                            Name nextState = chosenAction.Parameters[1];
                            Name meaning = chosenAction.Parameters[2];
                            Name style = chosenAction.Parameters[3];
                            var possibleDialogs = _iat.GetDialogueActions(IATConsts.AGENT, currentState, nextState, meaning, style);
                            var dialog = getUtterance(possibleDialogs);

                            Console.WriteLine(_agentName + "---" + dialog);
                            EmotionalSuecaPlayer.SuecaPub.StartedUtterance(_esp._id, ev.Name, "");
                            EmotionalSuecaPlayer.SuecaPub.PerformUtteranceWithTags("", dialog, tags, meanings);

                            break;

                        case "Animation":
                            Name state = chosenAction.Parameters[0];
                            string emotionName = chosenAction.Parameters[1].ToString().ToLower();
                            if (emotionName == "distress" || emotionName == "shame")
                            {
                                emotionName = "sadness";
                            }
                            else if (emotionName == "pride")
                            {
                                emotionName = "joy";
                            }
                            else if (emotionName == "reproach")
                            {
                                emotionName = "anger";
                            }
                            Console.WriteLine("[ANIMATION] reaction to " + state + " with the style " + emotionName);
                            EmotionalSuecaPlayer.SuecaPub.PlayAnimation("", emotionName + "3");
                            break;

                        default:
                            Console.WriteLine(_agentName + "---" + "Unknown Action");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(_agentName + "---" + e.ToString());
            }
            
            
        }

        /// <summary>
        /// Checks if any of the dialogs is contained within a list of used dialogues.
        /// If been used it goes into a list of candidates to be reused.
        /// If no unused dialogues, the method returns a random dialog with the least uses.
        /// </summary>
        /// <param name="dialogs">List of possible dialogues</param>
        /// <returns>An unused dialogue or, alternatively, a least used dialogue</returns>
        private string getUtterance(List<DialogueStateActionDTO> dialogs)
        {
            List<Utterance> candidates = new List<Utterance>();

            foreach (DialogueStateActionDTO dialog in dialogs)
            {
                int i = usedUtterances.FindIndex(o => string.Equals(dialog.Utterance, o.text, StringComparison.OrdinalIgnoreCase));

                if (i == -1)
                {
                    //usedUtterances.Add(new Utterance(dialog.Utterance, 1));
                    candidates.Add(new Utterance(dialog.Utterance, 0));
                    //return dialog.Utterance;
                }

                else if (i > -1)
                {
                    var temp = usedUtterances[i];
                    candidates.Add(temp);
                }
            }

            int min = candidates.Min(x => x.uses);
            var result = candidates.Where(t => t.uses == min).Shuffle().FirstOrDefault();
            int j = usedUtterances.FindIndex(o => string.Equals(result.text, o.text, StringComparison.OrdinalIgnoreCase));

            if (j == -1)
            {
                usedUtterances.Add(new Utterance(result.text, ++result.uses));
            }
            else if (j >= -1)
            {
                usedUtterances.RemoveAt(j);
                usedUtterances.Add(new Utterance(result.text, ++result.uses));
            }

            return result.text;
        }
    }
}
