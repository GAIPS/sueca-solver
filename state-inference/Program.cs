using System;
using System.Collections.Generic;
using System.IO;

namespace SuecaSolver
{
    public class Program
    {
        static int numFinishedGames = 0;
        static int numFinishedAndTrumpFound = 0;

        public static void Main()
        {
            const string searchPattern = "*.log";
            const string logsPath = "sueca-logs";

            if (Directory.Exists(logsPath))
            {
                var filesInDirectory = Directory.EnumerateFiles(logsPath, searchPattern);
                IterateFiles(filesInDirectory, "sueca-logs");
            }
            else
            {
                Console.WriteLine("Log path does nto exist or has a different name.");
            }
            Console.ReadLine();
        }

        private static void IterateFiles(IEnumerable<string> files, string directory)
        {
            int lastGameLine = 0;
            int gameLines = 0;
            List<Move> currentGame = new List<Move>(10);
            int leadSuit = -1;
            bool trumpFound = false;
            int trumpSuit = -1;

            foreach (var file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == 0 && String.Equals(lines[i],"-- GAME --"))
                        {
                            gameLines = 0;
                            currentGame = new List<Move>(10);
                            leadSuit = -1;
                            trumpFound = false;
                            trumpSuit = -1;
                        }
                        else if (String.Equals(lines[i],"-- /GAME --"))
                        {
                            if (i - lastGameLine == 15 && gameLines == 14)
                            {
                                numFinishedGames++;
                                if (trumpFound)
                                {
                                    numFinishedAndTrumpFound++;
                                }
                            }
                            lastGameLine = i;
                            gameLines = 0;
                        }
                        else if (lines[i].Length > 2 && lines[i][0] == 'P' && (lines[i][1] == '0' || lines[i][1] == '1' || lines[i][1] == '2' || lines[i][1] == '3'))
                        {
                            gameLines++;
                        
                            if (lines[i].Length > 3 && lines[i][3] != 'H')
                            {
                                string[] trick = lines[i].Split(' ');

                                if (trick.Length < 8)
                                {
                                    break;
                                }
                                
                                for (int j = 0; j < 7; j = j + 2)
                                {
                                    int playerId = Convert.ToInt16(trick[j][1]);
                                    int card = Card.CreateFromLog(trick[j + 1]);
                                    int suit = Card.GetSuit(card);

                                    if (j == 0)
                                    {
                                        if (currentGame.Count >= 4 && !trumpFound)
                                        {
                                            for (int k = currentGame.Count - 4; k < currentGame.Count; k++)
                                            {
                                                if (playerId == currentGame[k].PlayerId && Card.GetSuit(currentGame[k].Card) != leadSuit)
                                                {
                                                    trumpFound = true;
                                                    trumpSuit = Card.GetSuit(currentGame[k].Card);
                                                    break;
                                                }
                                            }
                                        }
                                        leadSuit = suit;
                                    }
                                    currentGame.Add(new Move(playerId, card));
                                }
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    //Handle File may be in use...
                }
            }
            Console.WriteLine("Finished games: " + numFinishedGames);
            Console.WriteLine("Finished games with trump: " + numFinishedAndTrumpFound);
        }
    }
}