use std::collections::{HashMap, HashSet};
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve(grid_size: i32, antennae: &HashMap<char, Vec<(i32, i32)>>) -> (usize, usize) {
    let mut first_antinodes = HashSet::new();
    let mut all_antinodes = HashSet::new();

    for locs in antennae.values() {
        for (i, (r_a, c_a)) in locs.iter().enumerate() {
            for (j, (r_b, c_b)) in locs.iter().enumerate() {
                if i == j {
                    continue;
                }

                let d_r = r_b - r_a;
                let d_c = c_b - c_a;

                for i in 0.. {
                    let r = r_a + i * d_r;
                    let c = c_a + i * d_c;

                    if r < 0 || r >= grid_size || c < 0 || c >= grid_size {
                        break;
                    }

                    all_antinodes.insert((r, c));

                    if i == 2 {
                        first_antinodes.insert((r, c));
                    }
                }
            }
        }
    }

    (first_antinodes.len(), all_antinodes.len())
}

fn parse_input(input: &str) -> (i32, HashMap<char, Vec<(i32, i32)>>) {
    let lines = input.trim().lines().collect::<Vec<_>>();
    let mut antennae = HashMap::<char, Vec<(i32, i32)>>::new();

    for (r, line) in lines.iter().enumerate() {
        for (c, col) in line.chars().enumerate() {
            if col.is_alphanumeric() {
                antennae.entry(col).or_default().push((r as i32, c as i32));
            }
        }
    }

    (lines.len() as i32, antennae)
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 8: Resonant Collinearity");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let (grid_size, antennae) = parse_input(&input);

    let (part_one, part_two) = solve(grid_size, &antennae);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
