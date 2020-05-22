using System;
using Xunit;
using System.Linq;
using Xunit.Abstractions;

namespace ConwaysGameOfLife
{
    public class UnitTest1
    {
        public int[,] _board;
        private readonly ITestOutputHelper _logger;
        private readonly GameOfLife _game;

        public UnitTest1(ITestOutputHelper log)
        {
            _logger = log;
            _game = GameOfLife.Create(size: 10);
            _board = _game.Display();
        }

        [Fact]
        public void Can_Create_Game_And_View_Initial_Board()
        {
            Assert.NotNull(_board);
            Assert.IsType<int[,]>(_board);
            Assert.Equal(10, _board.GetLength(0));
        }

        [Fact]
        public void Game_Is_Initialised_With_Random_State()
        {
            _logger.WriteLine(_game.CountAlive().ToString());
            _logger.WriteLine(_game.CountDead().ToString());
            Assert.True(_game.CountAlive() >= 1);
            Assert.True(_game.CountAlive() <= 50);
        }

        [Fact]
        public void When_Game_Ticks_State_Changes()
        {
            var initialBoard = new int[10, 10];

            initialBoard[5, 5] = 1;

            var game = new GameOfLife(initialBoard);

            var newBoard = game.Tick();

            Assert.Equal(0, newBoard[5, 5]);
        }

        [Fact]
        public void When_Cell_Has_Two_Living_Neighbour_Cell_Stays_Alive()
        {
            var initialBoard = new int[10, 10];

            initialBoard[5, 5] = 1;
            initialBoard[5, 6] = 1;
            initialBoard[6, 6] = 1;

            var game = new GameOfLife(initialBoard);
            var newBoard = game.Tick();

            Assert.Equal(1, newBoard[5, 5]);
            Assert.Equal(1, newBoard[5, 6]);
            Assert.Equal(1, newBoard[6, 6]);
            Assert.Equal(3, game.CountAlive());
        }
    }

    public static class ArrayExtensions
    {
        public static int Count(this int[,] array, int state)
        {
            int total = 0;
            foreach (var x in array)
            {
                if (x == state)
                {
                    total++;
                }
            }
            return total;
        }
    }

    public class GameOfLife
    {
        private readonly int[,] currentBoard;
        private readonly int _size;

        public GameOfLife(int size)
        {
            currentBoard = RandomizeBoard(size);
            _size = size;
        }

        public GameOfLife(int[,] initialBoard)
        {
            currentBoard = initialBoard;
            _size = initialBoard.GetLength(0);
        }

        private static int[,] RandomizeBoard(int size)
        {
            var board = new int[size, size];

            var seed = new Random();
            var aliveCells = seed.Next(0, (size * size) / 2);

            for (int i = 0; i < aliveCells; i++)
            {
                var x = seed.Next(0, 10);
                var y = seed.Next(0, 10);
                board[x, y] = 1;
            }

            return board;
        }

        public int[,] Tick()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (CountNeighbours(i, j) == 2)
                    {
                        currentBoard[i, j] = 1;
                    }
                    else
                    {
                        currentBoard[i, j] = 0;
                    }
                }
            }
            return currentBoard;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;
            // Bottom Row
            count += x > 0 && y > 0 && currentBoard[x - 1, y - 1] == 1 ? 1 : 0;
            count += x > 0 && currentBoard[x - 1, y] == 1 ? 1 : 0;
            count += x > 0 && y < _size - 1 && currentBoard[x - 1, y + 1] == 1 ? 1 : 0;

            // Middle
            count += y > 0 && currentBoard[x, y - 1] == 1 ? 1 : 0;
            count += y < _size - 1 && currentBoard[x, y + 1] == 1 ? 1 : 0;

            // Top Row
            count += x < _size - 1 && y > 0 && currentBoard[x + 1, y - 1] == 1 ? 1 : 0;
            count += x < _size - 1 && currentBoard[x + 1, y] == 1 ? 1 : 0;
            count += x < _size - 1 && y < _size - 1 && currentBoard[x + 1, y + 1] == 1 ? 1 : 0;
            return count;
        }

        public int CountAlive()
        {
            return currentBoard.Count(1);
        }

        public int CountDead()
        {
            return currentBoard.Count(0);
        }

        public int[,] Display()
        {
            return currentBoard;
        }

        public static GameOfLife Create(int size)
        {
            return new GameOfLife(size);
        }
    }
}
