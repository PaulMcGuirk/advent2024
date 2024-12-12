using System.Collections.Immutable;

namespace Day12;

public class Solver(string input)
{
    private readonly ImmutableList<ImmutableList<char>> _map = 
        ImmutableList.CreateRange(input.Trim().Split("\n")
            .Select(line => ImmutableList.CreateRange(line)));

    private static readonly ImmutableList<(int, int)> _deltas = 
        ImmutableList.CreateRange([(-1, 0), (1, 0), (0, -1), (0,1)]);

    public (long, long) Solve()
    {
        var size = _map.Count;
        var counted = new HashSet<(int, int)>();

        var partOne = 0L;
        var partTwo = 0L;

        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                if (counted.Contains((r, c)))
                {
                    continue;
                }

                var ch = _map[r][c];

                Stack<(int R, int C)> toVisit = [];
                toVisit.Push((r, c));
                var region = new HashSet<(int, int)>();
                var area = 0;
                var perim = 0;

                var minR = int.MaxValue;
                var maxR = int.MinValue;
                var minC = int.MaxValue;
                var maxC = int.MinValue;

                while (toVisit.TryPop(out var pt))
                {
                    if (counted.Contains(pt))
                    {
                        continue;
                    }

                    counted.Add(pt);
                    region.Add(pt);
                    minR = Math.Min(pt.R, minR);
                    maxR = Math.Max(pt.R, maxR);
                    minC = Math.Min(pt.C, minC);
                    maxC = Math.Max(pt.C, maxC);
                    area++;

                    foreach (var (dr, dc) in _deltas)
                    {
                        var (nr, nc) = (pt.R + dr, pt.C + dc);
                        if (nr < 0 || nr >= size || nc < 0 || nc >= size)
                        {
                            perim++;
                            continue;
                        }

                        if (_map[nr][nc] == ch)
                        {
                            toVisit.Push((nr, nc));
                        }
                        else
                        {
                            perim++;
                        }
                    }
                }

                var sides = 0L;

                for (var rr = minR; rr <= maxR; rr++)
                {
                    // upward facing sides
                    var sidePart = false;
                    for (var cc = minC; cc <= maxC; cc++)
                    {
                        var newIsSidePart = region.Contains((rr, cc)) && !region.Contains((rr - 1, cc));
                        if (!newIsSidePart && sidePart)
                        {
                            sides++;
                        }
                        sidePart = newIsSidePart;
                    }

                    if (sidePart)
                    {
                        sides++;
                    }

                    // downward facing sides
                    sidePart = false;
                    for (var cc = minC; cc <= maxC; cc++)
                    {
                        var newIsSidePart = region.Contains((rr, cc)) && !region.Contains((rr + 1, cc));
                        if (!newIsSidePart && sidePart)
                        {
                            sides++;
                        }
                        sidePart = newIsSidePart;
                    }

                    if (sidePart)
                    {
                        sides++;
                    }
                }

                for (var cc = minC; cc <= maxC; cc++)
                {
                    // leftward facing sides
                    var sidePart = false;
                    for (var rr = minR; rr <= maxR; rr++)
                    {
                        var newIsSidePart = region.Contains((rr, cc)) && !region.Contains((rr, cc - 1));
                        if (!newIsSidePart && sidePart)
                        {
                            sides++;
                        }
                        sidePart = newIsSidePart;
                    }

                    if (sidePart)
                    {
                        sides++;
                    }

                    // rightward facing sides
                    sidePart = false;
                    for (var rr = minR; rr <= maxR; rr++)
                    {
                        var newIsSidePart = region.Contains((rr, cc)) && !region.Contains((rr, cc + 1));
                        if (!newIsSidePart && sidePart)
                        {
                            sides++;
                        }
                        sidePart = newIsSidePart;
                    }

                    if (sidePart)
                    {
                        sides++;
                    }
                }

                partOne += area * perim;
                partTwo += area * sides;
            }


        }

        return (partOne, partTwo);
    }
}
