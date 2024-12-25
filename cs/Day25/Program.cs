Console.WriteLine("Advent of Code 2024");
Console.WriteLine("Day 25: Code Chronicle");

const string INPUT_FILE = "input/input.txt";

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

var input = File.ReadAllText(INPUT_FILE);
var solver = new Day25.Solver(input);

var partOne = solver.SolvePartOne();

Console.WriteLine($"Part One: {partOne}");

Console.WriteLine($"Time elapsed {stopwatch.ElapsedMilliseconds}ms");