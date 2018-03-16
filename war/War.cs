using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace SuecaSolver
{
    public class War
    {
        public const int GAMEMODE = 9;
        public const int NUMSESSIONS = 50;
        public const bool PARALLEL = true;
        public const int NUM_THREADS = Sueca.WAR_NUM_THREADS;
        ////public const string SAVE_DIR = @"Z:\Devel\sueca-solver\results\";
        public const string SAVE_DIR = @"C:\temp\";
        //public const string SAVE_DIR = "results/";

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int gameMode, numSessions, numThreads;
            bool parallel, saveCards;
            

            if (args.Length == 6)
            {
                //Assuming the input is correct
                gameMode = Int16.Parse(args[0]);
                numSessions = Int16.Parse(args[1]);
                parallel = Boolean.Parse(args[2]);
                numThreads = Int16.Parse(args[3]);
                saveCards = Boolean.Parse(args[5]);
            }
            else
            {
                Console.WriteLine("Unspecified parameters. The program will use default parameters.");
                gameMode = GAMEMODE;
                numSessions = NUMSESSIONS;
                parallel = PARALLEL;
                numThreads = NUM_THREADS;
            }

            int firstTeamWins = 0, secondTeamWins = 0, draws = 0, nullGames = 0;

            //Shared data between threads!
            List<List<int>[]> cardsPerPlayer = new List<List<int>[]>(numSessions);
            List<int> trumps = new List<int>(numSessions);
            List<int> firstPlayers = new List<int>(numSessions);
            List<int> finalBotTeamPoints = new List<int>(numSessions);
            List<ulong[]> timePerTrick = new List<ulong[]>(numSessions);
            Object allGamesLock = new Object();

            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA TEST WAR ||||||||||||||||||||");
            Console.WriteLine("");
            switch (gameMode)
            {
                case 1:
                    Console.WriteLine("Mode 1 (1 RuleBased 3 Random)");
                    break;
                case 2:
                    Console.WriteLine("Mode 2 (1 TrickPlayer 3 RuleBased)");
                    break;
                case 3:
                    Console.WriteLine("Mode 3 (1 Smart 3 RuleBased)");
                    break;
                case 4:
                    Console.WriteLine("Mode 4 (1 TimeLimited 3 RuleBased)");
                    break;
                case 5:
                    Console.WriteLine("Mode 5 (1 RBO 3 RuleBased)");
                    break;
                case 6:
                    Console.WriteLine("Mode 6 (1 Hybrid 3 RuleBased)");
                    break;
                case 7:
                    Console.WriteLine("Mode 7 (1 Worst 3 Smart)");
                    break;
                case 8:
                    Console.WriteLine("Mode 8 (1 Worst 3 RuleBased)");
                    break;
                case 9:
                    Console.WriteLine("Mode 9 (1 Worst 1 RuleBased VS 1 RBO 1 UleBased)");
                    break;
                case 10:
                    Console.WriteLine("Mode 10 (2 Worst 2 RuleBased)");
                    break;
                default:
                    break;
            }
            Console.WriteLine("#Games: " + numSessions);


            if (parallel)
            {
                Parallel.For(0, numSessions,
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
                    });
            }
            else
            {
                for (int i = 0; i < numSessions; i++)
                {
                    int[] localCount = new int[14];
                    processGames(i,
                        localCount,
                        gameMode,
                        cardsPerPlayer,
                        trumps,
                        firstPlayers,
                        finalBotTeamPoints,
                        timePerTrick,
                        allGamesLock);
                    draws += localCount[0];
                    firstTeamWins += localCount[1];
                    secondTeamWins += localCount[2];
                    nullGames += localCount[3];
                }
            }


            Console.WriteLine("");
            Console.WriteLine("----------------- Summary -----------------");
            Console.WriteLine("FirstTeam won " + firstTeamWins + "/" + numSessions);
            Console.WriteLine("SecondTeam 1 won " + secondTeamWins + "/" + numSessions);
            Console.WriteLine("Draws " + draws + "/" + numSessions);
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
            List<int> finalBotTeamPoints,
            List<ulong[]> timePerTrick,
            Object allGamesLock)
        {

            int team0sessionResult = 0;
            int team1sessionResult = 0;

            for (int z = 0; z < 3; z++)
            {

                int seed = Guid.NewGuid().GetHashCode();
                Random randomNumber = new Random(seed);
                ArtificialPlayer[] players = new ArtificialPlayer[4];
                List<List<int>> playersHands = new List<List<int>>(
                    new List<int>[] {
                    new List<int>(10),
                    new List<int>(10),
                    new List<int>(10),
                    new List<int>(10) });
                Deck deck = new Deck();
                deck.SampleHands(ref playersHands);
                int currentPlayerID = i % 4;
                int first = currentPlayerID;
                int trumpPlayerId = (first - 1 + 4) % 4;
                int trumpCard = playersHands[trumpPlayerId][0];//the trump card is the first card of player that is seated before the one that will start the game
                int trumpSuit = Card.GetSuit(trumpCard);
                SuecaGame game = new SuecaGame(trumpSuit, first);
                int[] firstPlayer = new int[4] { 0, 0, 0, 0 };
                firstPlayer[first] = 1;
                ulong[] timeTemp = new ulong[10];

                List<int>[] handsPerPlayer = new List<int>[4];
                for (int k = 0; k < 4; k++)
                {
                    handsPerPlayer[k] = new List<int>(10);
                    for (int j = 0; j < 10; j++)
                    {
                        handsPerPlayer[k].Add(playersHands[k][j]);
                    }
                }

                switch (gameMode)
                {
                    case 1:
                        players[0] = new RuleBasedPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RandomPlayer(1, playersHands[1]);
                        players[2] = new RandomPlayer(2, playersHands[2]);
                        players[3] = new RandomPlayer(3, playersHands[3]);
                        break;
                    case 2:
                        players[0] = new TrickPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 3:
                        players[0] = new SmartPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 4:
                        players[0] = new TimeLimitedPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 5:
                        players[0] = new RBOPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 6:
                        players[0] = new HybridPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 7:
                        players[0] = new WorstPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new SmartPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new SmartPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new SmartPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 8:
                        players[0] = new WorstPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new RuleBasedPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 9:
                        players[0] = new WorstPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RBOPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new WorstPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new SmartPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
                        break;
                    case 10:
                        players[0] = new WorstPlayer(0, playersHands[0], trumpCard, trumpPlayerId);
                        players[1] = new RuleBasedPlayer(1, playersHands[1], trumpCard, trumpPlayerId);
                        players[2] = new WorstPlayer(2, playersHands[2], trumpCard, trumpPlayerId);
                        players[3] = new RuleBasedPlayer(3, playersHands[3], trumpCard, trumpPlayerId);
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
                        TimeSpan ts = sw.Elapsed;
                        ulong realTime = (ulong)ts.Minutes * 60000 + (ulong)ts.Seconds * 1000 + (ulong)ts.Milliseconds;
                        int trick = j / 4;
                        timeTemp[trick] = timeTemp[trick] + (ulong)realTime;
                    }
                    else
                    {
                        chosenCard = players[currentPlayerID].Play();
                    }
                    game.PlayCard(currentPlayerID, chosenCard);
                    //Console.WriteLine("Player " + currentPlayerID + " has played " + Card.ToString(chosenCard));
                    for (int k = 0; k < 4; k++)
                    {
                        players[k].AddPlay(currentPlayerID, chosenCard);
                    }
                    currentPlayerID = game.GetNextPlayerId();
                }

                int[] points = game.GetGamePoints();
                if (points[0] == 120)
                {
                    team0sessionResult += 4;
                }
                else if (points[0] > 90)
                {
                    team0sessionResult += 2;
                }
                else if (points[0] > 60)
                {
                    team0sessionResult += 1;
                }
                else if (points[1] == 120)
                {
                    team1sessionResult += 4;
                }
                else if (points[1] > 90)
                {
                    team1sessionResult += 2;
                }
                else if (points[1] > 60)
                {
                    team1sessionResult += 1;
                }

            }
            if (team0sessionResult == team1sessionResult)
            {
                localCount[0]++;
            }
            else if (team0sessionResult > team1sessionResult)
            {
                localCount[1]++;
            }
            else
            {
                localCount[2]++;
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