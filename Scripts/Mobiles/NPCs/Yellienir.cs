using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Yellienir : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        private DateTime m_Spoken;
        [Constructable]
        public Yellienir()
            : base("the bark weaver")
        {
            Name = "Yellienir";

            m_Spoken = DateTime.UtcNow;
        }

        public Yellienir(Serial serial)
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
            CantWalk = true;
            Race = Race.Elf;

            Hue = 0x851D;
            HairItemID = 0x2FCE;
            HairHue = 0x35;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new Cloak(0x3B2));
            AddItem(new FemaleLeafChest());
            AddItem(new LeafArms());
            AddItem(new LeafTonlet());
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                int range = 5;

                if (range >= 0 && InRange(m, range) && !InRange(oldLocation, range) && DateTime.UtcNow >= m_Spoken + TimeSpan.FromMinutes(1))
                {
                    /* Human.  Do you crave the chance to denounce your humanity and prove your elven ancestry.  
                    Do you yearn to accept the responsibilities of a caretaker of our beloved Sosaria and so 
                    redeem yourself.  Then human, seek out Darius the Wise in Moonglow. */
                    Say(1072801);

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
