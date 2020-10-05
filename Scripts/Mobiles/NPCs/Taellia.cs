using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Taellia : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        private DateTime m_Spoken;
        [Constructable]
        public Taellia()
            : base("the wise")
        {
            Name = "Elder Taellia";

            m_Spoken = DateTime.UtcNow;
        }

        public Taellia(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => false;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8385;
            HairItemID = 0x2FCD;
            HairHue = 0x368;
        }

        public override void InitOutfit()
        {
            AddItem(new Boots(0x74B));
            AddItem(new FemaleElvenRobe(0x44));
            AddItem(new Circlet());
            AddItem(new Item(0xDF2));
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                int range = 5;

                if (range >= 0 && InRange(m, range) && !InRange(oldLocation, range) && DateTime.UtcNow >= m_Spoken + TimeSpan.FromMinutes(1))
                {
                    /* Welcome Seeker.  Do you wish to embrace your elven heritage, casting 
                    aside your humanity, and accepting the responsibilities of a caretaker 
                    of our beloved Sosaria.  Then seek out Darius the Wise in Moonglow.  
                    He will place you on the path. */
                    Say(1072800);

                    m_Spoken = DateTime.UtcNow;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Spoken = DateTime.UtcNow;
        }
    }
}
