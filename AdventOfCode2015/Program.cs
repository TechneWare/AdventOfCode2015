using System.Diagnostics;
using System.Windows.Input;
using AdventOfCode2015.Commands;
using ICommand = AdventOfCode2015.Commands.ICommand;

namespace AdventOfCode2015
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var availableCommands = Utils.GetAvailableCommands();
            var parser = new CommandParser(availableCommands);

            parser.ParseCommand(new string[] { "Cls" }).Run();
            parser.ParseCommand(new string[] { "Welcome" }).Run();

            Settings.ShowPuzzleText = false;// !Debugger.IsAttached;
            parser.ParseCommand(new string[] { "RunPuzzle", "Last" }).Run();
            Settings.ShowPuzzleText = false;

            ICommand? lastCommand = null;
            do
            {
                args = GetInput().Split(' ');

                if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                    Utils.PrintUsage(availableCommands);
                else
                {
                    ICommand? command = parser.ParseCommand(args);
                    if (command != null)
                    {
                        lastCommand = command;
                        command.Run();
                    }
                }
            } while (lastCommand == null || lastCommand.GetType() != typeof(QuitCommand));
        }

        static string GetInput()
        {
            Console.WriteLine($"\nLogging: {(Settings.ShowLog ? "On" : "Off")}\t Puzzle Text:{(Settings.ShowPuzzleText ? "On" : "Off")}");
            Console.Write("$> ");
            string commandInput = Console.ReadLine();
            return commandInput;
        }
    }
}

