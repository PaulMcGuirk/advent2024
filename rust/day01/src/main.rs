use std::collections::HashMap;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve_part_one(room_ids: &Vec<Vec<u32>>) -> u32 {
    let lists = {
        let mut lists = (0..2)
            .map(|i| room_ids.iter().map(|r| r[i].to_owned()).collect::<Vec<_>>())
            .collect::<Vec<_>>();

        lists[0].sort();
        lists[1].sort();

        lists
    };

    (0..room_ids.len())
        .map(|i| (lists[0][i] as i32 - lists[1][i] as i32).abs() as u32)
        .sum()
}

fn solve_part_two(room_ids: &Vec<Vec<u32>>) -> u32 {
    let counts = (0..2)
        .map(|list_id| {
            (0..room_ids.len()).fold(HashMap::<u32, u32>::new(), |mut acc, i| {
                *acc.entry(room_ids[i][list_id]).or_insert(0) += 1;
                acc
            })
        })
        .collect::<Vec<_>>();

    counts[0]
        .iter()
        .map(|(value, count)| value * count * counts[1].get(value).unwrap_or(&0))
        .sum()
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 1: Historian Hysteria");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let room_ids = input
        .trim()
        .lines()
        .map(|l| {
            l.split_whitespace()
                .map(|pc| pc.parse::<u32>().unwrap())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let part_one = solve_part_one(&room_ids);
    let part_two = solve_part_two(&room_ids);
    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
