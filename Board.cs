using System;
using static System.Console;
namespace Assignment_2
{
    public class Board
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Token[,] boardTokens;

        public Board(int cols, int rows)
        {
            Row = rows;
            Col = cols;
            boardTokens = new Token[Col, Row];
            CreateBoard();
        }

        public void CreateBoard()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    boardTokens[j, i] = new Token(".");
                }
            }
        }
        public void PrintBoard()
        {
            WriteLine("-----------------------------------------------------------------------------------------------------------\n");
            for (int boardC = 0; boardC < Col; boardC++)
            {
                Write("   {0}  ", boardC + 1);
            }
            WriteLine();
            for (int boardC = 0; boardC < Col; boardC++)
            {
                Write(" -----");
            }
            WriteLine();

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    Write("|  " + boardTokens[j, i].GetSymbol() + "  ");

                }
                WriteLine("| " + (i + 1));
            }

            for (int boardC = 0; boardC < Col; boardC++)
            {
                Write(" -----");
            }
            Write("\n\n");
        }
        public void PlaceToken(Token token, int col, int row)
        {
            boardTokens[col, row] = token;
        }
    }
}

