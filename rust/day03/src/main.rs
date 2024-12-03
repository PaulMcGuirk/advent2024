use regex::Regex;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn main() {
    println!("Advent of Code 2024");
    println!("Day 3: Mull It Over");

    let now = Instant::now();

    let insts = fs::read_to_string(FILEPATH).expect("Could not read file");

    let re =
        Regex::new(r"(?P<dont>don\'t\(\))|(?P<do>do\(\))|(mul\((?P<a>\d+),(?P<b>\d+)\))").unwrap();

    let (_, part_one, part_two) =
        re.captures_iter(&insts)
            .fold((true, 0u64, 0u64), |(do_, part_one, part_two), cap| {
                if cap.name("dont").is_some() {
                    (false, part_one, part_two)
                } else if cap.name("do").is_some() {
                    (true, part_one, part_two)
                } else {
                    let a = cap["a"].parse::<u64>().unwrap();
                    let b = cap["b"].parse::<u64>().unwrap();
                    let prod = a * b;
                    let part_one = part_one + prod;
                    let part_two = part_two + if do_ { prod } else { 0 };
                    (do_, part_one, part_two)
                }
            });

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
