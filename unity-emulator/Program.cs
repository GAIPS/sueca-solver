using System;

namespace unity_emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            UnityEmulator sp = new UnityEmulator();
            Console.ReadLine();
            sp.Dispose();
        }
    }
}
