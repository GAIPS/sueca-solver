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
            string c0 = new Card(Suit.Diamonds, Rank.Four).SerializeToJson();
            string c1 = new Card(Suit.Diamonds, Rank.Ace).SerializeToJson();
            string c2 = new Card(Suit.Diamonds, Rank.King).SerializeToJson();
            string c3 = new Card(Suit.Clubs, Rank.Two).SerializeToJson();
            string c4 = new Card(Suit.Clubs, Rank.Five).SerializeToJson();
            string c5 = new Card(Suit.Hearts, Rank.Seven).SerializeToJson();
            string c6 = new Card(Suit.Spades, Rank.Two).SerializeToJson();
            string c7 = new Card(Suit.Spades, Rank.Four).SerializeToJson();
            string c8 = new Card(Suit.Spades, Rank.Queen).SerializeToJson();
            string c9 = new Card(Suit.Spades, Rank.King).SerializeToJson();

            startPublisher.GameStart(1, 1, Suit.Diamonds.ToString(), new string[] { c0, c1, c2, c3, c4, c5, c6, c7, c8, c9 });
            Thread.Sleep(5000);
        }

    }
}
