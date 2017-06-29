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
        const string logsPath = "../../../sueca-logs";

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
            List<Move> currentGame = new List<Move>(40);
            int leadSuit = -1;
            bool trumpFound = false;
            int trumpSuit = -1;
            string[] processedPlays = new string[1317520 + 2];
            processedPlays[0] = "1,16,1317520";
            //string[] processedPlays = new string[329380 + 2];
            //processedPlays[0] = "1,16,329380";
            processedPlays[1] = "Label,IndexInTrick,NumTrumps,NumAces,NumSevens,NumFigs,Handsize,NumHandCardsLeadsuit,NumPlayedCardsLeadsuit,NumUnplayedCardsLeadsuit,IsPlayedAceLeadsuit,IsPlayedSevenLeadsuit,IsPlayedKingLeadsuit,IsPlayedAceTrump,IsPlayedSevenTrump,IsPlayedKingSuit,IsCurrentWinnerTeam";
            int playCounter = 2;

            foreach (var file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    List<int>[] playersHands = new List<int>[4];

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == 0 && String.Equals(lines[i], "-- GAME --"))
                        {
                            gameLines = 0;
                            currentGame = new List<Move>(40);
                            leadSuit = -1;
                            trumpFound = false;
                            trumpSuit = -1;
                            playersHands = new List<int>[4];
                        }
                        else if (String.Equals(lines[i], "-- /GAME --"))
                        {
                            if (i - lastGameLine == 15 && gameLines == 14 && playersHands[0] != null && playersHands[1] != null && playersHands[2] != null && playersHands[3] != null && playersHands[0].Count == 10 && playersHands[1].Count == 10 && playersHands[2].Count == 10 && playersHands[3].Count == 10)
                            {
                                numFinishedGames++;
                                if (trumpFound)
                                {
                                    numFinishedAndTrumpFound++;

                                    getPlayFeatures(ref processedPlays, ref playCounter, currentGame, playersHands, trumpSuit);
                                }
                            }
                            lastGameLine = i;
                            gameLines = 0;
                            currentGame = new List<Move>(40);
                            playersHands = new List<int>[4];
                        }
                        else if (lines[i].Length > 2 && lines[i][0] == 'P' && (lines[i][1] == '0' || lines[i][1] == '1' || lines[i][1] == '2' || lines[i][1] == '3'))
                        {
                            gameLines++;

                            if (lines[i].Length > 3 && lines[i][3] == 'H')
                            {
                                if (playersHands[0] == null || playersHands[1] == null || playersHands[2] == null || playersHands[3] == null)
                                {
                                    string[] logHand = lines[i].Split(' ', ',', '.');
                                    if (logHand.Length == 22)
                                    {
                                        List<int> hand = new List<int>();
                                        for (int k = 2; k <= 20; k = k + 2)
                                        {
											int card = Card.CreateFromLog(logHand[k]);
											if (card > 39 || card < -1)
											{
												Console.WriteLine(logHand[k]);
											}
                                            hand.Add(card);
                                        }
                                        int playerID = Int16.Parse("" + logHand[0][1]);
                                        playersHands[playerID] = new List<int>(hand);
                                    }
                                }
                            }
                            else if(lines[i].Length > 3 && playersHands.Length == 4)
                            {
                                string[] trick = lines[i].Split(' ');

                                if (trick.Length != 9)
                                {
                                    break;
                                }
                                
                                for (int j = 0; j < 7; j = j + 2)
                                {
                                    int playerId = Int16.Parse("" + trick[j][1]);
                                    string logCard = trick[j + 1];
                                    int card = Card.CreateFromLog(logCard);
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

            System.IO.File.WriteAllLines(logsPath + "\\processedPlays.txt", processedPlays);

            Console.WriteLine("Finished games: " + numFinishedGames);
            Console.WriteLine("Finished games with trump: " + numFinishedAndTrumpFound);
            Console.WriteLine("handCounter: " + (playCounter - 2));
        }

        private static void getPlayFeatures(ref string[] processedPlays, ref int playCounter, List<Move> game, List<int>[] playersHands, int trump)
        {
            List<int> playedCards = new List<int>();
            for (int i = 0; i < game.Count; i++)
            {
                Move move = game[i];
                int playerID = move.PlayerId;

                //if (playerID == 0) // collect only human plays
                {
                    string label = Sueca.GetPlayLabel(move, i, game, trump);
                    int[] features = Sueca.GetFeaturesFromIthPlay(playerID, playersHands[playerID], game, i, trump);
                    string stringOfFeatures = "" + label;
                    for (int j = 0; j < features.Length; j++)
                    {
                        stringOfFeatures += "," + features[j];
                    }
                    processedPlays[playCounter] = stringOfFeatures;
                    playCounter++;
                }

                playedCards.Add(move.Card);
                playersHands[move.PlayerId].Remove(move.Card);
            }

        }
    }
}