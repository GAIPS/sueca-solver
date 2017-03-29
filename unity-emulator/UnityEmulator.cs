using System;
using Thalamus;
using System.Threading;
using SuecaTypes;
using SuecaSolver;
using System.Collections.Generic;
using SuecaMessages;
// using System.Collections.Generic;

namespace unity_emulator
{
    public interface IStartPublisher : IThalamusPublisher,
        SuecaMessages.ISuecaPerceptions
	{}


    public class UnityEmulator : ThalamusClient, ISuecaActions
    {
        private class StartPublisher : IStartPublisher
        {
            dynamic publisher;
            public StartPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
            {
                publisher.SessionStart(sessionId, numGames, agentsIds, shouldGreet);
            }

            public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
            {
                publisher.GameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards);
            }

            public void GameEnd(int team0Score, int team1Score)
            {
                publisher.GameEnd(team0Score, team1Score);
            }

            public void SessionEnd(int sessionId, int team0Score, int team1Score)
            {
                publisher.SessionEnd(sessionId, team0Score, team1Score);
            }

            public void Shuffle(int playerId)
            {
                publisher.Shuffle(playerId);
            }

            public void Cut(int playerId)
            {
                publisher.Cut(playerId);
            }

            public void Deal(int playerId)
            {
                publisher.Deal(playerId);
            }

            public void ReceiveRobotCards(int playerId)
            {
                publisher.ReceiveRobotCards(playerId);
            }

            public void NextPlayer(int id)
            {
                publisher.NextPlayer(id);
            }

            public void TrickEnd(int winnerId, int trickPoints)
            {
                publisher.TrickEnd(winnerId, trickPoints);
            }

            public void Play(int id, string card)
            {
                publisher.Play(id, card);
            }


            public void Renounce(int playerId)
            {
                publisher.Renounce(playerId);
            }

            public void ResetTrick()
            {
                publisher.ResetTrick();
            }
            
            public void TrumpCard(string trumpCard, int trumpCardPlayer)
            {
                publisher.TrumpCard(trumpCard, trumpCardPlayer);
            }
        }
            
        StartPublisher startPublisher;
        private SuecaTypes.Card botCard;


        public UnityEmulator(string character) : base("UnityEmulator", character)
		{
            botCard = null;

            SetPublisher<IStartPublisher>();
			startPublisher = new StartPublisher(Publisher);
		}


        public override void ConnectedToMaster()
        {
            emulateSingleGame();
        }

        public override void Dispose()
        {
        }


        private void emulateGameEvents()
        {
            Debug("<<<<<Emulator will simulate the first events of a session");

            Thread.Sleep(5000);
            startPublisher.SessionStart(0, 1, new int[] {0}, 1);
            Thread.Sleep(10000);
            startPublisher.Shuffle(0);
            Thread.Sleep(5000);
            startPublisher.Cut(2);
            Thread.Sleep(5000);
            startPublisher.Deal(3);

            string c0 = new SuecaTypes.Card(SuecaTypes.Rank.Four, SuecaTypes.Suit.Diamonds).SerializeToJson();
            string c1 = new SuecaTypes.Card(SuecaTypes.Rank.Ace, SuecaTypes.Suit.Diamonds).SerializeToJson();
            string c2 = new SuecaTypes.Card(SuecaTypes.Rank.King, SuecaTypes.Suit.Diamonds).SerializeToJson();
            string c3 = new SuecaTypes.Card(SuecaTypes.Rank.Two, SuecaTypes.Suit.Clubs).SerializeToJson();
            string c4 = new SuecaTypes.Card(SuecaTypes.Rank.Five, SuecaTypes.Suit.Clubs).SerializeToJson();
            string c5 = new SuecaTypes.Card(SuecaTypes.Rank.Seven, SuecaTypes.Suit.Hearts).SerializeToJson();
            string c6 = new SuecaTypes.Card(SuecaTypes.Rank.Two, SuecaTypes.Suit.Spades).SerializeToJson();
            string c7 = new SuecaTypes.Card(SuecaTypes.Rank.Four, SuecaTypes.Suit.Spades).SerializeToJson();
            string c8 = new SuecaTypes.Card(SuecaTypes.Rank.Queen, SuecaTypes.Suit.Spades).SerializeToJson();
            string c9 = new SuecaTypes.Card(SuecaTypes.Rank.King, SuecaTypes.Suit.Spades).SerializeToJson();

            Thread.Sleep(5000);
            Console.WriteLine("A Começar o jogo");
            startPublisher.GameStart(0, 1, 1, c0, 0, new string[] { c0, c1, c2, c3, c4, c5, c6, c7, c8, c9 });
            //Thread.Sleep(5000);
            //startPublisher.NextPlayer(3);
            //Thread.Sleep(5000);
            //string c10 = new Card(Rank.Ace, Suit.Spades).SerializeToJson();
            //startPublisher.Play(3, c10);
            //Thread.Sleep(5000);
            //startPublisher.NextPlayer(0);
            //Thread.Sleep(5000);
            //string c11 = new Card(Rank.Seven, Suit.Spades).SerializeToJson();
            //startPublisher.Play(0, c11);
            //Thread.Sleep(5000);
            //startPublisher.NextPlayer(1);

            //Thread.Sleep(2000);
            //startPublisher.Play(1, c9);
            //Thread.Sleep(2000);
            //string c11 = new Card(Rank.Three, Suit.Diamonds).SerializeToJson();
            //startPublisher.Play(2, c11);
        }

        private void emulateSingleGame()
        {

            startPublisher.SessionStart(0, 1, new int[] { 3 }, 1);

            string input;
            string[] playersNames = new string[4];
            playersNames[3] = "Bot";
            int firstPlayerID;
            Console.WriteLine("");
            Console.WriteLine("|||||||||||||||||||| SUECA GAME ||||||||||||||||||||");
            Console.WriteLine("");

            Console.Write("Press Enter twice to start.");
            Console.ReadLine();
            Console.Write("Player 0 name: ");
            playersNames[0] = Console.ReadLine();
            Console.Write("Player 1 name: ");
            playersNames[1] = Console.ReadLine();
            Console.Write("Player 2 name: ");
            playersNames[2] = Console.ReadLine();
            Console.WriteLine("Player 3 name: Bot");
            Console.Write("First player ID: ");
            input = Console.ReadLine();
            firstPlayerID = Convert.ToInt16(input);
            Console.WriteLine("");

            int seed = Guid.NewGuid().GetHashCode();
            Random randomNumber = new Random(seed);
            Deck deck = new Deck();
            List<List<int>> playersHand = new List<List<int>>(
                new List<int>[] {
                    new List<int>(10),
                    new List<int>(10),
                    new List<int>(10),
                    new List<int>(10) });
            deck.SampleHands(ref playersHand);
            List<int> currentHand;
            int trumpCardPlayer = (firstPlayerID - 1 + 4) % 4;
            int trumpCard = playersHand[trumpCardPlayer][0];
            int cardIndex, currentPlayerID = firstPlayerID;


            startPublisher.GameStart(0, 3, 1, serializeCard(trumpCard), trumpCardPlayer, serializeCards(playersHand[3]));
            SuecaGame game = new SuecaGame(SuecaSolver.Card.GetSuit(trumpCard), firstPlayerID);


            for (int i = 0; i < 40; i++)
            {
                startPublisher.NextPlayer(currentPlayerID);

                currentHand = playersHand[currentPlayerID];
                Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||");
                Console.WriteLine("                 Trick " + (i / 4));
                Console.WriteLine("                 Player " + currentPlayerID + " - " + playersNames[currentPlayerID]);
                Console.WriteLine("                 Trump is " + SuecaSolver.Card.ToString(trumpCard)[1]);
                Console.WriteLine("");

                game.PrintLastTrick();
                game.PrintCurrentTrick();
                Sueca.PrintCurrentHand(currentHand);
                int chosenCard;

                if (currentPlayerID != 3)
                {
                    Console.Write("Pick the card you want to play by its index: ");
                    input = Console.ReadLine();
                    cardIndex = Convert.ToInt16(input);
                    chosenCard = currentHand[cardIndex];
                    
                }
                else
                {
                    while (botCard == null)
                    {

                    }
                    SuecaSolver.Rank myRank = (SuecaSolver.Rank)Enum.Parse(typeof(SuecaSolver.Rank), botCard.Rank.ToString());
                    SuecaSolver.Suit mySuit = (SuecaSolver.Suit)Enum.Parse(typeof(SuecaSolver.Suit), botCard.Suit.ToString());
                    chosenCard = SuecaSolver.Card.Create(myRank, mySuit);
                    botCard = null;
                }

                startPublisher.Play(currentPlayerID, serializeCard(chosenCard));

                game.PlayCard(currentPlayerID, chosenCard);
                currentHand.Remove(chosenCard);

                if ((i + 1) % 4 == 0)
                {
                    //startPublisher.TrickEnd(game.);
                }

                currentPlayerID = game.GetNextPlayerId();
            }

            Console.WriteLine("|||||||||||||||||||||||| END |||||||||||||||||||||||");
            Console.WriteLine("");
            int[] points = game.GetGamePoints();
            Console.WriteLine("Team " + playersNames[0] + " and " + playersNames[2] + " - " + points[0] + " points");
            Console.WriteLine("Team " + playersNames[1] + " and " + playersNames[3] + " - " + points[1] + " points");
            // game.PrintPoints(playersNames);
            Console.WriteLine("");
            Console.ReadLine();
        }

        private string[] serializeCards(List<int> list)
        {
            string[] serializedCards = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                SuecaTypes.Rank rank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), SuecaSolver.Card.GetRank(list[i]).ToString());
                SuecaTypes.Suit suit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), SuecaSolver.Card.GetSuit(list[i]).ToString());
                serializedCards[i] = new SuecaTypes.Card(rank, suit).SerializeToJson();
            }

            return serializedCards;
        }

        private string serializeCard(int card)
        {
            SuecaSolver.Rank cardRank = (SuecaSolver.Rank)SuecaSolver.Card.GetRank(card);
            SuecaSolver.Suit cardSuit = (SuecaSolver.Suit)SuecaSolver.Card.GetSuit(card);
            SuecaTypes.Rank msgRank = (SuecaTypes.Rank)Enum.Parse(typeof(SuecaTypes.Rank), cardRank.ToString());
            SuecaTypes.Suit msgSuit = (SuecaTypes.Suit)Enum.Parse(typeof(SuecaTypes.Suit), cardSuit.ToString());
            return new SuecaTypes.Card(msgRank, msgSuit).SerializeToJson();
        }

        public void Play(int id, string card)
        {
            botCard = JsonSerializable.DeserializeFromJson<SuecaTypes.Card>(card);
        }
    }
}
