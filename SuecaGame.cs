using System;

namespace SuecaSolver
{
	public class SuecaGame
	{

		private Player[] players = new Player[4];
		private int firstPlayerId;
		private Suit trump;

		public SuecaGame(Card[] hand, Suit trumpSuit, int firstPlayer)
		{
			firstPlayerId = firstPlayer;
			trump = trumpSuit;
			Deck deck = new Deck(hand);
			players[0] = new MaxPlayer(0, 10, hand);
			players[1] = new MinPlayer(1, 10, deck.getHand());
			players[2] = new MaxPlayer(2, 10, deck.getHand());
			players[3] = new MinPlayer(3, 10, deck.getHand());
			players[0].NextPlayer = players[1];
			players[1].NextPlayer = players[2];
			players[2].NextPlayer = players[3];
			players[3].NextPlayer = players[0];

			printPlayersHands();
		}

		public int SampleGame()
		{
			Player player = players[firstPlayerId];
			GameState gameState = new GameState(trump);
			int bestmove = player.PlayGame(gameState);
			Console.WriteLine(bestmove);

			return 0;
		}

		private void printPlayersHands()
		{
			Console.WriteLine("---------Cards---------");
			players[0].PrintHand();
			players[1].PrintHand();
			players[2].PrintHand();
			players[3].PrintHand();
			Console.WriteLine("-----------------------");
		}
	}
}