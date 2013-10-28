using System;

namespace Server.Items
{
    [FlipableAttribute(0x236E, 0x2371)]
    public class LightOfTheWinterSolstice : Item
    {
        private static readonly string[] m_StaffNames = new string[]
        {
            "Aenima",
            "Alkiser",
            "ASayre",
            "David",
            "Krrios",
            "Mark",
            "Merlin",
            "Merlix", //LordMerlix
            "Phantom",
            "Phenos",
            "psz",
            "Ryan",
            "Quantos",
            "Outkast", //TheOutkastDev
            "V", //Admin_V
            "Zippy"
        };
        private string m_Dipper;
        [Constructable]
        public LightOfTheWinterSolstice()
            : this(m_StaffNames[Utility.Random(m_StaffNames.Length)])
        {
        }

        [Constructable]
        public LightOfTheWinterSolstice(string dipper)
            : base(0x236E)
        {
            this.m_Dipper = dipper;

            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this.Light = LightType.Circle300;
            this.Hue = Utility.RandomDyedHue();
        }

        public LightOfTheWinterSolstice(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Dipper
        {
            get
            {
                return this.m_Dipper;
            }
            set
            {
                this.m_Dipper = value;
            }
        }
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            this.LabelTo(from, 1070881, this.m_Dipper); // Hand Dipped by ~1_name~
            this.LabelTo(from, 1070880); // Winter 2004
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070881, this.m_Dipper); // Hand Dipped by ~1_name~
            list.Add(1070880); // Winter 2004
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((string)this.m_Dipper);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Dipper = reader.ReadString();
                        break;
                    }
                case 0:
                    {
                        this.m_Dipper = m_StaffNames[Utility.Random(m_StaffNames.Length)];
                        break;
                    }
            }

            if (this.m_Dipper != null)
                this.m_Dipper = String.Intern(this.m_Dipper);
        }
    }
}