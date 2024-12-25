using System.Collections.Immutable;

namespace Day25;

public class Solver
{
    private readonly ImmutableList<ImmutableList<int>> _locks;
    private readonly ImmutableList<ImmutableList<int>> _keys;

    public Solver(string input)
    {
        var locks = new List<List<int>>();
        var keys = new List<List<int>>();

        var chunks = input.Trim().Split("\n\n");

        foreach (var chunk in chunks)
        {
            var lines = chunk.Split("\n");
            var isLock = lines[0] == "#####";
            if (!isLock && lines[6] != "#####")
            {
                throw new Exception();
            }

            if (isLock && lines[6] != ".....")
            {
                throw new Exception();
            }
            if (!isLock && lines[0] != ".....")
            {
                throw new Exception();
            }

            var heights = new List<int>();
            for (var c = 0; c < 5; c++)
            {
                var height = isLock
                    ? Enumerable.Range(0, 6).TakeWhile(r => lines[r][c] == '#').Last()
                    : Enumerable.Range(0, 6).TakeWhile(r => lines[6 - r][c] == '#').Last();
                heights.Add(height);
            }

            if (isLock)
            {
                locks.Add(heights);
            }
            else
            {
                keys.Add(heights);
            }
        }

        _locks = [..locks.Select(ImmutableList.CreateRange)];
        _keys = [..keys.Select(ImmutableList.CreateRange)];
    }

    public int SolvePartOne() => _locks.Select(@lock => _keys.Where(key => Fit(@lock, key)).Count()).Sum();

    private static bool Fit(ImmutableList<int> @lock, ImmutableList<int> key)
        => Enumerable.Range(0, 5)
            .All(i => @lock[i] + key[i] <= 5);
}