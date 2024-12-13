using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdventOfCode2015.Commands
{
    public class WelcomeCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Welcome";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Displays the Welcome Message";

        public bool WithLogging { get; set; }
        public ICommand MakeCommand(string[] args)
        {
            return new WelcomeCommand();
        }

        public void Run()
        {
            var table = new Table()
                .AddColumn(new TableColumn(""))
                .HideHeaders()
                .Expand()
                .Border(TableBorder.None);
            
            var panel = new Panel(new FigletText("Advent Of Code 2015")
                                 .Centered().Color(Spectre.Console.Color.Teal))
                .SquareBorder()
                .Expand()
                .BorderColor(Spectre.Console.Color.LightSkyBlue1);

            table.AddRow(panel);
            table.AddRow(new Markup("[green]-- By Brian Wham[/]"));
            table.AddRow(new Markup("[green]--[/] [dim linke=https://github.com/TechneWare]https://github.com/TechneWare[/]"));
            AnsiConsole.Write(table);
            Console.WriteLine();
        }
    }
}
