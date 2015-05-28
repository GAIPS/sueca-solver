using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SolverFoundation.Services;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuecaSolver
{
    public class Deck
    {

        private Random random;
        private List<int> deck;
        private static SolverContext solver;

        public Deck()
        {
            random = new Random();
            deck = new List<int>(40);
            solver = SolverContext.GetContext();

            for (int i = 0; i < 40; i++)
            {
                deck.Add(i);
            }
        }

        public Deck(List<int> cards)
        {
            random = new Random();
            deck = new List<int>(40 - cards.Count);
            solver = SolverContext.GetContext();

            for (int i = 0; i < 40; i++)
            {
                if (!cards.Contains(i))
                {
                    deck.Add(i);
                }
            }
        }

        public void RemoveCard(int card)
        {
            deck.Remove(card);
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


        public List<List<int>> SampleHands(int[] handSizes)
        {
            List<List<int>> players = new List<List<int>>();
            List<int> deckCopy = new List<int>(deck);

            for (int i = 0; i < handSizes.Length; i++)
            {
                if (deckCopy.Count == handSizes[i])
                {
                    players.Add(deckCopy);
                    break;
                }

                players.Add(new List<int>());
                for (int randomIndex = 0, j = 0; j < handSizes[i]; j++)
                {
                    randomIndex = random.Next(0, deckCopy.Count);
                    int randomCard = deckCopy[randomIndex];
                    players[i].Add(randomCard);
                    deckCopy.RemoveAt(randomIndex);
                }
                players[i].Sort();
            }

            return players;
        }


        private List<List<int>> getDomains(int[] handSizes)
        {
            List<List<int>> list = new List<List<int>>(3);
            for (int i = 0; i < 3; i++)
            {
                list.Add(new List<int>(handSizes[i]));
                for (int j = 0; j < handSizes[i]; j++)
                {
                    list[i].Add((i + 1) * 10 + j);
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

        //Sampling a card distribution consedering which suits the players have
        //It uses a CSP
        public List<List<int>> SampleHands(Dictionary<int,List<int>> suitHasPlayer, int[] handSizes)
        {
            Console.WriteLine("1");
            deck = shuffle(deck);
            var model = solver.CreateModel();
            Console.WriteLine("2 " + deck.Count + " p0 " + handSizes[0] + " p1 " + handSizes[1] + " p2 " + handSizes[2]);
            Decision[] decisions = new Decision[deck.Count];
            List<List<int>> players = getDomains(handSizes);
            List<int> player1 = shuffle(players[0]);
            List<int> player1Copy = new List<int>(player1);
            List<int> player2 = shuffle(players[1]);
            List<int> player3 = shuffle(players[2]);
            List<int> player3Copy = new List<int>(player3);
            Domain domain1 = Domain.Set(player1.ToArray());
            Domain domain2 = Domain.Set(player2.ToArray());
            Domain domain3 = Domain.Set(player3.ToArray());
            player1.AddRange(player2);
            player1 = shuffle(player1);
            Domain domain12 = Domain.Set(player1.ToArray());
            player2.AddRange(player3);
            player2 = shuffle(player2);
            Domain domain23 = Domain.Set(player2.ToArray());
            player3.AddRange(player1Copy);
            player3 = shuffle(player3);
            Domain domain13 = Domain.Set(player3.ToArray());
            player1.AddRange(player3Copy);
            player1 = shuffle(player1);
            Domain domain123 = Domain.Set(player1.ToArray());


            for (int i = 0; i < deck.Count; i++)
            {
                List<int> playersThatHaveSuit = suitHasPlayer[Card.GetSuit(deck[i])];

                if (playersThatHaveSuit.Count == 3)
                {
                    decisions[i] = new Decision(domain123, "c" + i);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 1 && playersThatHaveSuit[1] == 2)
                {
                    decisions[i] = new Decision(domain12, "c" + i);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 1 && playersThatHaveSuit[1] == 3)
                {
                    decisions[i] = new Decision(domain13, "c" + i);
                }
                else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 2 && playersThatHaveSuit[1] == 3)
                {
                    decisions[i] = new Decision(domain23, "c" + i);
                }
                else if (playersThatHaveSuit[0] == 1)
                {
                    decisions[i] = new Decision(domain1, "c" + i);
                }
                else if (playersThatHaveSuit[0] == 2)
                {
                    decisions[i] = new Decision(domain2, "c" + i);
                }
                else
                {
                    decisions[i] = new Decision(domain3, "c" + i);
                }

                model.AddDecision(decisions[i]);
            }
            model.AddConstraint("allDiff", Model.AllDifferent(decisions));
            var solution = solver.Solve();

            if (solution.Quality != SolverQuality.Feasible)
            {
                Console.Write("CSP Problem - solution {0}", solution.Quality);
            }

            List<List<int>> cardsPerPlayer = new List<List<int>>(3);
            cardsPerPlayer.Add(new List<int>(handSizes[0]));
            cardsPerPlayer.Add(new List<int>(handSizes[1]));
            cardsPerPlayer.Add(new List<int>(handSizes[2]));

            for (int i = 0; i < deck.Count; i++)
            {
                int decision = Convert.ToInt16(decisions[i].ToString());
                decision = decision / 10;
                cardsPerPlayer[decision - 1].Add(deck[i]);
            }
            solver.ClearModel();
            return cardsPerPlayer;
        }
    }
}