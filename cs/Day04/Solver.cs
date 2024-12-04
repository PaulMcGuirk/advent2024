namespace Day04;

public partial class Solver
{
    private readonly IReadOnlyList<IReadOnlyList<char>> _grid;
    private readonly int _gridSize;
    private const string TARGET = "XMAS";

    public Solver(string input)
    {
        _grid = input
           .Trim()
           .Split("\n")
           .Where(line => !string.IsNullOrEmpty(line))
           .Select(line => line.ToList().AsReadOnly())
           .ToList()
           .AsReadOnly();
        _gridSize = _grid.Count;
    }

    public int SolvePartOne()
    {
        var result = 0;
        for (var i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                // right
                if (j <= _gridSize - TARGET.Length)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i][j + k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // left
                if (j >= TARGET.Length - 1)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i][j - k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // down
                if (i <= _gridSize - TARGET.Length)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i + k][j]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // up
                if (i >= TARGET.Length - 1)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i - k][j]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // up and left
                if (i >= TARGET.Length - 1 && j >= TARGET.Length - 1)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i - k][j - k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // up right
                if (i >= TARGET.Length - 1 && j <= _gridSize - TARGET.Length)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i - k][j + k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // down and left
                if (i <= _gridSize - TARGET.Length && j >= TARGET.Length - 1)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i + k][j - k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }

                // down right
                if (i <= _gridSize - TARGET.Length && j <= _gridSize - TARGET.Length)
                {
                    var chars = new List<char>();
                    for (var k = 0; k < TARGET.Length; k++)
                    {
                        chars.Add(_grid[i + k][j + k]);
                    }
                    if (string.Join("", chars) == TARGET)
                    {
                        result++;
                    }
                }
            }
        }

        return result;
    }

    public int SolvePartTwo()
    {
        var result = 0;
        for (var i = 1; i < _gridSize - 1; i++)
        {
            for (var j = 1; j < _gridSize - 1; j++)
            {
                if (_grid[i][j] != 'A')
                {
                    continue;
                }

                var corners = new List<char> {
                    _grid[i - 1][j - 1],
                    _grid[i - 1][j + 1],
                    _grid[i + 1][j - 1],
                    _grid[i + 1][j + 1]
                };

                if (corners.Count(c => c == 'M') == 2 && 
                    corners.Count(c => c == 'S') == 2 &&
                    _grid[i - 1][j - 1] != _grid[i + 1][j + 1]) 
                {
                    result++;
                }
            }
        }

        return result;
    }
}