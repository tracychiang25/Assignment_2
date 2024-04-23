using System;
using static System.Console;
namespace Assignment_2
{
    public class Player
    {
        public string PlayerType { get; set; }
        public string PlayerName { get; set; }
    }

    public class Human : Player
    {
        public Human(string playerName)
        {
            PlayerType = "Human";
            PlayerName = playerName;
        }
    }

    public class Computer : Player
    {
        public Computer(string playerName)
        {
            PlayerType = "Computer";
            PlayerName = playerName;
        }
    }
}

