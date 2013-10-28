using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Jothan : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Jothan()
            : base("the wise")
        { 
            this.Name = "Elder Jothan";
        }

        public Jothan(Serial serial)
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
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8579;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x2C2;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ThighBoots());
            this.AddItem(new ElvenPants(0x57A));
            this.AddItem(new ElvenShirt(0x711));
            this.AddItem(new Cloak(0x21));
            this.AddItem(new Circlet());
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
        }
    }
}