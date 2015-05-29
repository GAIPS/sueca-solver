using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RandomPlayer : ArtificialPlayer
    {
        private List<int> hand;
        private int leadSuit;
        private int currentPlay;
        private Random randomNumber;

        public RandomPlayer(List<int> initialHand)
        {
            hand = new List<int>(initialHand);
            currentPlay = 0;
            randomNumber = new Random(Guid.NewGuid().GetHashCode());
        }

        override public void AddPlay(int playerID, int card)
        {
            if (currentPlay == 0)
            {
                leadSuit = Card.GetSuit(card);
            }
            currentPlay = (currentPlay + 1) % 4;
        }


        override public int Play()
        {
            if (currentPlay == 0)
            {
                leadSuit = (int)Suit.None;
            }

            List<int> possibleMoves = SuecaGame.PossibleMoves(hand, leadSuit);
            int randomIndex = randomNumber.Next(0, possibleMoves.Count);
            int chosenCard = possibleMoves[randomIndex];
            hand.Remove(chosenCard);
            currentPlay = (currentPlay + 1) % 4;

            return chosenCard;
        }
    }
}