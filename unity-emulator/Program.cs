using System;

namespace unity_emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string character = "";
            character = args[0];
            UnityEmulator sp = new UnityEmulator(character);
            Console.ReadLine();
            sp.Dispose();
        }
    }
}
