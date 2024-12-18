using System.Collections.Immutable;

namespace Day18;

public class Runner(int gridSize, HashSet<(int, int)> initialBytes)
{
    private readonly int _gridSize = gridSize;
    private readonly HashSet<(int, int)> _occupied = [..initialBytes];

    private List<(int, int)>? _lastPath;

    public int? Run()
    {
        if (_lastPath == null)
        {
            RunNewPath();
        }

        return _lastPath?.Count - 1;
    }

    public void AddObstacle((int, int) obstacle)
    {
        _occupied.Add(obstacle);
        if (_lastPath?.Contains(obstacle) == true)
        {
            _lastPath = null;
        }
    }

    private readonly ImmutableList<(int R, int C)> _deltas = ImmutableList.CreateRange([(-1, 0), (1, 0), (0, 1), (0, -1)]);
    private void RunNewPath()
    {
        _lastPath = null;

        var distances = new Dictionary<(int, int), int>();
        var prev = new Dictionary<(int, int), (int, int)>();
        var queue = new PriorityQueue<(int R, int C), int>();
        for (var r = 0; r < _gridSize; r++)
        {
            for (var c = 0; c < _gridSize; c++)
            {
                if (r == 0 && c == 0)
                {
                    continue;
                }
                distances[(r, c)] = int.MaxValue;
            }
        }

        distances[(0, 0)] = 0;
        queue.Enqueue((0, 0), 0);

        var visited = new HashSet<(int, int)>();

        while (queue.TryDequeue(out var pos, out var dist))
        {
            if (visited.Contains(pos))
            {
                continue;
            }

            visited.Add(pos);

            if (pos == (_gridSize - 1, _gridSize - 1))
            {
                _lastPath = [];
                while (true)
                {
                    _lastPath.Add(pos);
                    if (pos == (0, 0))
                    {
                        break;
                    }
                    pos = prev[pos];
                } 
            }

            var newDist = dist + 1;
            foreach (var (dr, dc) in _deltas)
            {
                var newPos = (R: pos.R + dr, C: pos.C + dc);
                if (newPos.R < 0 || newPos.R >= _gridSize || newPos.C < 0 || newPos.C >= _gridSize)
                {
                    continue;
                }

                if (_occupied.Contains(newPos))
                {
                    continue;
                }

                if (distances[newPos] <= newDist)
                {
                    continue;
                }

                distances[newPos] = newDist;
                queue.Enqueue(newPos, newDist);
                prev[newPos] = pos;
            }
        }
    }
}

public class Solver(int takeOne, int gridSize, string input)
{
    private readonly int _takeOne = takeOne;
    private readonly int _gridSize = gridSize;
    private readonly ImmutableList<(int R, int C)> _bytes = ImmutableList.CreateRange(input.Trim()
        .Split("\n")
        .Select(pair =>
        {
            var coords = pair.Split(",").Select(int.Parse).ToList();
            return (coords[0], coords[1]);
        }));

    private readonly ImmutableList<(int R, int C)> _deltas = ImmutableList.CreateRange([(-1, 0), (1, 0), (0, 1), (0, -1)]);

    public int SolvePartOne()
    {
        var runner = new Runner(_gridSize, _bytes.Take(_takeOne).ToHashSet());
        return runner.Run()!.Value;
    }

    public string SolvePartTwo()
    {
        var runner = new Runner(_gridSize, _bytes.Take(_takeOne).ToHashSet());

        foreach (var b in _bytes.Skip(_takeOne))
        {
            runner.AddObstacle(b);
            if (runner.Run() is null)
            {
                return $"{b.R},{b.C}";
            }
        }
        throw new Exception("no solution found");
    }

    // public int SolvePartOne()
    // {
    //     return Solve(12) ?? throw new Exception("No solution found");
    // }

    // public (int R, int C) SolvePartTwo()
    // {
    //     for (var t = _takeOne; t < _bytes.Count; t++)
    //     {
    //         if (Solve(t) is null)
    //         {
    //             return _bytes[t - 1];
    //         }
    //     }

    //     throw new Exception("No Solution found");
    // }

    // private int? Solve(int take)
    // {
    //     var occupied = new HashSet<(int, int)>(_bytes.Take(take));

    //     var distances = new Dictionary<(int, int), int>();
    //     var queue = new PriorityQueue<(int R, int C), int>();
    //     for (var r = 0; r < _gridSize; r++)
    //     {
    //         for (var c = 0; c < _gridSize; c++)
    //         {
    //             if (r == 0 && c == 0)
    //             {
    //                 continue;
    //             }
    //             distances[(r, c)] = int.MaxValue;
    //         }
    //     }

    //     distances[(0, 0)] = 0;
    //     queue.Enqueue((0, 0), 0);

    //     var visited = new HashSet<(int, int)>();

    //     while (queue.TryDequeue(out var pos, out var dist))
    //     {
    //         if (visited.Contains(pos))
    //         {
    //             continue;
    //         }

    //         visited.Add(pos);

    //         if (pos == (_gridSize - 1, _gridSize - 1))
    //         {
    //             return dist;
    //         }

    //         var newDist = dist + 1;
    //         foreach (var (dr, dc) in _deltas)
    //         {
    //             var newPos = (R: pos.R + dr, C: pos.C + dc);
    //             if (newPos.R < 0 || newPos.R >= _gridSize || newPos.C < 0 || newPos.C >= _gridSize)
    //             {
    //                 continue;
    //             }

    //             if (occupied.Contains(newPos))
    //             {
    //                 continue;
    //             }

    //             if (distances[newPos] <= newDist)
    //             {
    //                 continue;
    //             }

    //             distances[newPos] = newDist;
    //             queue.Enqueue(newPos, newDist);
    //         }
    //     }

    //     return null;
    // }
}