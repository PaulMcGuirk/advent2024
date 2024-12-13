use std::fs;
use std::time::Instant;

use regex::Regex;

const FILEPATH: &str = "./input/input.txt";

struct Point {
    x: u64,
    y: u64,
}

struct Claw {
    a: Point,
    b: Point,
    prize: Point,
}

impl Claw {
    fn solve(&self, boost: bool) -> Option<u64> {
        let adjustment = if boost { 10000000000000 } else { 0 };
        let (t_x, t_y) = (self.prize.x + adjustment, self.prize.y + adjustment);

        let det = (self.a.x * self.b.y) as i64 - (self.a.y * self.b.x) as i64;

        if det == 0 {
            panic!("Determinant is 0");
        }

        let b_num = self.a.x as i64 * t_y as i64 - self.a.y as i64 * t_x as i64;
        if b_num % det != 0 {
            return None;
        }

        let b = b_num / det;

        if b < 0 {
            return None;
        }

        let a_num = t_x as i64 - b * (self.b.x as i64);
        if a_num < 0 {
            return None;
        }

        if a_num % self.a.x as i64 != 0 {
            return None;
        }

        let a = a_num / self.a.x as i64;

        Some(3 * a as u64 + b as u64)
    }
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 13: Claw Contraption");

    let now = Instant::now();

    let button_regex =
        Regex::new(r"^Button [A-Z]: X\+(?P<delta_x>\d+), Y\+(?P<delta_y>\d+)$").unwrap();
    let prize_regex = Regex::new(r"^Prize: X=(?P<x>\d+), Y=(?P<y>\d+)$").unwrap();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let claws = input
        .trim()
        .split("\n\n")
        .map(|chunk| {
            let mut pcs = chunk.trim().lines();

            let a = button_regex.captures(pcs.next().unwrap()).unwrap();
            let b = button_regex.captures(pcs.next().unwrap()).unwrap();
            let prize = prize_regex.captures(pcs.next().unwrap()).unwrap();

            Claw {
                a: Point {
                    x: a["delta_x"].parse().unwrap(),
                    y: a["delta_y"].parse().unwrap(),
                },
                b: Point {
                    x: b["delta_x"].parse().unwrap(),
                    y: b["delta_y"].parse().unwrap(),
                },
                prize: Point {
                    x: prize["x"].parse().unwrap(),
                    y: prize["y"].parse().unwrap(),
                },
            }
        })
        .collect::<Vec<_>>();

    let part_one = claws.iter().filter_map(|c| c.solve(false)).sum::<u64>();
    let part_two = claws.iter().filter_map(|c| c.solve(true)).sum::<u64>();

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
