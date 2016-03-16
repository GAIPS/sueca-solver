using System;
using System.Collections.Generic;
using Microsoft.SolverFoundation.Services;

namespace SuecaSolver
{
    public class Deck
    {

        private Random random;
        private List<int> deck;

        public Deck()
        {
            random = new Random();
            deck = new List<int>(40);

            for (int i = 0; i < 40; i++)
            {
                deck.Add(i);
            }
        }

        public Deck(List<int> cards)
        {
            random = new Random();
            deck = new List<int>(40 - cards.Count);

            for (int i = 0; i < 40; i++)
            {
                if (!cards.Contains(i))
                {
                    deck.Add(i);
                }
            }
        }

        public int GetRandomCard()
        {
            int randomIndex = random.Next(0, deck.Count);
            return deck[randomIndex];
        }

        public void RemoveCard(int card)
        {
            deck.Remove(card);
        }

        public void Add(int card)
        {
            deck.Add(card);
        }

        public List<int> GetHand(int handSize)
        {
            List<int> hand = new List<int>(handSize);
            for (int randomIndex = 0, i = 0; i < handSize; i++)
            {
                randomIndex = random.Next(0, deck.Count);
                hand.Add(deck[randomIndex]);
                deck.RemoveAt(randomIndex);
            }
            hand.Sort();
            return hand;
        }


        public void SampleHands(ref List<List<int>> hands)
        {
            List<int> deckCopy = new List<int>(deck);

            for (int i = 0; i < hands.Count; i++)
            {
                int handSize = hands[i].Capacity - hands[i].Count;
                for (int randomIndex = 0, j = 0; j < handSize; j++)
                {
                    randomIndex = random.Next(0, deckCopy.Count);
                    int randomCard = deckCopy[randomIndex];
                    hands[i].Add(randomCard);
                    deckCopy.RemoveAt(randomIndex);
                }
                //players[i].Sort();
            }
        }


        private List<List<int>> getDomains(int[] playerIDs, int[] handSizes)
        {
            List<List<int>> list = new List<List<int>>(3);
            for (int i = 0; i < 3; i++)
            {
                list.Add(new List<int>(handSizes[i]));
                for (int j = 0; j < handSizes[i]; j++)
                {
                    list[i].Add(playerIDs[i] * 10 + j);
                }
            }
            return list;
        }


        private List<T> shuffle<T>(List<T> cards)
        {
            int deckSize = cards.Count;
            List<T> shuffled = new List<T>(deckSize);
            for (int randomIndex = 0, j = 0; j < deckSize; j++)
            {
                randomIndex = random.Next(0, cards.Count);
                T randomCard = cards[randomIndex];
                shuffled.Add(randomCard);
                cards.RemoveAt(randomIndex);
            }
            return shuffled;
        }

        //Sampling a card distribution considering which suits the players have
        //It uses a CSP from Microsoft SolverFoundation
        public List<List<int>> SampleHands(Dictionary<int, List<int>> suitHasPlayer, int[] playerIDs, ref List<List<int>> hands)
        {
            int[] handSizes = new int[] { hands[0].Capacity - hands[0].Count, hands[1].Capacity - hands[1].Count, hands[2].Capacity - hands[2].Count };
            if (deck.Count != handSizes[0] + handSizes[1] + handSizes[2])
            {
                //Remover este bocado de codigo se o erro nunca mais ocorrer
                Console.WriteLine("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + "] - PROBLEM! - deck.Count: " + deck.Count + " P0: " + handSizes[0] + " P1: " + handSizes[1] + " P2: " + handSizes[2] + " deck: " + deckToString());
                Console.Out.Flush();
                System.Environment.Exit(1);
            }

            deck = shuffle(deck);
            var solver = SolverContext.GetContext();
            var model = solver.CreateModel();
            List<Decision> decisions = new List<Decision>(deck.Count);
            List<List<int>> players = getDomains(playerIDs, handSizes);
            List<int> player1 = players[0];
            List<int> player1Copy = new List<int>(player1);
            List<int> player2 = players[1];
            List<int> player3 = players[2];
            List<int> player3Copy = new List<int>(player3);
            Domain domain1 = null, domain2 = null, domain3 = null, domain12 = null, domain23 = null, domain13 = null, domain123 = null;

            if (player1.Count > 0)
            {
                domain1 = Domain.Set(player1.ToArray());
            }
            if (player2.Count > 0)
            {
                domain2 = Domain.Set(player2.ToArray());
            }
            if (player3.Count > 0)
            {
                domain3 = Domain.Set(player3.ToArray());
            }
            if (player1.Count > 0 && player2.Count > 0)
            {
                player1.AddRange(player2);
                domain12 = Domain.Set(player1.ToArray());
            }
            if (player2.Count > 0 && player3.Count > 0)
            { 
                player2.AddRange(player3);
                domain23 = Domain.Set(player2.ToArray());
            }
            if (player1.Count > 0 && player3.Count > 0)
            {
                player3.AddRange(player1Copy);
                domain13 = Domain.Set(player3.ToArray());
            }
            if (player1.Count > 0 && player2.Count > 0 && player3.Count > 0)
            {
                player1.AddRange(player3Copy);
                domain123 = Domain.Set(player1.ToArray());
            }


            for (int i = 0; i < deck.Count; i++)
            {
                int card = deck[i];
                List<int> playersThatHaveSuit = suitHasPlayer[Card.GetSuit(card)];
                Decision d;

                if (playersThatHaveSuit.Count == 3)
                {
                    d = new Decision(domain123, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == playerIDs[0] && playersThatHaveSuit[1] == playerIDs[1])
                {
                    d = new Decision(domain12, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == playerIDs[0] && playersThatHaveSuit[1] == playerIDs[2])
                {
                    d = new Decision(domain13, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == playerIDs[1] && playersThatHaveSuit[1] == playerIDs[2])
                {
                    d = new Decision(domain23, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 1 && playersThatHaveSuit[0] == playerIDs[0])
                {
                    d = new Decision(domain1, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 1 && playersThatHaveSuit[0] == playerIDs[1])
                {
                    d = new Decision(domain2, "c" + card);
                }
                else if (playersThatHaveSuit.Count == 1 && playersThatHaveSuit[0] == playerIDs[2])
                {
                    d = new Decision(domain3, "c" + card);
                }
                else
                {
                    solver.ClearModel();
                    return null;
                }

                decisions.Add(d);
                model.AddDecision(d);
            }

            model.AddConstraint("allDiff", Model.AllDifferent(decisions.ToArray()));
            var solution = solver.Solve();


            while (solution.Quality != SolverQuality.Feasible)
            {
                Console.Write("CSP Problem - solution {0}", solution.Quality);
                System.Environment.Exit(1);
            }

            List<List<int>> cardsPerPlayer = new List<List<int>>(3);
            cardsPerPlayer.Add(new List<int>(handSizes[0]));
            cardsPerPlayer.Add(new List<int>(handSizes[1]));
            cardsPerPlayer.Add(new List<int>(handSizes[2]));

            for (int i = 0; i < deck.Count; i++)
            {
                int decision = Convert.ToInt16(decisions[i].ToString());
                decision = decision / 10;
                if (decision == playerIDs[0])
                {
                    cardsPerPlayer[0].Add(deck[i]);
                }
                else if (decision == playerIDs[1])
                {
                    cardsPerPlayer[1].Add(deck[i]);
                }
                else if (decision == playerIDs[2])
                {
                    cardsPerPlayer[2].Add(deck[i]);
                }
                else
                {
                    Console.WriteLine("Deck::SampleHands(with CSP) >> Unkown decision");
                }
            }
            solver.ClearModel();
            return cardsPerPlayer;
        }


        private string deckToString()
        {
            string str = "<";
            foreach (var card in deck)
            {
                str += Card.ToString(card);
                str += ",";
            }
            str += ">";

            return str;
        }
    }
}