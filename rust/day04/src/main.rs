use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";
const TARGET: &str = "XMAS";

fn solve_part_one(grid: &Vec<Vec<char>>) -> usize {
    let grid_size = grid.len();

    let deltas = vec![
        (-1isize, 1isize),
        (-1, -1),
        (1, -1),
        (1, 1),
        (0, 1),
        (0, -1),
        (1, 0),
        (-1, 0),
    ];

    (0..grid_size)
        .map(|i| {
            (0..grid_size)
                .map(|j| {
                    deltas
                        .iter()
                        .filter(|(d_i, d_j)| {
                            let s = (0..TARGET.len())
                                .map(|k| {
                                    let n_i = i as isize + d_i * k as isize;
                                    let n_j = j as isize + d_j * k as isize;

                                    if n_i < 0
                                        || n_i >= grid_size as isize
                                        || n_j < 0
                                        || n_j >= grid_size as isize
                                    {
                                        return ' ';
                                    }

                                    grid[n_i as usize][n_j as usize]
                                })
                                .collect::<String>();

                            s == TARGET
                        })
                        .count()
                })
                .sum::<usize>()
        })
        .sum::<usize>()
}

fn solve_part_two(grid: &Vec<Vec<char>>) -> usize {
    let grid_size = grid.len();
    let deltas = vec![(-1isize, 1isize), (-1, -1), (1, -1), (1, 1)];

    (1..(grid_size - 1))
        .map(|i| {
            (1..(grid_size - 1))
                .filter(|&j| {
                    if grid[i][j] != 'A' {
                        return false;
                    }

                    let neighbors = deltas
                        .iter()
                        .map(|&(d_i, d_j)| {
                            grid[(i as isize + d_i) as usize][(j as isize + d_j) as usize]
                        })
                        .collect::<Vec<_>>();

                    neighbors.iter().filter(|&&c| c == 'M').count() == 2
                        && neighbors.iter().filter(|&&c| c == 'S').count() == 2
                        && grid[(i - 1) as usize][(j - 1) as usize] != grid[i + 1][j + 1]
                })
                .count()
        })
        .sum()
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 4: Ceres Search");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");
    let grid = input
        .trim()
        .lines()
        .map(|line| line.chars().collect::<Vec<_>>())
        .collect::<Vec<_>>();

    let part_one = solve_part_one(&grid);
    let part_two = solve_part_two(&grid);

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
