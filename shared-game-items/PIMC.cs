using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class PIMC
    {

        private int N;

        public PIMC(int n)
        {
            N = n;
        }

        public int Execute(InformationSet infoSet)
        {
            int n = N;
            infoSet.CleanCardValues();
            List<int> possibleMoves = infoSet.GetPossibleMoves();

            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            for (int i = 0; i < n; i++)
            {
                List<List<int>> players = infoSet.Sample();
                List<int> p0 = players[0];
                List<int> p1 = players[1];
                List<int> p2 = players[2];
                List<int> p3 = players[3];

                SuecaGame game;
                int cardValueInTrick;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];

                    if (p0.Count > 5)
                    {
                        n = 1000;
                        // n = 1;
                        game = new SuecaGame(1, p0, p1, p2, p3, infoSet.Trump, infoSet.GetCardsOnTable());
                    }
                    else
                    {
                        n = 100;
                        // n = 1;
                        game = new SuecaGame(p0.Count, p0, p1, p2, p3, infoSet.Trump, infoSet.GetCardsOnTable());
                    }

                    cardValueInTrick = game.SampleGame(card);
                    infoSet.AddCardValue(card, cardValueInTrick);
                }
            }

            // infoSet.PrintInfoSet();
            int highestCard = infoSet.GetHighestCardIndex();
            return highestCard;
        }


        public void ExecuteTestVersion(InformationSet infoSet, List<int> hand, int num)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            SuecaGame game;

            if (possibleMoves.Count == 1)
            {
                Console.WriteLine("Only one move available: " + possibleMoves[0]);
                return;
            }

            if (num == 10)
            {
                List<List<int>> players = infoSet.Sample();
                List<int> p0 = players[0];
                List<int> p1 = players[1];
                List<int> p2 = players[2];
                List<int> p3 = players[3];
                game = new SuecaGame(p0.Count, p0, p1, p2, p3, infoSet.Trump, infoSet.GetCardsOnTable());
            }
            else
            {
                List<List<int>> players = infoSet.SampleThree(num);
                List<int> p0 = hand;
                List<int> p1 = players[0];
                List<int> p2 = players[1];
                List<int> p3 = players[2];
                game = new SuecaGame(p0.Count, p0, p1, p2, p3, infoSet.Trump, infoSet.GetCardsOnTable());

                SuecaGame.PrintCards("p0", p0);
                SuecaGame.PrintCards("p1", p1);
                SuecaGame.PrintCards("p2", p2);
                SuecaGame.PrintCards("p3", p3);
            }


            // for (int j = 0; j < possibleMoves.Count; j++)
            // {
            int card = possibleMoves[0];
            int cardValueInTrick = game.SampleGame(card);
            game.PrintNumCuts();
            // int cardValueInTrick = game.SampleTrick(card);
            // Console.WriteLine("cardValueInTrick - " + card + " " + cardValueInTrick);
            infoSet.AddCardValue(card, cardValueInTrick);
            // }

            infoSet.PrintInfoSet();
        }
    }
}