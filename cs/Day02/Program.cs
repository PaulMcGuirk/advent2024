Console.WriteLine("Advent of Code 2024");
Console.WriteLine("Day 2: Red-Nosed Reports");

const string INPUT_FILE = "input/input.txt";

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

var input = File.ReadAllText(INPUT_FILE);
var solver = new Day02.Solver(input);

var partOne = solver.SolvePartOne();
var partTwo = solver.SolvePartTwo();

Console.WriteLine($"Part One: {partOne}");
Console.WriteLine($"Part Two: {partTwo}");

Console.WriteLine($"Time elapsed {stopwatch.ElapsedMilliseconds}ms");