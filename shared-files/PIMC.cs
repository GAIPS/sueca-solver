using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class PIMC
    {

        public int Execute(InformationSet infoSet)
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
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable());
                    cardValueInTrick = game.SampleGame(depthLimit, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

//            infoSet.PrintInfoSet();
            return infoSet.GetHighestCardIndex();
        }


        public void ExecuteTestVersion(InformationSet infoSet)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            SuecaGame game;

            if (possibleMoves.Count == 1)
            {
                Console.WriteLine("Only one move available: " + possibleMoves[0]);
                return;
            }
            int handSize = infoSet.GetHandSize();
            List<List<int>> playersHands = infoSet.Sample();
            game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable());
           


            // for (int j = 0; j < possibleMoves.Count; j++)
            // {
            int card = possibleMoves[0];
            int cardValueInTrick = game.SampleGame(Int32.MaxValue, card);
            game.PrintNumCuts();
            // int cardValueInTrick = game.SampleTrick(card);
            // Console.WriteLine("cardValueInTrick - " + card + " " + cardValueInTrick);
            infoSet.AddCardValue(card, cardValueInTrick);
            // }

            infoSet.PrintInfoSet();
        }

        static void setNandDepthLimit(out int N, out int depthLimit, int handSize)
        {
            switch (handSize)
            {
                case 10:
                    N = 30;
                    depthLimit = 3;
                    break;
                case 9:
                    N = 30;
                    depthLimit = 3;
                    break;
                case 8:
                    N = 40;
                    depthLimit = 3;
                    break;
                case 7:
                    N = 10;
                    depthLimit = 4;
                    break;
                case 6:
                    N = 5;
                    depthLimit = 10;
                    break;
                case 5:
                    N = 30;
                    depthLimit = 10;
                    break;
                case 4:
                    N = 200;
                    depthLimit = 10;
                    break;
                case 3:
                    N = 2000;
                    depthLimit = 10;
                    break;
                case 2:
                    N = 10000;
                    depthLimit = 10;
                    break;
                default:
                    Console.WriteLine("PIMC.setNandDepthLimit: Invalid handSize.");
                    N = 0;
                    depthLimit = 0;
                    break;
            }
        }

        public int ExecuteTestVersion2(InformationSet infoSet)
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
                    game = new SuecaGame(handSize, playersHands, infoSet.Trump, infoSet.GetCardsOnTable());
                    cardValueInTrick = game.SampleGame(depthLimit, card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

            infoSet.PrintInfoSet();
            return infoSet.GetHighestCardIndex();
        }
    }
}