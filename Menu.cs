using System;
using static System.Console;

namespace Assignment_2
{
    public class Menu
    {
        public int Option { get; set; }

        public int GetOption()
        {
            Write("You option is: ");
            bool isInt = int.TryParse(ReadLine(), out int input);
            WriteLine("");
            while (!isInt || (input != 1 && input != 2))
            {
                Write("Invalid input! Please retry: ");
                isInt = int.TryParse(ReadLine(), out input);
                WriteLine("");
            }
            Option = input;
            return Option;
        }
        public void DisplayGameList()
        {
            WriteLine("Welcome!");
            WriteLine("Please select a game:");
            WriteLine("1.SOS\n2.Connect Four");
            GetOption();

        }
        public void DisplayGameMode()
        {
            WriteLine("Please select the mode:");
            WriteLine("1.New game\n2.Resume the game");
            GetOption();
        }
        public void DisplayPlayerMode()
        {
            WriteLine("Please select the player mode:");
            WriteLine("1.Human v. Human\n2.Human v. Computer");
            GetOption();
        }
    }
}