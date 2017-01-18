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
            if (args.Length == 2)
            {
                client = args[0];
                character = args[1];
            }
            else if (args.Length == 1)
            {
                client = "IA";
                character = args[0];
            }
            else
            {
                Console.WriteLine("Unspecified client name and character for Thalamus.");
            }

            if (client != "" && character != "")
            {
                sp = new EmotionalSuecaPlayer(client, character);
            }
            else
            {
                sp = new EmotionalSuecaPlayer("IA");
            }
            Console.ReadLine();
            sp.Dispose();
        }
    }
}
