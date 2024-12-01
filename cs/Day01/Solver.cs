namespace Day01;

public class Solver(string input)
{
    private readonly IReadOnlyList<IReadOnlyList<int>> _roomIds = input
        .Trim()
        .Split("\n")
        .Where(line => !string.IsNullOrEmpty(line))
        .Select(line => line.Split().Where(pc => !string.IsNullOrEmpty(pc)).Select(pc => int.Parse(pc)).ToList().AsReadOnly())
        .ToList()
        .AsReadOnly();

    public int SolvePartOne()
    {
        var lists = Enumerable.Range(0, 2)
            .Select(i => _roomIds.Select(r => r[i]).Order().ToList())
            .ToList();

        var result = Enumerable.Range(0, _roomIds.Count)
            .Select(i => Math.Abs(lists[0][i] - lists[1][i]))
            .Sum();

        return result;
    }

    public int SolvePartTwo()
    {
        var counts = new List<Dictionary<int, int>> { new(), new() };

        for (var i = 0; i < _roomIds.Count; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                var roomId = _roomIds[i][j];
                counts[j][roomId] = counts[j].GetValueOrDefault(roomId) + 1;
            }
        }

        var result = counts[0]
            .Select(pair => pair.Key * pair.Value * counts[1].GetValueOrDefault(pair.Key, 0))
            .Sum();

        return result;
    }
}