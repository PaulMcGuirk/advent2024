using System.Collections.Immutable;

namespace Day11;

public class Solver(string input)
{
    private readonly ImmutableList<long> _initialStones = ImmutableList.CreateRange(input.Trim().Split(" ").Select(long.Parse));

    public long SolvePartOne() => Solve(25);
    public long SolvePartTwo() => Solve(75);
    
    private long Solve(int numBlinks)
    {
        var stones = _initialStones.Distinct().ToDictionary(s => s, s => (long)_initialStones.Count(ss => ss == s));

        for (var blink = 0; blink < numBlinks; blink++)
        {
            var newStones = new Dictionary<long, long>();
            foreach (var (stone, count) in stones)
            {
                if (stone == 0)
                {
                    newStones[1] = newStones.GetValueOrDefault(1, 0) + count;
                    continue;
                }
                var s = stone.ToString();
                if (s.Length % 2 == 0)
                {
                    var s1 = long.Parse(s[..(s.Length / 2)]);
                    var s2 = long.Parse(s[(s.Length / 2)..]);
                    newStones[s1] = newStones.GetValueOrDefault(s1, 0) + count;
                    newStones[s2] = newStones.GetValueOrDefault(s2, 0) + count;
                }
                else
                {
                    newStones[stone * 2024] = newStones.GetValueOrDefault(stone * 2024, 0) + count;
                }
            }

            stones = newStones;
        }

        return stones.Values.Sum();
    }
}
