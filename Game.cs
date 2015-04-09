using System;

namespace SuecaSolver
{
	public class Game
	{

		private Player[] players = new Player[4];
		private int firstPlayerId;
		private Suit trump;

		public Game(Card[] hand, Suit trumpSuit, int firstPlayer)
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
		}

		public int Play()
		{
			Console.WriteLine("---------Cards---------");
			players[0].PrintCards(players[0].Hand);
			players[1].PrintCards(players[1].Hand);
			players[2].PrintCards(players[2].Hand);
			players[3].PrintCards(players[3].Hand);
			Console.WriteLine("-----------------------");

			Player player = players[firstPlayerId];
			GameState gameState = new GameState(trump);
			// Console.WriteLine(gameState.GetLeadSuit());
			int bestmove = player.Play(gameState);
			Console.WriteLine(bestmove);

			return 0;
		}
	}
}