using Server.Items;

using System;
using System.Linq;

namespace Server.Engines.ArtisanFestival
{
    public class RewardLantern : BaseLight, IFlipable
    {
        public override int LabelNumber => 1125100;

        public override int LitItemID => GetLitID();
        public override int UnlitItemID => GetUnlitID();

        public bool EastFacing => IDTable.Any(list => Array.IndexOf(list, ItemID) == 0 || Array.IndexOf(list, ItemID) == 2);

        [Constructable]
        public RewardLantern()
            : this(null)
        {
        }

        public RewardLantern(Mobile m)
            : base(GetID(m))
        {
        }

        public RewardLantern(Serial serial)
            : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

            if (list != null)
            {
                var index = Array.IndexOf(list, ItemID);

                if (index == 0 || index == 2)
                {
                    ItemID = list[index + 1];
                }
                else if (index == 1 || index == 3)
                {
                    ItemID = list[index - 1];
                }
            }
        }

        private static int GetID(Mobile m)
        {
            if (m == null)
            {
                return IDTable[Utility.Random(IDTable.Length)][0];
            }

            if (m.Karma < 0)
            {
                return IDTable[Utility.RandomMinMax(4, 7)][0];
            }

            return IDTable[Utility.RandomMinMax(0, 3)][0];
        }

        private int GetLitID()
        {
            var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

            if (list != null)
            {
                return EastFacing ? list[2] : list[3];
            }

            return ItemID;
        }

        private int GetUnlitID()
        {
            var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

            if (list != null)
            {
                return EastFacing ? list[0] : list[1];
            }

            return ItemID;
        }

        private static int[][] IDTable = new int[][]
        {
                    //       OFF             ON
                    //  East    South   East    South
            // Virtue
            new int[] { 0x9DE5, 0x9E0A, 0x9DFE, 0x9E0E },
            new int[] { 0x9DE7, 0x9E0C, 0x9E01, 0x9E11 },
            new int[] { 0xA084, 0xA088, 0xA085, 0xA089 },
            new int[] { 0xA08C, 0xA090, 0xA08D, 0xA091 },

            // Vice
            new int[] { 0x9DE6, 0x9E0B, 0x9E04, 0x9E14 },
            new int[] { 0x9DE8, 0x9E0D, 0x9E07, 0x9E17 },
            new int[] { 0xA074, 0xA078, 0xA075, 0xA079 },
            new int[] { 0xA07C, 0xA080, 0xA07D, 0xA081 }
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RewardPillow : Item, IFlipable
    {
        public override int LabelNumber => 1125137;

        public bool EastFacing => IDTable.Any(list => Array.IndexOf(list, ItemID) == 1);

        [Constructable]
        public RewardPillow()
            : this(null)
        {
        }

        public RewardPillow(Mobile m)
            : base(GetID(m))
        {
        }

        public RewardPillow(Serial serial)
          : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

            if (list != null)
            {
                var index = Array.IndexOf(list, ItemID);

                if (index == 0)
                {
                    ItemID = list[1];
                }
                else
                {
                    ItemID = list[0];
                }
            }
        }

        private static int GetID(Mobile m)
        {
            if (m == null)
            {
                return IDTable[Utility.Random(IDTable.Length)][0];
            }

            if (m.Karma < 0)
            {
                return IDTable[Utility.RandomMinMax(4, 7)][0];
            }

            return IDTable[Utility.RandomMinMax(0, 3)][0];
        }

        private static int[][] IDTable = new int[][]
        {
            new int[] { 0x9E1D, 0x9E1E },
            new int[] { 0x9E1F, 0x9E20 },
            new int[] { 0xA09E, 0xA09D },
            new int[] { 0xA0A0, 0xA09F },

            new int[] { 0x9E21, 0x9E22 },
            new int[] { 0x9E23, 0x9E24 },
            new int[] { 0xA099, 0xA09A },
            new int[] { 0xA09B, 0xA09C },
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RewardPainting : Item, IFlipable
    {
        public override int LabelNumber => 1125147;

        public bool EastFacing => IDTable.Any(list => Array.IndexOf(list, ItemID) == 1);

        [Constructable]
        public RewardPainting()
            : this(null)
        {
        }

        public RewardPainting(Mobile m)
            : base(GetID(m))
        {
        }

        public RewardPainting(Serial serial)
            : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

            if (list != null)
            {
                var index = Array.IndexOf(list, ItemID);

                if (index == 0)
                {
                    ItemID = list[1];
                }
                else
                {
                    ItemID = list[0];
                }
            }
        }

        private static int GetID(Mobile m)
        {
            if (m == null)
            {
                return IDTable[Utility.Random(IDTable.Length)][0];
            }

            if (m.Karma < 0)
            {
                return IDTable[Utility.RandomMinMax(4, 7)][0];
            }

            return IDTable[Utility.RandomMinMax(0, 3)][0];
        }

        private static int[][] IDTable = new int[][]
        {
            new int[] { 0x9E31, 0x9E32 },
            new int[] { 0x9E33, 0x9E34 },
            new int[] { 0xA0A4, 0xA0A3 },
            new int[] { 0xA0A8, 0xA0A7 },

            new int[] { 0x9E2D, 0x9E2E },
            new int[] { 0x9E2F, 0x9E30 },
            new int[] { 0xA0A6, 0xA0A5 },
            new int[] { 0xA0AA, 0xA0A9 },
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RewardSculpture : Item
    {
        public override int LabelNumber => 1125080;

        public bool Active => IDTable.Any(list => Array.IndexOf(list, ItemID) > 0);

        [Constructable]
        public RewardSculpture()
            : this(null)
        {
        }

        public RewardSculpture(Mobile m)
            : base(GetID(m))
        {
        }

        public RewardSculpture(Serial serial)
           : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Active && !IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                var list = IDTable.FirstOrDefault(l => l.Any(id => id == ItemID));

                if (list != null)
                {
                    var index = Array.IndexOf(list, ItemID);

                    if (index == 0)
                    {
                        ItemID = list[1];
                    }
                    else
                    {
                        ItemID = list[0];
                    }
                }
            }
        }

        private static int GetID(Mobile m)
        {
            if (m == null)
            {
                return IDTable[Utility.Random(IDTable.Length)][0];
            }

            if (m.Karma < 0)
            {
                return IDTable[Utility.RandomMinMax(8, 15)][0];
            }

            return IDTable[Utility.RandomMinMax(0, 7)][0];
        }

        private static int[][] IDTable = new int[][]
        {
            new int[] { 0x9E80, 0x9DEE },
            new int[] { 0x9E82, 0x9DF6 },
            new int[] { 0xA065, 0xA066 },
            new int[] { 0xA06F, 0xA070 },
            new int[] { 0xA252, 0xA253 },
            new int[] { 0xA257, 0xA258 },
            new int[] { 0xA499, 0xA49A },
            new int[] { 0xA49E, 0xA49F },

            new int[] { 0x9E81, 0x9DF2 },
            new int[] { 0x9E83, 0x9DFA },
            new int[] { 0xA060, 0xA061 },
            new int[] { 0xA06A, 0xA06B },
            new int[] { 0xA25C, 0xA25D },
            new int[] { 0xA261, 0xA262 },
            new int[] { 0xA4A3, 0xA4A4 },
            new int[] { 0xA4A8, 0xA4A9 }
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
