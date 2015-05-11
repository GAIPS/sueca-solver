using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class GameState
	{

		private List<Trick> tricks;
		private bool debugFlag;
		private Player[] players;
		private Suit trump;
		private AscendingComparer ac;
		private DescendingComparer dc;

		public GameState(int numTricks, Suit trumpSuit, Player[] playersList, bool debug)
		{
			ac = new AscendingComparer();
			dc = new DescendingComparer();
			players = new Player[4];
			tricks = new List<Trick>(numTricks);
			trump = trumpSuit;
			debugFlag = debug;

			for (int i = 0; i < 4; i++)
			{
				players[i] = playersList[i];
			}
		}

		private int getPlayInTrick()
		{
			int playInCurrentTrick = GetCurrentTrick().getPlayInTrick();
			if (playInCurrentTrick == 4)
			{
				return 0;
			}
			return playInCurrentTrick;
		}

		public Trick GetCurrentTrick()
		{
			if (tricks.Count == 0)
			{
				Console.WriteLine("Trouble at GameState in GetCurrentTrick");
			}
			return tricks[tricks.Count - 1];
		}

		// This function is always called after applying a move
		public Player GetNextPlayer()
		{
			int nextPlayerId;
			if (GetCurrentTrick().IsFull())
			{
				nextPlayerId = GetCurrentTrick().GetTrickWinnerId();
			} else {
				int lastPlayerId = GetCurrentTrick().GetLastPlayerId();
				nextPlayerId = (lastPlayerId + 1) % 4;
			}
			return players[nextPlayerId];
		}


		public List<Card> orderPossibleMoves(List<Card> moves, int playerID)
		{
			Console.WriteLine("orderPossibleMoves1");
			Suit leadSuit = GetLeadSuit();
			Console.WriteLine("orderPossibleMoves2");
			// List<Card> trumps = new List<Card>();
			// List<Card> nonTrumps = new List<Card>();
			int currentPlayInTrick = getPlayInTrick();
			Console.WriteLine("orderPossibleMoves3");

			if (currentPlayInTrick == 0)
			{
				Console.WriteLine("orderPossibleMoves4");
				return moves;
			}

			Console.WriteLine("orderPossibleMoves5");
			List<Move> currentTrick = GetCurrentTrick().GetMoves();
			Console.WriteLine("orderPossibleMoves6");
			int bestRank = 0;
			int trickWinner  = 0;
			bool cut = false;

			for (int i = 0; i < 4; i++)
			{
				Console.WriteLine("orderPossibleMoves7");
				int highestRankForPlayer;

				if (i < currentPlayInTrick)
				{
					highestRankForPlayer = (int) currentTrick[i].Card.Rank;
				}
				else
				{
					//checkar os casos limites disto
					highestRankForPlayer = players[i].HighestRankForSuit(leadSuit, trump);
				}

				if (!cut)
				{
					if (highestRankForPlayer > bestRank)
					{
						bestRank = highestRankForPlayer;
						trickWinner = i;
					}
					if (highestRankForPlayer < 0)
					{
						bestRank = highestRankForPlayer;
						trickWinner = i;
						cut = true;
					}
				}
				else if (highestRankForPlayer < bestRank)
				{
					bestRank = highestRankForPlayer;
					trickWinner = i;
				}
			}

			if (trickWinner == playerID || trickWinner == (playerID + 2) % 4)
			{
				moves.Sort(ac);
			}
			else
			{
				moves.Sort(dc);
			}

			return moves;

			// if (moves[0].Suit == leadSuit)
			// {
			// 	return moves;
			// }
			// else
			// {
			// 	for (int i = 0; i < moves.Count; i++)
			// 	{
			// 		if (moves[i].Suit == trump)
			// 		{
			// 			trumps.Add(moves[i]);
			// 		} else {
			// 			nonTrumps.Add(moves[i]);
			// 		}
			// 	}
			// 	trumps.AddRange(nonTrumps);
			// 	return trumps;
			// }

		}

		private bool cardsHaveSuit(List<Card> cards, Suit leadSuit)
		{
			foreach (Card card in cards)
			{
				if (card.Suit == leadSuit)
				{
					return true;
				}
			}
			return false;
		}


		public void ApplyMove(Move move)
		{
			// printTricks();
			if (tricks.Count == 0 || GetCurrentTrick().IsFull())
			{
				tricks.Add(new Trick(trump, debugFlag));
			}
			// Console.WriteLine("ApplyMove!!!");
			GetCurrentTrick().ApplyMove(move);
			// printTricks();
			// Console.WriteLine("");
		}

		public void UndoMove()
		{
			GetCurrentTrick().UndoMove();
			if (GetCurrentTrick().IsEmpty())
			{
				tricks.RemoveAt(tricks.Count - 1);
			}
		}

		public Suit GetLeadSuit()
		{
			return GetCurrentTrick().LeadSuit;
		}

		public bool IsNewTrick()
		{
			if (tricks.Count == 0 || GetCurrentTrick().IsEmpty())
			{
				return true;
			}
			return false;
		}

		public bool IsEndGame()
		{
			if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
			{
				// Console.WriteLine("ENG GAME!!!!");
				return true;
			}
			return false;
		}


		public bool IsEndFirstTrick()
		{
			if (tricks.Count > 0 && tricks[0].IsFull())
			{
				return true;
			}
			return false;
		}

		public bool IsEndTrick()
		{
			if (GetCurrentTrick().IsFull())
			{
				return true;
			}
			return false;
		}



		public int[] GetGamePoints()
		{
			int[] result = new int[2] {0, 0};
			for (int i = 0; i < tricks.Count; i++)
			{
				int trickResult = tricks[i].GetTrickPoints();
				if (trickResult > 0)
				{
					result[0] += trickResult;
				} else {
					result[1] += (-1 * trickResult);
				}
			}
			return result;
		}

		public int EvalGame()
		{
			// Console.WriteLine("EvalGame - tricks.Count " + tricks.Count);
			int result = 0;
			for (int i = 0; i < tricks.Count; i++)
			{
				if(debugFlag) Console.WriteLine("--- Trick " + i + ": ---");
				int trickResult = tricks[i].GetTrickPoints();
				result += trickResult;
				// Console.WriteLine("Trickresult: " + trickResult + " Sum: " + result);
				if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + result);
			}
			return result;
		}

		public int GetTrickWinnerId()
		{
			return GetCurrentTrick().GetTrickWinnerId();
		}

		public int GetFirstTrickPoints()
		{
			if(debugFlag) Console.WriteLine("--- Trick ---");
			int trickResult = tricks[0].GetTrickPoints();
			if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + trickResult);
			return trickResult;
		}

		public int GetTrickPoints()
		{
			if(debugFlag) Console.WriteLine("--- Trick ---");
			int trickResult = GetCurrentTrick().GetTrickPoints();
			if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + trickResult);
			return trickResult;
		}


		private void printTricks()
		{
			Console.WriteLine("printTricks - tricks.Count " + tricks.Count);
			for (int i = 0; i < tricks.Count; i++)
			{
				Console.WriteLine("--- Trick " + i + "---");
				tricks[i].PrintTrick();
			}
		}


		public void PrintLastTrick()
		{
			if (tricks.Count > 0 && tricks[0].IsFull())
			{
				Console.WriteLine("Last trick:");
				if (GetCurrentTrick().IsFull())
				{
					GetCurrentTrick().PrintTrick();
				}
				else
				{
					tricks[tricks.Count - 2].PrintTrick();
				}
				Console.WriteLine("");
			}
		}

		public void PrintCurrentTrick()
		{
			Console.WriteLine("Current trick:");
			if (tricks.Count > 0 && !GetCurrentTrick().IsFull())
			{
				GetCurrentTrick().PrintTrick();
			}
			Console.WriteLine("");
		}

	}
}