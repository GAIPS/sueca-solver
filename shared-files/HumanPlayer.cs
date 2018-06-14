using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HumanPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;
        private List<int> hand;
        private List<Move> game;
        private int trumpSuit;
        private int currentPlayIndex;

        private float[][] weightsPerClass;
        private int numClasses = 30;
        private int numFeatures = 41;
        private int[] classes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };

        public HumanPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            hand = new List<int>(initialHand);
            game = new List<Move>();
            trumpSuit = Card.GetSuit(trumpCard);
            currentPlayIndex = 0;
            InfoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
            weightsPerClass = new float[numClasses][];
            //string[] lines = System.IO.File.ReadAllLines("../../../state-inference/weights.txt");
            string[] lines = System.IO.File.ReadAllLines("state-inference/weights.txt");
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
            InfoSet.AddPlay(playerID, card);
            if (playerID == _id)
            {
                hand.Remove(card);
            }
            Move move = new Move(playerID, card);
            game.Add(move);
            currentPlayIndex++;
        }

        override public int Play()
        {
            List<int> possibleMoves = Sueca.PossibleMoves(hand, InfoSet.GetLeadSuit());
            int chosenCard = -1;

            if (possibleMoves.Count == 1)
            {
                chosenCard = possibleMoves[0];
            }
            else
            {   
                float[] features = Sueca.GetFeaturesFromState(_id, hand, game, currentPlayIndex, trumpSuit, ref InfoSet.suitHasPlayer);
                List<int> filteredClasses = Sueca.GetPossibleLabels(possibleMoves, InfoSet.GetLeadSuit(), trumpSuit);
                
                List<KeyValuePair<int, float>> classProbs = Sueca.GetClassesProbabilities(possibleMoves, features, classes, weightsPerClass);
                /* 
                Console.WriteLine("----filtered----");
                foreach (var item in filteredClasses)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine("----");
                */

                foreach (var classificationProb in classProbs)
                {
                    int classification = classificationProb.Key;
                    if (filteredClasses.Contains(classification))
                    {
                        chosenCard = Sueca.ChooseCardFromLabel(classification, possibleMoves, InfoSet.GetLeadSuit(), trumpSuit);
                        if (chosenCard != -1)
                        {
                            //Console.WriteLine("Class " + classification + " with prob " + classificationProb.Value + " WAS not present");
                            break;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Class " + classification + " with prob " + classificationProb.Value + " not present");
                    }
                }
                if (chosenCard == -1)
                {
                    Console.WriteLine("No other classification is suitable for choosing a card.");
                    int randomIndex = new Random().Next(0, hand.Count);
                    chosenCard = hand[randomIndex];
                }
            }

            return chosenCard;
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return InfoSet.GetWinnerAndPointsAndTrickNumber();
        }

        public int GetCurrentTrickWinner()
        {
            return InfoSet.GetCurrentTrickWinner();
        }

        public int GetCurrentTrickPoints()
        {
            return InfoSet.GetCurrentTrickPoints();
        }

        public bool HasNewTrickWinner()
        {
            return InfoSet.HasNewTrickWinner();
        }

        public bool HasNewTrickTeamWinner()
        {
            return InfoSet.HasNewTrickTeamWinner();
        }

        public int GetTrickIncrease()
        {
            return InfoSet.GetTrickIncrease();
        }

        public float PointsPercentage()
        {
            float alreadyMadePoints = InfoSet.MyTeamPoints + InfoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 0.5f;
            }
            return InfoSet.MyTeamPoints / alreadyMadePoints;
        }

        public int GetHandSize()
        {
            return InfoSet.GetHandSize();
        }

        public string GetLastPlayInfo()
        {
            return InfoSet.GetLastPlayInfo();
        }

        public bool IsLastPlayOfTrick()
        {
            return InfoSet.IsLastPlayOfTrick();
        }


        //attribute the event to the winner when he is from my team and blame himself or the partner when winner is an opponent
        public int GetResposibleForLastTrick()
        {
            return InfoSet.GetCurrentTrickResponsible();
        }
    }
}