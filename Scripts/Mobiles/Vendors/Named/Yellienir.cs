using System;
using System.Collections.Generic;
using Server.Items;

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
            this.Name = "Yellienir";
			
            this.m_Spoken = DateTime.UtcNow;
        }

        public Yellienir(Serial serial)
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
            this.CantWalk = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x851D;
            this.HairItemID = 0x2FCE;
            this.HairHue = 0x35;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new Cloak(0x3B2));
            this.AddItem(new FemaleLeafChest());
            this.AddItem(new LeafArms());
            this.AddItem(new LeafTonlet());
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;
					
                int range = 5;
				
                if (range >= 0 && this.InRange(m, range) && !this.InRange(oldLocation, range) && DateTime.UtcNow >= this.m_Spoken + TimeSpan.FromMinutes(1))
                {
                    /* Human.  Do you crave the chance to denounce your humanity and prove your elven ancestry.  
                    Do you yearn to accept the responsibilities of a caretaker of our beloved Sosaria and so 
                    redeem yourself.  Then human, seek out Darius the Wise in Moonglow. */
                    this.Say(1072801);
					
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