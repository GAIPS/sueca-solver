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
        private int numClasses = 12;
        private int numFeatures = 17;
        private int[] classes = new int[] { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 16 };

        public HumanPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            hand = new List<int>(initialHand);
            game = new List<Move>();
            trumpSuit = Card.GetSuit(trumpCard);
            currentPlayIndex = 0;
            InfoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
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
            int[] features = Sueca.GetFeaturesFromState(_id, hand, game, currentPlayIndex, trumpSuit, ref InfoSet.suitHasPlayer);
            int[] filteredClasses;
            if ((currentPlayIndex % 4) == 0) //lead
            {
                filteredClasses = new int[] { 1, 2, 4 };
            }
            else if (possibleMoves.Count < hand.Count)
            {
                filteredClasses = new int[] { 5, 6, 8 };
            }
            else
            {
                filteredClasses = new int[] { 9, 10, 12, 13, 14, 16 };
            }

            List<KeyValuePair<int, float>> classProbs = Sueca.GetClassesProbabilities(possibleMoves, features, classes, filteredClasses, weightsPerClass);

            foreach (var classificationProb in classProbs)
            {
                int classification = classificationProb.Key;
                int card = Sueca.ChooseCardFromLabel(classification, possibleMoves, InfoSet.GetLeadSuit(), trumpSuit);
                if (card != -1)
                {
                    return card;
                }
            }

            Console.WriteLine("No other classification is suitable for choosing a card.");
            int randomIndex = new Random().Next(0, hand.Count);
            return hand[randomIndex];
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