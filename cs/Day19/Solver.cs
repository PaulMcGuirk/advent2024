using System.Collections.Immutable;

namespace Day19;

public class Solver
{
    private readonly ImmutableList<string> _substrings;
    private readonly ImmutableList<string> _targetStrings;

    public Solver(string input)
    {
        var chunks = input.Trim().Split("\n\n");
        _substrings = ImmutableList.CreateRange(chunks[0].Trim().Split(",").Select(s => s.Trim()));
        _targetStrings = ImmutableList.CreateRange(chunks[1].Trim().Split("\n"));
    }

    public (int, long) Solve()
    {
        var numWays = new Dictionary<string, long>() { [""] = 0 };

        foreach (var target in _targetStrings)
        {
            CountWays(target, numWays);
        }

        var partOne = _targetStrings.Where(s => numWays[s] > 0).Count();
        var partTwo = _targetStrings.Select(s => numWays[s]).Sum();

        return (partOne, partTwo);
    }

    private long CountWays(string s, Dictionary<string, long> numWays)
    {
        if (!numWays.TryGetValue(s, out var res))
        {
            res = 0L;
            foreach (var ss in _substrings)
            {
                if (s == ss)
                {
                    res++;
                }
                else if (s.StartsWith(ss))
                {
                    res += CountWays(s[ss.Length..], numWays);
                }
            }
            numWays[s] = res;
        }

        return res;
    }
}