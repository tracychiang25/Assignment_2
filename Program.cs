using System;
using static System.Console;
namespace Assignment_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            BoardGame game;
            bool isExecute = true;
            while (isExecute)
            {
                menu.DisplayGameList();
                if (menu.Option == 1)
                {
                    game = new SOS();
                    game.StartGame();
                    isExecute = false;
                }
                else if (menu.Option == 2)
                {
                    game = new ConnectFour();
                    game.StartGame();
                    isExecute = false;
                }
            }
        }
    }
}

