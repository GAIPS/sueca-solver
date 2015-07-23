using System;
using Thalamus;
using System.Threading;
using SuecaMessages;
using System.Collections.Generic;

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

            public void GameStart(int id, int teamId, string trump, string[] cards)
            {
                publisher.GameStart(id, teamId, trump, cards);
            }

            public void GameEnd(int team0Score, int team1Score)
            {
                publisher.GameEnd(team0Score, team1Score);
            }

            public void NextPlayer(int id)
            {
                publisher.NextPlayer(id);
            }

            public void Play(int id, string card)
            {
                publisher.Play(id, card);
            }

        }
            
        StartPublisher startPublisher;
        Thread emulateUserThread;
        Random r;


        public UnityEmulator() : base("UnityEmulator", "tiago")
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
            Debug("<<<<<Emulator will init the game");
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

            startPublisher.GameStart(1, 1, Suit.Diamonds.ToString(), new string[] { c0, c1, c2, c3, c4, c5, c6, c7, c8, c9 });
            Thread.Sleep(2000);
            startPublisher.NextPlayer(0);
            Thread.Sleep(2000);
            string c10 = new Card(Rank.Ace, Suit.Spades).SerializeToJson();
            startPublisher.Play(0, c10);
            Thread.Sleep(2000);
            startPublisher.NextPlayer(1);
        }

    }
}
