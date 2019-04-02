using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Data
{
    public class Classes
    {
        public static Dictionary<string, ushort> NamesToTypes = new Dictionary<string, ushort>() {
            ["Rogue"] = 768,
            ["Archer"] = 775,
            ["Wizard"] = 782,
            ["Priest"] = 784,
            ["Warrior"] = 797,
            ["Knight"] = 798,
            ["Paladin"] = 799,
            ["Assassin"] = 800,
            ["Necromancer"] = 801,
            ["Huntress"] = 802,
            ["Mystic"] = 803,
            ["Trickster"] = 804,
            ["Sorcerer"] = 805,
            ["Ninja"] = 806,
            ["Samurai"] = 785
        };
    }
}
