using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Unturned.Skills
{
    public class UnturnedSkill
    {
        public static readonly UnturnedSkill Overkill = new UnturnedSkill(0, 0);
        public static readonly UnturnedSkill Sharpshooter = new UnturnedSkill(0, 1);
        public static readonly UnturnedSkill Dexerity = new UnturnedSkill(0, 2);
        public static readonly UnturnedSkill Cardio = new UnturnedSkill(0, 3);
        public static readonly UnturnedSkill Exercise = new UnturnedSkill(0, 4);
        public static readonly UnturnedSkill Diving = new UnturnedSkill(0, 5);
        public static readonly UnturnedSkill Parkour = new UnturnedSkill(0, 6);
        public static readonly UnturnedSkill Sneakybeaky = new UnturnedSkill(1, 0);
        public static readonly UnturnedSkill Vitality = new UnturnedSkill(1, 1);
        public static readonly UnturnedSkill Immunity = new UnturnedSkill(1, 2);
        public static readonly UnturnedSkill Toughness = new UnturnedSkill(1, 3);
        public static readonly UnturnedSkill Strength = new UnturnedSkill(1, 4);
        public static readonly UnturnedSkill Warmblooded = new UnturnedSkill(1, 5);
        public static readonly UnturnedSkill Survival = new UnturnedSkill(1, 6);
        public static readonly UnturnedSkill Healing = new UnturnedSkill(2, 0);
        public static readonly UnturnedSkill Crafting = new UnturnedSkill(2, 1);
        public static readonly UnturnedSkill Outdoors = new UnturnedSkill(2, 2);
        public static readonly UnturnedSkill Cooking = new UnturnedSkill(2, 3);
        public static readonly UnturnedSkill Fishing = new UnturnedSkill(2, 4);
        public static readonly UnturnedSkill Agriculture = new UnturnedSkill(2, 5);
        public static readonly UnturnedSkill Mechanic = new UnturnedSkill(2, 6);
        public static readonly UnturnedSkill Engineer = new UnturnedSkill(2, 7);

        internal byte Speciality;
        internal byte Skill;

        internal UnturnedSkill(byte speciality, byte skill)
        {
            Speciality = speciality;
            Skill = skill;
        }
    };

}
