using System;

namespace SuecaSolver
{
	public class SuecaGame
	{

		private Player[] players = new Player[4];
		private int firstPlayerId;
		private Suit trump;
		private GameState gameState;

		public SuecaGame(Card[] hand, Suit trumpSuit, int firstPlayer)
		{
			firstPlayerId = firstPlayer;
			trump = trumpSuit;
			Deck deck = new Deck(hand);
			players[0] = new MaxPlayer(0, hand);
			players[1] = new MinPlayer(1, deck.getHand());
			players[2] = new MaxPlayer(2, deck.getHand());
			players[3] = new MinPlayer(3, deck.getHand());
			players[0].NextPlayer = players[1];
			players[1].NextPlayer = players[2];
			players[2].NextPlayer = players[3];
			players[3].NextPlayer = players[0];

			gameState = new GameState(10, trump);
		}

		public SuecaGame(Card[] p0, Card[] p1, Card[] p2, Card[] p3, Suit trumpSuit, int firstPlayer)
		{
			players[0] = new MaxPlayer(0, p0);
			players[1] = new MinPlayer(1, p1);
			players[2] = new MaxPlayer(2, p2);
			players[3] = new MinPlayer(3, p3);
			players[0].NextPlayer = players[1];
			players[1].NextPlayer = players[2];
			players[2].NextPlayer = players[3];
			players[3].NextPlayer = players[0];

			gameState = new GameState(1, trump);
		}

		public int SampleGame()
		{
			int bestmove = players[firstPlayerId].PlayGame(gameState);
			Console.WriteLine("Game sampling result: " + bestmove);
			return bestmove;
		}

		public int SampleTrick()
		{
			Player p = players[firstPlayerId];
			int bestmove = p.PlayTrick(gameState, p.Hand[0]);
			Console.WriteLine("Trick sampling result: " + bestmove);
			return bestmove;
		}

		public void PrintPlayersHands()
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