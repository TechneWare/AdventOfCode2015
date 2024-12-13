using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode2015.Puzzles;

namespace AdventOfCode2015.Commands
{
    internal static class Utils
    {
        /// <summary>
        /// </summary>
        /// <returns>All command objects that are marked public</returns>
        public static IEnumerable<ICommandFactory> GetAvailableCommands()
        {
            var type = typeof(ICommandFactory);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && p.IsPublic)
                .Select(t => Activator.CreateInstance(type: t) as ICommandFactory);

            return types != null ? (IEnumerable<ICommandFactory>)types : [];
        }

        /// <summary>
        /// </summary>
        /// <returns>All Puzzles</returns>
        public static IEnumerable<IPuzzle> GetAllPuzzles()
        {
            var type = typeof(Puzzle);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.Name != type.Name)
                .Select(t => Activator.CreateInstance(type: t) as IPuzzle);

            return types != null ? (IEnumerable<IPuzzle>)types : [];
        }
        public static void PrintUsage(IEnumerable<ICommandFactory> availableCommands)
        {
            Console.WriteLine("\nUsage: commandName Arguments");
            Console.WriteLine("Commands:");
            foreach (var command in availableCommands)
            {
                string alts = "";
                if (command.CommandAlternates.Any())
                    foreach (var altCommand in command.CommandAlternates)
                        alts += $" | {altCommand}";

                Console.Write($"{command.CommandName}{alts} {command.CommandArgs}".PadRight(35));
                Console.WriteLine($"-{command.Description}");
            }
            Console.WriteLine();
        }
    }
}
