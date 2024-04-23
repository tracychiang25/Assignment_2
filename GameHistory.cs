using System;
using static System.Console;
namespace Assignment_2
{
    public class GameHistory
    {
        public List<Tuple<Player, Player, Token, int, int>> Moves { get; set; }

        //constructor
        public GameHistory()
        {
            Moves = new List<Tuple<Player, Player, Token, int, int>>();
        }
        public void UpdateHistory(Player playerName, Player playerType, Token token, int col, int row)
        {
            Tuple<Player, Player, Token, int, int> newMove = Tuple.Create(playerName, playerType, token, col, row);
            Moves.Add(newMove);
        }
    }
}
