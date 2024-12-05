namespace Day05;

public class Solver
{
    private readonly IReadOnlyList<IReadOnlySet<int>> _pageOrderingRules;
    private readonly IReadOnlyList<IReadOnlyList<int>> _updates;

    public Solver(string input)
    {
        var maxPage = int.MinValue;
        var pageOrderingRules = new Dictionary<int, List<int>>();
        
        var chunks = input.Trim().Split("\n\n");

        foreach (var ruleLine in chunks[0].Trim().Split("\n"))
        {
            var pcs = ruleLine.Split("|");
            var before = int.Parse(pcs[0]);
            var after = int.Parse(pcs[1]);

            maxPage = Math.Max(Math.Max(before, after), maxPage);

            if (!pageOrderingRules.TryGetValue(before, out var afters))
            {
                afters = [];
                pageOrderingRules[before] = afters;
            }
            afters.Add(after);
        }

        _pageOrderingRules = Enumerable.Range(0, maxPage + 1)
            .Select(i =>
            {
                if (!pageOrderingRules.TryGetValue(i, out var afters))
                {
                    return (IReadOnlySet<int>)new HashSet<int>();
                }
                return new HashSet<int>(afters);
            })
            .ToList()
            .AsReadOnly();

        _updates = chunks[1]
            .Trim()
            .Split("\n")
            .Select(line => line.Split(",").Select(int.Parse).ToList().AsReadOnly())
            .ToList().AsReadOnly();
    }

    public (int, int) Solve()
    {
        var rightOrders = new List<int>();
        var wrongOrders = new List<int>();

        for (var i = 0; i < _updates.Count; i++)
        {
            if (RightOrder(_updates[i]))
            {
                rightOrders.Add(i);
            }
            else
            {
                wrongOrders.Add(i);
            }
        }

        var partOne = rightOrders.Select(i => Score(_updates[i])).Sum();

        var partTwo = wrongOrders.Select(i => Score(Reorder(_updates[i]))).Sum();

        return (partOne, partTwo);
        
    }

    private bool RightOrder(IReadOnlyList<int> update)
    {
        for (var i = 0; i < update.Count - 1; i++)
        {
            for (var j = i + 1; j < update.Count; j++)
            {
                if (_pageOrderingRules[update[j]].Contains(update[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IReadOnlyList<int> Reorder(IReadOnlyList<int> update)
    {
        var toAdd = update.ToHashSet();
        var rev = new List<int>();

        while (true)
        {
            if (toAdd.Count == 1)
            {
                rev.Add(toAdd.First());
                break;
            }
            var last = toAdd.First(i => !toAdd.Any(j => _pageOrderingRules[j].Contains(i)));
            toAdd.Remove(last);
            rev.Add(last);
        }

        rev.Reverse();
        return rev.AsReadOnly();
    }

    private static int Score(IReadOnlyList<int> update) => update[update.Count / 2];
        
}