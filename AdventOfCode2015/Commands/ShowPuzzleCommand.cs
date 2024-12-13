using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    public class ShowPuzzleCommand : ICommand, ICommandFactory
    {
        public string CommandName => "ShowPuzzle";

        public string CommandArgs => "[Day Number] [Part Number [1-2]]";

        public string[] CommandAlternates => new string[] { "day" };

        public string Description => "Show Puzzle Text for a Day";

        public double DayNumber { get; set; } = 0;
        private int PartNumber { get; set; } = 1;
        public string arg { get; set; } = "";

        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new ShowPuzzleCommand();

            try
            {
                if (args.Length > 1)
                {
                    ((ShowPuzzleCommand)(cmd)).arg = args[1];
                    if (double.TryParse(args[1], out double dayNum))
                        ((ShowPuzzleCommand)(cmd)).DayNumber = dayNum;
                    else if (args[1].ToLower().StartsWith('l')) //run latest puzzle
                        if (Utils.GetAllPuzzles().Any())
                            ((ShowPuzzleCommand)cmd).DayNumber = (double)Utils.GetAllPuzzles().Max(p => p.DayNumber);
                        else
                            cmd = new BadCommand("No Puzzles have been created yet, go make one!");
                }
                else
                    cmd = new BadCommand("Usage: day [Day Number | last]");

                if (cmd is ShowPuzzleCommand)
                {
                    ((ShowPuzzleCommand)cmd).PartNumber = 1;

                    if (args.Length > 2)
                    {
                        if (int.TryParse(args[2], out int partNum))
                        {
                            if (partNum == 1 || partNum == 2)
                                ((ShowPuzzleCommand)cmd).PartNumber = partNum;
                        }
                    }
                }
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
                    puzzle.ShowPuzzleText(PartNumber);
                else
                    Console.WriteLine($"Unkown Day Number [{DayNumber}], no puzzle for this day was found");
            }
            else
                Console.WriteLine($"Invalid Day Number [{arg}], must be 1 or higher");
        }
    }
}
