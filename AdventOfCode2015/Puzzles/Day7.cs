using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 7: Some Assembly Required ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/7"/>
    internal class Day7 : Puzzle
    {
        private List<Wire> Wires { get; set; } = [];
        private List<IGate> Gates { get; set; } = [];

        public Day7()
            : base(Name: "Some Assembly Required", DayNumber: 7) { }

        public override void ParseData()
        {
            Gates.Clear();
            Wires.Clear();

            foreach (var line in DataRaw.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("NOT") || line.Contains("SHIFT"))
                {
                    //operator
                    if (line.StartsWith("NOT"))
                    {
                        var l = line.Replace("NOT ", "");
                        var wires = l.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                        var inputWire = GetWire(wires[0]);
                        var outputWire = GetWire(wires[1]);

                        var notOp = new NotOperator(inputWire, outputWire);

                        Gates.Add(notOp);
                    }
                    else if (line.Contains("RSHIFT"))
                    {
                        var l = line.Replace(" RSHIFT", "").Replace(" ->", "");
                        var wires = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var input1 = GetWire(wires[0]);
                        var bitCount = ushort.Parse(wires[1]);
                        var output = GetWire(wires[2]);

                        var shiftOp = new ShiftRightOperator(input1, output, bitCount);

                        Gates.Add(shiftOp);
                    }
                    else if (line.Contains("LSHIFT"))
                    {
                        var l = line.Replace(" LSHIFT", "").Replace(" ->", "");
                        var wires = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var input1 = GetWire(wires[0]);
                        var bitCount = ushort.Parse(wires[1]);
                        var output = GetWire(wires[2]);

                        var shiftOp = new ShiftLeftOperator(input1, output, bitCount);

                        Gates.Add(shiftOp);
                    }
                    else
                    {
                        throw new InvalidDataException(line);
                    }
                }
                else if (line.Contains("AND") || line.Contains("OR"))
                {
                    //gate
                    if (line.Contains("AND"))
                    {
                        var l = line.Replace(" AND", "").Replace(" ->", "");
                        var wires = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var input1 = GetWire(wires[0]);
                        var input2 = GetWire(wires[1]);
                        var output = GetWire(wires[2]);

                        var andGate = new AndGate(input1, input2, output);

                        Gates.Add(andGate);

                    }
                    else if (line.Contains("OR"))
                    {
                        var l = line.Replace(" OR", "").Replace(" ->", "");
                        var wires = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var input1 = GetWire(wires[0]);
                        var input2 = GetWire(wires[1]);
                        var output = GetWire(wires[2]);

                        var orGate = new OrGate(input1, input2, output);

                        Gates.Add(orGate);
                    }
                    else
                    {
                        throw new InvalidDataException(line);
                    }
                }
                else
                {
                    //wire only 
                    var wires = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                    if (ushort.TryParse(wires[0], out var value))
                    {
                        //Value only wire
                        var wire = new Wire(wires[1], value);
                        Wires.Add(wire);
                    }
                    else
                    {
                        //wire connected to another wire, connect with No operation operator
                        var wire1 = GetWire(wires[0]);
                        var wire2 = GetWire(wires[1]);

                        var noop = new NoopOperator(wire1, wire2);

                        Gates.Add(noop);
                    }
                }
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            Update();

            var answer = "";
            if (isTestMode)
            {
                foreach (var wire in Wires.OrderBy(w => w.Id))
                {
                    answer += $"{wire.Id}: {wire.Signal}\n";
                }
            }
            else
            {
                var wireA = Wires.Where(w => w.Id == "a").Single();
                answer += $"wire A = {wireA.Signal}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            var answer = "Tests Skipped\n";

            if (!isTestMode)
            {
                ParseData();
                Update();
                var wireASignal = Wires.Where(w => w.Id == "a").Single().Signal;

                ParseData();
                var wireB = Wires.Where(w => w.Id == "b").Single();
                wireB.Signal = wireASignal;

                Update();
                wireASignal = Wires.Where(w => w.Id == "a").Single().Signal;

                answer = $"wire A = {wireASignal}";
            }
            
            Part2Result = answer;
        }

        private void Update()
        {
            while (Gates.Any(g => g.CanUpdate()))
            {
                var gatesToUpdate = Gates.Where(gt => gt.CanUpdate()).ToList();
                foreach (var g in gatesToUpdate)
                {
                    g.Update();
                }
            }

            var notRun = Gates.Where(g => !g.IsUpdated).ToList();
            if (notRun.Any())
                Debugger.Break();
        }

        private Wire GetWire(string id)
        {
            Wire? wire = null;

            if (ushort.TryParse(id, out ushort value))
            {
                //literal value
                wire = new Wire("", value);
            }
            else
            {
                //refrence to wire
                wire = Wires.Where(w => w.Id == id).SingleOrDefault();
                if (wire == null)
                {
                    wire = new Wire(id);
                    Wires.Add(wire);
                }
            }

            return wire;
        }

        internal class Wire(string id)
        {
            public string Id { get; set; } = id;
            public ushort? Signal { get; set; }

            public Wire(string id, ushort signal) : this(id)
            {
                Signal = signal;
            }
        }

        interface IGate
        {
            Wire Output { get; set; }
            void Update();

            bool IsUpdated { get; set; }
            bool CanUpdate();

        }
        interface ILogicGate : IGate
        {
            Wire Input1 { get; set; }
            Wire Input2 { get; set; }
        }
        interface IOperator : IGate
        {
            Wire Input { get; set; }
        }

        internal class NotOperator(Wire input, Wire output) : IOperator
        {
            public Wire Input { get; set; } = input;
            public Wire Output { get; set; } = output;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input.Signal != null;
            }

            public void Update()
            {
                Output.Signal = (ushort)~Input.Signal!;
                IsUpdated = true;
            }
        }
        internal class ShiftLeftOperator(Wire input, Wire output, ushort shiftBits) : IOperator
        {
            public Wire Input { get; set; } = input;
            public Wire Output { get; set; } = output;
            public ushort Shift { get; internal set; } = shiftBits;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input.Signal != null;
            }

            public void Update()
            {
                Output.Signal = (ushort)(Input.Signal! << Shift);
                IsUpdated = true;
            }
        }
        internal class ShiftRightOperator(Wire input, Wire output, ushort shiftBits) : IOperator
        {
            public Wire Input { get; set; } = input;
            public Wire Output { get; set; } = output;
            public ushort Shift { get; internal set; } = shiftBits;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input.Signal != null;
            }

            public void Update()
            {
                Output.Signal = (ushort)(Input.Signal! >>> Shift);
                IsUpdated = true;
            }
        }
        internal class NoopOperator(Wire input, Wire output) : IOperator
        {
            public Wire Input { get; set; } = input;
            public Wire Output { get; set; } = output;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input.Signal != null;
            }

            public void Update()
            {
                Output.Signal = Input.Signal;
                IsUpdated = true;
            }
        }

        internal class AndGate(Wire input1, Wire input2, Wire output) : ILogicGate
        {
            public Wire Input1 { get; set; } = input1;
            public Wire Input2 { get; set; } = input2;
            public Wire Output { get; set; } = output;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input1.Signal != null && Input2.Signal != null;
            }

            public void Update()
            {
                Output.Signal = (ushort)(Input1.Signal! & Input2.Signal!);
                IsUpdated = true;
            }
        }
        internal class OrGate(Wire input1, Wire input2, Wire output) : ILogicGate
        {
            public Wire Input1 { get; set; } = input1;
            public Wire Input2 { get; set; } = input2;
            public Wire Output { get; set; } = output;
            public bool IsUpdated { get; set; }

            public bool CanUpdate()
            {
                return !IsUpdated && Input1.Signal != null && Input2.Signal != null;
            }

            public void Update()
            {
                Output.Signal = (ushort)(Input1.Signal! | Input2.Signal!);
                IsUpdated = true;
            }
        }
    }
}
