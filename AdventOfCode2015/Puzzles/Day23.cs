using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2015.Puzzles.Day23;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 23: Opening the Turing Lock ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/23"/>
    internal class Day23 : Puzzle
    {
        private List<string> Program = [];
        public Day23()
            : base(Name: "Opening the Turing Lock", DayNumber: 23) { }

        public override void ParseData()
        {
            Program.Clear();
            Program = [.. DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries)];
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var cpu = new Computer([new RegisterA(), new RegisterB()]);
            cpu.ParseProgram(Program);
            cpu.RunProgram();

            if (isTestMode)
                Part1Result = $"Register a = {cpu.Registers.Where(r => r.Name == "a").Single().Value}\n";
            else
                Part1Result = $"Register b = {cpu.Registers.Where(r => r.Name == "b").Single().Value}\n";
        }

        public override void Part2(bool isTestMode)
        {
            if (!isTestMode)
            {
                ParseData();

                var regA = new RegisterA();
                regA.Push(1);

                var cpu = new Computer([regA, new RegisterB()]);
                cpu.ParseProgram(Program);
                cpu.RunProgram();

                Part2Result = $"Register b = {cpu.Registers.Where(r => r.Name == "b").Single().Value}\n";
            }
            else
                Part2Result = "No Tests for Part2\n";
        }

        internal class Computer
        {
            public IEnumerable<IRegister> Registers { get; private set; }
            public IEnumerable<Instruction> Instructions { get; private set; } = [];

            private int InstructionPointer = 0;

            public Computer(IEnumerable<IRegister> registers)
            {
                Registers = registers;
                InstructionPointer = 0;
            }

            public void RunProgram()
            {
                var instructions = Instructions.ToArray();
                while (InstructionPointer >= 0 && InstructionPointer < Instructions.Count())
                {
                    var jumpAddress = instructions[InstructionPointer].Execute();
                    if (jumpAddress != null)
                        InstructionPointer = jumpAddress.Value;
                    else
                        InstructionPointer++;
                }
            }
            public void ParseProgram(List<string> program)
            {
                var instructions = Instructions.ToList();
                int address = 0;
                foreach (var line in program)
                {
                    var d = line.Replace("+", "").Replace(",", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var instName = d[0];
                    int? offset = null;

                    if (d.Length == 2)
                    {
                        if (int.TryParse(d[1], out int off))
                        {
                            offset = off;

                            if (instName != "jmp")
                                throw new InvalidDataException($"Expected 'jmp' instruction but was '{instName}'");

                            instructions.Add(new Jump(address++, argument: offset));
                        }
                        else
                        {
                            IRegister register = Registers.Where(r => r.Name == d[1]).Single();

                            switch (instName)
                            {
                                case "hlf":
                                    instructions.Add(new Half(address++, register));
                                    break;
                                case "tpl":
                                    instructions.Add(new Triple(address++, register));
                                    break;
                                case "inc":
                                    instructions.Add(new Increment(address++, register));
                                    break;
                                default:
                                    throw new InvalidDataException($"Invalid Instruction detected at line {address}");
                            }
                        }
                    }
                    else if (d.Length == 3 && int.TryParse(d[2], out int off))
                    {
                        IRegister register = Registers.Where(r => r.Name == d[1]).Single();
                        if (int.TryParse(d[2], out int off1))
                            offset = off;

                        switch (instName)
                        {
                            case "jie":
                                instructions.Add(new JumpIfEven(address++, register, offset));
                                break;
                            case "jio":
                                instructions.Add(new JumpIfOne(address++, register, offset));
                                break;
                            default:
                                throw new InvalidDataException($"Invalid Instruction detected at line {address}");
                        }
                    }
                    else
                        throw new InvalidDataException($"Invalid Instruction detected at line {address}");
                }

                Instructions = instructions;
            }
        }

        internal interface IRegister
        {
            string Name { get; }
            uint Value { get; }

            void Push(uint value);
        }
        internal class Register : IRegister
        {
            public string Name { get; private set; }

            public uint Value { get; private set; }

            public Register(string name)
            {
                Name = name;
                Value = 0;
            }

            public void Push(uint value)
            {
                Value = value;
            }
        }
        internal class RegisterA : Register
        {
            public RegisterA()
                : base("a")
            { }
        }
        internal class RegisterB : Register
        {
            public RegisterB()
                : base("b")
            { }
        }
        internal interface IInstruction
        {
            string Name { get; }
            IRegister? Register { get; }
            int? Argument { get; }
            int Address { get; }

            int? Execute();
        }
        internal abstract class Instruction : IInstruction
        {
            public string Name { get; private set; }
            public int Address { get; private set; }

            public IRegister? Register { get; private set; }
            public int? Argument { get; private set; }
            public Instruction(string name, int address, IRegister? register = null, int? argument = null)
            {
                Name = name;
                Address = address;
                Register = register;
                Argument = argument;
            }

            public abstract int? Execute();

            public virtual int JumpTo(int offset)
            {
                return Address + offset;
            }
        }
        internal class Half : Instruction
        {
            public Half(int address, IRegister? register = null, int? argument = null)
                : base("hlf", address, register, argument)
            {
                if (register == null)
                    throw new InvalidOperationException("hlf instructions require a register");
            }
            public override int? Execute()
            {
                if (Register?.Value == 0)
                    throw new InvalidOperationException("unable to half zero");

                Register!.Push(Register.Value / 2);

                return null;
            }
        }
        internal class Triple : Instruction
        {
            public Triple(int address, IRegister? register = null, int? argument = null)
                : base("tpl", address, register, argument)
            {
                if (register == null)
                    throw new InvalidOperationException("tpl instructions require a register");
            }
            public override int? Execute()
            {
                Register!.Push(Register.Value * 3);

                return null;
            }
        }
        internal class Increment : Instruction
        {
            public Increment(int address, IRegister? register = null, int? argument = null)
                : base("inc", address, register, argument)
            {
                if (register == null)
                    throw new InvalidOperationException("inc instructions require a register");
            }

            public override int? Execute()
            {
                Register!.Push(Register.Value + 1);

                return null;
            }
        }
        internal class Jump : Instruction
        {
            public Jump(int address, IRegister? register = null, int? argument = null)
                : base("jmp", address, register, argument)
            {
                if (argument == null)
                    throw new InvalidOperationException("jmp instructions require an argument");
            }

            public override int? Execute()
            {
                return JumpTo(offset: (int)Argument!);
            }
        }
        internal class JumpIfEven : Instruction
        {
            public JumpIfEven(int address, IRegister? register = null, int? argument = null)
                : base("jie", address, register, argument)
            {
                if (register == null)
                    throw new InvalidOperationException("jie instructions require a register");
                if (argument == null)
                    throw new InvalidOperationException("jie instructions require an argument");

            }

            public override int? Execute()
            {
                if (Register!.Value % 2 == 0)
                    return JumpTo(offset: (int)Argument!);
                else
                    return null;
            }
        }
        internal class JumpIfOne : Instruction
        {
            public JumpIfOne(int address, IRegister? register = null, int? argument = null)
                : base("jio", address, register, argument)
            {
                if (register == null)
                    throw new InvalidOperationException("jio instructions require a register");
                if (argument == null)
                    throw new InvalidOperationException("jio instructions require an argument");
            }

            public override int? Execute()
            {
                if (Register!.Value == 1)
                    return JumpTo(offset: (int)Argument!);
                else
                    return null;
            }
        }
    }
}