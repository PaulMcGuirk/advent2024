namespace Day24;

public enum Operation
{
    And,
    Or,
    Xor
}

public class Solver
{
    private readonly Dictionary<string, (Operation Op, string Left, string Right)> _operations;
    private readonly Dictionary<string, bool> _initialValues;

    public Solver(string input)
    {
        _operations = [];   
        _initialValues = [];

        var chunks = input.Trim().Split("\n\n");

        foreach (var line in chunks[0].Trim().Split("\n"))
        {
            var pcs = line.Split(":");
            var wire = pcs[0];
            var value = pcs[1].Trim() switch
            {
                "1" => true,
                "0" => false,
                _ => throw new Exception()
            };

            _initialValues[wire] = value;
        }

        foreach (var line in chunks[1].Trim().Split("\n"))
        {
            var pcs = line.Split(" ");
            var left = pcs[0];
            var right = pcs[2];
            var result = pcs[4];
            var op = pcs[1] switch
            {
                "AND" => Operation.And,
                "OR" => Operation.Or,
                "XOR" => Operation.Xor,
                _ => throw new Exception()
            };

            _operations[result] = (op, left, right);
        }
    }

    public long SolvePartOne() => Calculate(_operations)!.Value;

    private readonly List<List<int>> _swaps = [
        [1, 2, 0],
        [2, 0, 1]
    ];

    public (long, string) Solve()
    {
        // I couldn't figure this out - following this reddit post
        // https://www.reddit.com/r/adventofcode/comments/1hla5ql

        var partOne = Calculate(_operations)!.Value;

        var numZs = _initialValues.Keys.Concat(_operations.Keys).Distinct()
            .Where(w => w.StartsWith('z'))
            .Count();
        var lastZ = "z" + (numZs - 1).ToString("D2");

        var x = GetInput('x');
        var y = GetInput('y');
        var expected = x + y;

        var brokenOutputs = _operations.Where(pair => pair.Key.StartsWith('z') && pair.Value.Op != Operation.Xor && pair.Key != lastZ).ToList();
        var brokenCarries = _operations.Where(pair => pair.Value.Op == Operation.Xor && !pair.Key.StartsWith('z')
            && !pair.Value.Left.StartsWith('x') && !pair.Value.Left.StartsWith('y')
            && !pair.Value.Right.StartsWith('x') && !pair.Value.Right.StartsWith('y'))
            .ToList();
        var gatesSwaped = brokenOutputs.Concat(brokenCarries).Select(pair => pair.Key).ToHashSet();

        if (brokenOutputs.Count != brokenCarries.Count || brokenOutputs.Count != 3)
        {
            throw new Exception("out of expected pattern");
        }

        Dictionary<string, (Operation Op, string Left, string Right)> swapped = _operations;

        foreach (var swap in _swaps)
        {
            var operations = _operations.ToDictionary();
            for (var i = 0; i < 3; i++)
            {
                (operations[brokenOutputs[i].Key], operations[brokenCarries[swap[i]].Key]) = (operations[brokenCarries[swap[i]].Key], operations[brokenOutputs[i].Key]);
            }

            var res = Calculate(operations);
            if (res == null)
            {
                continue; // don't know how general it is but one of the swaps added a loop
            }
            swapped = operations;
        }

        brokenOutputs = swapped.Where(pair => pair.Key.StartsWith('z') && pair.Value.Op != Operation.Xor && pair.Key != lastZ).ToList();
        brokenCarries = swapped.Where(pair => pair.Value.Op == Operation.Xor && !pair.Key.StartsWith('z')
            && !pair.Value.Left.StartsWith('x') && !pair.Value.Left.StartsWith('y')
            && !pair.Value.Right.StartsWith('x') && !pair.Value.Right.StartsWith('y'))
            .ToList();

        for (var i = 0; i < numZs - 1; i++)
        {
            var xin = "x" + i.ToString("D2");
            var yin = "y" + i.ToString("D2");
            var xorOp = swapped.Where(pair => pair.Value.Op == Operation.Xor && ((pair.Value.Left == xin && pair.Value.Right == yin) || (pair.Value.Right == xin && pair.Value.Left == yin))).Single();
            var andOp = swapped.Where(pair => pair.Value.Op == Operation.And && ((pair.Value.Left == xin && pair.Value.Right == yin) || (pair.Value.Right == xin && pair.Value.Left == yin))).Single();

            var repairedSwap = swapped.ToDictionary();
            (repairedSwap[xorOp.Key], repairedSwap[andOp.Key]) = (repairedSwap[andOp.Key], repairedSwap[xorOp.Key]);

            var res = Calculate(repairedSwap);
            if (res == expected)
            {
                gatesSwaped.Add(xorOp.Key);
                gatesSwaped.Add(andOp.Key);
                break;
            }
        }

        var gatesList = gatesSwaped.ToList();
        gatesList.Sort();

        var partTwo = string.Join(",", gatesList);
        
        return (partOne, partTwo);
        
    }

    private long? Calculate(Dictionary<string, (Operation Op, string Left, string Right)> operations)
    {
        var values = _initialValues.ToDictionary();

        var zs = _initialValues.Keys.Concat(operations.Keys).Distinct()
            .Where(w => w.StartsWith('z'))
            .ToList();

        var toEval = new Stack<string>(zs);
        var inStack = new HashSet<string>();
        
        while (toEval.TryPeek(out var next))
        {
            if (values.ContainsKey(next))
            {
                continue;
            }

            var (op, leftName, rightName) = operations[next];

            if (!values.TryGetValue(leftName, out var left))
            {
                if (inStack.Contains(leftName))
                {
                    return null; // circular dependency
                }
                inStack.Add(leftName);
                toEval.Push(leftName);
                continue;
            }

            if (!values.TryGetValue(rightName, out var right))
            {
                if (inStack.Contains(rightName))
                {
                    return null; // circular dependency
                }
                inStack.Add(rightName);
                toEval.Push(rightName);
                continue;
            }

            var val = op switch
            {
                Operation.And => left & right,
                Operation.Or => left | right,
                Operation.Xor => left ^ right,
                _ => throw new Exception()
            };

            values[next] = val;
            toEval.Pop();

        }

        zs.Sort();
        zs.Reverse();

        var res = 0L;
        foreach (var z in zs)
        {
            res <<= 1;
            res += values[z] ? 1 : 0;
        }
        
        return res;
    }

    private long GetInput(char prefix)
    {
        var bits = _initialValues.Keys.Where(k => k.StartsWith(prefix)).ToList();
        bits.Sort();
        bits.Reverse();

        var res = 0L;
        foreach (var bit in bits)
        {
            res <<= 1;
            res += _initialValues[bit] ? 1 : 0;
        }
        
        return res;
    }
}