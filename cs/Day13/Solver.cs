using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Day13;

public record Button(long DeltaX, long DeltaY, long Tokens);

public partial class Claw
{
    public Button A { get; }
    public Button B { get; }
    public long TargetX { get; }
    public long TargetY { get; }

    [GeneratedRegex(@"^Button [A-Z]: X\+(?<deltaX>\d+), Y\+(?<deltaY>\d+)$")]
    private partial Regex ButtonRegex();

    [GeneratedRegex(@"^Prize: X=(?<targetX>\d+), Y=(?<targetY>\d+)$")]
    private partial Regex TargetRegex();

    public Claw(string joyStickString)
    {
        var lines = joyStickString.Trim().Split("\n");

        var buttons = lines.Take(lines.Length - 1)
            .Select((line, idx) =>
            {
                var matches = ButtonRegex().Match(line);
                var deltaX = long.Parse(matches.Groups["deltaX"].Value);
                var deltaY = long.Parse(matches.Groups["deltaY"].Value);
                var cost = idx switch
                {
                    0 => 3L,
                    1 => 1L,
                    _ => throw new Exception()
                };
                return new Button(deltaX, deltaY, cost);
            }).ToList();
        A = buttons[0];
        B = buttons[1];

        var targetMatch = TargetRegex().Match(lines[lines.Length - 1]);
        TargetX = long.Parse(targetMatch.Groups["targetX"].Value);
        TargetY = long.Parse(targetMatch.Groups["targetY"].Value);
    }

    public long? Solve(bool boost)
    {
        var targetX = TargetX + (boost ? 10000000000000 : 0);
        var targetY = TargetY + (boost ? 10000000000000 : 0);
    
        var det = A.DeltaX * B.DeltaY - B.DeltaX * A.DeltaY;

        if (det == 0)
        {
            throw new Exception();
        }

        if (Math.Abs(A.DeltaX * targetY - A.DeltaY * targetX) % Math.Abs(det) != 0)
        {
            return null;
        }

        var b = (A.DeltaX * targetY - A.DeltaY * targetX) / det;
        if (targetX - b * B.DeltaX < 0)
        {
            return null;
        }

        if ((targetX - b * B.DeltaX) % A.DeltaX != 0)
        {
        
            return null;
        }

        var a = (targetX - b * B.DeltaX) / A.DeltaX;

        return A.Tokens * a + B.Tokens * b;
    }
}

public class Solver(string input)
{
    private readonly ImmutableList<Claw> _claws = 
        ImmutableList.CreateRange(input.Trim().Split("\n\n")
            .Select(line => new Claw(line)));

    public long SolvePartOne() => Solve(false);
    public long SolvePartTwo() => Solve(true);
    
    
    private long Solve(bool boost) => _claws.Select(c => c.Solve(boost))
        .Where(c => c != null)
        .Select(c => c!.Value)
        .Sum();
}