using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HumanPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;
        private List<int> hand;
        private List<Move> game;
        private int trumpSuit;
        private int currentPlayIndex;
        private float[][] weightsPerClass;
        private int numClasses = 12;
        private int numFeatures = 17;
        private int[] classes = new int[] { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 16 };
        private List<KeyValuePair<int,float>> classProb = new List<KeyValuePair<int, float>>
        {
            new KeyValuePair<int, float>(1, 0),
            new KeyValuePair<int, float>(2, 0),
            new KeyValuePair<int, float>(4, 0),
            new KeyValuePair<int, float>(5, 0),
            new KeyValuePair<int, float>(6, 0),
            new KeyValuePair<int, float>(8, 0),
            new KeyValuePair<int, float>(9, 0),
            new KeyValuePair<int, float>(10, 0),
            new KeyValuePair<int, float>(12, 0),
            new KeyValuePair<int, float>(13, 0),
            new KeyValuePair<int, float>(14, 0),
            new KeyValuePair<int, float>(16, 0)
        };
        private List<int> followClasses = new List<int> { 5, 6, 8};

        public HumanPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            hand = new List<int>(initialHand);
            game = new List<Move>();
            trumpSuit = Card.GetSuit(trumpCard);
            currentPlayIndex = 0;
            infoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
            weightsPerClass = new float[numClasses][];
            string[] lines = System.IO.File.ReadAllLines("../../../state-inference/weights.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                weightsPerClass[i] = new float[numFeatures];
                string[] line = lines[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                {
                    weightsPerClass[i][j] = float.Parse(line[j]);
                }
            }
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
            if (playerID == _id)
            {
                hand.Remove(card);
            }
            Move move = new Move(playerID, card);
            game.Add(move);
            currentPlayIndex++;
        }

        public class ClassProbComparer : IComparer<KeyValuePair<int,float>>
        {
            public int Compare(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
            {
                if (b.Value > a.Value)
                    return 0;
                if ((a.Value > b.Value) || (a.Value == b.Value))
                    return -1;

                return 1;
            }
        }

        override public int Play()
        {
            List<int> possibleMoves = Sueca.PossibleMoves(hand, infoSet.GetLeadSuit());

            int[] features = Sueca.GetFeaturesFromState(_id, hand, game, currentPlayIndex - 1, trumpSuit);

            for (int i = 0; i < weightsPerClass.Length; i++)
            {
                float total = 0;
                for (int j = 0; j < weightsPerClass[i].Length; j++)
                {
                    if (j == weightsPerClass[i].Length - 1)
                    {
                        total += weightsPerClass[i][j];
                    }
                    else
                    {
                        total += features[j] * weightsPerClass[i][j];
                    }
                }
                classProb.Add(new KeyValuePair<int, float>(classes[i], total));
            }

            classProb.Sort(new ClassProbComparer());

            //float max = float.MinValue;
            //int classIndex = 0;
            //for (int i = 0; i < classesProba.Length; i++)
            //{
            //    if (classesProba[i] > max && (possibleMoves.Count == hand.Count || followClasses.Contains(classes[i])))
            //    {
            //        max = classesProba[i];
            //        classIndex = i;
            //    }
            //}

            int classification = classes[classIndex];
            return Sueca.ChooseCardFromLabel(classification, possibleMoves, infoSet.GetLeadSuit(), trumpSuit);
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return infoSet.GetWinnerAndPointsAndTrickNumber();
        }

        public int GetCurrentTrickWinner()
        {
            return infoSet.GetCurrentTrickWinner();
        }

        public int GetCurrentTrickPoints()
        {
            return infoSet.GetCurrentTrickPoints();
        }

        public bool HasNewTrickWinner()
        {
            return infoSet.HasNewTrickWinner();
        }

        public bool HasNewTrickTeamWinner()
        {
            return infoSet.HasNewTrickTeamWinner();
        }

        public int GetTrickIncrease()
        {
            return infoSet.GetTrickIncrease();
        }

        public float PointsPercentage()
        {
            float alreadyMadePoints = infoSet.MyTeamPoints + infoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 0.5f;
            }
            return infoSet.MyTeamPoints / alreadyMadePoints;
        }

        public int GetHandSize()
        {
            return infoSet.GetHandSize();
        }

        public string GetLastPlayInfo()
        {
            return infoSet.GetLastPlayInfo();
        }

        public bool IsLastPlayOfTrick()
        {
            return infoSet.IsLastPlayOfTrick();
        }


        //attribute the event to the winner when he is from my team and blame himself or the partner when winner is an opponent
        public int GetResposibleForLastTrick()
        {
            return infoSet.GetCurrentTrickResponsible();
        }
    }
}