namespace SuecaSolver
{
	public class Game
	{

		private Trick[] tricks = new Trick[10];
		private Player[] players;

		public Game(Card[] hand)
		{
			Deck deck = new Deck(hand);
			MaxPlayer p0 = new MaxPlayer(0, hand);
			MinPlayer p1 = new MinPlayer(1, deck.getHand());
			MaxPlayer p2 = new MaxPlayer(2, deck.getHand());
			MinPlayer p3 = new MinPlayer(3, deck.getHand());
		}
	}
}