using System;

namespace unity_emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string character = "";
            UnityEmulator sp = null;
            if (args.Length == 1)
            {
                character = args[0];
                sp = new UnityEmulator(character);
            }
            else
            {
                Console.WriteLine("Unspecified client name for Thalamus.");
            }
            Console.ReadLine();

            if (sp != null)
            {
                sp.Dispose();
            }
        }
    }
}
