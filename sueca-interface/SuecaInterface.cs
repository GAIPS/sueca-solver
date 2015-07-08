using System;
using Thalamus;

namespace SuecaInterface
{

    public interface IStartEvents : Thalamus.IPerception
    {
        void Init(int trump, int firstPlayer, int[] initialCards);
    }

    public interface ICardsEvents : Thalamus.IPerception
    {
        void PlayedCard(int playerId, int card);
        void Play();
    }
}
