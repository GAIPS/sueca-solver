using System;

namespace SuecaPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            SuecaPlayer sp;
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
                sp = new SuecaPlayer(client, character);
            }
            else
            {
                sp = new SuecaPlayer("IA");
            }
            Console.ReadLine();
            sp.Dispose();
        }
    }
}
