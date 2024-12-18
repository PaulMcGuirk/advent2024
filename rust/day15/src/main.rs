use std::collections::HashSet;
use std::fs;
use std::time::Instant;

const FILEPATH: &str = "./input/input.txt";

#[derive(Debug, Clone, Copy, Eq, PartialEq)]
enum EntityType {
    Bot,
    Wall,
    Box,
    Empty,
    BoxLeft,
    BoxRight,
}

struct Warehouse {
    grid: Vec<Vec<EntityType>>,
    num_rows: usize,
    num_cols: usize,
    bot_pos: (usize, usize),
    insts: Vec<(i32, i32)>,
}

impl Warehouse {
    fn from_str(s: &str, expand: bool) -> Self {
        let mut chunks = s.trim().split("\n\n");

        let mut grid = vec![];
        let grid_str = chunks.next().unwrap();

        for line in grid_str.lines() {
            let mut row = vec![];
            for ch in line.chars() {
                match ch {
                    '#' => {
                        row.push(EntityType::Wall);
                        if expand {
                            row.push(EntityType::Wall);
                        }
                    }
                    '.' => {
                        row.push(EntityType::Empty);
                        if expand {
                            row.push(EntityType::Empty);
                        }
                    }
                    '@' => {
                        row.push(EntityType::Bot);
                        if expand {
                            row.push(EntityType::Empty);
                        }
                    }
                    'O' => {
                        if expand {
                            row.push(EntityType::BoxLeft);
                            row.push(EntityType::BoxRight);
                        } else {
                            row.push(EntityType::Box);
                        }
                    }
                    _ => panic!(),
                }
            }
            grid.push(row);
        }

        let insts = chunks
            .next()
            .unwrap()
            .chars()
            .filter_map(|ch| match ch {
                '^' => Some((-1, 0)),
                'v' => Some((1, 0)),
                '>' => Some((0, 1)),
                '<' => Some((0, -1)),
                '\n' => None,
                _ => panic!(),
            })
            .collect::<Vec<_>>();

        let num_rows = grid.len();
        let num_cols = grid[0].len();

        let bot_pos = (0..num_rows)
            .flat_map(|r| (0..num_cols).map(move |c| (r, c)))
            .find(|(r, c)| grid[*r][*c] == EntityType::Bot)
            .unwrap();

        Self {
            grid,
            num_rows,
            num_cols,
            bot_pos,
            insts,
        }
    }

    fn run(&mut self) -> usize {
        for inst in self.insts.iter() {
            let mut to_check = vec![self.bot_pos.clone()];
            let mut to_move = vec![];

            let mut can_move = true;

            while let Some(pos) = to_check.pop() {
                let (r, c) = pos;
                let new_pos = ((r as i32 + inst.0) as usize, (c as i32 + inst.1) as usize);
                to_move.push(pos.clone());

                match self.grid[new_pos.0][new_pos.1] {
                    EntityType::Box => {
                        to_check.push(new_pos);
                    }
                    EntityType::Wall => {
                        can_move = false;
                    }
                    EntityType::Empty => {}
                    EntityType::BoxLeft if inst.1 == -1 => panic!(),
                    EntityType::BoxLeft if inst.1 == 1 => {
                        to_move.push(new_pos.clone());
                        to_check.push((new_pos.0, new_pos.1 + 1));
                    }
                    EntityType::BoxLeft => {
                        to_check.push((new_pos.0, new_pos.1));
                        to_check.push((new_pos.0, new_pos.1 + 1));
                    }
                    EntityType::BoxRight if inst.1 == 1 => panic!(),
                    EntityType::BoxRight if inst.1 == -1 => {
                        to_move.push(new_pos.clone());
                        to_check.push((new_pos.0, new_pos.1 - 1));
                    }
                    EntityType::BoxRight => {
                        to_check.push((new_pos.0, new_pos.1));
                        to_check.push((new_pos.0, new_pos.1 - 1));
                    }
                    _ => panic!(),
                }
            }

            if !can_move {
                continue;
            }

            let mut moved = HashSet::new();
            while let Some(pos) = to_move.pop() {
                let (r, c) = pos;
                if moved.contains(&pos) {
                    continue;
                }
                moved.insert(pos);
                let new_pos = ((r as i32 + inst.0) as usize, (c as i32 + inst.1) as usize);
                self.grid[new_pos.0][new_pos.1] = self.grid[r][c];
                self.grid[r][c] = EntityType::Empty;
            }

            self.bot_pos = (
                (self.bot_pos.0 as i32 + inst.0) as usize,
                (self.bot_pos.1 as i32 + inst.1) as usize,
            );
        }

        let mut res = 0;
        for i in 0..self.num_rows {
            for j in 0..self.num_cols {
                if self.grid[i][j] == EntityType::Box || self.grid[i][j] == EntityType::BoxLeft {
                    res += 100 * i + j;
                }
            }
        }

        res
    }
}

fn main() {
    println!("Advent of Code 2024");
    println!("Day 15: Warehouse Woes");

    let now = Instant::now();

    let input = fs::read_to_string(FILEPATH).expect("Could not read file");

    let mut warehouse = Warehouse::from_str(&input, false);
    let part_one = warehouse.run();

    let mut warehouse = Warehouse::from_str(&input, true);
    let part_two = warehouse.run();

    println!("Part one: {}", part_one);
    println!("Part two: {}", part_two);

    println!("Elasped time: {}ms", now.elapsed().as_millis());
}
