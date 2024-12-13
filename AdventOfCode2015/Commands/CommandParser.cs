using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    internal class CommandParser
    {
        private readonly IEnumerable<ICommandFactory> commands;

        public CommandParser(IEnumerable<ICommandFactory> commands)
        {
            this.commands = commands;
        }

        internal ICommand ParseCommand(string[] args)
        {
            var requestedCommand = args[0];

            var command = FindRequestedCommand(requestedCommand);
            if (command == null)
                return new NotFoundCommand { Name = requestedCommand };

            if (command is RunPuzzleCommand && double.TryParse(args[0], out double dayNum))
            {
                args = new string[2] { "runpuzzle", $"{dayNum}" };
            }

            return command.MakeCommand(args);
        }

        private ICommandFactory FindRequestedCommand(string requestedCommand)
        {
            ICommandFactory cmd = null;
            var matched = commands.Where(c =>
                        c.CommandName.ToLower().StartsWith(requestedCommand.ToLower())
                        || c.CommandAlternates.Any(ca => ca.ToLower().StartsWith(requestedCommand.ToLower())));

            if (!matched.Any() && double.TryParse(requestedCommand, out double dayNum))
            {
                matched = commands.Where(c => c.CommandName.ToLower().StartsWith("runpuzzle"));
            }

            if (matched.Any())
            {
                if (matched.Count() == 1)
                    cmd = matched.First();
                else
                {
                    var msg = "Please Be More Specific." +
                              "\nDid you mean one of these?";
                    foreach (var c in matched)
                        msg += $"\n{c.CommandName.PadRight(25)}-{c.Description}";

                    cmd = new BadCommand(msg);
                }
            }

            return cmd;
        }
    }
}
