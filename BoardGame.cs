using System;
using static System.Console;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace Assignment_2
{
    public abstract class BoardGame
    {
        protected Player playerOne;
        protected Player playerTwo;
        protected Menu menu = new Menu();
        protected Board board;
        protected GameHistory history;
        protected GameHistory undoHistory = new GameHistory();
        protected Player currentPlayer;
        protected HelpSystem helpSystem = HelpSystem.Instance;
        protected bool isEnd;
        protected bool isNew = false;
        public int InputRow { get; set; }
        public int InputColumn { get; set; }
        public int UndoCount { get; set; }
        public string MoveHistoryFile { get; set; }

        protected abstract void InitializeGame();
        protected abstract void PlayGame();
        protected abstract void MakeMove(Board board, Token token);
        protected abstract bool CheckWin(Player playerOne, Player playerTwo);
        protected abstract bool IsEnd();

        //template method
        public void StartGame()
        {
            InitializeGame();
            helpSystem.ProvideHelp(this); // 'this' refers to the current game object
            PlayGame();
        }
        protected void PrintHelpSystem()
        {
            helpSystem.ProvideHelp(this); // 'this' refers to the current game object
        }
        
        protected void SaveGame(string filePath)
        {
            MoveHistoryFile = filePath;
            

            //create a new file if the file is not exist
            if (!File.Exists(MoveHistoryFile))
            {
                FileStream stream = File.Create(MoveHistoryFile);
                stream.Close();
            }
            using (StreamWriter sw = new StreamWriter(MoveHistoryFile))
            {
                //print the title of csv file
                sw.WriteLine("Player,Type,Token,Column,Row");
                foreach (var move in history.Moves)
                {    
                    string playerName = move.Item1.PlayerName;
                    string type = move.Item2.PlayerType;
                    string token = move.Item3.Symbol;
                    int column = move.Item4;
                    int row = move.Item5;
                    string csvLine = $"{playerName}, {type},{token},{column},{row}";
                    sw.WriteLine(csvLine);
                }
            }
            WriteLine("Game saved!");
            
        }
        protected void ResumeGame(Board board, string filePath)
        {
            MoveHistoryFile = filePath;

            if (!File.Exists(MoveHistoryFile))
            {
                WriteLine();
                WriteLine("No saved game found! You need to save a game before loading. ");
                WriteLine();
                return;
            }
            using (StreamReader sr = new StreamReader(MoveHistoryFile))
            {
                //Read the headline of csv file
                history = new GameHistory();
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');
                    if (values[0] == "P1")
                    {
                        playerOne = new Human("P1");
                        Token tokenSymbol = new Token(values[2]);
                        board.PlaceToken(tokenSymbol, int.Parse(values[3]) - 1, int.Parse(values[4]) - 1);
                        SaveMove(playerOne, playerOne, tokenSymbol, int.Parse(values[3]), int.Parse(values[4]));
                    }
                    else
                    {
                        if (values[1] == " Human")
                        {
                            playerTwo = new Human("P2");
                            Token tokenSymbol = new Token(values[2]);
                            board.PlaceToken(tokenSymbol, int.Parse(values[3]) - 1, int.Parse(values[4]) - 1);
                            SaveMove(playerTwo, playerTwo, tokenSymbol, int.Parse(values[3]), int.Parse(values[4]));
                        }
                        else if (values[1] == " Computer")
                        {
                            playerTwo = new Computer("P2");
                            Token tokenSymbol = new Token(values[2]);
                            board.PlaceToken(tokenSymbol, int.Parse(values[3]) - 1, int.Parse(values[4]) - 1);
                            SaveMove(playerTwo, playerTwo, tokenSymbol, int.Parse(values[3]), int.Parse(values[4]));
                        }
                    }
                }
            }
            WriteLine("\nGame successfully loaded!\n");
        }
        protected void QuitGame()
        {
            // Quit the game
            WriteLine();
            WriteLine("***** See you! *****");
            Environment.Exit(0);
        }
        protected void CreatePlayer()
        {
            bool isExecute = true;
            while (isExecute)
            {
                menu.DisplayPlayerMode();
                switch (menu.Option)
                {
                    case 1: // Human vs Human
                        playerOne = new Human("P1");
                        playerTwo = new Human("P2");
                        isExecute = false;
                        break;
                    case 2: // Human vs Computer
                        playerOne = new Human("P1");
                        playerTwo = new Computer("P2");
                        isExecute = false;
                        break;
                }
            }
            Clear();
        }
        protected void SwitchTurn()
        {
            if (currentPlayer == null)
            {
                currentPlayer = playerOne;
            }
            else if (currentPlayer == playerOne)
            {
                currentPlayer = playerTwo;
            }
            else
            {
                currentPlayer = playerOne;
            }
        }
        protected void PlayerMakeMove(Board board, Player currentPlayer, Token token)
        {
            if (currentPlayer.PlayerType == "Human")
            {
                MakeMove(board, token);
            }
            else if (currentPlayer.PlayerType == "Computer")
            {
                MakeRandomMove(board);
            }
        }
        protected void Redo(Board board)
        {
            if (undoHistory.Moves.Count > 1)
            {
                int lastIndex = undoHistory.Moves.Count - 1;
                int secondLastIndex = undoHistory.Moves.Count - 2;
                history.Moves.Add(undoHistory.Moves[secondLastIndex]);
                history.Moves.Add(undoHistory.Moves[lastIndex]);
                string lastTokenS = undoHistory.Moves[lastIndex].Item3.Symbol;
                string secondLastTokenS = undoHistory.Moves[secondLastIndex].Item3.Symbol;
                Token lastToken = new Token(lastTokenS);
                Token secondLastToken = new Token(secondLastTokenS);
                board.PlaceToken(secondLastToken, undoHistory.Moves[secondLastIndex].Item4 - 1, undoHistory.Moves[secondLastIndex].Item5 - 1);
                board.PlaceToken(lastToken, undoHistory.Moves[lastIndex].Item4 - 1, undoHistory.Moves[lastIndex].Item5 - 1);
                undoHistory.Moves.RemoveAt(lastIndex);
                undoHistory.Moves.RemoveAt(secondLastIndex);
                Clear();
                board.PrintBoard();
            }
        }
        protected void Undo(Board board)
        {
            Token undoToken = new Token(".");
            if (history.Moves.Count == 0 || history.Moves.Count == 1)
            {
                WriteLine();
                WriteLine("You cannot undo!\n");
                return;
            }
            int lastIndex = history.Moves.Count - 1;
            int secondLastIndex = history.Moves.Count - 2;
            undoHistory.Moves.Add(history.Moves[secondLastIndex]);
            undoHistory.Moves.Add(history.Moves[lastIndex]);
            board.PlaceToken(undoToken, history.Moves[secondLastIndex].Item4 - 1, history.Moves[secondLastIndex].Item5 - 1);
            board.PlaceToken(undoToken, history.Moves[lastIndex].Item4 - 1, history.Moves[lastIndex].Item5 - 1);
            history.Moves.RemoveAt(lastIndex);
            history.Moves.RemoveAt(secondLastIndex);
            UndoCount++;
            Clear();
            board.PrintBoard();
        }
        protected virtual void MakeRandomMove(Board board)
        {
            Random random = new Random();
            int randomToken;
            do
            {
                InputColumn = random.Next(1, board.Col + 1);
                InputRow = random.Next(1, board.Row + 1);
                randomToken = random.Next(1, 3);// select random token
            }
            while (!CheckValid(board, InputColumn, InputRow));
            Token token;
            if (randomToken == 1)
            {
                token = new Token("S");
            }
            else
            {
                token = new Token("O");
            }
            board.PlaceToken(token, InputColumn - 1, InputRow - 1);
            SaveMove(currentPlayer, currentPlayer, token, InputColumn, InputRow);
            Clear();
            board.PrintBoard();
            SwitchTurn();
            PlayerMakeMove(board, currentPlayer, token);
        }
        protected virtual bool CheckValid(Board board, int col, int row)
        {
            // Check if the move is valid
            if (row <= board.Row && row > 0 && col <= board.Col && col > 0)
            {
                foreach (var item in history.Moves)
                {
                    if (col == item.Item4 && row == item.Item5)
                    {
                        return false; // return false if move is in the move history
                    }
                }
                return true; //return true if not matches are found
            }
            return false; // Return false if not within the board size
        }
        protected void SaveMove(Player player, Player type, Token token, int col, int row)
        {
            history.UpdateHistory(player, type, token, col, row);
        }
    }

    public class SOS : BoardGame
    {
        private int playerOneScore;
        private int playerTwoScore;

        private Board sosBoard;
        Token token;

        protected override void InitializeGame()
        {
            bool isExecute = true;
            while (isExecute)
            {
                menu.DisplayGameMode();
                switch (menu.Option)
                {
                    case 1: //New SOS game
                        CreatePlayer();
                        WriteLine("Welcome to SOS game!");
                        sosBoard = new Board(3, 3);
                        history = new GameHistory();
                        isExecute = false;
                        break;
                    case 2:
                        sosBoard = new Board(3, 3);
                        ResumeGame(sosBoard, "sos_saved_game.csv");
                        isNew = true;
                        isExecute = false;
                        break;
                }
            }
        }
        protected override void PlayGame()
        {
            SwitchTurn();
            sosBoard.PrintBoard();
            while (!IsEnd())
            {
                PlayerMakeMove(sosBoard, currentPlayer, token);
                Clear();
                sosBoard.PrintBoard();
            }
            playerOneScore = CountScore(playerOne);
            playerTwoScore = CountScore(playerTwo);

            CheckWin(playerOne, playerTwo);
        }
        protected override void MakeMove(Board board, Token token)
        {
            bool isVaild = false;
            WriteLine("{0} It's your turn!", currentPlayer.PlayerName);
            WriteLine("If you need any help, please enter \" H \"");
            //Write("Please make a move by entering col_num,row_num (e.g. 1,2): ");
            while (!isVaild)
            {
                Write("Please make a move by entering col_num,row_num (e.g. 1,2): ");
                string moveInput = ReadLine().ToUpper(); ;
                try
                {
                    if (moveInput == "U")
                    {
                        if (!isNew)
                        {
                            Undo(board);
                        }
                        WriteLine("You have to make a new move!");
                        isVaild = false;
                    }
                    else if (moveInput == "R")
                    {
                        if (!isNew)
                        {
                            if (UndoCount != 0)
                            {
                                Redo(board);
                                UndoCount--;
                            }
                            else
                            {
                                WriteLine("You cannot redo! Please make a move.");
                            }
                        }
                        else
                        {
                            WriteLine("You cannot redo! Please make a move.");
                        }
                        isVaild = false;
                    }
                    else if (moveInput == "A")
                    {
                        SaveGame("sos_saved_game.csv");
                        isVaild = false;
                        //Environment.Exit(0);
                    }
                    else if (moveInput == "Q")
                    {
                        QuitGame();
                        //isVaild = false;
                    }
                    else if (moveInput == "H")
                    {
                        PrintHelpSystem();
                        //isVaild = false;
                    }
                    else
                    {
                        //UndoCount = 0; // if new move added then can't redo
                        isNew = false;
                        string[] moves = moveInput.Split(",");
                        InputColumn = int.Parse(moves[0]);
                        InputRow = int.Parse(moves[1]);
                        if (CheckValid(board, InputColumn, InputRow))
                        {
                            if (currentPlayer.PlayerType == "Human")
                            {
                                token = ChooseToken();
                            }
                            board.PlaceToken(token, InputColumn - 1, InputRow - 1);
                            SaveMove(currentPlayer, currentPlayer, token, InputColumn, InputRow);
                            //board.PrintBoard();
                            SwitchTurn();
                            isVaild = true;
                        }
                        else
                        {
                            WriteLine("Unavailable move! Please try again!");
                            isVaild = false;
                        }
                    }
                }
                catch
                {
                    WriteLine("Unavailable choice! Please try again!");
                    isVaild = false;

                }

            }
        }
        protected Token ChooseToken()
        {
            Write("Please select your token - S/O :");
            string selection = ReadLine().ToUpper();
            while (selection != "S" && selection != "O")
            {
                Write("Invalid! Please re-enter: ");
                selection = ReadLine().ToUpper();
            }
            Token token = new Token(selection);
            return token;
        }
        protected override bool IsEnd()
        {
            for (int i = 0; i < sosBoard.Row; i++)
            {
                for (int j = 0; j < sosBoard.Col; j++)
                {
                    if (sosBoard.boardTokens[j, i].Symbol == ".")
                    {
                        isEnd = false;
                        return isEnd;
                    }
                }
            }
            isEnd = true;
            return isEnd;
        }
        protected int CountScore(Player player)
        {
            int score = 0;
            // Count scores for SOS game
            foreach (var move in history.Moves)
            {
                if (move.Item1.PlayerName == player.PlayerName)
                {
                    int col = move.Item4 - 1;
                    int row = move.Item5 - 1;


                    // Check horizontally
                    if (col + 2 < 3 && sosBoard.boardTokens[col, row].Symbol == "S" &&
                        sosBoard.boardTokens[col + 1, row].Symbol == "O" && sosBoard.boardTokens[col + 2, row].Symbol == "S")
                    {
                        score++;
                    }

                    // Check vertically
                    if (row + 2 < 3 && sosBoard.boardTokens[col, row].Symbol == "S" &&
                        sosBoard.boardTokens[col, row + 1].Symbol == "O" && sosBoard.boardTokens[col, row + 2].Symbol == "S")
                    {
                        score++;
                    }

                    // Check diagonally (top-left to bottom-right)
                    if (row + 2 < 3 && col + 2 < 3 && sosBoard.boardTokens[col, row].Symbol == "S" &&
                        sosBoard.boardTokens[col + 1, row + 1].Symbol == "O" && sosBoard.boardTokens[col + 2, row + 2].Symbol == "S")
                    {
                        score++;
                    }

                    // Check diagonally (bottom-left to top-right)
                    if (row - 2 >= 0 && col + 2 < 3 && sosBoard.boardTokens[col, row].Symbol == "S" &&
                        sosBoard.boardTokens[col + 1, row - 1].Symbol == "O" && sosBoard.boardTokens[col + 2, row - 2].Symbol == "S")
                    {
                        score++;
                    }
                }
            }
            return score;
        }
        protected override bool CheckWin(Player playerOne, Player playerTwo)
        {
            // Check for SOS board is fulle

            playerOneScore = CountScore(playerOne);
            playerTwoScore = CountScore(playerTwo);

            if (isEnd)
            {
                WriteLine("\nGame Over!");
                WriteLine("Final Scores:");
                WriteLine("Player 1 Score: {0}, Player 2 Score: {1}", playerOneScore, playerTwoScore);
                // Compare scores and declare the winner
                if (playerOneScore > playerTwoScore)
                {
                    WriteLine("Player 1 wins!");
                }
                else if (playerTwoScore > playerOneScore)
                {
                    WriteLine("Player 2 wins!");
                }
                else
                {
                    WriteLine("It's a tie!");
                }


                return true; // The game is over
            }
            return false; // The game is still ongoing
        }
    }

    public class ConnectFour : BoardGame
    {
        private Board connectFourBoard;
        Token playerOnetoken = new Token("O");
        Token playerTwotoken = new Token("X");

        protected override void InitializeGame()
        {
            // Initialize ConnectFour game logic
            bool isExecute = true;
            while (isExecute)
            {
                menu.DisplayGameMode();
                switch (menu.Option)
                {
                    case 1: //New ConnectFour game
                        CreatePlayer();
                        WriteLine("Welcome to ConnectFour game!");
                        connectFourBoard = new Board(7, 6);
                        history = new GameHistory();
                        isExecute = false;
                        break;
                    case 2:
                        connectFourBoard = new Board(7, 6);
                        ResumeGame(connectFourBoard, "cnf_saved_game.csv");
                        isNew = true;
                        isExecute = false;
                        break;

                }
            }
        }
        protected override void PlayGame()
        {
            connectFourBoard.PrintBoard();
            SwitchTurn();
            while (!IsEnd())
            {
                if (currentPlayer.PlayerName == "P1")
                {
                    PlayerMakeMove(connectFourBoard, currentPlayer, playerOnetoken);
                    SaveMove(currentPlayer, currentPlayer, playerOnetoken, InputColumn, InputRow);
                    Clear();
                    connectFourBoard.PrintBoard();
                }
                else if (currentPlayer.PlayerName == "P2")
                {
                    PlayerMakeMove(connectFourBoard, currentPlayer, playerTwotoken);
                    SaveMove(currentPlayer, currentPlayer, playerTwotoken, InputColumn, InputRow);
                    Clear();
                    connectFourBoard.PrintBoard();
                }
                SwitchTurn();
            }
            CheckWin(playerOne, playerTwo);

        }
        protected override void MakeMove(Board board, Token token)
        {
            bool isVaild = false;
            WriteLine("{0} It's your turn!", currentPlayer.PlayerName);
            WriteLine("If you need any help, please enter [ H ]");
            while (!isVaild)
            {
                Write("Please make a move by entering col_num,row_num (e.g. 1,6): ");
                string moveInput = ReadLine().ToUpper(); ;
                try
                {
                    if (moveInput == "U")
                    {
                        if (!isNew)
                        {
                            Undo(board);
                        }
                        WriteLine("You have to make a new move!");
                        isVaild = false;
                    }
                    else if (moveInput == "R")
                    {
                        if (!isNew)
                        {
                            if (UndoCount != 0)
                            {
                                Redo(board);
                                UndoCount--;
                            }
                            else
                            {
                                WriteLine("You cannot redo! Please make a move.");
                            }
                        }
                        else
                        {
                            WriteLine("You cannot redo! Please make a move.");
                        }
                        
                        isVaild = false;
                    }
                    else if (moveInput == "A")
                    {
                        SaveGame("cnf_saved_game.csv");
                        isVaild = false;
                    }
                    else if (moveInput == "Q")
                    {
                        QuitGame();
                        //isVaild = false;
                    }
                    else if (moveInput == "H")
                    {
                        PrintHelpSystem();
                        //isVaild = false;
                    }
                    else
                    {
                        isNew = false;
                        string[] moves = moveInput.Split(",");
                        InputColumn = int.Parse(moves[0]);
                        InputRow = int.Parse(moves[1]);
                        if (CheckValid(board, InputColumn, InputRow))
                        {
                            connectFourBoard.PrintBoard();
                            board.PlaceToken(token, InputColumn - 1, InputRow - 1);
                            //SwitchTurn();
                            isVaild = true;
                        }
                        else
                        {
                            WriteLine("Unavailable move! Please try again!");
                            isVaild = false;
                        }
                    }
                }
                catch
                {
                    WriteLine("Unavailable choice! Please try again! ");
                    isVaild = false;
                }
            }
        }
        protected override void MakeRandomMove(Board board)
        {
            Random random = new Random();
            int randomToken;
            do
            {
                InputColumn = random.Next(1, board.Col + 1);
                InputRow = random.Next(1, board.Row + 1);
                randomToken = random.Next(1, 3);// select random token
            }
            while (!CheckValid(board, InputColumn, InputRow));
            Token token = new Token("X");
            board.PlaceToken(token, InputColumn - 1, InputRow - 1);

        }
        protected override bool CheckValid(Board board, int col, int row)
        {
            if (row <= board.Row && row > 0 && col <= board.Col && col > 0)
            {

                foreach (var item in history.Moves)
                {
                    if (col == item.Item4 && row == item.Item5)
                    {
                        return false; // return false if move is in the move history
                    }
                }
                if (row == board.Row)
                {
                    return true; // Return true if the row is the bottom row
                }
                else
                {
                    if (board.boardTokens[col - 1, row].Symbol == ".")
                    {
                        WriteLine("Tokens can only be placed from the bottom row or above another token.");
                        return false; // Return false if the position below is empty
                    }
                    return true; // Return true otherwise
                }

            }
            return false; // Return false if not within the board size   
        }
        protected override bool IsEnd()
        {
            int numRows = connectFourBoard.Row;
            int numCols = connectFourBoard.Col;
            string currentPlayerSymbol;
            if (currentPlayer.PlayerName == "P1")
            {
                currentPlayerSymbol = "O";
            }
            else
            {
                currentPlayerSymbol = "X";
            }

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    // Check for a win horizontally
                    if (col <= numCols - 4)
                    {
                        bool isWinHorizontal = true;
                        for (int k = 0; k < 4; k++)
                        {
                            if (connectFourBoard.boardTokens[col + k, row].Symbol != currentPlayerSymbol)
                            {
                                isWinHorizontal = false;
                                break;
                            }
                        }
                        if (isWinHorizontal)
                        {
                            isEnd = true;
                            return isEnd;
                        }
                    }

                    // Check for a win vertically
                    if (row <= numRows - 4)
                    {
                        bool isWinVertical = true;
                        for (int k = 0; k < 4; k++)
                        {
                            if (connectFourBoard.boardTokens[col, row + k].Symbol != currentPlayerSymbol)
                            {
                                isWinVertical = false;
                                break;
                            }
                        }
                        if (isWinVertical)
                        {
                            isEnd = true;
                            return isEnd;
                        }
                    }

                    // Check for a win diagonally (from bottom-left to top-right)
                    if (row >= 3 && col <= numCols - 4)
                    {
                        bool isWinDiagonal1 = true;
                        for (int k = 0; k < 4; k++)
                        {
                            if (connectFourBoard.boardTokens[col + k, row - k].Symbol != currentPlayerSymbol)
                            {
                                isWinDiagonal1 = false;
                                break;
                            }
                        }
                        if (isWinDiagonal1)
                        {
                            isEnd = true;
                            return isEnd;
                        }
                    }

                    // Check for a win diagonally (from top-left to bottom-right)
                    if (row <= numRows - 4 && col <= numCols - 4)
                    {
                        bool isWinDiagonal2 = true;
                        for (int k = 0; k < 4; k++)
                        {
                            if (connectFourBoard.boardTokens[col + k, row + k].Symbol != currentPlayerSymbol)
                            {
                                isWinDiagonal2 = false;
                                break;
                            }
                        }
                        if (isWinDiagonal2)
                        {
                            isEnd = true;
                            return isEnd;
                        }
                    }
                }
            }

            // No win found, continue the game
            return false;
        }
        protected override bool CheckWin(Player playerOne, Player playerTwo)
        {

            if (isEnd)
            {
                WriteLine("\nGame Over!");
                WriteLine("{0} is the Winner!", currentPlayer.PlayerName);
                return true; // The game is over
            }
            return false; // The game is still ongoing
        }
    }
}

