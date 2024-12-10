using System.Collections.Immutable;

namespace Day10;

public class Solver(string input)
{
    private readonly ImmutableList<ImmutableList<int>> _initialBlocks = 
        ImmutableList.CreateRange(input.Trim().Split("\n")
            .Select(line => ImmutableList.CreateRange(line.Select(ch => ch - '0'))));

    public (int, int) Solve()
    {
        var mapSize = _initialBlocks.Count;

        var locs = new HashSet<(int Row, int Col)>();
        // starting building the paths to the 9s. each trail is
        // represented as a ;-delimited list of (row, column) pairs starting with
        // the 9 and ending with the 0
        // as a side effect - we'll populate locs with the locations of nines
        var connectedNines = Enumerable.Range(0, mapSize)
            .Select(r => Enumerable.Range(0, mapSize).Select(c =>
                {
                    if (_initialBlocks[r][c] == 9)
                    {
                        locs.Add((r, c));
                        return new HashSet<string> { $"({r},{c})" };
                    }
                    else
                    {
                        return [];
                    }
                }).ToList())
            .ToList();

        var curr = 9;
        while (curr > 0)
        {
            var nextLocs = new HashSet<(int, int)>();

            foreach (var (r, c) in locs)
            {
                var neighbors = new List<(int Row, int Col)> { (r - 1, c), (r + 1, c), (r, c - 1), (r, c + 1)}
                    .Where(pair => pair.Row >= 0 && pair.Row < mapSize && pair.Col >= 0 && pair.Col < mapSize)
                    .Where(pair => _initialBlocks[pair.Row][pair.Col] == curr - 1)
                    .ToList();

                foreach (var (n_r, n_c) in neighbors)
                {
                    nextLocs.Add((n_r, n_c));
                    connectedNines[n_r][n_c].UnionWith(connectedNines[r][c].Select(route => $"{route};({n_r},{n_c})"));
                }
            }
            locs = nextLocs;
            curr--;
        }

        var partOne = locs.Select(pair => connectedNines[pair.Row][pair.Col]
            .Select(r => r.Split(";")[0])
            .Distinct()
            .Count())
            .Sum();


        var partTwo = locs.Select(pair => connectedNines[pair.Row][pair.Col].Count).Sum();

        return (partOne, partTwo);
    }
}
