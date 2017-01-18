using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;
using SuecaSolver;
using SuecaMessages;

namespace emotionalPlayer
{
    public interface ISuecaPublisher : IThalamusPublisher, ISuecaActions { }


    class EmotionalSuecaPlayer : ThalamusClient, ISuecaPerceptions
    {
        private class SuecaPublisher : ISuecaPublisher
        {
            dynamic publisher;

            public SuecaPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void Play(int id, string card)
            {
                this.publisher.Play(id, card);
            }
        }


        private ISuecaPublisher suecaPublisher;

        public EmotionalSuecaPlayer(string clientName, string charactersNames = "")
            : base(clientName, charactersNames)
        {
            SetPublisher<ISuecaPublisher>();
            suecaPublisher = new SuecaPublisher(Publisher);
        }

        public void Cut(int playerId)
        {
            throw new NotImplementedException();
        }

        public void Deal(int playerId)
        {
            throw new NotImplementedException();
        }

        public void GameEnd(int team0Score, int team1Score)
        {
            throw new NotImplementedException();
        }

        public void GameStart(int gameId, int playerId, int teamId, string trumpCard, int trumpCardPlayer, string[] cards)
        {
            throw new NotImplementedException();
        }

        public void NextPlayer(int id)
        {
            throw new NotImplementedException();
        }

        public void Play(int id, string card)
        {
            throw new NotImplementedException();
        }

        public void ReceiveRobotCards(int playerId)
        {
            throw new NotImplementedException();
        }

        public void Renounce(int playerId)
        {
            throw new NotImplementedException();
        }

        public void ResetTrick()
        {
            throw new NotImplementedException();
        }

        public void SessionEnd(int sessionId, int team0Score, int team1Score)
        {
            throw new NotImplementedException();
        }

        public void SessionStart(int sessionId, int numGames, int[] agentsIds, int shouldGreet)
        {
            throw new NotImplementedException();
        }

        public void Shuffle(int playerId)
        {
            throw new NotImplementedException();
        }

        public void TrickEnd(int winnerId, int trickPoints)
        {
            throw new NotImplementedException();
        }

        public void TrumpCard(string trumpCard, int trumpCardPlayer)
        {
            throw new NotImplementedException();
        }
    }
}
