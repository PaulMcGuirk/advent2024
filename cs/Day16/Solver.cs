using System.Collections.Immutable;

namespace Day16;

public partial class Solver
{
    private readonly ImmutableList<ImmutableList<bool>> _grid;
    private readonly (int Row, int Col, int DeltaRow, int DeltaCol) _start;
    private readonly (int Row, int Col) _end;
    private readonly int _gridSize;

    private const int TURN_COST = 1000;

    public Solver(string input)
    {
        (int, int, int, int)? start = null;
        (int, int)? end = null;

        _grid = ImmutableList.CreateRange(input.Trim().Split("\n")
            .Select((line, r) => ImmutableList.CreateRange(line.Select((ch, c) =>
                {
                    if (ch == 'S')
                    {
                        start = (r, c, 0, 1);
                        return true;
                    }
                    if (ch == 'E')
                    {
                        end = (r, c);
                        return true;
                    }
                    if (ch == '.')
                    {
                        return true;
                    }
                    if (ch == '#')
                    {
                        return false;
                    }
                    throw new Exception();
                }))));

        _gridSize = _grid.Count;
        if (_grid[0].Count != _gridSize)
        {
            throw new Exception();
        }

        _start = start!.Value;
        _end = end!.Value;
    }

    private static readonly ImmutableList<(int R, int C)> _deltas = ImmutableList.CreateRange([(0, 1), (0, -1), (1, 0), (-1, 0)]);

    public (long, int) Solve()
    {
        var distances = new Dictionary<(int Row, int Col, int DeltaRow, int DeltaCol), long>
        {
            [_start] = 0
        };

        var prevs = new Dictionary<(int Row, int Col, int DeltaRow, int DeltaCol), List<(int Row, int Col, int DeltaRow, int DeltaCol)>>();

        var queue = new PriorityQueue<(int Row, int Col, int DeltaRow, int DeltaCol), long>();
        queue.Enqueue(_start, 0);


        for (var r = 0; r < _gridSize; r++)
        {
            for (var c = 0; c < _gridSize; c++)
            {
                foreach (var (dr, dc) in _deltas)
                {
                    var node = (r, c, dr, dc);

                    prevs[node] = [];

                    if (node == _start)
                    {
                        continue;
                    }

                    distances[node] = long.MaxValue;
                    queue.Enqueue(node, long.MaxValue);
                }
            }
        }

        var visited = new HashSet<(int, int, int, int)>();
        long? bestCost = null;
        var backStack = new Stack<(int Row, int Col, int DeltaRow, int DeltaCol)>();

        while (queue.TryDequeue(out var node, out var cost))
        {
            var (r, c, dr, dc) = node;

            if ((r, c) == _end)
            {
                if (bestCost is null)
                {
                    bestCost = cost;
                }
                backStack.Push(node);
                continue;
            }

            if (cost >= bestCost)
            {
                break;
            }

            if (visited.Contains(node))
            {
                continue;
            }

            visited.Add(node);

            foreach (var newDir in _deltas)
            {
                var (newR, newC) = (r + newDir.R, c + newDir.C);
                if (!_grid[newR][newC])
                {
                    continue;
                }

                long turnCost;
                if (newDir == (dr, dc))
                {
                    turnCost = 0;
                }
                else if (newDir == (-dr, -dc))
                {
                    turnCost = 2 * TURN_COST;
                }
                else
                {
                    turnCost = TURN_COST;
                }

                var newCost = turnCost + 1 + cost;

                var newNode = (newR, newC, newDir.R, newDir.C);
                if (newCost > distances[newNode])
                {
                    continue;
                }
                if (newCost < distances[newNode])
                {
                    prevs[newNode] = [];
                }

                distances[newNode] = newCost;
                prevs[newNode].Add(node);
                queue.Enqueue(newNode, newCost);
            }
        }

        var best = new HashSet<(int, int)>();
        while (backStack.TryPop(out var node))
        {
            var (r, c, _, _)= node;
            best.Add((r, c));
            foreach (var p in prevs[node])
            {
                backStack.Push(p);
            }
        }

        return (bestCost!.Value, best.Count);
    }
}