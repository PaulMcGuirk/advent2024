use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve_part_one(initial_blocks: &[u32]) -> u64 {
    let mut blocks = initial_blocks.to_vec();

    let mut checksum = 0;
    let mut left = 0;
    let mut right = blocks.len() - 1;
    let mut block_pos = 0u64;

    while left <= right && left < blocks.len() {
        if left % 2 == 0 {
            // initially occupied
            let file_id = left / 2;
            while blocks[left] > 0 {
                checksum += file_id as u64 * block_pos;
                block_pos += 1;
                blocks[left] -= 1;
            }
        } else {
            while blocks[left] > 0 && right > 0 {
                let file_id = right / 2;

                while blocks[right] > 0 && blocks[left] > 0 {
                    checksum += file_id as u64 * block_pos;
                    block_pos += 1;
                    blocks[right] -= 1;
                    blocks[left] -= 1;
                }

                if blocks[left] > 0 {
                    right -= 2;
                }
            }
        }

        left += 1;
    }

    checksum
}

fn solve_part_two(initial_blocks: &[u32]) -> u64 {
    let mut blocks = initial_blocks
        .iter()
        .enumerate()
        .map(|(idx, &size)| {
            if idx % 2 == 0 {
                (size as u64, Some(idx / 2))
            } else {
                (size as u64, None)
            }
        })
        .collect::<Vec<_>>();

    let mut right = blocks.len() - 1;
    while right > 0 {
        let (size, file_id) = blocks[right].clone();
        if file_id.is_none() {
            right -= 1;
            continue;
        }

        let file_id = file_id.unwrap();

        let left = (0..right).find(|&idx| {
            let (s, f_id) = blocks[idx];
            f_id.is_none() && s >= size
        });

        if left.is_none() {
            right -= 1;
            continue;
        }

        let left = left.unwrap();

        let init_size = blocks[left].0;
        blocks[left] = (size, Some(file_id));
        blocks[right] = (size, None);
        if init_size - size > 0 {
            blocks.insert(left + 1, (init_size - size, None));
            right += 1;
        }

        right -= 1;
    }

    let mut checksum = 0u64;
    let mut block_pos = 0;

    for (size, file_id) in blocks {
        if let Some(file_id) = file_id {
            checksum += size * (2 * block_pos + size - 1) * file_id as u64 / 2;
        }

        block_pos += size;
    }

    checksum
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 9: Disk Fragmenter");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let initial_blocks = input
        .trim()
        .chars()
        .map(|c| c.to_digit(10).unwrap())
        .collect::<Vec<_>>();

    let part_one = solve_part_one(&initial_blocks);
    let part_two = solve_part_two(&initial_blocks);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
