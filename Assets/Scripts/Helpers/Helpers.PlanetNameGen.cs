using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    ///     Credit to: https://github.com/hbi99/namegen/tree/master
    /// </summary>
    public static class PlanetNameGen
    {

        private static string GetRandomSyllable(int syllableGroupIndex) {
            return Syllables[syllableGroupIndex][Random.Range(0, Syllables[syllableGroupIndex].Count)];
        }

        public static List<string> GenerateNames(int nameCount) {
            var names = new List<string>();

            var loopCounter = new Helpers.WrappedLoopCounter(SyllablePatterns.Count);

            for (var i = 0; i < nameCount; i++)
            {
                var name = "";
                var syllablePattern = SyllablePatterns[loopCounter.CurrentIncrement];
                syllablePattern.ForEach(syllableGroupIndex => name += GetRandomSyllable(syllableGroupIndex));

                // https://learn.microsoft.com/en-us/dotnet/standard/base-types/changing-case
                var ti = CultureInfo.CurrentCulture.TextInfo;
                name = ti.ToTitleCase(name);

                names.Add(name);
                loopCounter.Increment();
            }

            return names;
        }

        #region SyllableData

        private static readonly List<List<string>> Syllables = new()
        {
            new List<string>
            {
                "b",
                "c",
                "d",
                "f",
                "g",
                "h",
                "i",
                "j",
                "k",
                "l",
                "m",
                "n",
                "p",
                "q",
                "r",
                "s",
                "t",
                "v",
                "w",
                "x",
                "y",
                "z"
            },
            new List<string>
            {
                "a",
                "e",
                "o",
                "u"
            },
            new List<string>
            {
                "br",
                "cr",
                "dr",
                "fr",
                "gr",
                "pr",
                "str",
                "tr",
                "bl",
                "cl",
                "fl",
                "gl",
                "pl",
                "sl",
                "sc",
                "sk",
                "sm",
                "sn",
                "sp",
                "st",
                "sw",
                "ch",
                "sh",
                "th",
                "wh"
            },
            new List<string>
            {
                "ae",
                "ai",
                "ao",
                "au",
                "a",
                "ay",
                "ea",
                "ei",
                "eo",
                "eu",
                "e",
                "ey",
                "ua",
                "ue",
                "ui",
                "uo",
                "u",
                "uy",
                "ia",
                "ie",
                "iu",
                "io",
                "iy",
                "oa",
                "oe",
                "ou",
                "oi",
                "o",
                "oy"
            },
            new List<string>
            {
                "turn",
                "ter",
                "nus",
                "rus",
                "tania",
                "hiri",
                "hines",
                "gawa",
                "nides",
                "carro",
                "rilia",
                "stea",
                "lia",
                "lea",
                "ria",
                "nov",
                "phus",
                "mia",
                "nerth",
                "wei",
                "ruta",
                "tov",
                "zuno",
                "vis",
                "lara",
                "nia",
                "liv",
                "tera",
                "gantu",
                "yama",
                "tune",
                "ter",
                "nus",
                "cury",
                "bos",
                "pra",
                "thea",
                "nope",
                "tis",
                "clite"
            },
            new List<string>
            {
                "una",
                "ion",
                "iea",
                "iri",
                "illes",
                "ides",
                "agua",
                "olla",
                "inda",
                "eshan",
                "oria",
                "ilia",
                "erth",
                "arth",
                "orth",
                "oth",
                "illon",
                "ichi",
                "ov",
                "arvis",
                "ara",
                "ars",
                "yke",
                "yria",
                "onoe",
                "ippe",
                "osie",
                "one",
                "ore",
                "ade",
                "adus",
                "urn",
                "ypso",
                "ora",
                "iuq",
                "orix",
                "apus",
                "ion",
                "eon",
                "eron",
                "ao",
                "omia"
            }
        };

        private static readonly List<List<int>> SyllablePatterns = new()
        {
            new List<int>
            {
                0,
                1,
                4
            },
            new List<int>
            {
                1,
                2,
                5
            },
            new List<int>
            {
                2,
                3,
                4
            },
            new List<int>
            {
                3,
                2,
                5
            },
            new List<int>
            {
                2,
                3,
                1,
                4
            },
            new List<int>
            {
                1,
                0,
                2,
                5
            },
            new List<int>
            {
                2,
                3,
                1,
                4
            },
            new List<int>
            {
                3,
                2,
                0,
                5
            },
            new List<int>
            {
                2,
                3,
                0,
                3,
                4
            },
            new List<int>
            {
                3,
                0,
                3,
                2,
                5
            }
        };

        #endregion

    }
}