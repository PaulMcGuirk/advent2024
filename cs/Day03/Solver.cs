using System.Text.RegularExpressions;

namespace Day03;

public partial class Solver(string input)
{
    private readonly string _code = input;

    [GeneratedRegex(@"(?<dont>don\'t\(\))|(?<do>do\(\))|(mul\((?<a>\d+),(?<b>\d+)\))")]
    private static partial Regex MulRegex();
    
    public (long, long) Solve()
    {
        var partOne = 0L;
        var partTwo = 0L;
        var matches = MulRegex().Matches(_code).ToList();

        var incl = true;
        foreach (var match in matches)
        {
            if (!string.IsNullOrEmpty(match.Groups["do"].Value))
            {
                incl = true;
            }
            else if (!string.IsNullOrEmpty(match.Groups["dont"].Value))
            {
                incl = false;
            }
            else
            {
                var a = long.Parse(match.Groups["a"].Value);
                var b = long.Parse(match.Groups["b"].Value);
                var prod = a * b;
                if (incl)
                {
                    partTwo += prod;
                }
                partOne += prod;
            }
        }

        return (partOne, partTwo);
    }
}