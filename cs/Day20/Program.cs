Console.WriteLine("Advent of Code 2024");
Console.WriteLine("Day 20: Race Condition");

const string INPUT_FILE = "input/input.txt";

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

var input = File.ReadAllText(INPUT_FILE);
var solver = new Day20.Solver(input);

var partOne = solver.SolvePartOne(100);
var partTwo = solver.SolvePartTwo(100);

Console.WriteLine($"Part One: {partOne}");
Console.WriteLine($"Part Two: {partTwo}");

Console.WriteLine($"Time elapsed {stopwatch.ElapsedMilliseconds}ms");