using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    internal interface ICommandFactory
    {
        string CommandName { get; }
        string CommandArgs { get; }
        string[] CommandAlternates { get; }
        string Description { get; }
        ICommand MakeCommand(string[] args);
    }
}
