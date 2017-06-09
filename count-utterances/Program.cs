using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utilities;

namespace ConsoleApp1
{
    class Program
    {
        class Utterance { public string text; public int times; public string emotion; }


        static Dictionary<int,Utterance> utterances = new Dictionary<int,Utterance>();

        static Dictionary<string,Dictionary<string,int>> emotionsPerSession = new Dictionary<string, Dictionary<string, int>>();
        static void Main(string[] args)
        {
            AddAllUtterances();


            string path = args[0];
            int total = 0;

            if (Directory.Exists(path))
            {
                string[] filesindirectory = Directory.GetDirectories(path);

                foreach (string subdir in filesindirectory)
                {
                    string logsPath = subdir + "\\Logs";
                    string[] folders = subdir.Split('\\');
                    string sessionName = folders[folders.Length - 1];

                    foreach (string file in Directory.GetFiles(logsPath))
                    {
                        if (file.Contains(".log"))
                        {
                            // This path is a file
                            ProcessFile(sessionName, file);
                            total++;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }

            printEmotionsFrequenciesPerSession();
            //printUtterancesFrequencies();
            Console.ReadLine();

        }

        public static void printUtterancesFrequencies()
        {
            foreach (var u in utterances)
            {
                Console.WriteLine(u.Value.times + " " + u.Value.emotion + " " + u.Value.text);
            }
        }

        public static void printEmotionsFrequenciesPerSession()
        {
            foreach (string session in emotionsPerSession.Keys)
            {
                Console.WriteLine("------------- Session: " + session + " --------------");
                var emotionsInSession = emotionsPerSession[session];
                foreach (string emotion in emotionsInSession.Keys)
                {
                    Console.WriteLine("Emotion: " + emotion + " Count: " + emotionsInSession[emotion]);
                }
            }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string sessionName, string path)
        {
            int performWithTags = 0, matchUtterance = 0, utteranceSent = 0;
            Dictionary<string, int> emotionsInSession = new Dictionary<string, int>();

            var file = new System.IO.StreamReader(path);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                if (line.RemoveWhiteSpace().Contains("PerformUtteranceWithTags:("))
                {
                    performWithTags++;
                    foreach (var u in utterances)
                    {
                        if (line.RemoveWhiteSpace().Contains(u.Value.text.RemoveWhiteSpace()))
                        {
                            matchUtterance++;
                            string[] thamausMessage = line.RemoveWhiteSpace().Split(':');
                            if (thamausMessage[5].Length != 0 && thamausMessage[5].StartsWith("EmotionalSuecaPlayer"))
                            {
                                utteranceSent++;
                                u.Value.times++;

                                char robotId = thamausMessage[5][thamausMessage[5].Length - 1];
                                string emotionKey = u.Value.emotion + robotId;

                                if (emotionsInSession.ContainsKey(emotionKey))
                                {
                                    emotionsInSession[emotionKey]++;
                                }
                                else
                                {
                                    emotionsInSession[emotionKey] = 1;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("sessionName: " + sessionName + " performWithTags: " + performWithTags + " matchUtterance: " + matchUtterance + " utteranceSent: " + utteranceSent);
            emotionsPerSession.Add(sessionName, emotionsInSession);
        }

        public static void AddAllUtterances()
        {
            var file = new System.IO.StreamReader("../../utterances.txt");
            string line;
            int index = 0;

            while ((line = file.ReadLine()) != null)
            {
                string[] utt = line.Split('\t');
                string[] style = utt[2].Split(',');
                string em = style[0];
                if (em == "A" || em == "B" || em == "-")
                {
                    em = "Neutral";
                }

                utterances.Add(index, new Utterance { text = utt[0], emotion = em });
                index++;
            }
        }

     
    }
}
