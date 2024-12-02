namespace Day02;

public class Solver(string input)
{
    private readonly IReadOnlyList<IReadOnlyList<int>> _reports = input
        .Trim()
        .Split("\n")
        .Where(line => !string.IsNullOrEmpty(line))
        .Select(line => line.Split().Where(pc => !string.IsNullOrEmpty(pc)).Select(pc => int.Parse(pc)).ToList().AsReadOnly())
        .ToList()
        .AsReadOnly();

    public int SolvePartOne() => _reports.Where(IsSafe).Count();
    public int SolvePartTwo() => _reports.Where(IsSafeWithDamper).Count();

    private static bool IsSafe(IReadOnlyList<int> levels)
    {
        if (levels[1] == levels[0])
        {
            return false;
        }

        var isIncreasing = levels[1] > levels[0];
        
        return Enumerable.Range(0, levels.Count - 1)
            .All(i =>
            {
                var diff = levels[i + 1] - levels[i];
                if (Math.Abs(diff) > 3 || diff == 0)
                {
                    return false;
                }
                return isIncreasing ? diff > 0 : diff < 0;
            });
    }

    private static bool IsSafeWithDamper(IReadOnlyList<int> levels)
    {
        if (IsSafe(levels))
        {
            return true;
        }

        return Enumerable.Range(0, levels.Count)
            .Any(i => IsSafe(levels.Take(i).Concat(levels.Skip(i+1)).ToList().AsReadOnly()));
    }
}