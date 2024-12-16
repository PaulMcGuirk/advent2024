use std::collections::HashSet;
use std::fs;
use std::time::Instant;

use regex::Regex;

const FILEPATH: &str = "./input/input.txt";

fn solve_part_one(bots: &[(i32, i32, i32, i32)], time: i32, x_max: usize, y_max: usize) -> usize {
    let bots = bots
        .iter()
        .map(|(p_x, p_y, v_x, v_y)| (mod_(p_x + v_x * time, x_max), mod_(p_y + v_y * time, y_max)))
        .collect::<Vec<_>>();

    let q1 = bots
        .iter()
        .filter(|(x, y)| *x < x_max / 2 && *y < y_max / 2)
        .count();

    let q2 = bots
        .iter()
        .filter(|(x, y)| *x > x_max / 2 && *y < y_max / 2)
        .count();

    let q3 = bots
        .iter()
        .filter(|(x, y)| *x < x_max / 2 && *y > y_max / 2)
        .count();

    let q4 = bots
        .iter()
        .filter(|(x, y)| *x > x_max / 2 && *y > y_max / 2)
        .count();

    q1 * q2 * q3 * q4
}

fn solve_part_two(bots: &[(i32, i32, i32, i32)], x_max: usize, y_max: usize) -> usize {
    let mut bots = bots.iter().map(|tup| tup.clone()).collect::<HashSet<_>>();

    let mut res = None;
    let mut best = 0;

    for t in 1..=(x_max * y_max) {
        bots = bots
            .iter()
            .map(|(p_x, p_y, v_x, v_y)| {
                (
                    mod_(p_x + v_x, x_max) as i32,
                    mod_(p_y + v_y, y_max) as i32,
                    *v_x,
                    *v_y,
                )
            })
            .collect::<HashSet<_>>();

        let closeness = bots
            .iter()
            .filter(|(p_x, p_y, _, _)| {
                bots.iter().any(|(o_p_x, o_p_y, _, _)| {
                    (p_x - o_p_x).abs() == 1 && (p_y - o_p_y).abs() == 1
                })
            })
            .count();

        if res.is_none() || closeness > best {
            res = Some(t);
            best = closeness;
        }
    }

    res.unwrap()
}

fn mod_(val: i32, divider: usize) -> usize {
    ((val % divider as i32) + divider as i32) as usize % divider
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 14: Restroom Redoubt");

    let now = Instant::now();

    let regex =
        Regex::new(r"^p=(?P<p_x>\-?\d+),(?P<p_y>\-?\d+) v=(?P<v_x>\-?\d+),(?P<v_y>\-?\d+)$")
            .unwrap();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let bots = input
        .trim()
        .lines()
        .map(|line| {
            let captures = regex.captures(line).unwrap();
            let p_x = captures["p_x"].parse::<i32>().unwrap();
            let p_y = captures["p_y"].parse::<i32>().unwrap();
            let v_x = captures["v_x"].parse::<i32>().unwrap();
            let v_y = captures["v_y"].parse::<i32>().unwrap();

            (p_x, p_y, v_x, v_y)
        })
        .collect::<Vec<_>>();

    let part_one = solve_part_one(&bots, 100, 101, 103);
    let part_two = solve_part_two(&bots, 101, 103);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
