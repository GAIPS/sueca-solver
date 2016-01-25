using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SuecaSolver
{
    public static class PIMC
    {

        public static int Execute(InformationSet infoSet, List<int> numIterations = null, List<int> depthLimits = null, bool USE_CACHE = false)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int card in possibleMoves)
            {
                dict.Add(card, 0);
            }


            int N, depthLimit, handSize = infoSet.GetHandSize();
            if (numIterations != null)
            {
                N = numIterations[handSize - 1];
                if (depthLimits != null)
                {
                    depthLimit = depthLimits[handSize - 1];
                }
                else
                {
                    depthLimit = 10;
                }
            }
            else
            {
                N = 50;
                depthLimit = 1;
            }
           
            for (int i = 0; i < N; i++)
            {
                List<List<int>> playersHands = infoSet.Sample();

                //MinMaxGame game;
                PerfectInformationGame game;
                int cardUtility;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    //game = new MinMaxGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE);
                    MaxNode p0 = new MaxNode(0, playersHands[0], false);
                    MinNode p1 = new MinNode(1, playersHands[1], false);
                    MaxNode p2 = new MaxNode(2, playersHands[2], false);
                    MinNode p3 = new MinNode(3, playersHands[3], false);
                    game = new PerfectInformationGame(p0, p1, p2, p3, handSize, infoSet.Trump, infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    cardUtility = game.SampleGame(depthLimit, card);
                    dict[card] += cardUtility; 
                }
            }

            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in dict)
            {
                if (cardValue.Value > bestValue)
                {
                    bestValue = (int)cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }


        public static int ExecuteWithTimeLimit(InformationSet infoSet, List<int> depthLimits, bool USE_CACHE = false)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int card in possibleMoves)
            {
                dict.Add(card, 0);
            }

            int depthLimit, handSize = infoSet.GetHandSize();
            int n = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long time = sw.ElapsedMilliseconds;
            depthLimit = depthLimits[handSize - 1];
            for (; time < 2000; )
            {
                n++;
                List<List<int>> playersHands = infoSet.Sample();

                //MinMaxGame game;
                PerfectInformationGame game;
                int cardUtility;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    //game = new MinMaxGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable(), infoSet.BotTeamPoints, infoSet.OtherTeamPoints, USE_CACHE); MaxNode p0 = new MaxNode(0, playersHands[0], false);
                    MaxNode p0 = new MaxNode(0, playersHands[0], false); 
                    MinNode p1 = new MinNode(1, playersHands[1], false);
                    MaxNode p2 = new MaxNode(2, playersHands[2], false);
                    MinNode p3 = new MinNode(3, playersHands[3], false);
                    game = new PerfectInformationGame(p0, p1, p2, p3, handSize, infoSet.Trump, infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    cardUtility = game.SampleGame(depthLimit, card);
                    dict[card] += cardUtility; 
                }
                time = sw.ElapsedMilliseconds;
            }

            sw.Stop();

            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in dict)
            {
                if (cardValue.Value > bestValue)
                {
                    bestValue = (int)cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }
    }
}