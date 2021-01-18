using System;
using System.Linq;

using Server.Items;

namespace Server.Misc
{
    public class RaceDefinitions
    {
        public static void Configure()
        {
            /* Here we configure all races. Some notes:
            * 
            * 1) The first 32 races are reserved for core use.
            * 2) Race 0x7F is reserved for core use.
            * 3) Race 0xFF is reserved for core use.
            * 4) Changing or removing any predefined races may cause server instability.
            */
            RegisterRace(new Human(0, 0));
            RegisterRace(new Elf(1, 1));
            RegisterRace(new Gargoyle(2, 2));
        }

        public static void RegisterRace(Race race)
        {
            Race.Races[race.RaceIndex] = race;
            Race.AllRaces.Add(race);
        }

        private class Human : Race
        {
            public Human(int raceID, int raceIndex)
                : base(raceID, raceIndex, "Human", "Humans", 400, 401, 402, 403)
            {
            }

            public override bool ValidateHair(bool female, int itemID)
            {
                if (itemID == 0)
                    return true;

                if (female && itemID == 0x2048 || !female && itemID == 0x2046)
                    return false;	//Buns & Receeding Hair

                if (itemID >= 0x203B && itemID <= 0x203D)
                    return true;

                if (itemID >= 0x2044 && itemID <= 0x204A)
                    return true;

                return false;
            }

            public override int RandomHair(bool female)	//Random hair doesn't include baldness
            {
                switch (Utility.Random(9))
                {
                    case 0:
                        return 0x203B;	//Short
                    case 1:
                        return 0x203C;	//Long
                    case 2:
                        return 0x203D;	//Pony Tail
                    case 3:
                        return 0x2044;	//Mohawk
                    case 4:
                        return 0x2045;	//Pageboy
                    case 5:
                        return 0x2047;	//Afro
                    case 6:
                        return 0x2049;	//Pig tails
                    case 7:
                        return 0x204A;	//Krisna
                    default:
                        return female ? 0x2046 : 0x2048;	//Buns or Receeding Hair
                }
            }

            public override bool ValidateFacialHair(bool female, int itemID)
            {
                if (itemID == 0)
                    return true;

                if (female)
                    return false;

                if (itemID >= 0x203E && itemID <= 0x2041)
                    return true;

                if (itemID >= 0x204B && itemID <= 0x204D)
                    return true;

                return false;
            }

            public override int RandomFacialHair(bool female)
            {
                if (female)
                    return 0;

                int rand = Utility.Random(7);

                return (rand < 4 ? 0x203E : 0x2047) + rand;
            }

            public override bool ValidateFace(bool female, int itemID)
            {
                if (itemID.Equals(0))
                    return false;

                if (itemID >= 0x3B44 && itemID <= 0x3B4D)
                    return true;

                return false;
            }

            public override int RandomFace(bool female)
            {
                int rand = Utility.Random(9);

                return 15172 + rand;
            }

            public override bool ValidateEquipment(Item item)
            {
                var elfOrHuman = item as ICanBeElfOrHuman;

                if (elfOrHuman != null)
                {
                    return !elfOrHuman.ElfOnly;
                }

                var itemID = item.ItemID;

                return !GargoyleOnlyIDs.Any(id => id == itemID) && !ElfOnlyIDs.Any(id => id == itemID);
            }

            public override int ClipSkinHue(int hue)
            {
                if (hue < 1002)
                {
                    return 1002;
                }

                if (hue > 1058)
                {
                    return 1058;
                }

                return hue;
            }

            public override int RandomSkinHue()
            {
                return Utility.Random(1002, 57) | 0x8000;
            }

            public override int ClipHairHue(int hue)
            {
                if (hue < 1102)
                {
                    return 1102;
                }

                if (hue > 1149)
                {
                    return 1149;
                }

                return hue;
            }

            public override int RandomHairHue()
            {
                return Utility.Random(1102, 48);
            }

            public override int ClipFaceHue(int hue)
            {
                return ClipSkinHue(hue);
            }

            public override int RandomFaceHue()
            {
                return RandomSkinHue();
            }
        }

        private class Elf : Race
        {
            private static readonly int[] m_SkinHues =
            {
                0x4DE, 0x76C, 0x835, 0x430, 0x24D, 0x24E, 0x24F, 0x0BF,
                0x4A7, 0x361, 0x375, 0x367, 0x3E8, 0x3DE, 0x353, 0x903,
                0x76D, 0x384, 0x579, 0x3E9, 0x374, 0x389, 0x385, 0x376,
                0x53F, 0x381, 0x382, 0x383, 0x76B, 0x3E5, 0x51D, 0x3E6
            };

            private static readonly int[] m_HairHues =
            {
                0x034, 0x035, 0x036, 0x037, 0x038, 0x039, 0x058, 0x08E,
                0x08F, 0x090, 0x091, 0x092, 0x101, 0x159, 0x15A, 0x15B,
                0x15C, 0x15D, 0x15E, 0x128, 0x12F, 0x1BD, 0x1E4, 0x1F3,
                0x207, 0x211, 0x239, 0x251, 0x26C, 0x2C3, 0x2C9, 0x31D,
                0x31E, 0x31F, 0x320, 0x321, 0x322, 0x323, 0x324, 0x325,
                0x326, 0x369, 0x386, 0x387, 0x388, 0x389, 0x38A, 0x59D,
                0x6B8, 0x725, 0x853
            };

            public Elf(int raceID, int raceIndex)
                : base(raceID, raceIndex, "Elf", "Elves", 605, 606, 607, 608)
            {
            }

            public override bool ValidateHair(bool female, int itemID)
            {
                if (itemID == 0)
                    return true;

                if ((female && (itemID == 0x2FCD || itemID == 0x2FBF)) || (!female && (itemID == 0x2FCC || itemID == 0x2FD0)))
                    return false;

                if (itemID >= 0x2FBF && itemID <= 0x2FC2)
                    return true;

                if (itemID >= 0x2FCC && itemID <= 0x2FD1)
                    return true;

                return false;
            }

            public override int RandomHair(bool female)	//Random hair doesn't include baldness
            {
                switch (Utility.Random(8))
                {
                    case 0:
                        return 0x2FC0;	//Long Feather
                    case 1:
                        return 0x2FC1;	//Short
                    case 2:
                        return 0x2FC2;	//Mullet
                    case 3:
                        return 0x2FCE;	//Knob
                    case 4:
                        return 0x2FCF;	//Braided
                    case 5:
                        return 0x2FD1;	//Spiked
                    case 6:
                        return (female ? 0x2FCC : 0x2FBF);	//Flower or Mid-long
                    default:
                        return female ? 0x2FD0 : 0x2FCD;	//Bun or Long
                }
            }

            public override bool ValidateFacialHair(bool female, int itemID)
            {
                return itemID == 0;
            }

            public override int RandomFacialHair(bool female)
            {
                return 0;
            }

            public override bool ValidateFace(bool female, int itemID)
            {
                if (itemID.Equals(0))
                    return false;

                if (itemID >= 0x3B44 && itemID <= 0x3B4D)
                    return true;

                return false;
            }
            public override int RandomFace(bool female)
            {
                int rand = Utility.Random(9);

                return 15172 + rand;
            }

            public override bool ValidateEquipment(Item item)
            {
                var elfOrHuman = item as ICanBeElfOrHuman;

                if (elfOrHuman != null && elfOrHuman.ElfOnly)
                {
                    return true;
                }

                var itemID = item.ItemID;

                return !GargoyleOnlyIDs.Any(id => id == itemID);
            }

            public override int ClipSkinHue(int hue)
            {
                for (int i = 0; i < m_SkinHues.Length; i++)
                    if (m_SkinHues[i] == hue)
                        return hue;

                return m_SkinHues[0];
            }

            public override int RandomSkinHue()
            {
                return m_SkinHues[Utility.Random(m_SkinHues.Length)] | 0x8000;
            }

            public override int ClipHairHue(int hue)
            {
                for (int i = 0; i < m_HairHues.Length; i++)
                    if (m_HairHues[i] == hue)
                        return hue;

                return m_HairHues[0];
            }

            public override int RandomHairHue()
            {
                return m_HairHues[Utility.Random(m_HairHues.Length)];
            }

            public override int ClipFaceHue(int hue)
            {
                return ClipSkinHue(hue);
            }

            public override int RandomFaceHue()
            {
                return RandomSkinHue();
            }
        }

        private class Gargoyle : Race
        {
            public Gargoyle(int raceID, int raceIndex)
                : base(raceID, raceIndex, "Gargoyle", "Gargoyles", 666, 667, 695, 694)
            {
            }

            public override bool ValidateHair(bool female, int itemID)
            {
                if (female == false)
                {
                    return itemID >= 0x4258 && itemID <= 0x425F;
                }

                return itemID == 0x4261 || itemID == 0x4262 || itemID >= 0x4273 && itemID <= 0x4275 || itemID == 0x42B0 || itemID == 0x42B1 || itemID == 0x42AA || itemID == 0x42AB;
            }

            public override int RandomHair(bool female)
            {
                if (Utility.Random(9) == 0)
                {
                    return 0;
                }

                if (!female)
                {
                    return 0x4258 + Utility.Random(8);
                }

                switch (Utility.Random(9))
                {
                    case 0:
                        return 0x4261;
                    case 1:
                        return 0x4262;
                    case 2:
                        return 0x4273;
                    case 3:
                        return 0x4274;
                    case 4:
                        return 0x4275;
                    case 5:
                        return 0x42B0;
                    case 6:
                        return 0x42B1;
                    case 7:
                        return 0x42AA;
                    case 8:
                        return 0x42AB;
                }

                return 0;
            }

            public override bool ValidateFacialHair(bool female, int itemID)
            {
                if (female)
                {
                    return false;
                }

                return itemID >= 0x42AD && itemID <= 0x42B0;
            }

            public override int RandomFacialHair(bool female)
            {
                if (female)
                {
                    return 0;
                }

                return Utility.RandomList(0, 0x42AD, 0x42AE, 0x42AF, 0x42B0);
            }

            public override bool ValidateFace(bool female, int itemID)
            {
                if (itemID.Equals(0))
                {
                    return false;
                }

                if (itemID >= 0x5679 && itemID <= 0x567E)
                {
                    return true;
                }

                return false;
            }
            public override int RandomFace(bool female)
            {
                int rand = Utility.Random(5);

                return 22137 + rand;
            }

            public override bool ValidateEquipment(Item item)
            {
                var itemID = item.ItemID;

                return !(item is BaseQuiver) && GargoyleOnlyIDs.Any(id => id == itemID);
            }

            public override int ClipSkinHue(int hue)
            {
                return hue; // for hue infomation gathering
            }

            public override int RandomSkinHue()
            {
                return Utility.Random(1755, 25) | 0x8000;
            }

            private static readonly int[] m_HornHues =
            {
                0x709, 0x70B, 0x70D, 0x70F, 0x711, 0x763,
                0x765, 0x768, 0x76B, 0x6F3, 0x6F1, 0x6EF,
                0x6E4, 0x6E2, 0x6E0, 0x709, 0x70B, 0x70D
            };

            public override int ClipHairHue(int hue)
            {
                for (int i = 0; i < m_HornHues.Length; i++)
                    if (m_HornHues[i] == hue)
                        return hue;

                return m_HornHues[0];
            }

            public override int RandomHairHue()
            {
                return m_HornHues[Utility.Random(m_HornHues.Length)];
            }

            public override int ClipFaceHue(int hue)
            {
                return ClipSkinHue(hue);
            }

            public override int RandomFaceHue()
            {
                return RandomSkinHue();
            }
        }

        public static bool ValidateEquipment(Mobile from, Item equipment)
        {
            return ValidateEquipment(from, equipment, true);
        }

        public static bool ValidateEquipment(Mobile from, Item equipment, bool message)
        {
            if ((AllRaceTypes.Any(type => type == equipment.GetType()) || AllRaceIDs.Any(id => id == equipment.ItemID)) && ValidateElfOrHuman(from, equipment))
            {
                return true;
            }

            if (!from.Race.ValidateEquipment(equipment))
            {
                if (from.Race == Race.Gargoyle)
                {
                    if (message)
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1111708); // Gargoyles can't wear this.
                    }
                }
                else
                {
                    var required = GetRequiredRace(equipment);

                    if (required == Race.Elf)
                    {
                        if (from.FindItemOnLayer(Layer.Earrings) is MorphEarrings)
                        {
                            return true;
                        }

                        if (message)
                        {
                            from.SendLocalizedMessage(1072203); // Only Elves may use this.
                        }
                    }
                    else if (required == Race.Gargoyle)
                    {
                        if (message)
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1111707); // Only gargoyles can wear this.
                        }
                    }
                    else if (required != Race.Human)
                    {
                        if (message)
                        {
                            from.SendMessage("Only {0} may use this.", required.PluralName);
                        }
                    }
                }

                return false;
            }

            return true;
        }

        public static bool ValidateElfOrHuman(Mobile from, Item equipment)
        {
            var elfOrHuman = equipment as ICanBeElfOrHuman;

            if (elfOrHuman != null)
            {
                return from.Race == Race.Elf || !elfOrHuman.ElfOnly;
            }

            return true;
        }

        public static Race GetRequiredRace(Item item)
        {
            var itemID = item.ItemID;

            if (GargoyleOnlyIDs.Any(id => id == itemID))
            {
                return Race.Gargoyle;
            }

            var elfOrHuman = item as ICanBeElfOrHuman;

            if (elfOrHuman != null)
            {
                return elfOrHuman.ElfOnly ? Race.Elf : Race.Human;
            }

            if (ElfOnlyIDs.Any(id => id == itemID))
            {
                return Race.Elf;
            }

            return Race.Human;
        }

        public static Type[] AllRaceTypes => _AllRaceTypes;
        private static Type[] _AllRaceTypes =
        {
            typeof(BootsOfBallast), typeof(DetectiveCredentials)
        };

        public static int[] AllRaceIDs => _AllRaceIDs;
        private static int[] _AllRaceIDs =
        {
            0xA289, 0xA28A, 0xA28B, 0xA291, 0xA292, 0xA293, // whips
            0xE85, 0xE86,                                   // Tools
            0x1F03, 0x1F04, 0x26AE,                         // Robe & Arcane Robe
            0xE81,                                          // Crook
            0x1086, 0x108A, 0x1F06, 0x1F09,                 // Rings/Bracelet
            0xA412                                          // Tabard
        };

        public static int[] GargoyleOnlyIDs => _GargoyleOnlyIDs;
        private static int[] _GargoyleOnlyIDs =
        {
            0x283, 0x284, 0x285, 0x286, 0x287, 0x288, 0x289, 0x28A, // Armor
            0x301, 0x302, 0x303, 0x304, 0x305, 0x306, 0x310, 0x311, 
            0x307, 0x308, 0x309, 0x30A, 0x30B, 0x30C, 0x30D, 0x30E, 
            0x403, 0x404, 0x405, 0x406, 0x407, 0x408, 0x409, 0x40A,

            0x41D8, 0x41D9, 0x42DE, 0x42DF,                         // Talons

            0x8FD, 0x8FE, 0x8FF, 0x900, 0x901, 0x902, 0x903, 0x904, // Weapons
            0x905, 0x906, 0x907, 0x908, 0x909, 0x90A, 0x90B, 0x90C,
            0x48AE, 0x48AF, 0x48B0, 0x48B1, 0x48B2, 0x48B3, 0x48B4,
            0x48B5, 0x48B6, 0x48B7, 0x48B8, 0x48B9, 0x48BA, 0x48BB,
            0x48BC, 0x48BD, 0x48BE, 0x48BF, 0x48C0, 0x48C1, 0x48C2,
            0x48C3, 0x48C4, 0x48C5, 0x48C6, 0x48C7, 0x48C8, 0x48C9,
            0x48CA, 0x48CB, 0x48CC, 0x48CD, 0x48CE, 0x48CF, 0x48D0,
            0x48D1, 0x48D2, 0x48D3,

            0x4000, 0x4001, 0x4002, 0x4003,                         // Robes
            0x9986,                                                 // Apaulets

            0x4047, 0x4048, 0x4049, 0x404A, 0x404B, 0x404C, 0x404D, // Armor/Weapons
            0x404E, 0x404F, 0x4050, 0x4051, 0x4052, 0x4053, 0x4054,
            0x4055, 0x4056, 0x4057, 0x4058, 0x4058, 0x4059, 0x405A,
            0x405B, 0x405C, 0x405D, 0x405E, 0x405F, 0x4060, 0x4061,
            0x4062, 0x4063, 0x4064, 0x4065, 0x4066, 0x4067, 0x4068,
            0x4068, 0x4069, 0x406A, 0x406B, 0x406C, 0x406D, 0x406E,
            0x406F, 0x4070, 0x4071, 0x4072, 0x4072, 0x7073, 0x4074,
            0x4075, 0x4076,

            0x4200, 0x4201, 0x4202, 0x4203, 0x4204, 0x4205, 0x4206, // Shields
            0x4207, 0x4208, 0x4209, 0x420A, 0x420B, 0x4228, 0x4229,
            0x422A, 0x422C,

            0x4210, 0x4211, 0x4212, 0x4213, 0x4D0A, 0x4D0B,        // Jewelry

            0x450D, 0x450E, 0x50D8,                                 // Belts and Half Apron
            0x457E, 0x457F, 0x45A4, 0x45A5,                         // Wing Armor
            0x45B1, 0x45B3, 0x4644, 0x4645,                         // Glasses
            0x46AA, 0x46AB, 0x46B4, 0x46B5,                         // Sashes

            0xA1C9, 0xA1CA                                          // Special
        };

        public static int[] ElfOnlyIDs => _ElfOnlyIDs;
        private static int[] _ElfOnlyIDs =
        {
            0x2B67, 0x2B68, 0x2B69, 0x2B6A, 0x2B6B, 0x2B6C, 0x2B6D, // Armor
            0x2B6E, 0x2B6F, 0x2B70, 0x2B71, 0x2B72, 0x2B73, 0x2B74,
            0x2B75, 0x2B76, 0x2B77, 0x2B78, 0x2B79, 0x3160, 0x3161,
            0x3162, 0x3163, 0x3164, 0x3165, 0x3166, 0x3167, 0x3168,
            0x3169, 0x316A, 0x316B, 0x316C, 0x316D, 0x316E, 0x316F,
            0x3170, 0x3171, 0x3172, 0x3173, 0x3174, 0x3175, 0x3176,
            0x3177, 0x3178, 0x3179, 0x317A, 0x317B, 0x317C, 0x317D,
            0x317E, 0x317F, 0x3180, 0x3181,

            0x2FB9, 0x2FBA,                                         // Robes

            0x2FC3, 0x2FC4, 0x2FC5, 0x2FC6, 0x2FC7, 0x2FC8, 0x2FC9, // Cloths
            0x2FCA, 0x2FCB,

            0x315F // Belt
        };
    }
}
