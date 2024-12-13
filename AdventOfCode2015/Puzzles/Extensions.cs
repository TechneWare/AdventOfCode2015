using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    internal static class Extensions
    {
        public static T ToEnum<T>(this string value, T defaultValue)
        {
            T result = defaultValue;

            try { result = (T)Enum.Parse(typeof(T), value, true); }
            catch (Exception) { }

            return result;
        }
        public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }
        public static T? FromJson<T>(this string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        public static T Clone<T>(this T obj)
        {
            if (obj == null)
                return obj;

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto, // Include type information for polymorphic objects
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(obj, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static List<string> ToLines(this string rawData)
        {
            return [.. rawData.Replace("\r", "").Split("\n")];
        }
    }
}
