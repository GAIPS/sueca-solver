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
            string type = "";

            if (args.Length == 4)
            {
                client = args[0];
                path = args[1];
                type = args[2];
                character = args[3];
                sp = new EmotionalSuecaPlayer(client, path, type, character);
            }
            else
            {
                Console.WriteLine("Unspecified number of arguments: [thisClientName], [scenarioPath], [agentType:\"Group\"|\"Individual\"], [ThalamusCharacter(s)]");
            }

            Console.ReadLine();

            if (sp != null)
            {
                sp.Dispose();
            }
        }
    }
}
