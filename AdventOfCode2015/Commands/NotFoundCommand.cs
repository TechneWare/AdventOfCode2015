using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Commands
{
    internal class NotFoundCommand : ICommand
    {
        public bool WithLogging { get; set; } = false;
        public string Name { get; set; }
        public void Run()
        {
            Console.WriteLine($"Command Not Found: {Name}");
        }
    }
}
