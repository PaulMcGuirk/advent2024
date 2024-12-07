using System.Collections.Immutable;

namespace Day07;

public class Solver(string input)
{
    private readonly ImmutableList<(long Value, ImmutableList<long> Operands)> _equations = [..input.Trim().Split("\n").Select(line =>
        {
            var pcs = line.Split(":");
            var value = long.Parse(pcs[0].Trim());
            ImmutableList<long> operands = [..pcs[1].Trim().Split(" ").Select(long.Parse)];
            return (value, operands);
        })];

    public long SolvePartOne() => Solve(false);
    public long SolvePartTwo() => Solve(true);

    private long Solve(bool inclucdeConcatenation) => _equations
        .Where(e => PossiblyTrue(e.Value, e.Operands, inclucdeConcatenation))
        .Select(e => e.Value)
        .Sum();

    private static bool PossiblyTrue(long value, IReadOnlyList<long> operands, bool inclucdeConcatenation)
    {
        if (operands.Count == 1)
        {
            return value == operands[0];
        }

        return PossiblyTrue(value, [operands[0] + operands[1], ..operands.Skip(2)], inclucdeConcatenation) ||
            PossiblyTrue(value, [operands[0] * operands[1], ..operands.Skip(2)], inclucdeConcatenation) ||
            (inclucdeConcatenation && PossiblyTrue(value, [long.Parse($"{operands[0]}{operands[1]}"), ..operands.Skip(2)], inclucdeConcatenation));
    }
}