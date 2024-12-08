Console.WriteLine("Advent of Code 2024");
Console.WriteLine("Day 8: Resonant Collinearity");

const string INPUT_FILE = "input/example.txt";

var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

var input = File.ReadAllText(INPUT_FILE);
var solver = new Day08.Solver(input);

var (partOne, partTwo) = solver.Solve();

Console.WriteLine($"Part One: {partOne}");
Console.WriteLine($"Part Two: {partTwo}");

Console.WriteLine($"Time elapsed {stopwatch.ElapsedMilliseconds}ms");