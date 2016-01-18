using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class PIMC
    {

        public int Execute(InformationSet infoSet, bool USE_CACHE = false)
        {
            infoSet.CleanCardValues();
            List<int> possibleMoves = infoSet.GetPossibleMoves();

            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            int N, depthLimit, handSize = infoSet.GetHandSize();
            setNandDepthLimit(out N, out depthLimit, handSize);
           
            for (int i = 0; i < N; i++)
            {
                List<List<int>> playersHands = infoSet.Sample();

                SuecaGame game;
                int cardValueInTrick;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE);
                    cardValueInTrick = game.SampleGame(depthLimit, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

            infoSet.calculateAverageCardValues(N);
            return infoSet.GetBestCard();
        }

        public int TrickExecute(InformationSet infoSet, bool USE_CACHE = false)
        {
            infoSet.CleanCardValues();
            List<int> possibleMoves = infoSet.GetPossibleMoves();

            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            int N = 500, depthLimit = 1, handSize = infoSet.GetHandSize();

            for (int i = 0; i < N; i++)
            {
                List<List<int>> playersHands = infoSet.Sample();

                SuecaGame game;
                int cardValueInTrick;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE);
                    cardValueInTrick = game.SampleGame(depthLimit, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

            infoSet.calculateAverageCardValues(N);
            return infoSet.GetBestCard();
        }


        public int ExecuteTestVersion(InformationSet infoSet, List<int> numIterations, bool USE_CACHE = false)
        {
            infoSet.CleanCardValues();
            List<int> possibleMoves = infoSet.GetPossibleMoves();

            int handSize = infoSet.GetHandSize();
            int N = numIterations[handSize - 1];

            for (int i = 0; i < N; i++)
            {
                List<List<int>> playersHands = infoSet.Sample();

                SuecaGame game;
                int cardValueInTrick;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE);
                    cardValueInTrick = game.SampleGame(10000, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

            infoSet.calculateAverageCardValues(N);
            return infoSet.GetBestCard();
        }


        public int ExecuteWithTimeLimit(InformationSet infoSet, bool USE_CACHE = false)
        {

            infoSet.CleanCardValues();
            List<int> possibleMoves = infoSet.GetPossibleMoves();

            int N, depthLimit, handSize = infoSet.GetHandSize();
            int n = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long time = sw.ElapsedMilliseconds;
            setNandDepthLimit(out N, out depthLimit, handSize);
            for (; time < 2000; )
            {
                n++;
                List<List<int>> playersHands = infoSet.Sample();

                SuecaGame game;
                int cardValueInTrick;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE);
                    cardValueInTrick = game.SampleGame(depthLimit, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
                time = sw.ElapsedMilliseconds;
            }

            sw.Stop();
            infoSet.calculateAverageCardValues(n);
            return infoSet.GetBestCard();
        }

        static void setNandDepthLimit(out int N, out int depthLimit, int handSize)
        {
            switch (handSize)
            {
                case 10:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 9:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 8:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 7:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 6:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 5:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 4:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 3:
                    N = 5;
                    depthLimit = 5;
                    break;
                case 2:
                    N = 5;
                    depthLimit = 5;
                    break;
                default:
                    Console.WriteLine("PIMC.setNandDepthLimit: Invalid handSize.");
                    N = 0;
                    depthLimit = 0;
                    break;
            }
        }
    }
}