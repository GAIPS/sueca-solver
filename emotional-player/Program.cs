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
            EmotionalSuecaPlayer sp;
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

            }
            else if (args.Length == 1)
            {
                client = "EmotionalAgent";
                character = args[0];
                type = "group";
            }
            else
            {
                Console.WriteLine("Unspecified client name and character for Thalamus.");
            }

            if (client != "" && character != "" && path != "" && type != "")
            {
                sp = new EmotionalSuecaPlayer(client, path, type, character);
            }
            else
            {
                sp = new EmotionalSuecaPlayer("EmotionalAgent", path, "group");
            }
            Console.ReadLine();
            sp.Dispose();
        }
    }
}
