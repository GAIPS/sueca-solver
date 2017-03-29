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
            string client = "";
            string character = "";
            string path = "";
            string type = "";

            EmotionalSuecaPlayer emotionalPlayer = null;


            if (args.Length == 4)
            {
                client = args[0];
                path = args[1];
                type = args[2];
                character = args[3];
                emotionalPlayer = new EmotionalSuecaPlayer(client, path, type, character);
            }
            else
            {
                Console.WriteLine("Incorrect number of parameters. 4 are required: [thalamusClientName], [scenarioPath], [agentType], [thalamusCharacter]");
            }
            
            Console.ReadLine();
            if (emotionalPlayer != null)
            {
                emotionalPlayer.Dispose();
            }
        }
    }
}
