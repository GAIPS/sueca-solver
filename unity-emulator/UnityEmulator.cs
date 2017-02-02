using System;
using Thalamus;
using System.Threading;
using SuecaTypes;
// using System.Collections.Generic;

namespace unity_emulator
{
    public interface IStartPublisher : IThalamusPublisher,
        SuecaMessages.ISuecaPerceptions
	{}


    public class UnityEmulator : ThalamusClient
    {
        private class StartPublisher : IStartPublisher
        {
            dynamic publisher;
            public StartPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void SessionStart(int numGames, int[] agentsIds)
            {
                publisher.SessionStart(numGames, agentsIds);
            }

            public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
            {
                publisher.GameStart(gameId, playerId, teamId, trumpCard, trumpCardPlayer, cards);
            }

            public void GameEnd(int team0Score, int team1Score)
            {
                publisher.GameEnd(team0Score, team1Score);
            }

            public void SessionEnd(int team0Score, int team1Score)
            {
                publisher.SessionEnd(team0Score, team1Score);
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
        Thread emulateUserThread;
        Random r;


        public UnityEmulator() : base("UnityEmulator", "")
		{
            r = new Random();
			//never forget to first set the publisher, even if we are going to use a publisher wrapper
			SetPublisher<IStartPublisher>();

			//create an instance of the publisher wrapper, and pass the dynamic publisher to the constructor
			startPublisher = new StartPublisher(Publisher);

			//launch a thread that will randomly generate user events
			emulateUserThread = new Thread(new ThreadStart (EmulateUser));
		}


        public override void ConnectedToMaster()
        {
            if (emulateUserThread.ThreadState != ThreadState.Running)
            {
                emulateUserThread = new Thread(new ThreadStart(EmulateUser));
                emulateUserThread.Start();
            }
        }

        public override void Dispose()
        {
            emulateUserThread.Abort();
        }


        public void EmulateUser()
        {
            Debug("<<<<<Emulator will simulate a session");

            Thread.Sleep(5000);
            startPublisher.SessionStart(1, new int[] {0});
            Thread.Sleep(10000);
            startPublisher.Shuffle(0);
            Thread.Sleep(5000);
            startPublisher.Cut(2);
            Thread.Sleep(5000);
            startPublisher.Deal(3);

            string c0 = new Card(Rank.Four, Suit.Diamonds).SerializeToJson();
            string c1 = new Card(Rank.Ace, Suit.Diamonds).SerializeToJson();
            string c2 = new Card(Rank.King, Suit.Diamonds).SerializeToJson();
            string c3 = new Card(Rank.Two, Suit.Clubs).SerializeToJson();
            string c4 = new Card(Rank.Five, Suit.Clubs).SerializeToJson();
            string c5 = new Card(Rank.Seven, Suit.Hearts).SerializeToJson();
            string c6 = new Card(Rank.Two, Suit.Spades).SerializeToJson();
            string c7 = new Card(Rank.Four, Suit.Spades).SerializeToJson();
            string c8 = new Card(Rank.Queen, Suit.Spades).SerializeToJson();
            string c9 = new Card(Rank.King, Suit.Spades).SerializeToJson();

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

    }
}
