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
        private RolePlayCharacterAsset _rpc;
        private Object eventsLock;
        private List<SuecaEvent> _events;
        private List<Utterance> usedUtterances;
        private const string LOGS_PATH = "../../../Scenarios/Logs";
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
            Console.WriteLine("Emotions: " + emotions);
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

                    perceive(ev);

                    if (ev.Name == Consts.STATE_NEXT_PLAYER)
                    {
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
                        decide(ev);
                        EmotionalSuecaPlayer.SuecaPub.GazeAtTarget("cardsZone");
                        EmotionalSuecaPlayer.SuecaPub.Play(ev.OtherIntInfos[0], ev.OtherStringInfos[0], ev.OtherStringInfos[1]);
                    }
                    else if (ev.Name != Consts.INIT)
                    {
                        decide(ev);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void SleepForNextPlayerEvent(object data)
        {
            try
            {
                Thread.Sleep(_randomNumberGenerator.Next(3000, 7000));
                _sleepNotify = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Skipped a nextPlayer event.");
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
            foreach (var el in ev.Events)
            {
                Console.WriteLine(el.ToString());
            }
            _rpc.Perceive(ev.Events);
        }

        private void decide(SuecaEvent ev)
        {
            Console.WriteLine("DECING FOR EVENT: " + ev.Name);
            string[] tags = ev.Tags.ToArray();
            string[] meanings = ev.Meanings.ToArray();

            
            IEnumerable<ActionLibrary.IAction> possibleActions = _rpc.Decide();
            
            if (possibleActions == null || possibleActions.IsEmpty())
            {
                Console.WriteLine("No action");
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
                        _esp.RequestUtterance(ev.Name, "");
                        _esp.WaitForResponse();
                        if (_esp.Talking)
                        {
                            Name currentState = chosenAction.Parameters[0];
                            Name nextState = chosenAction.Parameters[1];
                            Name meaning = chosenAction.Parameters[2];
                            Name style = chosenAction.Parameters[3];
                            Console.WriteLine("Speak(" + currentState + ", " + nextState + ", " + meaning + ", " + style + ")");
                            var possibleDialogs = _iat.GetDialogueActions(IATConsts.AGENT, currentState, nextState, meaning, style);
                            var dialog = getUtterance(possibleDialogs);

                            //Console.WriteLine(dialog);
                            EmotionalSuecaPlayer.SuecaPub.StartedUtterance(_esp._id, ev.Name, "");
                            EmotionalSuecaPlayer.SuecaPub.PerformUtteranceWithTags("", dialog, tags, meanings);
                        }

                        break;

                    case "Animation":
                        Name state = chosenAction.Parameters[0];
                        Name emotionName = chosenAction.Parameters[1];
                        //Console.WriteLine("[ANIMATION] Soft reaction to " + state + " with the style " + emotionName);
                        EmotionalSuecaPlayer.SuecaPub.PlayAnimation("", emotionName.ToString());
                        break;

                    default:
                        Console.WriteLine("Unknown Action");
                        break;
                }
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
