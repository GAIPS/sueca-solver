using System;
using Thalamus;
using SuecaSolver;
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

            public void GameStart(int id, int teamId, int trump, string[] cards)
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


        public UnityEmulator() : base("UnityEmulator", "Tiago")
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
            //startPublisher.Init(0, 0, new int[] {0, 7, 8, 9, 11, 23, 28, 30, 38, 39});
            Thread.Sleep(5000);
        }

    }
}
