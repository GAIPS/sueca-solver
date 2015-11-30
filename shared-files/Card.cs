using System;
using System.Collections.Generic;

namespace SuecaSolver
{

    public static class Card
    {
        private static int[] cardValues = new int[10] { 0, 0, 0, 0, 0, 2, 3, 4, 10, 11 };
        private static string[] cardRanks = new string[10] { "2", "3", "4", "5", "6", "Q", "J", "K", "7", "A" };


        public static int Create(Rank rank, Suit suit)
        {
            return (int)suit * 10 + (int)rank;
        }

        public static int GetRank(int card)
        {
            return card % 10;
        }

        public static int GetSuit(int card)
        {
            return (int)(card / 10);
        }

        public static int GetValue(int card)
        {
            int rank = card % 10;
            return cardValues[rank];
        }

        public static string ToString(int card)
        {
            string str = "";
            int rank = GetRank(card);
            int suit = GetSuit(card);

            str += cardRanks[rank];

            if (suit == 0)
            {
                str += "C";
            }
            else if (suit == 1)
            {
                str += "D";
            }
            else if (suit == 2)
            {
                str += "H";
            }
            else if (suit == 3)
            {
                str += "S";
            }
            else
            {
                Console.WriteLine("int.ToSring: Invalid Suit");
            }

            return str;
        }
    }


    public class AscendingComparer: IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == -1 || y == -1)
            {
                Console.WriteLine("AscendingComparer.Compare: InvalidCastException arguments");
                return 1;
            }
            else if (Card.GetRank(x) > Card.GetRank(y))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }


    public class DescendingComparer: IComparer<int>
    {


        public int Compare(int x, int y)
        {
            if (x == -1 || y == -1)
            {
                Console.WriteLine("AscendingComparer.Compare: InvalidCastException arguments");
                return 1;
            }
            else if (Card.GetRank(x) < Card.GetRank(y))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }


    public class AscendingCutComparer: IComparer<int>
    {

        private int trump;

        public AscendingCutComparer(int trumpSuit)
        {
            trump = trumpSuit;
        }


        public int Compare(int x, int y)
        {
            if (x == -1 || y == -1)
            {
                Console.WriteLine("AscendingComparer.Compare: InvalidCastException arguments");
                return 1;
            }
            else if (Card.GetSuit(x) == trump && Card.GetSuit(y) == trump && Card.GetRank(x) > Card.GetRank(y))
            {
                return -1;
            }
            else if (Card.GetSuit(x) == trump && Card.GetSuit(y) != trump)
            {
                return -1;
            }
            else if (Card.GetSuit(x) != trump && Card.GetSuit(y) != trump && Card.GetRank(x) > Card.GetRank(y))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }


    public class DescendingCutComparer: IComparer<int>
    {

        private int trump;

        public DescendingCutComparer(int trumpSuit)
        {
            trump = trumpSuit;
        }


        public int Compare(int x, int y)
        {
            if (x == -1 || y == -1)
            {
                Console.WriteLine("AscendingComparer.Compare: InvalidCastException arguments");
                return 1;
            }
            else if (Card.GetSuit(x) == trump && Card.GetSuit(y) == trump && Card.GetRank(x) < Card.GetRank(y))
            {
                return -1;
            }
            else if (Card.GetSuit(x) == trump && Card.GetSuit(y) != trump)
            {
                return -1;
            }
            else if (Card.GetSuit(x) != trump && Card.GetSuit(y) != trump && Card.GetRank(x) < Card.GetRank(y))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}