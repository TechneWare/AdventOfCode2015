using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    public class QuitCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Quit";
        public string CommandArgs => "";
        public string[] CommandAlternates => new string[] { "exit" };
        public string Description => "Ends the program";
        public bool WithLogging { get; set; } = false;
        public ICommand MakeCommand(string[] args)
        {
            return new QuitCommand();
        }

        public void Run()
        {
            Console.WriteLine("--Laterz");
        }
    }
}
