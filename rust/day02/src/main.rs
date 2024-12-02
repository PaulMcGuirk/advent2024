use std::cmp::Ordering;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn is_safe(report: &Vec<u32>) -> bool {
    let ord = match report[0].cmp(&report[1]) {
        Ordering::Equal => return false,
        o => o,
    };

    (0..(report.len() - 1)).all(|i| {
        if report[i].cmp(&report[i + 1]) != ord {
            return false;
        }
        (report[i + 1] as i32 - report[i] as i32).abs() <= 3
    })
}

fn is_safe_with_damnper(report: &Vec<u32>) -> bool {
    if is_safe(report) {
        return true;
    }

    (0..report.len()).any(|i| {
        let dampened_report = report
            .iter()
            .enumerate()
            .filter(|(idx, _)| idx != &i)
            .map(|(_, &elem)| elem)
            .collect::<Vec<_>>();
        is_safe(&dampened_report)
    })
}

fn solve_part_one(reports: &Vec<Vec<u32>>) -> usize {
    reports.iter().filter(|line| is_safe(line)).count()
}

fn solve_part_two(reports: &Vec<Vec<u32>>) -> usize {
    reports
        .iter()
        .filter(|line| is_safe_with_damnper(line))
        .count()
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 2: Red-Nosed Reports");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let reports = input
        .trim()
        .lines()
        .map(|l| {
            l.split_whitespace()
                .map(|pc| pc.parse::<u32>().unwrap())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let part_one = solve_part_one(&reports);
    let part_two = solve_part_two(&reports);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
