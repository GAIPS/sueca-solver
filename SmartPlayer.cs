using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class SmartPlayer : ArtificialPlayer
	{

		private PIMC pimc;
		private InformationSet infoSet;


		public SmartPlayer(List<Card> initialHand, Suit trumpSuit)
		{
			pimc = new PIMC(1);
			infoSet = new InformationSet(initialHand, trumpSuit);
		}

		override public void AddPlay(int playerID, Card card)
		{
			infoSet.AddPlay(playerID, card);
		}


		override public Card Play(){

			Card chosenCard = pimc.Execute(infoSet);
			infoSet.AddMyPlay(chosenCard);

			return chosenCard;
		}
	}
}