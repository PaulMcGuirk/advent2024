using System.Collections.Immutable;

namespace Day17;

public class Computer(
    ImmutableList<int> code,
    long initialA,
    long initialB,
    long initialC
)
{
    private readonly ImmutableList<int> _code = code;
    private readonly long _initialA = initialA;
    private readonly long _initialB = initialB;
    private readonly long _initialC = initialC;
    
    private long _a;
    private long _b;
    private long _c;
    private int _pos = 0;

    public void Reset()
    {
        _a = _initialA;
        _b = _initialB;
        _c = _initialC;
        _pos = 0;
    }

    public void SetA(long a)
    {
        _a = a;
    }

    public IEnumerable<long> Run()
    {
        while (true)
        {
            if (_pos >= _code.Count)
            {
                break;
            }

            var opCode = _code[_pos];
            var operand = _code[_pos + 1];

            switch (opCode)
            {
                case 0: Adv(operand); break;
                case 1: Bxl(operand); break;
                case 2: Bst(operand); break;
                case 3: Jnz(operand); break;
                case 4: Bxc(operand); break;
                case 5:
                    yield return Out(operand);
                    break;
                case 6: Bdv(operand); break;
                case 7: Cdv(operand); break;
                default: throw new Exception($"Bad opcode: {opCode}");
            }
        }

        yield break;
    }

    private long EvalCombo(int operand)
        => operand switch
        {
            4 => _a,
            5 => _b,
            6 => _c,
            _ when operand < 4 => operand,
            _ => throw new Exception($"Invalid combo operand {operand}")
        };

    private void Adv(int operand)
    {
        _a  = DvHelper(operand);
        _pos += 2;
    }

    private void Bxl(int operand)
    {
        _b ^= operand;
        _pos += 2;
    }

    private void Bst(int operand)
    {
        _b = EvalCombo(operand) % 8;
        _pos += 2;
    }

    private void Jnz(int operand)
    {
        if (_a == 0)
        {
            _pos += 2;
        }
        else
        {
            _pos = operand;
        }
    }

    private void Bxc(int _operand)
    {
        _b ^= _c;
        _pos += 2;
    }

    private long Out(int operand)
    {
        var val = EvalCombo(operand) % 8;
        _pos += 2;

        return val;
    }

    private void Bdv(int operand)
    {
        _b  = DvHelper(operand);
        _pos += 2;
    }

    private void Cdv(int operand)
    {
        _c  = DvHelper(operand);
        _pos += 2;
    }

    private long DvHelper(int operand)
    {
        var num = _a;
        var exp = EvalCombo(operand);
        checked
        {
        
            return num >> (int)exp;
        }
    }
}

public class Solver
{
    private readonly int _initialA;
    private readonly int _initialB;
    private readonly int _initialC;

    private readonly ImmutableList<int> _code;

    public Solver(string input)
    {
        var chunks = input.Trim().Split("\n\n");

        var regInit = chunks[0].Trim()
            .Split("\n")
            .Select(line => int.Parse(line.Split(":")[1].Trim()))
            .ToList();
        
        _initialA = regInit[0];
        _initialB = regInit[1];
        _initialC = regInit[2];

        _code = ImmutableList.CreateRange(chunks[1].Trim().Split(":")[1].Trim().Split(",").Select(int.Parse).ToList());
    }

    public string SolvePartOne()
    {
        var computer = new Computer(_code, _initialA, _initialB, _initialC);
        computer.Reset();
        var outs = computer.Run();
        return string.Join(",", outs);
    }

    public static string SolvePartOneEquiv(long a)
    {
        var outs = new List<long>();

        long b, c;

        while (a > 0)
        {
            b = a % 8;  // 2,4,
            b ^= 5; // 1,5,
            c = a >> (int)b; // 7,5,
            b ^= b ^ 6; // 1,6,
            a >>= 3;
            b ^= c;
            outs.Add(b % 8); // 5, 5
        } // 3, 0

        return string.Join(",", outs);
    }

    public long SolvePartTwo()
    {
        // little endian bits
        var possibleBits = new List<List<bool?>> {Enumerable.Repeat((bool?)null, _code.Count * 3 + 8).ToList() }; // pad this out a little

        for (var i = _code.Count - 1; i >=0; i--)
        {
            var newPossibleBits = new List<List<bool?>>();
            for (var aPiece = 0; aPiece < 8; aPiece++)
            {
                foreach (var bits in possibleBits)
                {
                    var newBits = bits.ToList();
                    newBits[3 * i] = aPiece % 2 == 1;
                    newBits[3 * i + 1] = aPiece / 2 % 2  == 1;
                    newBits[3 * i + 2] = aPiece / 4 % 2  == 1;

                    var b = new List<bool> { newBits[3 * i]!.Value, newBits[3 * i + 1]!.Value, newBits[3 * i + 2]!.Value}; // b = a % 8
                    
                    // b ^= 5 = 101
                    b[0] = !b[0];
                    b[2] = !b[2];

                    // shift = b
                    var shift = (b[0] ? 1 : 0) + (b[1] ? 2 : 0) + (b[2] ? 4 : 0);
                    
                    // c = a / 2^b which is a left-shift in little-endian
                    var c = new List<bool> { newBits[3 * i + shift] ?? false , newBits[3 * i + 1 + shift] ?? false, newBits[3 * i + 2 + shift] ?? false };

                    // b ^= 6 = 011
                    b[1] = !b[1];
                    b[2] = !b[2];

                    // b ^= c
                    b[0] = b[0] != c[0];
                    b[1] = b[1] != c[1];
                    b[2] = b[2] != c[2];

                    // a = a / 2^8,  which we're handling by the loop construct
                    var @out = (b[0] ? 1 : 0) + (b[1] ? 2 : 0) + (b[2] ? 4 : 0);
                    if (@out == _code[i])
                    {
                        newPossibleBits.Add(newBits);
                    }
                }
            }
            possibleBits = newPossibleBits;
        }

        var res = possibleBits.Select(pb => Enumerable.Range(0, _code.Count)
            .Select(i =>
            {
                var s = (1L << (3 * i) ) * ((pb[3 * i]!.Value ? 1 : 0) + (pb[3 * i + 1]!.Value ? 2 : 0) + (pb[3 * i + 2]!.Value ? 4 : 0));
                return s;
            })
            .Sum())
            .Min();

        return res;
    }
}