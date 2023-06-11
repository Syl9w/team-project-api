using Microsoft.AspNetCore.Mvc;

namespace sudoku_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBoard()
        {
            int[][] board = GeneratePuzzle();
            return Ok(board);
        }

        public int[][] GeneratePuzzle()
        {
            int[][] board = new int[9][];

            // Generate solved Sudoku
            Random random = new Random();
            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                board[rowIndex] = new int[9];
                for (int colIndex = 0; colIndex < 9; colIndex++)
                {
                    IEnumerable<int> alreadyUsedRowNumbers = Enumerable.Range(1, 9).Except(board[rowIndex]);

                    int[] alreadyUsedColumnNumbers = new int[rowIndex];
                    for (int k = 0; k < rowIndex; k++)
                    {
                        alreadyUsedColumnNumbers[k] = board[k][colIndex];
                    }
                    IEnumerable<int> validOptions = alreadyUsedRowNumbers.Except(alreadyUsedColumnNumbers);

                    bool validNumberFound = false;
                    foreach (int number in validOptions.OrderBy(x => random.Next()))
                    {
                        board[rowIndex][colIndex] = number;
                        if (rowIndex < 8 || colIndex < 8 || GeneratePuzzle(board, rowIndex, colIndex))
                        {
                            validNumberFound = true;
                            break;
                        }
                    }

                    if (!validNumberFound)
                    {
                        return GeneratePuzzle(); // Try again
                    }
                }
            }

            // Remove some cells to create a puzzle
            int numberOfCellsToRemove = random.Next(30, 61);
            for (int i = 0; i < numberOfCellsToRemove; i++)
            {
                int rowToEmpty = random.Next(9);
                int colToEmpty = random.Next(9);
                board[rowToEmpty][colToEmpty] = 0;
            }

            return board;
        }

        private bool GeneratePuzzle(int[][] board, int rowIndex, int colIndex)
        {
            if (rowIndex == 8 && colIndex == 9)
            {
                return true; // Solution is found
            }

            if (colIndex == 9)
            {
                rowIndex++;
                colIndex = 0;
            }

            if (board[rowIndex][colIndex] != 0)
            {
                return GeneratePuzzle(board, rowIndex, colIndex + 1);
            }

            Random random = new Random();
            IEnumerable<int> alreadyUsedRowNumbers = Enumerable.Range(1, 9).Except(board[rowIndex]);

            int[] alreadyUsedColumnNumbers = new int[rowIndex];
            for (int k = 0; k < rowIndex; k++)
            {
                alreadyUsedColumnNumbers[k] = board[k][colIndex];
            }
            IEnumerable<int> validOptions = alreadyUsedRowNumbers.Except(alreadyUsedColumnNumbers);

            foreach (int number in validOptions.OrderBy(x => random.Next()))
            {
                board[rowIndex][colIndex] = number;

                if (GeneratePuzzle(board, rowIndex, colIndex + 1))
                {
                    return true;
                }
            }

            board[rowIndex][colIndex] = 0;
            return false;
        }
    }
}