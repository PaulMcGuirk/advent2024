

using System.Reflection.PortableExecutable;

namespace Day21;

public class Solver(string input)
{
    private readonly List<string> _codes = [.. input.Trim().Split("\n")];
    // private readonly Dictionary<(char, char), List<string>> _numericPadPaths = new()
    // {
    //     [('7', '8')] = [">A"],
    //     [('7', '9')] = [">>A"],
    //     [('7', '4')] = ["v"],
    //     [('7', '5')] = ["v>", ">v"],
    //     [('7', '6')] = ["v>>", ">>v"],
    //     [('7', '1')] = ["vv"],
    //     [('7', '2')] = ["vv>"],
    //     [('7', '3')] = ["vv>>"],
    //     [('7', '0')] = ["vv>>"],
    // };
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

        foreach (var (key, value) in _numPadPaths)
        {
            Console.WriteLine($"Paths from {key.Item1} to {key.Item2}: {string.Join(",", value)}");
        }
        return 3;
        // var numSequences = GetShortestSequences(code, _numericPadPaths, false);

        // return numSequences.Select(c => GetMinSequenceHelper(c, hiddenRobots)).Min();
    }

    // private long GetMinSequenceHelper(string code, int layer)
    // {
    //     if (layer == 0)
    //     {
    //         return code.Length;
    //     }

    //     if (!_memo.TryGetValue((code, layer), out var res))
    //     {
    //         res = code.Split("A")
    //             .Where(pc => !string.IsNullOrEmpty(pc))
    //             .Select(pc => FindPaths
    //     }
        
    //     return res;
    // }

    // private static List<string> GetShortestSequences(string code, Dictionary<(char, char), List<List<char>>> paths, bool useMemo)
    // {


    //     var queue = new PriorityQueue<(int Progress, char Pos, List<char> Sequence), int>();
    //     int? minDist = null;

    //     queue.Enqueue((0, 'A', []), 0);
    //     var res = new List<string>();

    //     while (queue.TryDequeue(out var node, out var pathLength))
    //     {
    //         var (progress, pos, seq) = node;
    //         if (progress == code.Length)
    //         {
    //             minDist ??= pathLength;
    //             res.Add(string.Join("", seq));
    //             continue;
    //         }

    //         if (pathLength >= minDist)
    //         {
    //             continue;
    //         }

    //         var next = code[progress];
    //         foreach (var subPath in paths[(pos, next)])
    //         {
    //             var newSeq = seq.Concat(subPath).Concat(['A']).ToList();
    //             queue.Enqueue((progress + 1, next, newSeq), newSeq.Count);
    //         }
    //     }

    //     return res;
    // }


    // private int GetMinSequence(string code, int hiddenRobots)
    // {
    //     var numSequences = GetShortestSequences(code, _numericPadPaths, false);

    //     var x = new Dictionary<char, int>();

    //     foreach (var key in _dirPad.Keys)
    //     {
    //         var oneSeq = GetShortestSequences(key.ToString(), _directionPadPaths, false).Distinct().ToList();
    //         var minLength = oneSeq.Select(s => s.Length).Min();
    //         oneSeq = oneSeq.Where(s => s.Length == minLength).ToList();

    //         var twoSeq = oneSeq.SelectMany(ns => GetShortestSequences(ns, _directionPadPaths, true)).Distinct().ToList();
    //         minLength = twoSeq.Select(s => s.Length).Min();

    //         x[key] = minLength;
    //     }

    //     return numSequences.Select(ns => ns.Select(c => x[c]).Sum()).Min();

    //     // // var robotOneSequences = numSequences.SelectMany(ns => GetShortestSequences(ns, _directionPadPaths, false)).Distinct().ToList();
    //     // // var minLength = robotOneSequences.Select(s => s.Length).Min();
    //     // // robotOneSequences = robotOneSequences.Where(s => s.Length == minLength)
    //     // //     .OrderByDescending(s => s.Count(c => c == 'A'))
    //     // //     .ThenByDescending(s => s.Count(c => c == '^'))
    //     // //     .ThenByDescending(s => s.Count(c => c == '>'))
    //     // //     .ThenByDescending(s => s.Count(c => c == 'v'))
    //     // //     .ToList();
        
    //     // // ToList();

    //     // var robotTwoSequences = robotOneSequences.SelectMany(ns => GetShortestSequences(ns, _directionPadPaths, true)).Distinct().ToList();
    //     // minLength = robotTwoSequences.Select(s => s.Length).Min();

    //     // return minLength;
    // }

    // private static List<string> GetShortestSequences(string code, Dictionary<(char, char), List<List<char>>> paths, bool earlyQuit)
    // {
    //     var queue = new PriorityQueue<(int Progress, char Pos, List<char> Sequence), int>();
    //     int? minDist = null;

    //     queue.Enqueue((0, 'A', []), 0);
    //     var res = new List<string>();

    //     while (queue.TryDequeue(out var node, out var pathLength))
    //     {
    //         var (progress, pos, seq) = node;
    //         if (progress == code.Length)
    //         {
    //             minDist ??= pathLength;
    //             res.Add(string.Join("", seq));
    //             if (earlyQuit)
    //             {
    //                 break;
    //             }
    //             continue;
    //         }

    //         if (pathLength >= minDist)
    //         {
    //             continue;
    //         }

    //         var next = code[progress];
    //         foreach (var subPath in paths[(pos, next)])
    //         {
    //             var newSeq = seq.Concat(subPath).Concat(['A']).ToList();
    //             queue.Enqueue((progress + 1, next, newSeq), newSeq.Count);
    //         }
    //     }

    //     return res;
    // }

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
    // private static Dictionary<(char, char), List<List<char>>> PrecomputePaths(Dictionary<char, List<(char, char)>> pad)
    // {
    //     var paths = new Dictionary<(char, char), List<List<char>>>();

    //     var elems = pad.Keys.ToList();

    //     foreach (var start in elems)
    //     {
    //         foreach (var end in elems)
    //         {
    //             paths[(start, end)] = [];
    //         }
    //         paths[(start, start)].Add([]);
    //     }

    //     for (var i = 0; i < elems.Count; i++)
    //     {
    //         var start = elems[i];
    //         for (var j = i + 1; j < elems.Count; j++)
    //         {
    //             var end = elems[j];
    //             int? dist = null;

    //             var pathsFromStart = new Queue<(char Pos, List<char> Path)>();
    //             pathsFromStart.Enqueue((start, []));

    //             while (pathsFromStart.TryDequeue(out var node))
    //             {
    //                 var (pos, path) = node;

    //                 if (pos == end)
    //                 {
    //                     dist ??= path.Count;
    //                     paths[(start, end)].Add(path);
    //                     paths[(end, start)].Add(path.Select(c => c switch
    //                     {
    //                         '>' => '<',
    //                         'v' => '^',
    //                         '<' => '>',
    //                         '^' => 'v',
    //                         _ => throw new Exception()
    //                     }).Reverse().ToList());
    //                 }

    //                 if (path.Count >= dist)
    //                 {
    //                     continue;
    //                 }

    //                 foreach (var (dir, next) in pad[pos])
    //                 {
    //                     pathsFromStart.Enqueue((next, [..path.Concat([dir])]));
    //                 }
    //             }

    //         }
    //     }

    //     return paths;
    // }
}