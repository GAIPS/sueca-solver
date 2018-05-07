using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HumanNode : PlayerNode
    {
        private List<Move> game;
        private int trumpSuit;
        private int currentPlayIndex;

        private float[][] weightsPerClass;
        private int numClasses = 12;
        private int numFeatures = 17;
        private int[] classes = new int[] { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 16 };
        private List<int> followClasses = new List<int> { 5, 6, 8};

        public HumanNode(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id, initialHand, trumpCard, trumpPlayerId)
        {
            //hand = new List<int>(initialHand);
            game = new List<Move>();
            trumpSuit = Card.GetSuit(trumpCard);
            currentPlayIndex = 0;
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

        public class ClassProbComparer : IComparer<KeyValuePair<int,float>>
        {
            public int Compare(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
            {
                if (b.Value > a.Value)
                {
                    return 1;
                }
                if (b.Value < a.Value)
                {
                    return -1;
                }
                return 0;
            }
        }

        override public int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            if (Sueca.UTILITY_FUNC == 2 && pig.IsAnyTeamWinning())
            {
                return pig.EvalGame2();
            }
            if (pig.reachedDepthLimit(depthLimit) || pig.IsEndGame())
            {
                return pig.EvalGame1();
            }
            
            List<int> possibleMoves = Sueca.PossibleMoves(Hand, InfoSet.GetLeadSuit());
            int[] features = Sueca.GetFeaturesFromState(Id, Hand, game, currentPlayIndex, trumpSuit, ref InfoSet.suitHasPlayer);
            int[] filteredClasses;
            if ((currentPlayIndex % 4) == 0) //lead
            {
                filteredClasses = new int[] { 1, 2, 4 };
            }
            else if (possibleMoves.Count < Hand.Count)
            {
                filteredClasses = new int[] { 5, 6, 8 };
            }
            else
            {
                filteredClasses = new int[] { 9, 10, 12, 13, 14, 16 };
            }
            List<KeyValuePair<int, float>> classProbs = Sueca.GetClassesProbabilities(possibleMoves, features, classes, filteredClasses, weightsPerClass);
            int chosenCard = -1;

            foreach (var classificationProb in classProbs)
            {
                int classification = classificationProb.Key;
                chosenCard = Sueca.ChooseCardFromLabel(classification, possibleMoves, InfoSet.GetLeadSuit(), trumpSuit);
                if (chosenCard != -1)
                {
                    break;
                }
            }
            if (chosenCard == -1)
            {
                Console.WriteLine("No other classification is suitable for choosing a card.");
                int randomIndex = new Random().Next(0, Hand.Count);
                chosenCard = Hand[randomIndex];
            }

            Move move = new Move(Id, chosenCard);
            pig.ApplyMove(move);

            int value = pig.GetNextPlayer().PlayGame(pig, alpha, beta, depthLimit);
            pig.UndoMove(move);

            return value;
        }

        public override void ApplyMove(Move move)
        {
            currentPlayIndex++;
            game.Add(move);
            if (move.PlayerId == Id)
            {
                if (Hand.Remove(move.Card) == false)
                {
                    //Console.WriteLine("PLAYERNODE Trying to remove an nonexisting card!!!");
                }
            }
            else
            {
            }
            InfoSet.AddPlay(move.PlayerId, move.Card);
        }

        public override void UndoMove(Move move)
        {
            currentPlayIndex--;
            game.RemoveAt(game.Count - 1);
            if (move.PlayerId == Id)
            {
                Hand.Add(move.Card);
            }
            else
            {
            }
            InfoSet.RemovePlay(move.PlayerId, move.Card);
        }
    }
}