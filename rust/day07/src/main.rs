use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn is_possible(value: &u64, operands: &Vec<u64>, use_concatenation: bool) -> bool {
    struct Env<'a> {
        value: &'a u64,
        operands: &'a Vec<u64>,
        use_concatenation: bool,
    }

    fn helper(env: &Env, index: usize, curr: u64) -> bool {
        if index == env.operands.len() {
            return *env.value == curr;
        }

        if helper(env, index + 1, curr + env.operands[index]) {
            return true;
        }

        if helper(env, index + 1, curr * env.operands[index]) {
            return true;
        }

        if !env.use_concatenation {
            return false;
        }

        let val = if curr == 0 {
            env.operands[0]
        } else {
            format!("{}{}", curr, env.operands[index])
                .parse::<u64>()
                .unwrap()
        };
        helper(env, index + 1, val)
    }

    let env = Env {
        value,
        operands,
        use_concatenation,
    };

    helper(&env, 0, 0)
}

fn solve(equations: &Vec<(u64, Vec<u64>)>, use_concatenation: bool) -> u64 {
    equations
        .iter()
        .filter_map(|(value, operands)| {
            if is_possible(value, operands, use_concatenation) {
                Some(value)
            } else {
                None
            }
        })
        .sum()
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 7: Bridge Repair");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let equations = input
        .trim()
        .lines()
        .map(|line| {
            let (value, operands) = line.split_once(":").unwrap();
            (
                value.trim().parse::<u64>().unwrap(),
                operands
                    .trim()
                    .split_whitespace()
                    .map(|o| o.parse::<u64>().unwrap())
                    .collect::<Vec<_>>(),
            )
        })
        .collect::<Vec<_>>();

    let part_one = solve(&equations, false);
    let part_two = solve(&equations, true);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
