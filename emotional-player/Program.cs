using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            EmotionalSuecaPlayer sp = null;
            string client = "";
            string character = "";
            string path = "";
            string socialAgent = "";
            string playAgent = "";

            if (args.Length == 5)
            {
                character = args[0];
                client = args[1];
                path = args[2];
                socialAgent = args[3];
                playAgent = args[4];
                sp = new EmotionalSuecaPlayer(character, client, path, socialAgent, playAgent);
            }
            else
            {
                Console.WriteLine("Unspecified number of arguments: [thisClientName], [scenarioPath], [socialAgent:\"Group\"|\"Individual\"|\"LW\"|\"HW\"], [ThalamusCharacter(s)], [playingAgent:\"Worst\"|\"Best\"]");
            }

            Console.ReadLine();

            if (sp != null)
            {
                sp.Dispose();
            }
        }
    }
}
