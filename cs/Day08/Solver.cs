using System.Collections.Immutable;

namespace Day08;

public class Solver
{
    private readonly int _gridSize;
    private readonly ImmutableDictionary<char, ImmutableList<(int Row, int Col)>> _antennae;

    public Solver(string input)
    {
        var antennae = new Dictionary<char, List<(int Row, int Col)>>();

        var lines = input.Trim().Split("\n");
        _gridSize = lines.Length;

        for (var r = 0; r < _gridSize; r++)
        {
            var line = lines[r].Trim();
            for (var c = 0 ; c < _gridSize; c++)
            {
                var ch = line[c];
                if (ch == '.')
                {
                    continue;
                }

                if (!antennae.TryGetValue(ch, out var locs))
                {
                    locs = [];
                    antennae[ch] = locs;
                }
                locs.Add((r, c));
            }
        }

        _antennae = ImmutableDictionary.CreateRange(
            antennae.Select(pair => KeyValuePair.Create(pair.Key, ImmutableList.CreateRange(pair.Value)))
        );
    }

    public (int, int) Solve()
    {
        var antinodes = _antennae.Values.Where(v => v.Count > 1).SelectMany(v => v).ToHashSet();
        var nearestAntinodes = new HashSet<(int Row, int Col)>();

        foreach (var locs in _antennae.Values)
        {
            for (var i = 0; i < locs.Count; i++)
            {
                var (r_a, c_a) = locs[i];

                for (var j = 0; j < locs.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var (r_b, c_b) = locs[j];
                    var (d_r, d_c) = (r_b - r_a, c_b - c_a);
                    var (r, c) = (r_a + 2 * d_r, c_a + 2 * d_c);

                    if (!InGrid(r, c))
                    {
                        continue;
                    }

                    nearestAntinodes.Add((r, c));

                    do
                    {
                        antinodes.Add((r, c));
                        (r, c) = (r + d_r, c + d_c);
                    } while (InGrid(r, c));
                }
            }
        }

        return (nearestAntinodes.Count, antinodes.Count);
    }

    private bool InGrid(int r, int c) =>
        r >= 0 &&
        r < _gridSize &&
        c >= 0 &&
        c < _gridSize;

}