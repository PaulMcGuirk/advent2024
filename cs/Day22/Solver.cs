namespace Day22;

public class Solver(string input)
{
    private readonly List<long> _buyerSecrets = input.Split("\n").Where(line => !string.IsNullOrEmpty(line)).Select(long.Parse).ToList();
    
    public long SolvePartOne() => _buyerSecrets.Select(num => Secrets(num, 2000).Last()).Sum();

    private static IEnumerable<long> Secrets(long num, int counter)
    {
        yield return num;

        for (var i = 0; i < counter; i++)
        {
            num = ((num << 6) ^ num) & 16777215L;
            num = ((num >> 5) ^ num) & 16777215L;
            num = ((num << 11) ^ num) & 16777215L;
            yield return num;
        }
    }

    public long SolvePartTwo()
    {
        var changes = new Dictionary<(int, int, int, int), long>();
        for (var i = -9; i <= 9; i++)
        {
            for (var j = -9; j <= 9; j++)
            {
                for (var k = -9; k <= 9; k++)
                {
                    for (var l = -9; l <= 9; l++)
                    {
                        changes[(i, j, k, l)] = 0;
                    }
                }
            }
        }

        foreach (var secret in _buyerSecrets)
        {
            var seen = new HashSet<(int, int, int, int)>();
            var prices = Secrets(secret, 2000).Select(s => (int)(s % 10)).ToList();
            var diffs = prices.Zip(prices.Skip(1)).Select(pair => pair.Second - pair.First).ToList();

            for (var i = 4; i < prices.Count; i++)
            {
                var diff = (diffs[i - 4], diffs[i - 3], diffs[i - 2], diffs[i - 1]);
                if (seen.Contains(diff))
                {
                    continue;
                }
                seen.Add(diff);
                changes[diff] += prices[i];
            }

        }

        return changes.Values.Max();
    }
    
}