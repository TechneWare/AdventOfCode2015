using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    public class RunPuzzleCommand : ICommand, ICommandFactory
    {
        public string CommandName => "RunPuzzle";

        public string CommandArgs => "[Day Number]";

        public string[] CommandAlternates => new string[] { "day" };

        public string Description => "Run a Puzzle ([day number] by itself will run that day)";

        public double DayNumber { get; set; } = 0;
        public string arg { get; set; } = "";

        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new RunPuzzleCommand();

            try
            {
                if (args.Length > 1)
                {
                    ((RunPuzzleCommand)(cmd)).arg = args[1];
                    if (double.TryParse(args[1], out double dayNum))
                        ((RunPuzzleCommand)(cmd)).DayNumber = dayNum;
                    else if (args[1].ToLower().StartsWith('l')) //run latest puzzle
                        if (Utils.GetAllPuzzles().Any())
                            ((RunPuzzleCommand)cmd).DayNumber = (double)Utils.GetAllPuzzles().Max(p => p.DayNumber);
                        else
                            cmd = new BadCommand("No Puzzles have been created yet, go make one!");
                }
                else
                    cmd = new BadCommand("Usage: day [Day Number | last]");


            }
            catch (Exception)
            {
                cmd = new BadCommand("Unable to locate any puzzles");
            }


            return cmd;
        }

        public void Run()
        {
            if (DayNumber != 0)
            {
                var puzzle = Utils.GetAllPuzzles()
                                  .Where(p => p.DayNumber == DayNumber)
                                  .SingleOrDefault();

                if (puzzle != null)
                    puzzle.Run();
                else
                    Console.WriteLine($"Unkown Day Number [{DayNumber}], no puzzle for this day was found");
            }
            else
                Console.WriteLine($"Invalid Day Number [{arg}], must be 1 or higher");
        }
    }
}
