use std::collections::HashSet;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

fn map_path(
    obstacles: &HashSet<(i32, i32)>,
    grid_size: usize,
    initial_pos: (i32, i32),
    initial_dir: (i32, i32),
) -> (HashSet<(i32, i32)>, bool) {
    let mut path = HashSet::new();
    let mut visited = HashSet::new();

    let (mut r, mut c) = initial_pos;
    let (mut dr, mut dc) = initial_dir;

    loop {
        if visited.contains(&(r, c, dr, dc)) {
            return (path, true);
        }

        path.insert((r, c));
        visited.insert((r, c, dr, dc));

        let nr = r + dr;
        let nc = c + dc;

        if nr < 0 || nr >= grid_size as i32 || nc < 0 || nc >= grid_size as i32 {
            return (path, false);
        }

        if obstacles.contains(&(nr, nc)) {
            (dr, dc) = (dc, -dr);
        } else {
            (r, c) = (nr, nc);
        }
    }
}

fn parse_input(input: &str) -> (HashSet<(i32, i32)>, usize, (i32, i32), (i32, i32)) {
    let mut obstacles = HashSet::new();
    let mut initial_pos = (0, 0);
    let mut initial_dir = (0, 0);
    let mut grid_size = 0;

    for (r, line) in input.lines().enumerate() {
        grid_size += 1;
        for (c, ch) in line.chars().enumerate() {
            match ch {
                '#' => {
                    obstacles.insert((r as i32, c as i32));
                }
                '^' | 'v' | '<' | '>' => {
                    initial_pos = (r as i32, c as i32);
                    initial_dir = match ch {
                        '^' => (-1, 0),
                        'v' => (1, 0),
                        '<' => (0, -1),
                        '>' => (0, 1),
                        _ => unreachable!(),
                    };
                }
                _ => {}
            }
        }
    }

    (obstacles, grid_size, initial_pos, initial_dir)
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 6: Guard Gallivant");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let (obstacles, grid_size, initial_pos, initial_dir) = parse_input(&input);

    let (path, _) = map_path(&obstacles, grid_size, initial_pos, initial_dir);

    let part_one = path.len();

    let part_two = path
        .iter()
        .filter(|p| {
            let (_, is_loop) = map_path(
                &obstacles
                    .union(&HashSet::from([**p]))
                    .clone()
                    .map(|&e| e)
                    .collect(),
                grid_size,
                initial_pos,
                initial_dir,
            );
            is_loop
        })
        .count();
    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
