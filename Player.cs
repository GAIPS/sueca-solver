namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public Card[] Hand = new Card[10];

		public Player(int id, Card[] hand)
		{
			Id = id;
			for (int i = 0; i < 10; i++)
			{
				Hand[i] = hand[i];
			}
		}

		abstract public void Play();
	}
}