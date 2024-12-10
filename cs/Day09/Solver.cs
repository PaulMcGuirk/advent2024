using System.Collections.Immutable;

namespace Day09;

public class Solver(string input)
{
    private readonly ImmutableList<int> _initialBlocks = ImmutableList.CreateRange(input.Trim().Select(ch => (int)char.GetNumericValue(ch)));
    
    public long SolvePartOne()
    {
        if (_initialBlocks.Count % 2 != 1)
        {
            throw new NotImplementedException();
        }

        var blocks = _initialBlocks.ToList();

        var checksum = 0L;
        var left = 0;

        var right = blocks.Count - 1;
        
        var blockPos = 0L;

        while (left <= right && left < blocks.Count)
        {
            if (left % 2 == 0)
            {
                // initially occupied block
                var fileId = left / 2;
                while (blocks[left] > 0)
                {
                    checksum += fileId * blockPos++;
                    blocks[left]--;
                }
            }
            else
            {
                while (blocks[left] > 0 && right > 0)
                {
                    var fileId = right / 2;
                    while (blocks[right] > 0 && blocks[left] > 0)
                    {
                        checksum += fileId * blockPos++;
                        blocks[left]--;
                        blocks[right]--;
                    }

                    if (blocks[left] > 0)
                    {
                        right -=2;
                    }
                }
            }

            left++;
        }

        return checksum;
    }

    public long SolvePartTwo()
    {
        if (_initialBlocks.Count % 2 != 1)
        {
            throw new NotImplementedException();
        }
        
        List<(int Size, int? FileId)> blocks = _initialBlocks.Select((size, idx) => idx % 2 == 0 ? (size, idx / 2) : (size, (int?)null)).ToList();

        var right = blocks.Count - 1;
        while (right > 0)
        {
            var (size, fileId) = blocks[right];
            if (fileId is null)
            {
                right--;
                continue;
            }

            var left = Enumerable.Range(0, right).FirstOrDefault(i => blocks[i].FileId == null && blocks[i].Size >= size, -1);
            if (left < 0)
            {
                right--;
                continue;
            }

            var initSize = blocks[left].Size;
            blocks[left] = (size, fileId);
            if (initSize > size)
            {
                blocks.Insert(left + 1, (initSize - size, null));
                right++;
            }

            blocks[right] = (size, null);
            right--;
        }

        var blockPos = 0;
        var checksum = 0L;
        foreach (var (size, fId) in blocks)
        {
            checksum += (long)size * (2 * blockPos + size - 1) * fId.GetValueOrDefault(0) / 2;
            blockPos += size;
        }

        return checksum;
    }
}
