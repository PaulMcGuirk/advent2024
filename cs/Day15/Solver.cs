using System.Collections.Immutable;

namespace Day15;

public enum Tile
{
    Empty,
    Box,
    Wall,
    Robot,
    BoxLeft,
    BoxRight
}

public record Vec(int R, int C)
{
    public static Vec operator+ (Vec a, Vec b) => new (a.R + b.R, a.C + b.C);
    public static Vec operator+ (Vec a, (int R, int C) b) => new (a.R + b.R, a.C + b.C);
    public (int, int) ToTuple() => (R, C);
}

public class MapRunner
{
    private readonly int _numRows;
    private readonly int _numCols;
    private List<List<Tile>> _map;
    private readonly ImmutableList<Vec> _directions;
    private Vec _botPos;

    public MapRunner(
        ImmutableList<ImmutableList<Tile>> referenceMap,
        ImmutableList<Vec> directions,
        bool expand
        )
    {
        _numRows = referenceMap.Count;
        _numCols = referenceMap[0].Count * (expand ? 2 : 1);

        _map = referenceMap.Select(row => row
            .SelectMany(t =>
            {
                if (!expand)
                {
                    return new List<Tile> { t };
                }
                return t switch
                {
                    Tile.Box => [Tile.BoxLeft, Tile.BoxRight],
                    Tile.Robot => [Tile.Robot, Tile.Empty],
                    _ => [t, t]
                };
            }).ToList()
        ).ToList();

        _botPos = Enumerable.Range(0, _numRows)
            .SelectMany(r => Enumerable.Range(0, _numCols).Select(c => new Vec(r, c)))
            .Where(pt => _map[pt.R][pt.C] == Tile.Robot)
            .Single();
        
        _directions = directions;
    }

    public long Run()
    {
        foreach (var dir in _directions)
        {
            TryMoveRobot(dir);
        }

        return Score();
    }

    private long Score()
    {
        var res = 0 ;
        for (var r = 0; r < _numRows; r++)
        {
            for (var c = 0; c < _numCols; c++)
            {
                if (_map[r][c] is Tile.Box or Tile.BoxLeft)
                {
                    res += 100 * r + c;
                }
            }
        }
        return res;
    }

    private void TryMoveRobot(Vec dir)
    {
        var toCheck = new Stack<Vec>();
        toCheck.Push(_botPos);

        var toMove = new Stack<Vec>();

        while (toCheck.TryPop(out var pos))
        {
            var newPos = pos + dir;
            var targetTile = _map[newPos.R][newPos.C];

            if (targetTile is Tile.Wall)
            {
                return; // nothing moves
            }

            toMove.Push(pos); // going to move this element if everything moves

            if (targetTile is Tile.Empty)
            {
                continue;
            }

            if (targetTile is Tile.Box)
            {
                toCheck.Push(newPos);
            }

            if (targetTile is Tile.BoxLeft)
            {
                switch (dir.C)
                {
                    case -1:
                        throw new Exception();
                    case 1:
                        toMove.Push(newPos);
                        toCheck.Push(newPos + dir);
                        break;
                    default:
                        toCheck.Push(newPos);
                        toCheck.Push(newPos + (0, 1));
                        break;
                }
            }

            if (targetTile is Tile.BoxRight)
            {
                switch (dir.C)
                {
                    case 1:
                        throw new Exception();
                    case -1:
                        toMove.Push(newPos);
                        toCheck.Push(newPos + dir);
                        break;
                    default:
                        toCheck.Push(newPos);
                        toCheck.Push(newPos + (0, -1));
                        break;
                }
            }
        }

        // if we got here then everything can move
        var moved = new HashSet<(int, int)>();
        while (toMove.TryPop(out var pos))
        {
            var tup = pos.ToTuple();
            if (moved.Contains(tup))
            {
                continue;
            }
            moved.Add(tup);
            var newPos = pos + dir;
            _map[newPos.R][newPos.C] = _map[pos.R][pos.C];
            _map[pos.R][pos.C] = Tile.Empty;
        }
        _botPos += dir;
    }
}

public partial class Solver
{
    private readonly ImmutableList<ImmutableList<Tile>> _initialMap;
    private readonly ImmutableList<Vec> _directions;

    public Solver(string input)
    {
        var chunks = input.Trim().Split("\n\n");
        _initialMap = ImmutableList.CreateRange(chunks[0].Trim().Split("\n")
            .Select(line => ImmutableList.CreateRange(line
                .Select(ch => ch switch
                {
                    '#' => Tile.Wall,
                    '.' => Tile.Empty,
                    'O' => Tile.Box,
                    '@' => Tile.Robot,
                    _ => throw new Exception()

                }))));

        _directions = ImmutableList.CreateRange(chunks[1]
            .Where(ch => ch != '\n')
            .Select(ch => ch switch
            {
                '<' => new Vec(0, -1),
                '^' => new Vec(-1, 0),
                '>' => new Vec(0, 1),
                'v' => new Vec(1, 0),
                _ => throw new Exception()
            }));
        
    }

    public long SolvePartOne() => Solve(false);
    public long SolvePartTwo() => Solve(true);

    private long Solve(bool expand)
    {
        var runner = new MapRunner(_initialMap, _directions, expand);
        return runner.Run();
    }
}