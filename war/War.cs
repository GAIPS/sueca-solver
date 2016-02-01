using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace SuecaSolver
{
    public class War
    {
        public const int GAMEMODE = 21;
        public const int NUMGAMES = 100;
        public const bool PARALLEL = false;
        public const int NUM_THREADS = Sueca.WAR_NUM_THREADS;
        public const bool SAVE_RESULTS = true;
        public const bool SAVE_CARDS = true; //if true log file will contain intial cards of players otherwise will contain specific features
        ////public const string SAVE_DIR = @"Z:\Devel\sueca-solver\results\";
        public const string SAVE_DIR = @"C:\temp\";
        //public const string SAVE_DIR = "results/";

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int gameMode, numGames, numThreads;
            bool parallel, saveResults, saveCards;
            

            if (args.Length == 6)
            {
                //Assuming the input is correct
                gameMode = Int16.Parse(args[0]);
                numGames = Int16.Parse(args[1]);
                parallel = Boolean.Parse(args[2]);
                numThreads = Int16.Parse(args[3]);
                saveResults = Boolean.Parse(args[4]);
                saveCards = Boolean.Parse(args[5]);
            }
            else
            {
                Console.WriteLine("Unspecified parameters. The program will use default parameters.");
                gameMode = GAMEMODE;
                numGames = NUMGAMES;
                parallel = PARALLEL;
                numThreads = NUM_THREADS;
                saveResults = SAVE_RESULTS;
                saveCards = SAVE_CARDS;
            }

            int firstTeamWins = 0, secondTeamWins = 0, draws = 0, nullGames = 0;

            //Shared data between threads!
            List<List<int>[]> cardsPerPlayer = new List<List<int>[]>(numGames);
            List<int> trumps = new List<int>(numGames);
            List<int> firstPlayers = new List<int>(numGames);
            List<int[]> pointsPerPlayer = new List<int[]>(numGames);
            List<int[]> pointsFromTrumpPerPlayer = new List<int[]>(numGames);
            List<int[]> trumpsPerPlayer = new List<int[]>(numGames);
            List<int[]> acesPerPlayer = new List<int[]>(numGames);
            List<int[]> sevensPerPlayer = new List<int[]>(numGames);
            List<int> teamHasTrumpAce = new List<int>(numGames);
            List<int[]> suitsPerPlayer = new List<int[]>(numGames);
            List<int[]> firstToPlay = new List<int[]>(numGames);
            List<int> finalBotTeamPoints = new List<int>(numGames);
            List<ulong[]> timePerTrick = new List<ulong[]>(numGames);
            Object allGamesLock = new Object();

            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA TEST WAR ||||||||||||||||||||");
            Console.WriteLine("");
            switch (gameMode)
            {
                case 1:
                    Console.WriteLine("Mode 1 (1 Smart 3 Random)");
                    break;
                case 2:
                    Console.WriteLine("Mode 2 (2 Smart 2 Random)");
                    break;
                case 3:
                    Console.WriteLine("Mode 3 (3 Smart 1 Random)");
                    break;
                case 4:
                    Console.WriteLine("Mode 4 (4 Smart)");
                    break;
                case 5:
                    Console.WriteLine("Mode 5 (2 Smart 2 RuleBased)");
                    break;
                case 6:
                    Console.WriteLine("Mode 6 (2 Smartest 2 Random)");
                    break;
                case 7:
                    Console.WriteLine("Mode 7 (2 Smartest 2 RuleBased)");
                    break;
                case 8:
                    Console.WriteLine("Mode 8 (2 RuleBased 2 Random)");
                    break;
                case 9:
                    Console.WriteLine("Mode 9 (2 RBO 2 Random)");
                    break;
                case 10:
                    Console.WriteLine("Mode 10 (2 TrickPlayer 2 Smart)");
                    break;
                case 11:
                    Console.WriteLine("Mode 11 (2 RuleBased 2 RuleBased)");
                    break;
                case 12:
                    Console.WriteLine("Mode 12 (1 RuleBased 3 Random)");
                    break;
                case 13:
                    Console.WriteLine("Mode 13 (1 TrickPlayer 3 Random)");
                    break;
                case 14:
                    Console.WriteLine("Mode 14 (2 TrickPlayer 2 Random)");
                    break;
                case 15:
                    Console.WriteLine("Mode 15 (1 TrickPlayer 3 RuleBased)");
                    break;
                case 16:
                    Console.WriteLine("Mode 16 (2 TrickPlayer 2 RuleBased)");
                    break;
                case 17:
                    Console.WriteLine("Mode 17 (2 TrickPlayer 2 TrickPlayer)");
                    break;
                case 18:
                    Console.WriteLine("Mode 18 (1 Smart 3 RuleBased)");
                    break;
                case 19:
                    Console.WriteLine("Mode 19 (1 RBO 3 RuleBased)");
                    break;
                case 20:
                    Console.WriteLine("Mode 20 (2 Smart 2 Smart)");
                    break;
                case 21:
                    Console.WriteLine("Mode 21 (1 Hybrid 3 RuleBased)");
                    break;
                default:
                    break;
            }
            Console.WriteLine("#Games: " + numGames);


            if (parallel)
            {
                Parallel.For(0, numGames,
                    new ParallelOptions { MaxDegreeOfParallelism = numThreads },
                    () => new int[4],

                    (int i, ParallelLoopState state, int[] localCount) =>
                    {
                        return processGames(i,
                            localCount,
                            gameMode,
                            cardsPerPlayer,
                            trumps,
                            firstPlayers,
                            pointsPerPlayer,
                            pointsFromTrumpPerPlayer,
                            trumpsPerPlayer,
                            acesPerPlayer,
                            sevensPerPlayer,
                            teamHasTrumpAce,
                            suitsPerPlayer,
                            firstToPlay,
                            finalBotTeamPoints,
                            timePerTrick,
                            allGamesLock);
                    },

                    (int[] localCount) =>
                    {
                        draws += localCount[0];
                        firstTeamWins += localCount[1];
                        secondTeamWins += localCount[2];
                        nullGames += localCount[3];
                        //for (int f = 4; f < 14; f++)
                        //{
                        //    timePerTrick[f - 4] = localCount[f] / numGames;
                        //}
                    });
            }
            else
            {
                for (int i = 0; i < numGames; i++)
                {
                    int[] localCount = new int[14];
                    processGames(i,
                        localCount,
                        gameMode,
                        cardsPerPlayer,
                        trumps,
                        firstPlayers,
                        pointsPerPlayer,
                        pointsFromTrumpPerPlayer,
                        trumpsPerPlayer,
                        acesPerPlayer,
                        sevensPerPlayer,
                        teamHasTrumpAce,
                        suitsPerPlayer,
                        firstToPlay,
                        finalBotTeamPoints,
                        timePerTrick,
                        allGamesLock);
                    draws += localCount[0];
                    firstTeamWins += localCount[1];
                    secondTeamWins += localCount[2];
                    nullGames += localCount[3];
                    //for (int f = 4; f < 14; f++)
                    //{
                    //    timePerTrick[f - 4] = localCount[f] / numGames;
                    //}
                }
            }


            if (saveResults)
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(SAVE_DIR);
                int count = dir.GetFiles("log*.txt").Length;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(SAVE_DIR + "log" + count + ".txt"))
                {
                    file.WriteLine("Mode: " + GAMEMODE + " #Games: " + numGames);

                    if (saveCards)
                    {
                        file.WriteLine("p0_0\tp0_1\tp0_2\tp0_3\tp0_4\tp0_5\tp0_6\tp0_7\tp0_8\tp0_9\tp2_0\tp2_1\tp2_2\tp2_3\tp2_4\tp2_5\tp2_6\tp2_7\tp2_8\tp2_9\ttrump\tfirst\tfp\tt_t0\tt_t1\tt_t2\tt_t3\tt_t4\tt_t5\tt_t6\tt_t7\tt_t8\tt_t9");
                        for (int i = 0; i < numGames; i++)
                        {
                            for (int k = 0; k < 4; k = k + 2)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    file.Write(cardsPerPlayer[i][k][j] + "\t");
                                }
                            }
                            file.Write(trumps[i] + "\t");
                            file.Write(firstPlayers[i] + "\t");
                            file.Write(finalBotTeamPoints[i] + "\t");
                            for (int l = 0; l < timePerTrick[i].Length; l++)
                            {
                                file.Write(timePerTrick[i][l] + "\t");
                            }
                            file.WriteLine("");
                        }
                    }
                    else
                    {
                        file.WriteLine("p_0\tp_1\tp_2\tp_3\tpt_0\tpt_1\tpt_2\tpt_3\tt_0\tt_1\tt_2\tt_3\tA_0\tA_1\tA_2\tA_3\tS_0\tS_1\t7_2\t7_3\tAt_02\tsuits_0\tsuits_1\tsuits_2\tsuit_3\tfirst_0\tfirst_1\tfirst_2\tfirst_3\tfp_02");
                        for (int i = 0; i < numGames; i++)
                        {
                            file.WriteLine(pointsPerPlayer[i][0] + "\t"
                                + pointsPerPlayer[i][1] + "\t"
                                + pointsPerPlayer[i][2] + "\t"
                                + pointsPerPlayer[i][3] + "\t"
                                + pointsFromTrumpPerPlayer[i][0] + "\t"
                                + pointsFromTrumpPerPlayer[i][1] + "\t"
                                + pointsFromTrumpPerPlayer[i][2] + "\t"
                                + pointsFromTrumpPerPlayer[i][3] + "\t"
                                + trumpsPerPlayer[i][0] + "\t"
                                + trumpsPerPlayer[i][1] + "\t"
                                + trumpsPerPlayer[i][2] + "\t"
                                + trumpsPerPlayer[i][3] + "\t"
                                + acesPerPlayer[i][0] + "\t"
                                + acesPerPlayer[i][1] + "\t"
                                + acesPerPlayer[i][2] + "\t"
                                + acesPerPlayer[i][3] + "\t"
                                + sevensPerPlayer[i][0] + "\t"
                                + sevensPerPlayer[i][1] + "\t"
                                + sevensPerPlayer[i][2] + "\t"
                                + sevensPerPlayer[i][3] + "\t"
                                + teamHasTrumpAce[i] + "\t"
                                + suitsPerPlayer[i][0] + "\t"
                                + suitsPerPlayer[i][1] + "\t"
                                + suitsPerPlayer[i][2] + "\t"
                                + suitsPerPlayer[i][3] + "\t"
                                + firstToPlay[i][0] + "\t"
                                + firstToPlay[i][1] + "\t"
                                + firstToPlay[i][2] + "\t"
                                + firstToPlay[i][3] + "\t"
                                + finalBotTeamPoints[i]);
                        }
                    }
                }
            }


            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("BotTeam won " + firstTeamWins + "/" + numGames);
            Console.WriteLine("OtherTeam 1 won " + secondTeamWins + "/" + numGames);
            Console.WriteLine("Draws " + draws + "/" + numGames);
            Console.WriteLine("Null Games " + nullGames);

            sw.Stop();
            Console.WriteLine("Total Time taken by functions is {0} seconds", sw.ElapsedMilliseconds / 1000); //seconds
            Console.WriteLine("Total Time taken by functions is {0} minutes", sw.ElapsedMilliseconds / 60000); //minutes
            Console.ReadLine();

        }

        static bool checkHands(List<List<int>> hands, int trump)
        {
            for (int i = 0; i < hands.Count; i++)
            {
                int points = 0;
                int trumps = 0;
                for (int j = 0; j < hands[i].Count; j++)
                {
                    int card = hands[i][j];
                    points += Card.GetValue(card);
                    if (Card.GetSuit(card) == trump)
                    {
                        trumps++;
                    }
                }

                if (trumps == 0 && points < 10)
                {
                    return false;
                }
            }
            return true;
        }

        static int[] processGames(int i,
            int[] localCount,
            int gameMode,
            List<List<int>[]> cardsPerPlayer,
            List<int> trumps,
            List<int> firstPlayers,
            List<int[]> pointsPerPlayer,
            List<int[]> pointsFromTrumpPerPlayer,
            List<int[]> trumpsPerPlayer,
            List<int[]> acesPerPlayer,
            List<int[]> sevensPerPlayer,
            List<int> teamHasTrumpAce,
            List<int[]> suitsPerPlayer,
            List<int[]> firstToPlay,
            List<int> finalBotTeamPoints,
            List<ulong[]> timePerTrick,
            Object allGamesLock)
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random randomNumber = new Random(seed);
            string[] playersNames = new string[4];
            ArtificialPlayer[] players = new ArtificialPlayer[4];
            List<List<int>> playersHands;

            Deck deck = new Deck();
            int trump = randomNumber.Next(0, 4);
            playersHands = deck.SampleHands(new int[] { 10, 10, 10, 10 });
            //while (!checkHands(playersHands, trump))
            //{
            //    playersHands = deck.SampleHands(new int[] { 10, 10, 10, 10 });
            //    localCount[3]++;
            //}

            List<int>[] handsPerPlayer = new List<int>[4];
            for (int k = 0; k < 4; k++)
            {
                handsPerPlayer[k] = new List<int>(10);
                for (int j = 0; j < 10; j++)
                {
                    handsPerPlayer[k].Add(playersHands[k][j]);
                }
            }
            int[] initialPoints = new int[4];
            initialPoints[0] = Sueca.CountPoints(playersHands[0]);
            initialPoints[1] = Sueca.CountPoints(playersHands[1]);
            initialPoints[2] = Sueca.CountPoints(playersHands[2]);
            initialPoints[3] = Sueca.CountPoints(playersHands[3]);
            int[] initialPointsFromTrumps = new int[4];
            initialPointsFromTrumps[0] = Sueca.CountPointsFromSuit(playersHands[0], trump);
            initialPointsFromTrumps[1] = Sueca.CountPointsFromSuit(playersHands[1], trump);
            initialPointsFromTrumps[2] = Sueca.CountPointsFromSuit(playersHands[2], trump);
            initialPointsFromTrumps[3] = Sueca.CountPointsFromSuit(playersHands[3], trump);
            int[] initialTrumps = new int[4];
            initialTrumps[0] = Sueca.CountCardsFromSuit(playersHands[0], trump);
            initialTrumps[1] = Sueca.CountCardsFromSuit(playersHands[1], trump);
            initialTrumps[2] = Sueca.CountCardsFromSuit(playersHands[2], trump);
            initialTrumps[3] = Sueca.CountCardsFromSuit(playersHands[3], trump);
            int[] initialAces = new int[4];
            initialAces[0] = Sueca.CountCardsFromRank(playersHands[0], (int)Rank.Ace);
            initialAces[1] = Sueca.CountCardsFromRank(playersHands[1], (int)Rank.Ace);
            initialAces[2] = Sueca.CountCardsFromRank(playersHands[2], (int)Rank.Ace);
            initialAces[3] = Sueca.CountCardsFromRank(playersHands[3], (int)Rank.Ace);
            int[] initialSevens = new int[4];
            initialSevens[0] = Sueca.CountCardsFromRank(playersHands[0], (int)Rank.Seven);
            initialSevens[1] = Sueca.CountCardsFromRank(playersHands[1], (int)Rank.Seven);
            initialSevens[2] = Sueca.CountCardsFromRank(playersHands[2], (int)Rank.Seven);
            initialSevens[3] = Sueca.CountCardsFromRank(playersHands[3], (int)Rank.Seven);
            int[] countSuits = new int[4];
            countSuits[0] = Sueca.CountSuits(playersHands[0]);
            countSuits[1] = Sueca.CountSuits(playersHands[1]);
            countSuits[2] = Sueca.CountSuits(playersHands[2]);
            countSuits[3] = Sueca.CountSuits(playersHands[3]);
            int botTeamHasTrumpAce = 0;
            if (Sueca.HasTrumpAce(playersHands[0], trump) || Sueca.HasTrumpAce(playersHands[2], trump))
            {
                botTeamHasTrumpAce++;
            }

            //MinMaxGame game = new MinMaxGame(10, playersHands, trump, null, 0, 0);
            int currentPlayerID = i % 4;
            int first = currentPlayerID;
            SuecaGame game = new SuecaGame(trump, first);
            int[] firstPlayer = new int[4] { 0, 0, 0, 0 };
            firstPlayer[first] = 1;
            ulong[] timeTemp = new ulong[10];

            switch (gameMode)
            {
                case 1:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "Random2";
                    players[2] = new RandomPlayer(2, playersHands[2]);
                    playersNames[3] = "Random3";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 2:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "Smart2";
                    players[2] = new SmartPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 3:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Smart2";
                    players[1] = new SmartPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smart3";
                    players[2] = new SmartPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random1";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 4:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Smart2";
                    players[1] = new SmartPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smart3";
                    players[2] = new SmartPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Smart4";
                    players[3] = new SmartPlayer(3, playersHands[3], trump);
                    break;
                case 5:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smart2";
                    players[2] = new SmartPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 6:
                    playersNames[0] = "Smartest1";
                    players[0] = new SmartestPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smartest2";
                    players[2] = new SmartestPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 7:
                    playersNames[0] = "Smartest1";
                    players[0] = new SmartestPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smartest2";
                    players[2] = new SmartestPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 8:
                    playersNames[0] = "RuleBased1";
                    players[0] = new RuleBasedPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 9:
                    playersNames[0] = "RBO1";
                    players[0] = new RBOPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "RBO2";
                    players[2] = new RBOPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 10:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Smart1";
                    players[1] = new SmartPlayer(1, playersHands[1], trump);
                    playersNames[2] = "TrickPlayer2";
                    players[2] = new TrickPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Smart2";
                    players[3] = new SmartPlayer(3, playersHands[3], trump);
                    break;
                case 11:
                    playersNames[0] = "RuleBased1";
                    players[0] = new RuleBasedPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased3";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased14";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 12:
                    playersNames[0] = "RuleBased1";
                    players[0] = new RuleBasedPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RandomPlayer3";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "RandomPlayer2";
                    players[2] = new RandomPlayer(2, playersHands[2]);
                    playersNames[3] = "RandomPlayer4";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 13:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "Random2";
                    players[2] = new RandomPlayer(2, playersHands[2]);
                    playersNames[3] = "Random3";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 14:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Random1";
                    players[1] = new RandomPlayer(1, playersHands[1]);
                    playersNames[2] = "TrickPlayer2";
                    players[2] = new TrickPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Random2";
                    players[3] = new RandomPlayer(3, playersHands[3]);
                    break;
                case 15:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased3";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 16:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "TrickPlayer2";
                    players[2] = new TrickPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased2";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 17:
                    playersNames[0] = "TrickPlayer1";
                    players[0] = new TrickPlayer(0, playersHands[0], trump);
                    playersNames[1] = "TrickPlayer2";
                    players[1] = new TrickPlayer(1, playersHands[1], trump);
                    playersNames[2] = "TrickPlayer3";
                    players[2] = new TrickPlayer(2, playersHands[2], trump);
                    playersNames[3] = "TrickPlayer4";
                    players[3] = new TrickPlayer(3, playersHands[3], trump);
                    break;
                case 18:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased3";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 19:
                    playersNames[0] = "RBO1";
                    players[0] = new RBOPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBasedPlayer";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                case 20:
                    playersNames[0] = "Smart1";
                    players[0] = new SmartPlayer(0, playersHands[0], trump);
                    playersNames[1] = "Smart2";
                    players[1] = new SmartPlayer(1, playersHands[1], trump);
                    playersNames[2] = "Smart3";
                    players[2] = new SmartPlayer(2, playersHands[2], trump);
                    playersNames[3] = "Smart4";
                    players[3] = new SmartPlayer(3, playersHands[3], trump);
                    break;
                case 21:
                    playersNames[0] = "Hybrid1";
                    players[0] = new HybridPlayer(0, playersHands[0], trump);
                    playersNames[1] = "RuleBased1";
                    players[1] = new RuleBasedPlayer(1, playersHands[1], trump);
                    playersNames[2] = "RuleBased2";
                    players[2] = new RuleBasedPlayer(2, playersHands[2], trump);
                    playersNames[3] = "RuleBased3";
                    players[3] = new RuleBasedPlayer(3, playersHands[3], trump);
                    break;
                default:
                    break;
            }


            for (int j = 0; j < 40; j++)
            {
                int chosenCard;

                if (currentPlayerID == 0)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    chosenCard = players[currentPlayerID].Play();
                    sw.Stop();
                    //long time = sw.ElapsedMilliseconds;
                    TimeSpan ts = sw.Elapsed;
                    ulong realTime = (ulong) ts.Minutes * 60000 + (ulong) ts.Seconds * 1000 + (ulong) ts.Milliseconds;
                    //Console.WriteLine("real: " + realTime + " time: " + time);
                    int trick = j / 4;
                    timeTemp[trick] = timeTemp[trick] + (ulong)realTime;
                }
                else
                {
                    chosenCard = players[currentPlayerID].Play();
                }
                game.PlayCard(currentPlayerID, chosenCard);
                for (int k = 0; k < 4; k++)
                {
                    players[k].AddPlay(currentPlayerID, chosenCard);
                }
                currentPlayerID = game.GetNextPlayerId();
            }

            int[] points = game.GetGamePoints();
            if (points[0] == 60)
            {
                localCount[0]++;
            }
            else if (points[0] > 60)
            {
                localCount[1]++;
            }
            else
            {
                localCount[2]++;
            }

            if (SAVE_RESULTS)
            {
                lock (allGamesLock)
                {
                    cardsPerPlayer.Add(handsPerPlayer);
                    trumps.Add(trump);
                    firstPlayers.Add(first);
                    pointsPerPlayer.Add(initialPoints);
                    pointsFromTrumpPerPlayer.Add(initialPointsFromTrumps);
                    trumpsPerPlayer.Add(initialTrumps);
                    acesPerPlayer.Add(initialAces);
                    sevensPerPlayer.Add(initialSevens);
                    teamHasTrumpAce.Add(botTeamHasTrumpAce);
                    suitsPerPlayer.Add(countSuits);
                    firstToPlay.Add(firstPlayer);
                    finalBotTeamPoints.Add(points[0]);
                    timePerTrick.Add(timeTemp);
                }
            }

            return localCount;
        }

        static long TimeMethod(Action methodToTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            methodToTime();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds / 1000;
        }
    }
}