using System;
using System.Collections.Generic;
using System.IO;

namespace SuecaSolver
{
    public class Program
    {
        static int numFinishedGames = 0;
        static int numFinishedAndTrumpFound = 0;
        const string searchPattern = "*.log";
        const string logsPath = "..\\..\\..\\sueca-logs";

        public static void Main()
        {

            if (Directory.Exists(logsPath))
            {
                var filesInDirectory = Directory.EnumerateFiles(logsPath, searchPattern);
                IterateFiles(filesInDirectory, "sueca-logs");
            }
            else
            {
                Console.WriteLine("Log path does not exist or has a different name.");
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
            int handCounter = 0;
            string[] processedHands = new string[139640];

            foreach (var file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    List<List<int>> playersHands = new List<List<int>>();

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == 0 && String.Equals(lines[i],"-- GAME --"))
                        {
                            gameLines = 0;
                            currentGame = new List<Move>(10);
                            leadSuit = -1;
                            trumpFound = false;
                            trumpSuit = -1;
                            playersHands = new List<List<int>>();
                        }
                        else if (String.Equals(lines[i],"-- /GAME --"))
                        {
                            if (i - lastGameLine == 15 && gameLines == 14)
                            {
                                numFinishedGames++;
                                if (trumpFound)
                                {
                                    numFinishedAndTrumpFound++;

                                    foreach (var hand in playersHands)
                                    {
                                        processedHands[handCounter] = getHandFeatures(hand, trumpSuit);
                                        handCounter++;
                                    }
                                }
                            }
                            lastGameLine = i;
                            gameLines = 0;
                        }
                        else if (lines[i].Length > 2 && lines[i][0] == 'P' && (lines[i][1] == '0' || lines[i][1] == '1' || lines[i][1] == '2' || lines[i][1] == '3'))
                        {
                            gameLines++;

                            if (lines[i].Length > 3 && lines[i][3] == 'H')
                            {
                                if (playersHands.Count < 4)
                                {
                                    string[] logHand = lines[i].Split(' ', ',', '.');
                                    if (logHand.Length == 22)
                                    {
                                        List<int> hand = new List<int>();
                                        for (int k = 2; k <= 20; k = k + 2)
                                        {
                                            int card = Card.CreateFromLog(logHand[k]);
                                            hand.Add(card);
                                        }
                                        playersHands.Add(hand);
                                    }
                                }
                            }
                            else if(lines[i].Length > 3 && playersHands.Count == 4)
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

            System.IO.File.WriteAllLines(logsPath + "\\processedHands.txt", processedHands);

            Console.WriteLine("Finished games: " + numFinishedGames);
            Console.WriteLine("Finished games with trump: " + numFinishedAndTrumpFound);
            Console.WriteLine("handCounter: " + handCounter);
        }

        private static string getHandFeatures(List<int> hand, int trump)
        {
            string features = "";
            features += Sueca.CountCardsFromSuit(hand, trump);
            features += "\t" + Sueca.CountCardsFromRank(hand, (int) Rank.Ace);
            features += "\t" + Sueca.CountCardsFromRank(hand, (int) Rank.Seven);
            features += "\t" + Sueca.CountCardsFromRank(hand, (int) Rank.King);
            return features;
        }
    }
}