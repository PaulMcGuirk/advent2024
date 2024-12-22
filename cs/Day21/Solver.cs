namespace Day21;

public class Solver(string input)
{
    private readonly List<string> _codes = [.. input.Trim().Split("\n")];
    private static readonly Dictionary<char, List<(char, char)>> _numPad = new()
    {
        ['7'] = [('>', '8'), ('v', '4')],
        ['8'] = [('<', '7'), ('>', '9'), ('v', '5')],
        ['9'] = [('<', '8'), ('v', '6')],
        ['4'] = [('^', '7'), ('>', '5'), ('v', '1')],
        ['5'] = [('<', '4'), ('^', '8'), ('>', '6'), ('v', '2')],
        ['6'] = [('<', '5'), ('^', '9'), ('v', '3')],
        ['1'] = [('^', '4'), ('>', '2')],
        ['2'] = [('<', '1'), ('^', '5'), ('>', '3'), ('v', '0')],
        ['3'] = [('<', '2'), ('^', '6'), ('v', 'A')],
        ['0'] = [('^', '2'), ('>', 'A')],
        ['A'] = [('<', '0'), ('^', '3')]
    };
    private readonly Dictionary<(char, char), List<string>> _numPadPaths = PrecomputePaths(_numPad);

    private static readonly Dictionary<char, List<(char, char)>> _dirPad = new()
    {
        ['^'] = [('>', 'A'), ('v', 'v')],
        ['A'] = [('<', '^'), ('v', '>')],
        ['<'] = [('>', 'v')],
        ['v'] = [('<', '<'), ('^', '^'), ('>', '>')],
        ['>'] = [('<', 'v'), ('^', 'A')]
    };
    private readonly Dictionary<(char, char), List<string>> _directionPadPaths = PrecomputePaths(_dirPad);

    public long SolvePartOne() => Solve(2);
    public long SolvePartTwo() => Solve(25);

    public long Solve(int hiddenRobots)
    {
        var memo = new Dictionary<(string, int), long>();
        return _codes.Select(code => GetMinSequence(code, 0, hiddenRobots, memo) * int.Parse(code[..^1])).Sum();
    }
    
    private long GetMinSequence(string code, int layer, int hiddenRobots, Dictionary<(string, int), long> memo)
    {
        if (layer > hiddenRobots)
        {
            return code.Length;
        }

        var precomputedPaths = layer == 0 ? _numPadPaths : _directionPadPaths;

        if (!memo.TryGetValue((code, layer), out var res))
        {
            var pcs = code[..^1].Split("A").ToList();
            foreach (var pc in pcs)
            {   
                var poses = new List<char> { 'A' };
                poses.AddRange(pc);
                poses.Add('A');

                var paths = new List<List<char>> { new() };

                for (var i = 0; i < poses.Count - 1; i++)
                {
                    var newPaths = new List<List<char>>();
                    foreach (var path in paths)
                    {
                        foreach (var subSeq in precomputedPaths[(poses[i], poses[i + 1])])
                        {
                            var newPath = path.Concat(subSeq).ToList();
                            newPaths.Add(newPath);
                        }
                    }

                    paths = newPaths;
                }
                res += paths.Select(s => GetMinSequence(string.Join("", s), layer + 1, hiddenRobots, memo)).Min();
            }
            memo[(code, layer)] = res;
        }

        return res;
    }

    private static Dictionary<(char, char), List<string>> PrecomputePaths(Dictionary<char, List<(char, char)>> pad)
    {
        var paths = new Dictionary<(char, char), List<string>>();

        var elems = pad.Keys.ToList();

        foreach (var start in elems)
        {
            foreach (var end in elems)
            {
                paths[(start, end)] = [];
            }
            paths[(start, start)].Add("A");
        }

        for (var i = 0; i < elems.Count; i++)
        {
            var start = elems[i];
            for (var j = i + 1; j < elems.Count; j++)
            {
                
                var end = elems[j];
                int? dist = null;

                var pathsFromStart = new Queue<(char Pos, List<char> Path)>();
                pathsFromStart.Enqueue((start, []));

                while (pathsFromStart.TryDequeue(out var node))
                {
                    var (pos, path) = node;

                    if (pos == end)
                    {
                        var numTurns = path.Zip(path.Skip(1)).Where(pair => pair.First != pair.Second).Count();
                        if (numTurns < 2)
                        {
                            paths[(start, end)].Add(string.Join("", path.Concat(['A'])));
                            paths[(end, start)].Add(string.Join("", path.Select(c => c switch
                            {
                                '>' => '<',
                                'v' => '^',
                                '<' => '>',
                                '^' => 'v',
                                _ => throw new Exception()
                            }).Reverse().Concat(['A']).ToList()));
                        }
                        dist ??= path.Count;
                        
                    }

                    if (path.Count >= dist)
                    {
                        continue;
                    }

                    foreach (var (dir, next) in pad[pos])
                    {
                        pathsFromStart.Enqueue((next, [..path.Concat([dir])]));
                    }
                }

            }
        }

        return paths;
    }
}