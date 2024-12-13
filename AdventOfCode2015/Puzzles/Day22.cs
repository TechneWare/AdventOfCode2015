using AdventOfCode2015.Tools;
using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2015.Puzzles.Day21;
using static AdventOfCode2015.Puzzles.Day22;
using static AdventOfCode2015.Puzzles.Day9;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 22: Wizard Simulator 20XX ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/22"/>
    internal class Day22 : Puzzle
    {
        private List<Boss> bossStats = [];
        public Day22()
            : base(Name: "Wizard Simulator 20XX", DayNumber: 22) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);

            int hp = 0;
            int dmg = 0;
            bossStats = [];

            foreach (var line in data)
            {
                if (line == "")
                    continue;

                var d = line.Split(":", StringSplitOptions.TrimEntries);
                if (d[0] == "Hit Points") hp = int.Parse(d[1]);
                if (d[0] == "Damage") dmg = int.Parse(d[1]);

                if (hp > 0 && dmg > 0)
                {
                    bossStats.Add(new Boss(hp, dmg));
                    hp = 0;
                    dmg = 0;
                }
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            string answer = "";

            for (int i = 0; i < bossStats.Count; i++)
            {
                var boss = bossStats[i];
                Wizard player = isTestMode
                    ? new Wizard(10, 250, 0)
                    : new Wizard(50, 500, 0);

                var state = State.Initial(player, boss);

                answer += $"Min Mana = {FindMinMana(state, tickDmg: 0)}\n";
            }

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            string answer = "";

            if (isTestMode)
            {
                answer += "No Tests for Part 2\n";
            }
            else
            {
                for (int i = 0; i < bossStats.Count; i++)
                {
                    var boss = bossStats[i];
                    var player = new Wizard(50, 500, 0);

                    var state = State.Initial(player, boss);

                    answer += $"Min Mana = {FindMinMana(state, tickDmg: 1)}\n";
                }
            }

            Part2Result = answer;
        }

        #region solver
        private static int FindMinMana(State initial, int tickDmg)
        {
            var visited = new HashSet<State>(collection: [initial]);
            var queue = new PriorityQueue<State, int>(items: [(initial, 0)]);

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (state.IsResolved)
                {
                    if (state.Boss.Hp <= 0)
                    {
                        return state.ManaUsed;
                    }

                    continue;
                }

                var adjacent = GetAdjacentStates(state, tickDmg);
                var unvisited = adjacent.Where(adj => !visited.Contains(adj));

                foreach (var adj in unvisited)
                {
                    visited.Add(adj);
                    queue.Enqueue(adj, adj.ManaUsed);
                }
            }

            throw new Exception("No Solution");
        }
        private static IEnumerable<State> GetAdjacentStates(State state, int tickDmg)
        {
            state = TickPlayerDmg(state, tickDmg);
            if (state.Player.Hp <= 0)
            {
                yield break;
            }

            state = TickEffects(state);
            if (state.IsResolved)
            {
                if (state.Player.Hp > 0)
                {
                    yield return state;
                }
                yield break;
            }

            var cachedPlayer = state.Player;
            var canCast = GameData.Spells
                .Where(s => !state.ActiveEffects.ContainsKey(s))
                .Where(s => GameData.SpellCosts[s] <= cachedPlayer.Mana)
                .ToArray();

            if (canCast.Length == 0)
            {
                yield break;
            }

            foreach (var spell in canCast)
            {
                var adjState = ExecutePlayerAttack(state, spell);

                if (!adjState.IsResolved)
                {
                    adjState = TickEffects(adjState);
                }

                if (!adjState.IsResolved)
                {
                    adjState = ExecuteBossAttack(adjState);
                }

                yield return adjState;
            }
        }
        private static State ExecutePlayerAttack(State state, Spell spell)
        {
            var player = state.Player;
            var boss = state.Boss;

            if (GameData.SpellDurations[spell] > 0)
            {
                var updatedEffects = new Dictionary<Spell, int>(state.ActiveEffects)
                { { spell, GameData.SpellDurations[spell] } };
                return new State(
                    player: player with { Mana = player.Mana - GameData.SpellCosts[spell] },
                    boss: boss,
                    manaUsed: state.ManaUsed + GameData.SpellCosts[spell],
                    activeEffects: updatedEffects);
            }

            var agents = CastSpell(spell, player, boss);
            return new State(
                player: agents.Player,
                boss: agents.Boss,
                manaUsed: state.ManaUsed + GameData.SpellCosts[spell],
                activeEffects: state.ActiveEffects);
        }
        private static State ExecuteBossAttack(State state)
        {
            var player = state.Player;
            var boss = state.Boss;
            player = state.Player with { Hp = player.Hp - Math.Max(1, boss.Dmg - player.Armor) };
            return new State(player, boss, state.ManaUsed, state.ActiveEffects);
        }
        private static (Wizard Player, Boss Boss) CastSpell(Spell spell, Wizard player, Boss boss)
        {
            switch (spell)
            {
                case Spell.MagicMissile:
                    boss = boss with { Hp = boss.Hp - 4 };
                    break;
                case Spell.Drain:
                    player = player with { Hp = player.Hp + 2 };
                    boss = boss with { Hp = boss.Hp - 2 };
                    break;
                case Spell.Shield:
                case Spell.Poison:
                case Spell.Recharge:
                    break;
                default:
                    throw new Exception("No Solution");
            }

            player = player with { Mana = player.Mana - GameData.SpellCosts[spell] };
            return (player, boss);
        }
        private static State TickPlayerDmg(State state, int amount)
        {
            var player = state.Player with { Hp = state.Player.Hp - amount };
            return new State(
                player,
                state.Boss,
                state.ManaUsed,
                state.ActiveEffects);
        }
        private static State TickEffects(State state)
        {
            var agents = (state.Player, state.Boss);
            var nextActive = new Dictionary<Spell, int>();

            foreach (var (effect, timer) in state.ActiveEffects)
            {
                if (timer >= 1)
                {
                    agents = TickEffect(effect, agents.Player, agents.Boss);
                    nextActive[effect] = timer - 1;
                }

                if (nextActive[effect] == 0)
                {
                    agents = RemoveEffect(effect, agents.Player, agents.Boss);
                    nextActive.Remove(effect);
                }
            }

            return new State(agents.Player, agents.Boss, state.ManaUsed, activeEffects: nextActive);
        }
        private static (Wizard Player, Boss Boss) TickEffect(Spell spell, Wizard player, Boss boss)
        {
            switch (spell)
            {
                case Spell.Poison:
                    boss = boss with { Hp = boss.Hp - 3 };
                    break;
                case Spell.Recharge:
                    player = player with { Mana = player.Mana + 101 };
                    break;
                case Spell.Shield:
                    player = player with { Armor = 7 };
                    break;
                case Spell.MagicMissile:
                case Spell.Drain:
                    break;
                default:
                    throw new Exception("No Solution");
            }

            return (player, boss);
        }
        private static (Wizard Player, Boss Boss) RemoveEffect(Spell spell, Wizard player, Boss boss)
        {
            switch (spell)
            {
                case Spell.Shield:
                    player = player with { Armor = 0 };
                    break;
                case Spell.MagicMissile:
                case Spell.Drain:
                case Spell.Poison:
                case Spell.Recharge:
                    break;
                default:
                    throw new Exception("No Solution");
            }

            return (player, boss);
        }
        #endregion
        #region gameObjects
        public static class GameData
        {
            public static readonly HashSet<Spell> Spells =
            [
                Spell.MagicMissile,
            Spell.Drain,
            Spell.Shield,
            Spell.Poison,
            Spell.Recharge
            ];

            public static readonly Dictionary<Spell, int> SpellCosts = new()
        {
            { Spell.MagicMissile,  53 },
            { Spell.Drain,         73 },
            { Spell.Shield,       113 },
            { Spell.Poison,       173 },
            { Spell.Recharge,     229 }
        };

            public static readonly Dictionary<Spell, int> SpellDurations = new()
        {
            { Spell.MagicMissile, 0 },
            { Spell.Drain,        0 },
            { Spell.Shield,       6 },
            { Spell.Poison,       6 },
            { Spell.Recharge,     5 }
        };
        }
        public enum Spell
        {
            MagicMissile,
            Drain,
            Shield,
            Poison,
            Recharge
        }
        public readonly record struct Boss(int Hp, int Dmg);
        public readonly record struct Wizard(int Hp, int Mana, int Armor);
        public readonly struct State(Wizard player, Boss boss, int manaUsed, Dictionary<Spell, int> activeEffects)
    : IEquatable<State>
        {
            private readonly string _key = $"[Player: Hp={player.Hp},M={player.Mana},U={manaUsed}]" +
                                           $"[Boss: Hp={boss.Hp}]" +
                                           $"[Effects: {string.Join(',', BuildActiveEffectsHash(activeEffects))}]";
            public Wizard Player { get; } = player;
            public Boss Boss { get; } = boss;
            public int ManaUsed { get; } = manaUsed;
            public Dictionary<Spell, int> ActiveEffects { get; } = activeEffects;

            public bool IsResolved => Player.Hp <= 0 || Boss.Hp <= 0;

            public static State Initial(Wizard player, Boss boss)
            {
                return new State(player, boss, manaUsed: 0, activeEffects: new Dictionary<Spell, int>());
            }

            private static string BuildActiveEffectsHash(Dictionary<Spell, int> effects)
            {
                return string.Concat(", ", effects
                    .Select(kvp => $"{kvp.Key}={kvp.Value}")
                    .Order());
            }

            public bool Equals(State other)
            {
                return _key == other._key;
            }

            public override bool Equals(object? obj)
            {
                return obj is State other && Equals(other);
            }

            public override int GetHashCode()
            {
                return _key.GetHashCode();
            }

            public static bool operator ==(State left, State right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(State left, State right)
            {
                return !left.Equals(right);
            }
        }

        #endregion
    }
}