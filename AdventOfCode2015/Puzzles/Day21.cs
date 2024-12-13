using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2015.Puzzles.Day21;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 21: RPG Simulator 20XX ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/21"/>
    internal class Day21 : Puzzle
    {
        private Item bossStats;
        public Day21()
            : base(Name: "RPG Simulator 20XX", DayNumber: 21) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            int hp = 0;
            int dmg = 0;
            int ac = 0;

            foreach (var line in data)
            {
                var d = line.Split(":", StringSplitOptions.TrimEntries);
                if (d[0] == "Hit Points") hp = int.Parse(d[1]);
                if (d[0] == "Damage") dmg = int.Parse(d[1]);
                if (d[0] == "Armor") ac = int.Parse(d[1]);
            }

            bossStats = new Item(Item.Type.BossBase, name: "BossBase", cost: 0, hitpoints: hp, damage: dmg, armor: ac);
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var playerBase = isTestMode
                ? new Item(Item.Type.PlayerBase, name: "PlayerBase", cost: 0, hitpoints: 8, damage: 5, armor: 5)
                : new Item(Item.Type.PlayerBase, name: "PlayerBase", cost: 0, hitpoints: 100, damage: 0, armor: 0);

            var player = new Character(Character.Type.Player, "Player");
            var boss = new Character(Character.Type.Boss, "Boss");
            var shop = new Shop(playerBase, bossStats);

            shop.Enter(boss);
            shop.Enter(player);

            var answer = "";
            if (isTestMode)
            {
                var battle = new Battle(boss, player);
                battle.Fight();

                foreach (var msg in battle.Log)
                    answer += $"{msg}\n";
            }
            else
            {
                (int cost, List<Item> items, List<string> Log) minWin = (-1, [], []);
                var carts = shop.GetAllCartCombos();
                foreach (var cart in carts)
                {
                    boss.Reset();
                    shop.Enter(boss);

                    player.Reset();
                    shop.Enter(player);
                    shop.BuyItems(cart);

                    var battle = new Battle(boss, player);
                    battle.Fight();

                    if (battle.Winner == player)
                    {
                        if (minWin.cost == -1 || player.TotalCost < minWin.cost)
                            minWin = (player.TotalCost, player.Items.Clone(), battle.Log.Clone());
                    }
                }
                var (minCost, bestItems, log) = minWin;

                if (WithLogging)
                {
                    foreach (var msg in log)
                        answer += $"{msg}\n";
                    answer += "\n";
                }

                foreach (var item in bestItems.Where(i => i.Name != "PlayerBase"))
                    answer += $"{item.Name}:{item.Cost} ";

                answer += "\n";

                answer += $"Min Cost = {minCost}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            var answer = "";
            if (!isTestMode)
            {
                ParseData();

                var player = new Character(Character.Type.Player, "Player");
                var boss = new Character(Character.Type.Boss, "Boss");

                var playerBase = new Item(Item.Type.PlayerBase, name: "PlayerBase", cost: 0, hitpoints: 100, damage: 0, armor: 0);
                var shop = new Shop(playerBase, bossStats);

                (int cost, List<Item> items, List<string> Log) maxLoss = (-1, [], []);
                var carts = shop.GetAllCartCombos();
                foreach (var cart in carts)
                {
                    boss.Reset();
                    shop.Enter(boss);

                    player.Reset();
                    shop.Enter(player);
                    shop.BuyItems(cart);

                    var battle = new Battle(boss, player);
                    battle.Fight();

                    if (battle.Winner == boss)
                    {
                        if (player.TotalCost > maxLoss.cost)
                            maxLoss = (player.TotalCost, player.Items.Clone(), battle.Log.Clone());
                    }
                }
                var (maxCost, worstItems, log) = maxLoss;

                if (WithLogging)
                {
                    foreach (var msg in log)
                        answer += $"{msg}\n";
                    answer += "\n";
                }

                foreach (var item in worstItems.Where(i => i.Name != "PlayerBase"))
                    answer += $"{item.Name}:{item.Cost} ";

                answer += "\n";

                answer += $"Max Cost = {maxCost}\n";
            }
            else
                answer += "Tests Skipped\n";

            Part2Result = answer;
        }
        internal class Battle(Character boss, Character player)
        {
            public Character Boss { get; internal set; } = boss;
            public Character Player { get; internal set; } = player;
            public Character? Winner { get; internal set; }
            public List<string> Log { get; internal set; } = [];

            public void Fight()
            {
                Log.Clear();
                while (Boss.IsAlive && Player.IsAlive)
                {
                    if (Player.IsAlive) Boss.TakeDamage(GetDamage(Player, Boss));
                    if (Boss.IsAlive) Player.TakeDamage(GetDamage(Boss, Player));

                    Log.Add($"Player HP({Player.HitPoints}) Boss HP({Boss.HitPoints})");
                }

                if (Boss.IsAlive) Winner = Boss;
                if (Player.IsAlive) Winner = Player;

                if (Winner != null)
                    Log.Add($"{Winner.Name} has won with {Winner.HitPoints} HP remaining");
                else
                    Log.Add($"No winner, both players are dead");
            }

            private int GetDamage(Character attacker, Character defender)
            {
                var damage = Math.Max(attacker.Damage - defender.Armor, 1);

                Log.Add($"{attacker.Name} hit {defender.Name} with their {attacker.WeaponName} for {damage} damage");

                return damage;
            }
        }

        internal class Shop
        {
            public List<Item> AvailableItems { get; set; } = [];
            private readonly List<Item> DefaultItems = [
                new(Item.Type.Weapon, name: "Dagger", cost: 8,hitpoints: 0, damage: 4, armor: 0),
                new(Item.Type.Weapon, name: "Shortsword", cost: 10,hitpoints: 0, damage: 5, armor: 0),
                new(Item.Type.Weapon, name: "Warhammer", cost: 25,hitpoints: 0, damage: 6, armor: 0),
                new(Item.Type.Weapon, name: "Longsword", cost: 40,hitpoints: 0, damage: 7, armor: 0),
                new(Item.Type.Weapon, name: "Greataxe", cost: 74,hitpoints: 0, damage: 8, armor: 0),

                new(Item.Type.Armor, name: "Leather", cost: 13,hitpoints: 0, damage: 0, armor: 1),
                new(Item.Type.Armor, name: "Chainmail", cost: 31,hitpoints: 0, damage: 0, armor: 2),
                new(Item.Type.Armor, name: "Splintmail", cost: 53,hitpoints: 0, damage: 0, armor: 3),
                new(Item.Type.Armor, name: "Bandedmail", cost: 75,hitpoints: 0, damage: 0, armor: 4),
                new(Item.Type.Armor, name: "Platemail", cost: 102,hitpoints: 0, damage: 0, armor: 5),

                new(Item.Type.Ring, name: "Damage +1", cost: 25,hitpoints: 0, damage: 1, armor: 0),
                new(Item.Type.Ring, name: "Damage +2", cost: 50,hitpoints: 0, damage: 2, armor: 0),
                new(Item.Type.Ring, name: "Damage +3", cost: 100,hitpoints: 0, damage: 3, armor: 0),
                new(Item.Type.Ring, name: "Defense +1", cost: 20,hitpoints: 0, damage: 0, armor: 1),
                new(Item.Type.Ring, name: "Defense +2", cost: 40,hitpoints: 0, damage: 0, armor: 2),
                new(Item.Type.Ring, name: "Defense +3", cost: 80,hitpoints: 0, damage: 0, armor: 3),
                ];

            private readonly Item playerBase;
            private readonly Item bossBase;

            private Character? currentCharacter;

            public Shop(Item playerBase, Item bossBase)
            {
                if (playerBase.TypeOfItem != Item.Type.PlayerBase) throw new InvalidDataException("Item playerBase Must be of type PlayerBase");
                if (bossBase.TypeOfItem != Item.Type.BossBase) throw new InvalidDataException("Item bossBase Must be of type BossBase");

                Reset();

                this.playerBase = playerBase;
                this.bossBase = bossBase;
            }
            public void Reset()
            {
                AvailableItems.Clear();
                foreach (var i in DefaultItems)
                    AvailableItems.Add(i);
            }

            public void Enter(Character character)
            {
                currentCharacter = character;
                if (currentCharacter.TypeOfCharacter == Character.Type.Player) character.AddItem(playerBase);
                if (currentCharacter.TypeOfCharacter == Character.Type.Boss) character.AddItem(bossBase);

                character.Validate();
            }
            public void BuyItems(List<string> items)
            {
                if (currentCharacter == null) throw new Exception("No one is in the shop to buy any items");
                if (items == null || items.Count == 0) return;

                var cart = new List<Item>();

                foreach (var item in items)
                    cart.Add(GetItem(item));

                if (!currentCharacter.SetItems(cart))
                    throw new Exception("Invalid Item Set Detected");

                Reset();
            }
            private Item GetItem(string ItemName)
            {
                var item = AvailableItems.Where(i => i.Name == ItemName).SingleOrDefault()
                    ?? throw new InvalidOperationException($"Item: {ItemName} is not available to buy");

                AvailableItems.Remove(item);

                return item;
            }
            public List<List<string>> GetAllCartCombos()
            {
                List<List<string>> carts = [];

                var Weapons = DefaultItems.Where(i => i.TypeOfItem == Item.Type.Weapon).ToList();
                var Armors = DefaultItems.Where(i => i.TypeOfItem == Item.Type.Armor).ToList();
                var Rings = DefaultItems.Where(i => i.TypeOfItem == Item.Type.Ring).ToList();

                var ringCombos = new List<(string ring1, string ring2)>();

                for (int r1 = -1; r1 < Rings.Count; r1++)
                {
                    for (int r2 = -1; r2 < Rings.Count; r2++)
                    {
                        if (r1 == r2 && r1 >= 0 && r2 >= 0)
                            continue;

                        var left = r1 < 0 ? "" : Rings[r1].Name;
                        var right = r2 < 0 ? "" : Rings[r2].Name;

                        ringCombos.Add((left, right));
                    }
                }

                for (int w = 0; w < Weapons.Count; w++)
                {
                    var weapon = Weapons[w].Name;

                    for (int a = -1; a < Armors.Count; a++)
                    {
                        var armor = a < 0 ? "" : Armors[a].Name;

                        for (int r = 0; r < ringCombos.Count; r++)
                        {
                            var cart = new List<string>();

                            if (weapon != "") cart.Add(weapon);
                            if (armor != "") cart.Add(armor);
                            if (ringCombos[r].ring1 != "") cart.Add(ringCombos[r].ring1);
                            if (ringCombos[r].ring2 != "") cart.Add(ringCombos[r].ring2);

                            carts.Add(cart);
                        }
                    }
                }

                return carts;
            }
        }

        internal class Character(Character.Type type, string name)
        {
            public enum Type
            {
                Player,
                Boss
            }

            public string Name { get; internal set; } = name;
            public Type TypeOfCharacter { get; internal set; } = type;
            public List<Item> Items { get; internal set; } = [];
            public string WeaponName
            {
                get
                {
                    var w = Items.Where(i => i.TypeOfItem == Item.Type.Weapon).SingleOrDefault();
                    if (w != null) return w.Name;
                    return "Fists";
                }
            }
            public int TotalCost => Items.Sum(i => i.Cost);
            public int Damage { get; internal set; } = 0;
            public int Armor { get; internal set; } = 0;
            public int HitPoints { get; internal set; } = 0;

            public bool IsAlive => HitPoints > 0;
            public List<string> Warnings { get; internal set; } = [];

            public bool SetItems(List<Item> items)
            {
                foreach (var i in items)
                    AddItem(i);

                if (!this.Validate())
                    return false;

                return true;
            }
            public void AddItem(Item item)
            {
                Items.Add(item);
            }

            private void DisplayWarnings()
            {
                AnsiConsole.MarkupLine("[red slowblink]Invalid Configuration[/]");
                foreach (var warn in Warnings)
                    AnsiConsole.MarkupLineInterpolated($"[bold yellow] => {warn}[/]");
            }
            public void TakeDamage(int damage)
            {
                HitPoints = Math.Max(HitPoints - damage, 0);
            }
            public void Reset()
            {
                Warnings.Clear();
                Items.Clear();
                Damage = 0;
                Armor = 0;
                HitPoints = 0;
            }
            public bool Validate()
            {
                if (!HasValidItems())
                {
                    DisplayWarnings();
                    return false;
                }

                Damage = Items.Sum(i => i.Damage);
                Armor = Items.Sum(i => i.Armor);
                HitPoints = Items.Sum(i => i.Hitpoints);

                return true;
            }
            private bool HasValidItems()
            {
                Warnings.Clear();

                //Allow fighting with fists (IE: juse the base stats are used)
                if (Items.Count == 1 &&
                (Items[0].TypeOfItem == Item.Type.PlayerBase
                || Items[0].TypeOfItem == Item.Type.BossBase))
                    return true;

                var hasBase = Items.Where(i => i.TypeOfItem == Item.Type.PlayerBase || i.TypeOfItem == Item.Type.BossBase).Count() == 1;
                var hasWeapon = Items.Where(i => i.TypeOfItem == Item.Type.Weapon).Count() == 1;
                var hasArmor = Items.Where(i => i.TypeOfItem == Item.Type.Armor).Count() <= 1;
                var hasRings = Items.Where(i => i.TypeOfItem == Item.Type.Ring).Count() <= 2;

                if (!hasBase) Warnings.Add($"Base Item Invalid, must have one of PlayerBase or BossBase - Base Items: {Items.Count(i => i.TypeOfItem == Item.Type.PlayerBase || i.TypeOfItem == Item.Type.BossBase)}");
                if (!hasWeapon) Warnings.Add($"Weapon Invalid, must have exactly one weapon - Weapons: {Items.Count(i => i.TypeOfItem == Item.Type.Weapon)}");
                if (!hasArmor) Warnings.Add($"Armor Invalid, must have 0-1 Armor Item - Armor: {Items.Count(i => i.TypeOfItem == Item.Type.Armor)}");
                if (!hasRings) Warnings.Add($"Rings Invalid, must have 0-2 Ring Items - Rings: {Items.Count(i => i.TypeOfItem == Item.Type.Ring)}");

                return hasBase && hasWeapon && hasArmor && hasRings;
            }
        }

        internal class Item(Item.Type type, string name, int cost, int hitpoints, int damage, int armor)
        {
            public enum Type
            {
                None,
                PlayerBase,
                BossBase,
                Weapon,
                Armor,
                Ring
            }

            public Type TypeOfItem { get; internal set; } = type;
            public string Name { get; internal set; } = name;
            public int Cost { get; internal set; } = cost;
            public int Hitpoints { get; internal set; } = hitpoints;
            public int Damage { get; internal set; } = damage;
            public int Armor { get; internal set; } = armor;
        }
    }
}
