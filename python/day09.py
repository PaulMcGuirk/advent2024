import time

start_time = time.time()

with open('input/input.txt') as f:
    content = f.read()

blocks = [(i // 2 if i % 2 == 0 else None, int(elem)) for (i, elem) in enumerate(content.strip())]

right = len(blocks) - 1
while right > 0:
    (file_id, size) = blocks[right]
    
    if file_id is None:
        right -= 1
        continue

    try:
        left = next(i for i in range(0, right) if blocks[i][0] is None and blocks[i][1] >= size)
    except StopIteration:
        right -= 1
        continue

    init_size = blocks[left][1]
    blocks[left] = (file_id, size)
    if init_size > size:
        blocks.insert(left + 1, (None, init_size - size))
        right += 1

    blocks[right] = (None, size)
    right -= 1

block_pos = 0
checksum = 0

for (file_id, size) in blocks:
    if file_id:
        checksum += size * (2 * block_pos + size - 1) * file_id // 2
    block_pos += size

end_time = time.time()

print(checksum)
print(f"Elapsed time: {1000 * (end_time - start_time) // 1} ms")