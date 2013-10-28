using System;
using System.Collections.Generic;
using Server.Items;

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
            this.Name = "Elder Taellia";
			
            this.m_Spoken = DateTime.UtcNow;
        }

        public Taellia(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return false;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return true;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        { 
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8385;
            this.HairItemID = 0x2FCD;
            this.HairHue = 0x368;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new Boots(0x74B));
            this.AddItem(new FemaleElvenRobe(0x44));
            this.AddItem(new Circlet());
            this.AddItem(new Item(0xDF2));
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;
					
                int range = 5;
				
                if (range >= 0 && this.InRange(m, range) && !this.InRange(oldLocation, range) && DateTime.UtcNow >= this.m_Spoken + TimeSpan.FromMinutes(1))
                {
                    /* Welcome Seeker.  Do you wish to embrace your elven heritage, casting 
                    aside your humanity, and accepting the responsibilities of a caretaker 
                    of our beloved Sosaria.  Then seek out Darius the Wise in Moonglow.  
                    He will place you on the path. */
                    this.Say(1072800);
					
                    this.m_Spoken = DateTime.UtcNow;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
	
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
	
            int version = reader.ReadInt();
			
            this.m_Spoken = DateTime.UtcNow;
        }
    }
}