using System.Collections.Immutable;

namespace Day23;

public class Solver
{
    private readonly Dictionary<string, HashSet<string>> _connections;

    public Solver(string input)
    {
        _connections = [];

        foreach (var line in input.Trim().Split("\n"))
        {
            var pcs = line.Split("-");

            if (!_connections.TryGetValue(pcs[0], out var pcOneConnections))
            {
                pcOneConnections = [];
                _connections[pcs[0]] = pcOneConnections;
            }
            pcOneConnections.Add(pcs[1]);

            if (!_connections.TryGetValue(pcs[1], out var pcTwoConnections))
            {
                pcTwoConnections = [];
                _connections[pcs[1]] = pcTwoConnections;
            }
            pcTwoConnections.Add(pcs[0]);
        }
    }

    public int SolvePartOne()
    {
        var count = 0;
        foreach (var (node, directs) in _connections)
        {
            foreach (var other in directs.Where(o => StringComparer.Ordinal.Compare(node, o) < 0))
            {
                foreach (var common in directs.Intersect(_connections[other]).Where(c => StringComparer.Ordinal.Compare(other, c) < 0))
                {
                    if (!node.StartsWith('t') && !other.StartsWith('t') && !common.StartsWith('t'))
                    {
                        continue;
                    }
                    count++;
                }
            }
        }

        return count;
    }

    public string SolvePartTwo()
    {
        var res = GenerateMaximalCliques([], [.. _connections.Keys], []).MaxBy(r => r.Count);

        var list = res!.ToList();
        list.Sort();

        return string.Join(",", list);
    }

    // Bron-Kerbosch
    private IEnumerable<ImmutableHashSet<string>> GenerateMaximalCliques(ImmutableHashSet<string> r, ImmutableHashSet<string> p, ImmutableHashSet<string> x)
    {
        if (p.Count == 0 && x.Count == 0)
        {
            yield return r;
        }

        var verts = p.ToList();
        foreach (var v in verts)
        {
            foreach (var t in GenerateMaximalCliques(r.Add(v), p.Intersect(_connections[v]), x.Intersect(_connections[v])))
            {
                yield return t;
            }
            p = p.Remove(v);
            x = x.Add(v);
        }
    }
}