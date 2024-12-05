use std::collections::HashSet;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn is_correct(update: &[usize], rules: &[HashSet<usize>]) -> bool {
    !(0..update.len() - 1)
        .any(|i| ((i + 1)..update.len()).any(|j| rules[update[j]].contains(&update[i])))
}

fn score(update: &[usize]) -> usize {
    update[update.len() / 2]
}

fn correct_order(update: &[usize], rules: &[HashSet<usize>]) -> Vec<usize> {
    let mut toAdd = HashSet::<usize>::from_iter(update.iter().cloned());
    let mut ordered = vec![];

    while !toAdd.is_empty() {
        let last = toAdd
            .iter()
            .find(|&i| rules[*i].iter().all(|&j| !toAdd.contains(&j)))
            .unwrap()
            .clone();

        ordered.push(last);
        toAdd.remove(&last);
    }

    ordered.reverse();

    ordered
}

fn parse_input(input: &str) -> (Vec<HashSet<usize>>, Vec<Vec<usize>>) {
    let mut pcs = input.trim().split("\n\n");

    let rule_pairs = pcs
        .next()
        .unwrap()
        .lines()
        .map(|line| {
            let mut pcs = line.split("|").map(|pc| pc.parse::<usize>().unwrap());
            let before = pcs.next().unwrap();
            let after = pcs.next().unwrap();

            (before, after)
        })
        .collect::<Vec<_>>();

    let max = rule_pairs
        .iter()
        .map(|(before, after)| before.max(after))
        .max()
        .unwrap();

    let mut rules = vec![HashSet::new(); max + 1];

    for (before, after) in rule_pairs {
        rules[before].insert(after);
    }

    let updates = pcs
        .next()
        .unwrap()
        .lines()
        .map(|line| {
            line.split(",")
                .map(|pc| pc.parse::<usize>().unwrap())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    (rules, updates)
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 5: Print Queue");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let (rules, updates) = parse_input(&input);

    let (correct, incorrect) = {
        let mut correct = vec![];
        let mut incorrect = vec![];

        for (i, update) in updates.iter().enumerate() {
            if is_correct(update, &rules) {
                correct.push(i);
            } else {
                incorrect.push(i);
            }
        }

        (correct, incorrect)
    };

    let part_one = correct.iter().map(|&i| score(&updates[i])).sum::<usize>();
    let part_two = incorrect
        .iter()
        .map(|&i| score(&correct_order(&updates[i], &rules)))
        .sum::<usize>();

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
