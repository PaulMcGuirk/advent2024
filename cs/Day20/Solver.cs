using System.Collections.Immutable;

namespace Day20;

public class Solver
{
    private readonly ImmutableHashSet<(int R, int C)> _walls;
    private readonly (int R, int C) _start;
    private readonly (int R, int C) _end;
    private readonly ImmutableList<(int R, int C)> _deltas = ImmutableList.CreateRange([(1, 0), (-1, 0), (0, -1), (0, 1)]);
    private readonly int _referenceTime;

    public Solver(string input)
    {
        var walls = new List<(int, int)>();

        var numRows = 0;
        var numCols = 0;

        foreach (var (line, r) in input.Trim().Split("\n").Select((line, r) => (line, r)))
        {
            numRows++;
            foreach (var (ch, c) in line.Trim().Select((ch, c) => (ch, c)))
            {
                if (r == 0)
                {
                    numCols++;
                }
                switch (ch)
                {
                    case '#':
                        walls.Add((r, c));
                        break;
                    case 'S':
                        _start = (r, c);
                        break;
                    case 'E':
                        _end = (r, c);
                        break;
                    case '.':
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        _walls = ImmutableHashSet.CreateRange(walls);

        _referenceTime = numCols * numRows - _walls.Count - 1;
    }

    public int SolvePartOne(int delta) => Solve(2, _referenceTime - delta);
    public int SolvePartTwo(int delta) => Solve(20, _referenceTime - delta);

    private int Solve(int cheatLength, int threshold)
    {
        var noCheatDistances = SolveWithoutCheating();

        var counts = Enumerable.Repeat(0, _referenceTime + 1).ToList();
        var res = 0;

        var pos = _start;
        while (pos != _end)
        {
            for (var dr = -cheatLength; dr <= cheatLength; dr++)
            {
                var max = cheatLength - Math.Abs(dr);
                for (var dc = -max; dc <= max; dc++)
                {
                    if (dr == 0 && dc == 0)
                    {
                        continue;
                    }

                    var jump = (pos.R + dr, pos.C + dc);
                    if (noCheatDistances.TryGetValue(jump, out var otherPos))
                    {
                        var pathDist = Math.Abs(dr) + Math.Abs(dc);
                        var dist = noCheatDistances[jump] + pathDist + (_referenceTime - noCheatDistances[pos]);
                        if (dist <= _referenceTime)
                        {
                            counts[dist]++;
                            if (dist <= threshold)
                            {
                                res++;
                            }
                        }
                        
                    }
                }
            }

            pos = _deltas.Select(delta => (pos.R + delta.R, pos.C + delta.C))
                .Where(np => !_walls.Contains(np) && noCheatDistances[np] < noCheatDistances[pos])
                .Single();
        }

        return res;
    }

    private Dictionary<(int R, int C), int> SolveWithoutCheating()
    {
        var distances = new Dictionary<(int R, int C), int>() { [_end] = 0 };

        var dist = 0;
        var pos = _end;
        var visited = new HashSet<(int, int)>();
        do
        {
            visited.Add(pos);
            pos = _deltas.Select(delta => (pos.R + delta.R, pos.C + delta.C))
                .Where(np => !_walls.Contains(np) && !visited.Contains(np))
                .First();
            distances[pos] = ++dist;
        } while (pos != _start);

        return distances;
    }
}