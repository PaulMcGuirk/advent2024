use std::collections::HashSet;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve(map: &Vec<Vec<u32>>) -> (usize, usize) {
    let grid_size = map.len();
    let mut paths = vec![vec![HashSet::<String>::new(); grid_size]; grid_size];

    let mut locs = HashSet::new();
    for r in 0..grid_size {
        for c in 0..grid_size {
            if map[r][c] != 9 {
                continue;
            }
            locs.insert((r, c));
            paths[r][c].insert(format!("({},{})", r, c));
        }
    }

    for cur in (1..10).rev() {
        let mut next_locs = HashSet::new();
        for (r, c) in locs.into_iter() {
            for (dr, dc) in [(0, 1), (1, 0), (0, -1), (-1, 0)] {
                let nr = r as i32 + dr;
                let nc = c as i32 + dc;
                if nr < 0 || nc < 0 {
                    continue;
                }
                let nr = nr as usize;
                let nc = nc as usize;
                if nr >= grid_size || nc >= grid_size {
                    continue;
                }

                if map[nr][nc] != cur - 1 {
                    continue;
                }

                next_locs.insert((nr, nc));

                let new_paths = paths[r][c]
                    .iter()
                    .map(|path| format!("{}<-({},{})", path, nr, nc))
                    .collect::<HashSet<_>>();

                paths[nr][nc].extend(new_paths);
            }
        }
        locs = next_locs;
    }

    if locs.iter().any(|&(r, c)| map[r][c] != 0) {
        panic!("wtf");
    }

    let part_one = locs
        .iter()
        .map(|&(r, c)| {
            paths[r][c]
                .iter()
                .map(|p| p.split("<-").next().unwrap())
                .collect::<HashSet<_>>()
                .len()
        })
        .sum::<usize>();
    let part_two = locs.iter().map(|&(r, c)| paths[r][c].len()).sum::<usize>();

    (part_one, part_two)
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 10: Hoof It");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let map = input
        .trim()
        .lines()
        .map(|line| {
            line.chars()
                .map(|c| c.to_digit(10).unwrap())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let (part_one, part_two) = solve(&map);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
