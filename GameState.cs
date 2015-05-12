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
		private AscendingCutComparer acc;
		private DescendingCutComparer dcc;

		public GameState(int numTricks, Suit trumpSuit, Player[] playersList, bool debug)
		{
			ac = new AscendingComparer();
			dc = new DescendingComparer();
			acc = new AscendingCutComparer(trumpSuit);
			dcc = new DescendingCutComparer(trumpSuit);
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
			// Console.WriteLine("----------- orderPossibleMoves -----------");
			// SuecaGame.PrintCards("Init moves", moves);

			Suit leadSuit = GetLeadSuit();
			int currentPlayInTrick = getPlayInTrick();

			if (currentPlayInTrick == 0 || moves.Count == 1)
			{
				// Console.WriteLine("Returning moves again!");
				// Console.WriteLine("------------------------------------------");
				return moves;
			}

			List<Move> currentTrick = GetCurrentTrick().GetMoves();
			int bestRank = 0;
			int trickWinner  = 0;
			bool cut = false;
			int firstPlayerId = (4 + (playerID - currentPlayInTrick)) % 4;

			// Console.WriteLine("currentPlayInTrick " + currentPlayInTrick);
			// Console.WriteLine("firstPlayerId " + firstPlayerId);
			for (int i = 0; i < 4; i++)
			{
				int highestRankForPlayer;
				int pID = (firstPlayerId + i) % 4;

				if (i < currentPlayInTrick)
				{
					// Console.WriteLine("Adding an already played card " + currentTrick[i].Card);
					highestRankForPlayer = (int) currentTrick[i].Card.Rank;
				}
				else
				{
					highestRankForPlayer = players[pID].HighestRankForSuit(leadSuit, trump);
					// Console.WriteLine("Adding from player " + pID + " card with rank " + highestRankForPlayer);
				}

				if (!cut)
				{
					if (highestRankForPlayer > bestRank)
					{
						bestRank = highestRankForPlayer;
						trickWinner = pID;
					}
					if (highestRankForPlayer < 0)
					{
						bestRank = highestRankForPlayer;
						trickWinner = pID;
						cut = true;
					}
				}
				else if (highestRankForPlayer < bestRank)
				{
					bestRank = highestRankForPlayer;
					trickWinner = pID;
				}
			}

			// Console.WriteLine("trickWinner " + trickWinner);

			if (!cut && (trickWinner == playerID || trickWinner == (playerID + 2) % 4))
			{
				moves.Sort(ac);
				// SuecaGame.PrintCards("ascending sort", moves);
			}
			else if (cut && (trickWinner == playerID || trickWinner == (playerID + 2) % 4))
			{
				moves.Sort(acc);
				// SuecaGame.PrintCards("ascending cut sort", moves);
			}
			else
			{
				moves.Sort(dc);
				// SuecaGame.PrintCards("descending sort", moves);
			}

			return moves;
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
			Trick currentTrick = GetCurrentTrick();
			if (currentTrick.IsFull())
			{
				return Suit.None;
			}
			return currentTrick.LeadSuit;
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