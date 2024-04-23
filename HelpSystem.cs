using System;
using System.Xml.Linq;
using Assignment_2;
using static System.Console;
namespace Assignment_2
{
    public class HelpSystem
    {
        public string HelpDesc { get; set; }
        //
        public string PlayOption { get; set; }

        //Singleton
        private static HelpSystem instance;
        private HelpSystem() { }
        public static HelpSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HelpSystem();
                }
                return instance;
            }
        }

        public void ProvideHelp(BoardGame game)
        {
            // Provide help for specific game type
            if (game is SOS)
            {
                ProvideSOSHelp();
            }
            else if (game is ConnectFour)
            {
                ProvideConnectFourHelp();
            }
        }

        private void ProvideSOSHelp()
        {
            HelpDesc = "You can choose 'S' or 'O' to place on this 3x3 board," +
                " and get one point for completing the SOS sequence\nvertically, horizontally or diagonally." +
                "Once the grid is filled, the player with the most SOS is the winner.\n";
            PlayOption = "(U)Undo  (R)Redo  (A)Save Game  (Q)Quit Game  (H)Help";
            DisplayHelpSystem();
        }

        private void ProvideConnectFourHelp()
        {
            HelpDesc = "You can place pieces on this 7x6 board. " +
                "* NOTICE: The token can only be placed from the bottom to top. *\n" +
                "Once a complete chain of four blocks is formed either horizontally, \n" +
                "vertically or diagonally, that player wins the game.\n";
            PlayOption = "(U)Undo  (R)Redo  (A)Save Game  (Q)Quit Game  (H)Help";
            DisplayHelpSystem();
        }
        public void DisplayHelpSystem()
        {
            WriteLine("\n-----------------------------------------------------------------------------------------------------------");
            WriteLine("-----------------------------------------------------------------------------------------------------------");
            WriteLine(HelpDesc);
            WriteLine("You can also choose from the following options to...: ");
            WriteLine(PlayOption);
            WriteLine("-----------------------------------------------------------------------------------------------------------");

        }
    }
}
