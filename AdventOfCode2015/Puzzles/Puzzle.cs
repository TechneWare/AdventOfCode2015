using AdventOfCode2015.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using System.Diagnostics;

namespace AdventOfCode2015.Puzzles
{
    public abstract class Puzzle : IPuzzle
    {
        private readonly string name;
        private readonly double dayNumber;
        private readonly string seeRef;

        public bool WithLogging = false;
        public double DayNumber => dayNumber;
        public string Name => name;
        public string SeeRef => seeRef;
        public string Part1Result { get; set; } = "Not Run";
        public string Part2Result { get; set; } = "Not Run";

        public string DataRaw { get; set; } = "";

        public Puzzle()
        {
            name = "No Name";
            dayNumber = 0;
            seeRef = "";
        }

        public Puzzle(string Name, double DayNumber)
        {
            name = Name;
            dayNumber = DayNumber;
            this.seeRef = $@"https://adventofcode.com/2015/day/{DayNumber}";
        }
        public void Run()
        {
            if (Settings.ShowPuzzleText)
            {
                var table = GetPartTable();
                ShowTitle();
                ShowPuzzleText(partNum: 1);

                RunPart1(table);
                AnsiConsole.Write(table);

                table = GetPartTable();
                ShowPuzzleText(partNum: 2);

                RunPart2(table);
                AnsiConsole.Write(table);
            }
            else
            {
                var table = GetFullDayTable();
                AnsiConsole.Progress()
                .AutoClear(true)
                .Columns(new ProgressColumn[]
                {
                new SpinnerColumn(),
                new RemainingTimeColumn(),
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                })
                .Start(ctx =>
                {
                    var taskMain = ctx.AddTask($"[green]Executing Day {dayNumber}[/]");
                    var task1 = ctx.AddTask("[green]Part 1[/]");
                    var task2 = ctx.AddTask("[green]Part 2[/]");

                    RunPart1(table, taskMain, task1);
                    RunPart2(table, taskMain, task2);
                });

                AnsiConsole.Write(table);
            }
        }

        private void RunPart1(Table table, ProgressTask? taskMain = null, ProgressTask? task1 = null)
        {
            var stopwatch = new Stopwatch();

            foreach (var mode in new bool[] { true, false })
            {
                WithLogging = Settings.ShowLog && mode;

                LoadData(isTestMode: mode, partNum: 1);
                task1?.Increment(25);
                taskMain?.Increment(12.5);

                stopwatch.Start();
                Part1(mode);
                stopwatch.Stop();
                var duration = 1000 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;

                if (Settings.ShowPuzzleText)
                    AddPartRow(table, Part1Result, mode, duration);
                else
                    AddDayRow(table, "1", Part1Result, mode, duration);

                task1?.Increment(25);
                taskMain?.Increment(12.5);
            }
        }
        private void RunPart2(Table table, ProgressTask? taskMain = null, ProgressTask? task1 = null)
        {
            var stopwatch = new Stopwatch();

            foreach (var mode in new bool[] { true, false })
            {
                WithLogging = Settings.ShowLog && mode;

                LoadData(isTestMode: mode, partNum: 2);
                task1?.Increment(25);
                taskMain?.Increment(12.5);

                stopwatch.Start();
                Part2(mode);
                stopwatch.Stop();
                var duration = 1000 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;

                if (Settings.ShowPuzzleText)
                    AddPartRow(table, Part2Result, mode, duration);
                else
                    AddDayRow(table, "2", Part2Result, mode, duration);

                task1?.Increment(25);
                taskMain?.Increment(12.5);
            }
        }

        public void LoadData(bool isTestMode, int partNum)
        {
            var fileName = GetInputFileName(isTestMode, partNum);
            if (File.Exists(fileName))
                DataRaw = File.ReadAllText(fileName);
            else
                Console.WriteLine($"Required Input File Missing:\n{fileName}");
        }

        public abstract void ParseData();
        public abstract void Part1(bool isTestMode);
        public abstract void Part2(bool isTestMode);

        private string GetInputFileName(bool TestMode, int partNum)
        {
            if (TestMode)
                return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/InputTest{partNum}.txt";
            else
                return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/Input.txt";
        }
        private string GetPuzzleFileName(int partNum)
        {
            return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/Part{partNum}Puzzle.txt";
        }
        public void Log(string Message)
        {
            Log("", Message);
        }
        public void Log(string Title, string Message)
        {
            if (WithLogging && (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Message)))
            {
                var forColor = Console.ForegroundColor;
                var bakColor = Console.BackgroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Blue;
                if (!string.IsNullOrEmpty(Title))
                    Console.WriteLine($"\n--> {Title}");
                if (!string.IsNullOrEmpty(Message))
                    DisplayWithMoreCommand(Message);
                Console.BackgroundColor = bakColor;
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\nPress any key to Continue or Q to stop logging\n");
                Console.ForegroundColor = forColor;

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                    WithLogging = false;
            }
        }
        public void ShowTitle()
        {
            AnsiConsole.Markup($"[bold yellow on blue]\n--- Day {dayNumber}: {name} ---[/]\n");
            AnsiConsole.Markup($"[underline italic dim link={seeRef}]{seeRef}[/]\n");
        }
        public void ShowPuzzleText(int partNum)
        {
            var fname = GetPuzzleFileName(partNum);
            if (File.Exists(fname))
            {
                var puzzleText = File.ReadAllText(fname);
                DisplayWithMoreCommand("\n" + puzzleText);
            }
        }
        private void DisplayWithMoreCommand(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight - 3;

            // Wrap text to fit the console window width
            string wrappedText = WrapText(text, consoleWidth);

            // Split the wrapped text into lines
            string[] lines = wrappedText.Split("\n");

            int currentPage = 0;
            int totalPages = (int)Math.Ceiling(lines.Length / (double)consoleHeight);
            int charCount = 0;

            while (currentPage < totalPages)
            {
                bool slowMode = true;
                for (int i = 0; i < consoleHeight && currentPage * consoleHeight + i < lines.Length; i++)
                {
                    var line = lines[currentPage * consoleHeight + i];

                    if (Console.KeyAvailable)
                    {
                        slowMode = false;
                        Console.ReadKey();
                    }

                    if (slowMode)
                    {
                        foreach (var ch in line)
                        {
                            charCount++;

#pragma warning disable IDE0071 // Simplify interpolation
                            //ch must be a string before passing to MarkupInterpolated
                            //Otherwise '[' or ']' will throw an error as an unescaped markup character
                            //I think it is a bug with AnsiConsole
                            AnsiConsole.MarkupInterpolated($"[green1]{ch.ToString()}[/]");
#pragma warning restore IDE0071 // Simplify interpolation

                            if (!Console.KeyAvailable)
                                Thread.Sleep(5);
                        }
                        Console.WriteLine();
                        if (line == string.Empty)
                        {
                            Thread.Sleep(charCount * 15);
                            charCount = 0;
                        }
                    }
                    else
                        AnsiConsole.MarkupInterpolated($"[green1]{line}[/]\n");
                }

                // Check if there's more content to display
                if (currentPage < totalPages - 1 && !slowMode)
                {
                    AnsiConsole.Markup("[invert]\nPress any key to see more...[/]");
                    Console.ReadKey();

                    Console.SetCursorPosition(0, Console.CursorTop);
                    for (int i = 0; i < 2; i++)
                    {
                        ClearLineFromCursorToEnd();
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }

                currentPage++;
            }
        }
        private void ClearLineFromCursorToEnd()
        {
            int currentLineCursor = Console.CursorTop;
            int currentColumnCursor = Console.CursorLeft;

            int widthToClear = Console.WindowWidth - currentColumnCursor;
            Console.Write(new string(' ', widthToClear));

            Console.SetCursorPosition(currentColumnCursor, currentLineCursor);
        }
        private string WrapText(string text, int consoleWidth)
        {
            string[] words = text.Replace("\r", "").Replace("\n", " [NEW_LINE] ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            string line = "";

            foreach (string word in words)
            {
                if (word == "[NEW_LINE]" || line.Length + word.Length + 1 > consoleWidth)
                {
                    result += line + "\n";
                    line = "";
                }

                if (word != "[NEW_LINE]")
                    line += word + " ";
            }

            if (line.Length > 0)
            {
                result += line; // Add the last line
            }

            return result.TrimEnd();
        }
        private Table GetFullDayTable()
        {
            return new Table()
            .Title($"[bold yellow on blue]--- Day {DayNumber} {name} ---[/]\n[italic dim link={seeRef}]{seeRef}[/]")
            .AddColumns(
                "[bold yellow]Part[/]",
                "[bold yellow]Solve[/]",
                "[bold yellow]Solution[/]",
                "[bold yellow]Elapsed time[/]")
            .RoundedBorder()
            .BorderColor(Spectre.Console.Color.LightSlateBlue);
        }
        private void AddDayRow(Table table, string partNum, string resultValue, bool mode, double duration)
        {
            table.AddRow(new Text[] {
                new Text(partNum).Centered(),
                new Text($"{(mode ? "Test" : "Actual")}").LeftJustified(),
                new Text($"{resultValue}").LeftJustified(),
                new Text($"{duration:F5} ms").RightJustified() });
        }
        private Table GetPartTable()
        {
            return new Table()
            .AddColumns(
                "[bold yellow]Solve[/]",
                "[bold yellow]Solution[/]",
                "[bold yellow]Elapsed time[/]")
            .RoundedBorder()
            .BorderColor(Spectre.Console.Color.LightSlateBlue);
        }
        private void AddPartRow(Table table, string resultValue, bool mode, double duration)
        {
            table.AddRow(new Text[] {
                new Text($"{(mode ? "Test" : "Actual")}").LeftJustified(),
                new Text($"{resultValue}").LeftJustified(),
                new Text($"{duration:F5} ms").RightJustified() });
        }
    }
}
