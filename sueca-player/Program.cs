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
