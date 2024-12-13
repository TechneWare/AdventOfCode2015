using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 11: Corporate Policy ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/11"/>
    internal class Day11 : Puzzle
    {
        private readonly List<SantaPassword> startingPasswords = [];
        public Day11()
            : base(Name: "Corporate Policy", DayNumber: 11) { }

        public override void ParseData()
        {
            startingPasswords.Clear();
            foreach (var p in DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries))
            {
                startingPasswords.Add(new SantaPassword(p));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            if (isTestMode)
            {
                answer += "Validators:\n";

                string[] expected = [
                        "Invalid",
                        "Invalid",
                        "Invalid",
                        "Invalid",
                        "Valid",
                        "Invalid",
                        "Valid"
                    ];

                for (int j = 0; j < startingPasswords.Count; j++)
                {
                    var p = startingPasswords[j];
                    answer += $"{p} -> {(p.IsValidPassword() ? "Valid" : "Invalid")} : Expected -> {expected[j]}\n";
                }

                answer += "Next Passwords:\n";
                var Pass1 = new SantaPassword("abcdefgh");
                answer += $"{Pass1} -> ";
                var next1 = Pass1.GetNextValidPassword();
                answer += $"{next1} | Expected: abcdffaa\n";

                var Pass2 = new SantaPassword("ghijklmn");
                answer += $"{Pass2} -> ";
                var next2 = Pass2.GetNextValidPassword();
                answer += $"{next2} | Expected: ghjaabcc\n";
            }
            else
            {
                var p = startingPasswords.First();
                answer += $"{p} -> ";
                p.GetNextValidPassword();
                answer += $"{p}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            if (isTestMode)
            {
                answer += "Tests Skipped\n";
            }
            else
            {
                var p = startingPasswords.First();
                p.GetNextValidPassword();
                answer += $"{p} -> ";
                p.GetNextValidPassword();
                answer += $"{p}\n";
            }

            Part2Result = answer;
        }

    }
    public class SantaPassword
    {
        private readonly StringBuilder value;

        public SantaPassword(string initialValue)
        {
            if (string.IsNullOrEmpty(initialValue) || !IsValidString(initialValue))
            {
                throw new ArgumentException("String must consist of lowercase letters only.");
            }
            value = new StringBuilder(initialValue);
        }

        public override string ToString() => value.ToString();

        public static SantaPassword operator ++(SantaPassword str)
        {
            for (int i = str.value.Length - 1; i >= 0; i--)
            {
                if (str.value[i] < 'z')
                {
                    str.value[i]++;
                    return str;
                }
                str.value[i] = 'a';
            }
            str.value.Insert(0, 'a');
            return str;
        }
        public static bool IsValidString(string str) => str.All(c => c >= 'a' && c <= 'z');
        public override bool Equals(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is not SantaPassword other)
            {
                return false;
            }
            return this.ToString() == other.ToString();
        }
        public override int GetHashCode() => value.ToString().GetHashCode();
    }
    public static class SantaPasswordExtensions
    {
        public static SantaPassword GetNextValidPassword(this SantaPassword s)
        {
            do
            {
                s++;
            } while (!s.IsValidPassword());

            return s;
        }
        public static bool IsValidPassword(this SantaPassword sp)
        {
            var password = sp.ToString();

            var hasValidChars = !ContainsInvalidLetters(password);
            if (!hasValidChars) return false;

            var hasProperChars = IsValidString(sp);
            var isProperLength = HasProperLength(password, 8);
            var hasStraight = HasOneStraight(password);
            var hasPairs = HasTwoPairs(password);

            return isProperLength && hasStraight && hasValidChars && hasPairs && hasProperChars;
        }
        public static bool IsValidString(this SantaPassword sp)
        {
            var str = sp.ToString();
            return str.All(c => c >= 'a' && c <= 'z');
        }
        private static bool HasProperLength(string password, int length)
        {
            return password.Length == length;
        }
        private static bool HasOneStraight(string str)
        {
            string allChars = "abcdefghijklmnopqrstuvwxyz";
            var straights = new List<string>();

            for (int i = 0; i < allChars.Length - 2; i++)
                straights.Add($"{allChars[i]}{allChars[i + 1]}{allChars[i + 2]}");

            return straights.Any(s => str.Contains(s));
        }
        private static readonly char[] invalidChars = ['i', 'o', 'l'];
        public static bool ContainsInvalidLetters(string str)
        {
            return str.Any(c => invalidChars.Contains(c));
        }
        private static bool HasTwoPairs(string str)
        {
            int pairs = 0;
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == str[i + 1])
                {
                    pairs++;
                    if (pairs == 2)
                        break;

                    i++;
                }
            }

            return pairs == 2;
        }
    }
}
