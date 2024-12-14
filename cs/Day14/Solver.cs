using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Day14;

public record Vec(int X, int Y);
public record Robot(Vec Pos, Vec Vel);

public partial class Solver(string input, int xSpan, int ySpan)
{
    [GeneratedRegex(@"^p=(?<px>\-?[0-9]+),(?<py>\-?[0-9]+) v=(?<vx>\-?[0-9]+),(?<vy>\-?[0-9]+)$")]
    private static partial Regex RobotRegex();

    private readonly ImmutableList<Robot> _robots = ImmutableList.CreateRange(input.Trim().Split("\n").Select(line =>
        {
            var match = RobotRegex().Match(line);
            var px = int.Parse(match.Groups["px"].Value);
            var py = int.Parse(match.Groups["py"].Value);
            var vx = int.Parse(match.Groups["vx"].Value);
            var vy = int.Parse(match.Groups["vy"].Value);

            var pos = new Vec(px, py);
            var vel = new Vec(vx, vy);

            return new Robot(pos, vel);
        }));

    private readonly Vec _span = new (xSpan, ySpan);

    public long SolvePartOne()
    {
        var locs = _robots.Select(r => (X: Mod(r.Pos.X + 100 * r.Vel.X, _span.X), Y: Mod(r.Pos.Y + 100 * r.Vel.Y, _span.Y))).ToList();

        var q1 = locs.Where(pair => pair.X < _span.X / 2 && pair.Y < _span.Y / 2).Count();
        var q2 = locs.Where(pair => pair.X > _span.X / 2 && pair.Y < _span.Y / 2).Count();
        var q3 = locs.Where(pair => pair.X < _span.X / 2 && pair.Y > _span.Y / 2).Count();
        var q4 = locs.Where(pair => pair.X > _span.X / 2 && pair.Y > _span.Y / 2).Count();

        return q1 * q2 * q3 * q4;
        
    }

    public long SolvePartTwo()
    {
        var prevRes = 0;
        for (var t = 0; ; t++)
        {
            var locs = _robots.Select(r => (X: Mod(r.Pos.X + t * r.Vel.X, _span.X), Y: Mod(r.Pos.Y + t * r.Vel.Y, _span.Y))).ToHashSet();
            var adjs = locs.Where(pair => locs.Contains((pair.X + 1, pair.Y + 1)) || locs.Contains((pair.X - 1, pair.Y + 1)) || locs.Contains((pair.X + 1, pair.Y - 1)) || locs.Contains((pair.X - 1, pair.Y - 1))).Count();
            if (adjs > prevRes)
            {
                for (var y = 0; y < ySpan; y++)
                {
                    Console.WriteLine();
                    for (var x = 0; x < xSpan; x++)
                    {
                        var ch = locs.Contains((x, y)) ? '█' : ' ';
                        Console.Write(ch);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"current t = {t}");
                Console.WriteLine("enter y to accept");
                var ok = Console.ReadLine();
                if (ok == "y")
                {
                    return t;
                }
                prevRes = adjs;
            }

        }
    }

    private static long Mod(long x, long m) => (x % m + m) % m;
}