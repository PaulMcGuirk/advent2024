use std::collections::HashMap;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve(stones: &Vec<u64>, num_steps: usize) -> u64 {
    let mut mem = HashMap::<(u64, usize), u64>::new();

    let mut stack = stones.iter().map(|&s| (s, num_steps)).collect::<Vec<_>>();

    while let Some((stone, left)) = stack.last() {
        let stone = *stone;
        let left = *left;
        if mem.contains_key(&(stone, left)) {
            stack.pop();
            continue;
        }

        if left == 0 {
            mem.insert((stone, 0), 1);
            stack.pop();
            continue;
        }

        let s = stone.to_string();
        let (sub_one, sub_two) = match stone {
            0 => (1, None),
            _ if s.to_string().len() % 2 == 0 => {
                let sub_one = s[..s.len() / 2].parse::<u64>().unwrap();
                let sub_two = s[s.len() / 2..].parse::<u64>().unwrap();
                (sub_one, Some(sub_two))
            }
            _ => (stone * 2024, None),
        };

        if !mem.contains_key(&(sub_one, left - 1)) {
            stack.push((sub_one, left - 1));
            continue;
        }

        let mut val = mem[&(sub_one, left - 1)];

        if let Some(sub_two) = sub_two {
            if !mem.contains_key(&(sub_two, left - 1)) {
                stack.push((sub_two, left - 1));
                continue;
            }

            val += mem[&(sub_two, left - 1)];
        }

        stack.pop();
        mem.insert((stone, left), val);
    }

    stones.iter().map(|&s| mem[&(s, num_steps)]).sum()
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 11: Plutonian Pebbles");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let stones = input
        .trim()
        .split_whitespace()
        .map(|s| s.parse::<u64>().unwrap())
        .collect();

    let part_one = solve(&stones, 25);
    let part_two = solve(&stones, 75);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
