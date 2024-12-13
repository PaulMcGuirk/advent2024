use std::collections::HashSet;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn solve(map: &Vec<Vec<char>>) -> (usize, usize) {
    let size = map.len();
    let mut visited = vec![vec![false; size]; size];

    let mut part_one = 0;
    let mut part_two = 0;

    for r in 0..size {
        for c in 0..size {
            if visited[r][c] {
                continue;
            }

            let mut to_visit = vec![(r, c)];
            let mut region = HashSet::new();
            let mut area = 0;
            let mut perim = 0;

            let ch = map[r][c];

            while let Some((r, c)) = to_visit.pop() {
                if visited[r][c] {
                    continue;
                }

                visited[r][c] = true;
                region.insert((r, c));
                area += 1;

                for (dr, dc) in [(0i32, 1i32), (1, 0), (0, -1), (-1, 0)] {
                    let nr = r as i32 + dr;
                    let nc = c as i32 + dc;

                    if nr < 0 || nc < 0 {
                        perim += 1;
                        continue;
                    }

                    let nr = nr as usize;
                    let nc = nc as usize;

                    if nr >= size || nc >= size {
                        perim += 1;
                        continue;
                    }

                    if map[nr][nc] != ch {
                        perim += 1;
                        continue;
                    }

                    to_visit.push((nr, nc));
                }
            }

            let min_r = *region.iter().map(|(r, _)| r).min().unwrap();
            let min_c = *region.iter().map(|(_, c)| c).min().unwrap();
            let max_r = *region.iter().map(|(r, _)| r).max().unwrap();
            let max_c = *region.iter().map(|(_, c)| c).max().unwrap();

            let mut sides = 0;

            for r in min_r..=max_r {
                let mut side_part = false;

                // upward facing sides
                for c in min_c..=max_c {
                    let new_side_part =
                        region.contains(&(r, c)) && (r == 0 || !region.contains(&(r - 1, c)));
                    if !new_side_part && side_part {
                        sides += 1;
                    }

                    side_part = new_side_part;
                }

                if side_part {
                    sides += 1;
                }

                // downward facing sides
                side_part = false;
                for c in min_c..=max_c {
                    let new_side_part = region.contains(&(r, c))
                        && (r == size - 1 || !region.contains(&(r + 1, c)));
                    if !new_side_part && side_part {
                        sides += 1;
                    }

                    side_part = new_side_part;
                }

                if side_part {
                    sides += 1;
                }
            }

            for c in min_c..=max_c {
                let mut side_part = false;

                // upward facing sides
                for r in min_r..=max_r {
                    let new_side_part =
                        region.contains(&(r, c)) && (c == 0 || !region.contains(&(r, c - 1)));
                    if !new_side_part && side_part {
                        sides += 1;
                    }

                    side_part = new_side_part;
                }

                if side_part {
                    sides += 1;
                }

                // downward facing sides
                side_part = false;
                for r in min_r..=max_r {
                    let new_side_part = region.contains(&(r, c))
                        && (c == size - 1 || !region.contains(&(r, c + 1)));
                    if !new_side_part && side_part {
                        sides += 1;
                    }

                    side_part = new_side_part;
                }

                if side_part {
                    sides += 1;
                }
            }

            part_one += area * perim;
            part_two += area * sides;
        }
    }

    (part_one, part_two)
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 12: Garden Group");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let map = input
        .lines()
        .map(|line| line.chars().collect::<Vec<_>>())
        .collect::<Vec<_>>();

    let (part_one, part_two) = solve(&map);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
