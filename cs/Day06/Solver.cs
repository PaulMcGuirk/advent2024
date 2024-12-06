using System.Collections.Immutable;

namespace Day06;

public class Solver
{
    private readonly (int, int) _initialPos;
    private readonly (int, int) _initialDir;
    private readonly int _gridSize;
    private readonly ImmutableHashSet<(int, int)> _obstacles;

    public Solver(string input)
    {
        var lines = input.Trim().Split("\n");
        _gridSize = lines.Length;

        var obstacles = new List<(int, int)>();

        var dirs = new Dictionary<char, (int, int)>
        {
            ['v'] = (1, 0),
            ['^'] = (-1, 0),
            ['<'] = (0, -1),
            ['>'] = (0, 1),
        };

        for (var r = 0; r < _gridSize; r++)
        {
            for (var c = 0; c < _gridSize; c++)
            {
                switch (lines[r][c])
                {
                    case '#':
                        obstacles.Add((r, c));
                        break;
                    case '.':
                        break;
                    case 'v':
                    case '>':
                    case '<':
                    case '^':
                        _initialPos = (r, c);
                        _initialDir = dirs[lines[r][c]];
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        _obstacles = ImmutableHashSet.Create([.. obstacles]);
    }

    public (int, int) Solve()
    {
        var (path, _) = Walk(_obstacles);

        var partOne = path.Count;
        var partTwo = path.Where(obs =>
             {
                 if (obs == _initialPos)
                 {
                     return false;
                 }
                 var (_, isLoop) = Walk(_obstacles.Add(obs));
                 return isLoop;
             })
        .Count();

        return (partOne, partTwo);
    }

    private (HashSet<(int, int)> Path, bool Loop) Walk(ImmutableHashSet<(int, int)> obstacles)
    {
        var visitedPositions = new HashSet<(int, int)>();
        var visitedPositionsWithDirections = new HashSet<(int, int, int, int)>();

        var (r, c) = _initialPos;
        var (d_r, d_c) = _initialDir;

        while (true)
        {
            if (visitedPositionsWithDirections.Contains((r, c, d_r, d_c)))
            {
                return (visitedPositions, true);
            }

            visitedPositions.Add((r, c));
            visitedPositionsWithDirections.Add((r, c, d_r, d_c));

            var (n_r, n_c) = (r + d_r, c + d_c);
            if (n_r < 0 || n_r >= _gridSize || n_c < 0 || n_c >= _gridSize)
            {
                break;
            }

            if (obstacles.Contains((n_r, n_c)))
            {
                (d_r, d_c) = (d_c, -d_r);
                continue;
            }
            (r, c) = (n_r, n_c);
        }

        return (visitedPositions, false);
    }
}