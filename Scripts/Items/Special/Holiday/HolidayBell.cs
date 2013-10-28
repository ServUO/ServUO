using System;

namespace Server.Items
{
    public class HolidayBell : Item
    {
        private static readonly string[] m_StaffNames = new string[]
        {
            "Adrick",
            "Alai",
            "Bulldoz",
            "Evocare",
            "FierY-iCe",
            "Greyburn",
            "Hanse",
            "Ignatz",
            "Jalek",
            "LadyMOI",
            "Lord Krum",
            "Malantus",
            "Nimrond",
            "Oaks",
            "Prophet",
            "Runesabre",
            "Sage",
            "Stellerex",
            "T-Bone",
            "Tajima",
            "Tyrant",
            "Vex"
        };
        private static readonly int[] m_Hues = new int[]
        {
            0xA, 0x24, 0x42, 0x56, 0x1A, 0x4C, 0x3C, 0x60, 0x2E, 0x55, 0x23, 0x38, 0x482, 0x6, 0x10
        };
        private string m_Maker;
        private int m_SoundID;
        [Constructable]
        public HolidayBell()
            : this(m_StaffNames[Utility.Random(m_StaffNames.Length)])
        {
        }

        [Constructable]
        public HolidayBell(string maker)
            : base(0x1C12)
        {
            this.m_Maker = maker;

            this.LootType = LootType.Blessed;
            this.Hue = m_Hues[Utility.Random(m_Hues.Length)];
            this.SoundID = 0x0F5 + Utility.Random(14);
        }

        public HolidayBell(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get
            {
                return this.m_SoundID;
            }
            set
            {
                this.m_SoundID = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Giver
        {
            get
            {
                return this.m_Maker;
            }
            set
            {
                this.m_Maker = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                return String.Format("A Holiday Bell From {0}", this.Giver);
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else
                from.PlaySound(this.m_SoundID);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((string)this.m_Maker);

            writer.WriteEncodedInt((int)this.m_SoundID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Maker = reader.ReadString();
            this.m_SoundID = reader.ReadEncodedInt();

            Utility.Intern(ref this.m_Maker);
        }
    }
}